using System;
using System.Windows;
using System.Windows.Controls;
using InTheHand.Net.Bluetooth;
using InTheHand.Net;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Threading;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Bluetooth_TEST
{    
    public partial class MainWindow : Window
    {
        public BluetoothAddress mchoice;
        public ObservableCollection<object> ObservableObj;
        InTheHand.Net.Sockets.BluetoothClient cli = new InTheHand.Net.Sockets.BluetoothClient();
        InTheHand.Net.Sockets.BluetoothDeviceInfo[] devices;
        private CancellationTokenSource cts;
        DispatcherTimer timer;                                                 //创建一个计时器线程
        Thread sendThread;                                                     //创建发送文件线程
        string file;
        public MainWindow()
        {
            InitializeComponent();
            cts = new CancellationTokenSource();
            ObservableObj = new ObservableCollection<object>();
            listView.DataContext = ObservableObj;
        }
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();                                     
            timer.Interval = TimeSpan.FromSeconds(60);
            timer.Tick += timer_Tick;
            timer.Start();                                          //开始进程，设置为60s刷新一次
            BluetoothRadio radio = BluetoothRadio.PrimaryRadio;     //获取当前PC的蓝牙适配器   
            radio.Mode = RadioMode.Discoverable;                    //设置本地蓝牙可被检测  
            if (radio == null)                                      //检查该电脑蓝牙是否可用
            {
                MessageBox.Show("请您打开电脑的蓝牙！");
                return;
            }
            ObservableObj.Clear();
            devices = cli.DiscoverDevices();
            foreach (InTheHand.Net.Sockets.BluetoothDeviceInfo device in devices) //设备搜寻           
            {
                device.Update();
                device.Refresh();
                int i;
                for (i = 0; i < devices.Length; i++)
                {
                    ObservableObj.Add(new { Name = devices[i].DeviceName,Address = devices[i].DeviceAddress, Statu= devices[i].Connected });
                }
                link.Text = "已打开蓝牙!";
                break;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            ObservableObj.Clear();
            timer.Stop();                                            //停止进程
            link.Text = "未连接";
        }

        private void connect_Click(object sender, RoutedEventArgs e)
        {
            int a = listView.Items.Count;                            //获取listview那一栏被选中
            mchoice = devices[a - 1].DeviceAddress;                  //取到被选中那一栏的地址
            Guid mGUID = BluetoothService.ObexObjectPush;            //得到本机guid
            cli.Connect(mchoice, mGUID);                             //链接蓝牙
            link.Text = "已连接";
        }

        void timer_Tick(object sender, EventArgs e)                  
        {
                ObservableObj.Clear();
                devices = cli.DiscoverDevices();
            foreach (InTheHand.Net.Sockets.BluetoothDeviceInfo device in devices) //设备搜寻           
            {
                device.Update();
                device.Refresh();
                int i;
                for (i = 0; i < devices.Length; i++)
                {
                    ObservableObj.Add(new { Name = devices[i].DeviceName, Address = devices[i].DeviceAddress, Statu = devices[i].Connected });
                }
                break;
            }
        }
        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void send_Click(object sender, RoutedEventArgs e)
        {
            sendThread = new Thread(sendFile);//开启发送文件线程  
            sendThread.Start();
        }
        private void sendFile()//发送文件方法  
        {
            ObexWebRequest request = new ObexWebRequest(mchoice, Path.GetFileName(file));            //创建网络请求
            WebResponse response = null;
            try
            {
                send.IsEnabled = false;
                request.ReadFile(file);//发送文件  
                link.Text = "开始发送!";
                response = request.GetResponse();//获取回应  
                link.Text = "发送完成!";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("发送失败！, 提示 "+ex.Message);
                link.Text = "发送失败!";
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    send.IsEnabled =true;
                }
            }
        }

        private void choice_button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog =new Microsoft.Win32.OpenFileDialog();
            dialog.ShowDialog();
            file = dialog.FileName;
            string[] temp = file.Split('\\');
            filename.Text = temp[temp.Length - 1];                           //运用字符串分割，得到文件名
        }
    }
}
