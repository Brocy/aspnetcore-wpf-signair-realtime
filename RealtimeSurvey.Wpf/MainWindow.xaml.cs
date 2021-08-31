using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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

namespace RealtimeSurvey.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public HubConnection hubConnection { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            init();
        }

        private async void init()
        {
            DeviceNameTextBox.Text = Environment.MachineName;

            string MAC = NetworkInterface.GetAllNetworkInterfaces()
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .Select(nic => nic.GetPhysicalAddress().ToString())
                .FirstOrDefault();

            DeviceMacTextBox.Text = MAC;

            DeviceIpTextBox.Text = GetLocalIPAddress();



        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:52553/exam?device=android")
                .Build();

            hubConnection.On<string, string>("newMessage", (sender, message) => MessageBox.Show($"{sender}: {message}"));

            await hubConnection.StartAsync();

            DisconnectButton.IsEnabled = true;
            ConnectButton.IsEnabled = false;

            DeviceIdTextBox.Text = hubConnection.ConnectionId;

            MessageBox.Show(" Connected! " + hubConnection.ConnectionId);
        }

        private async void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (null != hubConnection && hubConnection.State == HubConnectionState.Connected)
            {
                await hubConnection.StopAsync();
            }

            DisconnectButton.IsEnabled = false;
            ConnectButton.IsEnabled = true;

        }

        public string GetLocalIPAddress()
        {

            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());

            foreach (System.Net.IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            throw new System.Exception("No network adapters with IPv4 address in system!");
        }


    }
}
