using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using InTheHand.Net;
using System.Threading;
using System.Collections.ObjectModel;
using InTheHand.Windows.Forms;
using System.Net.Sockets;

namespace Bluetooth_TEST
{    
    public partial class MainWindow : Window
    {
        public BluetoothAddress mchoice;
        public ObservableCollection<object> ObservableObj;
        InTheHand.Net.Sockets.BluetoothClient cli = new InTheHand.Net.Sockets.BluetoothClient();
        InTheHand.Net.Sockets.BluetoothDeviceInfo[] devices;
        public MainWindow()
        {
            InitializeComponent();
            ObservableObj = new ObservableCollection<object>();
            listView.DataContext = ObservableObj;
        }
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            
            BluetoothRadio radio = BluetoothRadio.PrimaryRadio;//获取当前PC的蓝牙适配器   
            radio.Mode = RadioMode.Discoverable;//设置本地蓝牙可被检测  
            if (radio == null)//检查该电脑蓝牙是否可用
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
                link.Text = "搜索完成!";
                break;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            ObservableObj.Clear();
            link.Text = "未连接";
        }

        private void connect_Click(object sender, RoutedEventArgs e)
        {
            int a = listView.Items.Count;
            mchoice = devices[a - 1].DeviceAddress;
            Guid mGUID = BluetoothService.ObexObjectPush;
            cli.Connect(mchoice, mGUID);
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
