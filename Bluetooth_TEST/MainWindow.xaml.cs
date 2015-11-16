using System;
using System.Windows;
using System.Windows.Controls;
using InTheHand.Net.Bluetooth;
using InTheHand.Net;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace Bluetooth_TEST
{    
    public partial class MainWindow : Window
    {
        public BluetoothAddress mchoice;
        public ObservableCollection<object> ObservableObj;
        InTheHand.Net.Sockets.BluetoothClient cli = new InTheHand.Net.Sockets.BluetoothClient();
        InTheHand.Net.Sockets.BluetoothDeviceInfo[] devices;
        DispatcherTimer timer;                                                 //创建一个timer类
        string file;
        public MainWindow()
        {
            InitializeComponent();
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

        private void commit_Click(object sender, RoutedEventArgs e)
        {

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
