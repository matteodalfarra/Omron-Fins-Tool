using System;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OmronFinsTCP.Net;

namespace ToolOmronFins
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        EtherNetPLC ENT = new EtherNetPLC();
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Events
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (IPTextBox.Text.Length > 0 && (PortTextBox.Text.Length > 0 && int.TryParse(PortTextBox.Text, out int port)))
                    ConnectButton.IsEnabled = true;
                else
                    ConnectButton.IsEnabled = false;
            }
            catch (Exception ex) { this.LogMessage(ex.Message, false); }
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button conButton = (Button)sender;

                if (conButton.Content.ToString() == "CONNECT")
                {
                    if (ENT.Link(IPTextBox.Text, int.Parse(PortTextBox.Text)) == 0)
                    {
                        this.ActivateAll();
                        conButton.Content = "DISCONNECT";
                        IPTextBox.IsEnabled = false;
                        PortTextBox.IsEnabled = false;
                        this.LogMessage("Connection established successfully", true);
                    }
                    else
                    {
                        this.LogMessage("Connection failed", false);
                    }
                }
                else if (conButton.Content.ToString() == "DISCONNECT")
                {
                    ENT.Close();
                    this.DeactivateAll();
                    ENT = new EtherNetPLC();
                    conButton.Content = "CONNECT";
                    IPTextBox.IsEnabled = true;
                    PortTextBox.IsEnabled = true;
                    this.LogMessage("Disconnection successful", true);
                }
            }
            catch (System.Net.NetworkInformation.PingException pex) { this.LogMessage("Connection failed", false); }
            catch (Exception ex) { this.LogMessage(ex.Message, false); }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.DeactivateAll();
            }
            catch (Exception ex) { this.LogMessage(ex.Message, false); }
        }

        private void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AreaComboBox.SelectedItem != null && AddressTextBox.Text.Length > 0 && int.TryParse(AddressTextBox.Text.Replace(".", ""), out int address))
                {
                    string area = ((ComboBoxItem)AreaComboBox.SelectedItem).Content.ToString();
                    string node = area + AddressTextBox.Text;
                    string value = ReadNode(ENT, node);

                    this.LogMessage(node + ": " + value, true);
                }
                else
                {
                    this.LogMessage("Invalid address", false);
                    AddressTextBox.Clear();
                }
            }
            catch (Exception ex) { this.LogMessage(ex.Message, false); }
        }

        private void ReadMultipleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AreaMComboBox.SelectedItem != null && StartAddressTextBox.Text.Length > 0 && int.TryParse(StartAddressTextBox.Text.Replace(".", ""), out int startAddress) && EndAddressTextBox.Text.Length > 0 && int.TryParse(EndAddressTextBox.Text.Replace(".", ""), out int endAddress))
                {
                    string area = ((ComboBoxItem)AreaMComboBox.SelectedItem).Content.ToString();
                    string range = area + StartAddressTextBox.Text + "-" + area + EndAddressTextBox.Text;

                    string value = ReadRangeOfNodes(ENT, range);

                    this.LogMessage(range + ": " + value, true);
                }
                else
                {
                    this.LogMessage("Invalid address", false);
                    StartAddressTextBox.Clear();
                    EndAddressTextBox.Clear();
                }
            }
            catch (Exception ex) { this.LogMessage(ex.Message, false); }
        }

        private void WriteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AreaSComboBox.SelectedItem != null && StartAddressSTextBox.Text.Length > 0 && int.TryParse(StartAddressSTextBox.Text.Replace(".", ""), out int startAddress) && EndAddressSTextBox.Text.Length > 0 && int.TryParse(EndAddressSTextBox.Text.Replace(".", ""), out int endAddress) && ValueTextBox.Text.Length > 0 && LenghtTextBox.Text.Length > 0 && int.TryParse(LenghtTextBox.Text, out int lenght))
                {
                    string area = ((ComboBoxItem)AreaMComboBox.SelectedItem).Content.ToString();
                    string value = ValueTextBox.Text.PadRight(lenght, ' ').Substring(0, lenght);
                    WriteStringToPLC(ENT, area + startAddress.ToString(), value);
                    this.LogMessage("Value sent successfully", true);
                }
                else
                {
                    this.LogMessage("Invalid address or lenght", false);
                    StartAddressSTextBox.Clear();
                    EndAddressSTextBox.Clear();
                    LenghtTextBox.Clear();
                }
            }
            catch (Exception ex) { this.LogMessage(ex.Message, false); }
        }
        #endregion

        #region Private Functions
        private void LogMessage(string message, bool positive)
        {
            string logText = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - {message}\n";

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                TextRange range = new TextRange(LogRichTextBox.Document.ContentEnd, LogRichTextBox.Document.ContentEnd);
                range.Text = logText;

                range.ApplyPropertyValue(TextElement.ForegroundProperty, positive ? Brushes.Black : Brushes.Red);
                LogRichTextBox.ScrollToEnd();
            });
        }

        private void DeactivateAll()
        {
            SingleNodeTabItem.IsEnabled = false;
            MultiplesNodeTabItem.IsEnabled = false;
            AreaComboBox.IsEnabled = false;
            AddressTextBox.IsEnabled = false;
            ReadButton.IsEnabled = false;
            AreaMComboBox.IsEnabled = false;
            ReadMultipleButton.IsEnabled = false;
            EndAddressTextBox.IsEnabled = false;
            StartAddressTextBox.IsEnabled = false;
            //WriteBool.IsEnabled = false;
            WriteString.IsEnabled = false;
            AreaSComboBox.IsEnabled = false;
            StartAddressSTextBox.IsEnabled = false;
            EndAddressSTextBox.IsEnabled = false;
            ValueTextBox.IsEnabled = false;
            LenghtTextBox.IsEnabled = false;
            WriteButton.IsEnabled = false;
        }

        private void ActivateAll()
        {
            SingleNodeTabItem.IsEnabled = true;
            MultiplesNodeTabItem.IsEnabled = true;
            AreaComboBox.IsEnabled = true;
            AddressTextBox.IsEnabled = true;
            ReadButton.IsEnabled = true;
            AreaMComboBox.IsEnabled = true;
            ReadMultipleButton.IsEnabled = true;
            EndAddressTextBox.IsEnabled = true;
            StartAddressTextBox.IsEnabled = true;
            //WriteBool.IsEnabled = true;
            WriteString.IsEnabled = true;
            AreaSComboBox.IsEnabled = true;
            StartAddressSTextBox.IsEnabled = true;
            EndAddressSTextBox.IsEnabled = true;
            ValueTextBox.IsEnabled = true;
            LenghtTextBox.IsEnabled = true;
            WriteButton.IsEnabled = true;
        }

        static string GetBitValue(EtherNetPLC ENT, string node)
        {
            ENT.GetBitState(node, out short result);
            return result.ToString();
        }

        static string GetCIOBitValue(EtherNetPLC ENT, string node)
        {
            string cioIndirizzo = node.Replace("CIO", "");
            short ri = ENT.GetBitState(PlcMemory.CIO, cioIndirizzo, out short bs);
            return (ri == 0) ? bs.ToString() : "Errore";
        }

        static string GetBitFromWord(EtherNetPLC ENT, string node)
        {
            string[] parts = node.Split('.');
            string baseAddress = parts[0];
            int bitIndex = int.Parse(parts[1]);

            ENT.ReadInt32(baseAddress, out int result);
            int bitValue = (result >> bitIndex) & 1;

            return bitValue.ToString();
        }

        static string GetNumericValue(EtherNetPLC ENT, string node)
        {
            ENT.ReadInt32(node, out int result);
            return node.StartsWith("W") ? result.ToString("F1") : result.ToString();
        }

        static string ReadNode(EtherNetPLC ENT, string node)
        {
            return node switch
            {
                var a when a.StartsWith("H") => GetBitValue(ENT, node),
                var a when a.StartsWith("CIO") => GetCIOBitValue(ENT, node),
                var a when a.StartsWith("D") && a.Contains(".") => GetBitFromWord(ENT, node),
                var a when a.StartsWith("W") || a.StartsWith("D") => GetNumericValue(ENT, node),
                _ => "Error"
            };
        }
        
        static string ReadRangeOfNodes(EtherNetPLC ENT, string range)
        {
            string[] parts = range.Split('-');
            string startNode = parts[0];
            string endNode = parts[1];

            int start = int.Parse(startNode.Substring(1));
            int end = int.Parse(endNode.Substring(1));

            short length = (short)(end - start + 1);

            ENT.ReadWords(startNode, length, out short[] data);

            StringBuilder result = new StringBuilder();
            foreach (var value in data)
            {
                byte[] bytes = BitConverter.GetBytes(value);
                result.Append(Encoding.ASCII.GetString(bytes));
            }

            string resultString = result.ToString().TrimEnd('\0');
            return resultString.PadRight(20, ' ');
        }

        static void WriteStringToPLC(EtherNetPLC ENT, string start, string s)
        {
            byte[] sBytes = Encoding.ASCII.GetBytes(s);
            int wordCount = (sBytes.Length + 1) / 2;
            short[] wordData = new short[wordCount];

            for (int i = 0; i < wordCount; i++)
            {
                wordData[i] = (i * 2 + 1 < sBytes.Length) ?
                    (short)(sBytes[i * 2] | (sBytes[i * 2 + 1] << 8)) :
                    (short)(sBytes[i * 2]);
            }

            ENT.WriteWords(start, (short)wordCount, wordData);
        }
        #endregion


    }
}
