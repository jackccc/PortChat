using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.IO.Ports;


namespace PortChat
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private static readonly SerialPort SerialPortObj = new SerialPort();
        string _recievedData;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Connect_Comms(object sender, RoutedEventArgs e)
        {
            if ((BtnConnect.Content as string) == "连接")
            {
                //Sets up serial port
                SerialPortObj.PortName = CbbPortName.Text;
                SerialPortObj.BaudRate = int.Parse(CbbBaudRate.Text);
                SerialPortObj.Parity = (Parity)Enum.Parse(typeof(Parity), CbbPortParity.Text, true);
                SerialPortObj.DataBits = int.Parse(TbDataBits.Text);
                SerialPortObj.StopBits = (StopBits)Enum.Parse(typeof(StopBits), CbbStopBits.Text, true);
                SerialPortObj.Handshake = (Handshake)Enum.Parse(typeof(Handshake), CbbHandshake.Text, true);

                SerialPortObj.ReadTimeout = 500;
                SerialPortObj.WriteTimeout = 500;

                if (!SerialPortObj.IsOpen)
                {
                    SerialPortObj.Open();
                }

                //Sets button State and Creates function call on data recieved
                BtnConnect.Content = "断开";

                BtnConnect.Background = Brushes.Green;

                SerialPortObj.DataReceived += Recieve;
            }
            else
            {
                try // just in case serial port is not open could also be acheved using if(serial.IsOpen)
                {
                    SerialPortObj.Close();
                    BtnConnect.Content = "连接";
                    BtnConnect.Background = Brushes.White;
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        private delegate void UpdateUiTextDelegate(string text);

        private void Recieve(object sender, SerialDataReceivedEventArgs e)
        { 
            // 收集缓存中的信息
            _recievedData = SerialPortObj.ReadLine();
            Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(WriteDate), _recievedData);
        }

        private void WriteDate(string text)
        {
            TbReceive.Text += _recievedData;
        }

        private void Send_Data(object sender, RoutedEventArgs e)
        {
            SerialCmdSend(TbSend.Text);
            TbSend.Text = string.Empty;
        }

        private void SerialCmdSend(string data)
        {
            if (!SerialPortObj.IsOpen) return;
            try
            {
                SerialPortObj.WriteLine(data);
            }
            catch (Exception ex)
            {
                    
                TbSend.Text = "Failed to SEND" + data + "\n" + ex + "\n";
            }
        }

    }
}
