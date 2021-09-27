using System;
using System.IO.Ports;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace lab_first_toks
{
    public partial class MainWindow : Window
    {
        SerialPort serialport;
        public string[] AvailablePorts;
        public string SelectedPort;

        public MainWindow()
        {
            InitializeComponent();
            AvailablePorts = SerialPort.GetPortNames();
            foreach(string port in AvailablePorts)
            {
                TextBlock Com = new TextBlock();
                Com.Text = port;
                Com_Choice.Items.Add(Com);
            }
        }

        private void Com_Choice_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                SelectedPort = Com_Choice.Text;
                serialport = new SerialPort(SelectedPort, 9600,
                         Parity.None, 8, StopBits.One);
                serialport.Encoding = Encoding.UTF8;
                serialport.DataReceived += new SerialDataReceivedEventHandler(SerialPortDataReceived);
                serialport.Open();
                debug.Text += String.Format("You Choose " + Com_Choice.Text + " port\n");
            }
            catch
            {
                if (AvailablePorts.Length == 0)
                {
                    debug.Text += String.Format("No free ports\n");
                }
                else if(!serialport.IsOpen)
                {
                    debug.Text += String.Format("Port is busy \n");
                }
                else debug.Text += String.Format("You have not selected a port\n");
            }
        }

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string output = serialport.ReadExisting();
            this.Dispatcher.Invoke((Action)(() =>
            {
                OutputText.Text += output;
                OutputText.Text += "\n";
            }));
        }

        private void InputText_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Return)
                {
                    serialport.Write(InputText.Text);
                    InputText.Clear();
                }
            }
            catch
            {
                debug.Text += String.Format("You are not connected to a port \n");
            }
        }

        private void Find_COM_Click(object sender, RoutedEventArgs e)
        {
            AvailablePorts = SerialPort.GetPortNames();
            foreach (string port in AvailablePorts)
            {
                TextBlock Com = new TextBlock();
                Com.Text = port;
                Com_Choice.Items.Add(Com);
            }
        }
    }
}
