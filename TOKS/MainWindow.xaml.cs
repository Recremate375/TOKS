using System;
using System.IO.Ports;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace TOKS
{
    public partial class MainWindow : Window
    {
        SerialPort serialport;
        private string flag = "i";
        public string[] AvailablePorts;
        public string SelectedPort;

        public MainWindow()
        {
            InitializeComponent();
            AvailablePorts = SerialPort.GetPortNames();
            foreach (string port in AvailablePorts)
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
                debug.Text += String.Format(Com_Choice.Text + " port selected\n");
            }
            catch
            {
                if (AvailablePorts.Length == 0)
                {
                    debug.Text += String.Format("All ports are unavailable\n");
                }
                else if (SelectedPort.Length == 0)
                {
                    debug.Text += String.Format("Port is not selected\n");
                }
                else if (!serialport.IsOpen)
                {
                    debug.Text += String.Format("Port unavailable\n");
                }
            }
        }

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string output = serialport.ReadExisting();
            this.Dispatcher.Invoke((Action)(() =>
            {
                OutputText.Clear();
                OutputText.Text += output;
                OutputText.Text += "\n";
            }));
        }

        private void InputText_KeyDown(object sender, KeyEventArgs e)
        {
            string input = InputText.Text;
            try
            {
                if (e.Key == Key.Return)
                {
                    textCoder(input);
                    textDecoder(input);
                    serialport.Write(input);
                }
            }
            catch
            {
                debug.Text += String.Format("Port is not connected\n");
            }
        }

        private void Find_COM_Click(object sender, RoutedEventArgs e)
        {
            AvailablePorts = SerialPort.GetPortNames();
            Com_Choice.Items.Clear();
            foreach (string port in AvailablePorts)
            {
                TextBlock Com = new TextBlock();
                Com.Text = port;
                Com_Choice.Items.Add(Com);
            }
            debug.Text += String.Format("Port's searching \n");
        }

        private string textCoder(string str)
        {
            string code = "@#";
            string fcode = "@!";
            string strPrint = str;
            if(str.Contains("@"))
            {
                str = str.Replace("@", fcode);
                strPrint = strPrint.Replace("@", "[" + fcode + "]");
            }
            if(str.Contains(flag))
            {
                str = str.Replace(flag, code);
                strPrint = strPrint.Replace(flag, "["+code+"]");
            }
            Coder.Clear();
            Coder.Text += "Encrypted: " + strPrint;
            return str;
        }
        private string textDecoder(string str)
        {
            if(str.Contains("@#"))
            {
                str = str.Replace("@#", flag);
            }
            if(str.Contains("@!"))
            {
                str = str.Replace("@!", "@");
            }
            return str;
        }
    }
}
