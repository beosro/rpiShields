﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SixFabWpf
{
    /// <summary>
    /// Interaction logic for Window_Sms.xaml
    /// </summary>
    public partial class Window_Sms : Window
    {
        private SerialPort serialPort;
        private BackgroundWorker backgroundWorker_StartSendSms;
        private StringBuilder buffer;
        private int waitCounter;

        private bool workActive = false;

        public Window_Sms()
        {
            InitializeComponent();

            buffer = new StringBuilder();

            serialPort = new SerialPort("COM7", 115200);
            serialPort.DataReceived += serialPort_DataReceived;
            serialPort.ReadTimeout = 1500;

            try
            {
                if (!serialPort.IsOpen)
                {
                    serialPort.Open();
                }
            }
            catch (Exception ex)
            {
                setLabelText(ex.Message);
            }

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            MouseDown += MainWindow_MouseDown;
        }

        void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (!workActive)
            {
                toConsoleReceive(serialPort.ReadExisting());
            }
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                    DragMove();
            }
            catch (Exception ex)
            {
            }
        }

        private void CloseLabel_Click(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        void SendToSerial(string data)
        {
            try
            {
                serialPort.Write(data+"\r\n");
                toConsoleSend(data+"\r\n");
            }
            catch (Exception ex)
            {
                setLabelText(ex.Message);
            }
        }

        private bool WaitFor(string data)
        {
            waitCounter = 0;

            while (true)
            {
                try
                {
                    string s = ((char)serialPort.ReadChar()).ToString();
                    buffer.Append(s);
                    toConsoleReceive(s);

                    if (buffer.ToString().IndexOf(data) > -1)
                    {
                        buffer.Clear();
                        serialPort.ReadExisting();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    if (++waitCounter > 1)
                    {
                        workActive = false;
                        return false;
                    }
                }
            }
        }

        void backgroundWorker_StartSendSms_DoWork(object sender, DoWorkEventArgs e)
        {
            setLabelText("");

            do
            {
                SendToSerial("AT");

                if (WaitFor("OK\r\n"))
                {
                    break;
                }
                else
                {
                    setLabelText("Check On/Off");
                    return;
                }
            } while (true);
            


            do
            {
                SendToSerial("AT+CMGF=1");

                if (WaitFor("OK\r\n"))
                {
                    break;
                }
                else
                {
                    setLabelText("Error, Try Again!");
                    return;
                }
            } while (true);

            do
            {
                SendToSerial("AT+CSCS=\"GSM\"");

                if (WaitFor("OK\r\n"))
                {
                    break;
                }
                else
                {
                    setLabelText("Error, Try Again!");
                    return;
                }
            } while (true);

            do
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    SendToSerial("AT+CMGS=\"" + PhoneNumber.Text + "\"");
                }));
               

                if (WaitFor(">"))
                {
                    break;
                }
                else
                {
                    setLabelText("Error, Try Again!");
                    return;
                }
            } while (true);


            do
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    SendToSerial(SmsText.Text+"\x1A");
                }));


                if (WaitFor(">"))
                {
                    break;
                }
                else
                {
                    setLabelText("Error, Try Again!");
                    return;
                }
            } while (true);





            workActive = false;

            /*
                

                int i = 0;

                StringBuilder tmp = new StringBuilder();

                do
                {
                    try
                    {
                        char c = (char)serialPort.ReadChar();

                        tmp.Append(c);

                        toConsoleReceive(c);

                        string[] list = tmp.ToString().Replace("\r\n", "#").Split('#');

                        if (list.Length == 3)
                        {
                            if (list[1] == "OK")
                            {
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string[] list = tmp.ToString().Replace("\r\n", "#").Split('#');

                        Console.Write(tmp.ToString());
                        tmp.Clear();

                        if (list.Length == 1)
                        {
                            setLabelText("Check on/off");
                            return;
                        }
                        else if (list.Length == 3)
                        {

                            if (list[1] != "OK")
                            {
                                serialPort.WriteLine("AT");
                                toConsoleSend("AT");
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                } while (true);



                tmp.Clear();
                i = 0;
                serialPort.WriteLine("AT+CMGF=1");
                toConsoleSend("AT+CMGF=1");


                do
                {
                    try
                    {
                        char c = (char)serialPort.ReadChar();

                        tmp.Append(c);

                        toConsoleReceive(c);

                        string[] list = tmp.ToString().Replace("\r\n", "#").Split('#');

                        if (list.Length == 3)
                        {
                            if (list[1] == "OK")
                            {
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string[] list = tmp.ToString().Replace("\r\n", "#").Split('#');

                        Console.Write(tmp.ToString());
                        tmp.Clear();

                        if (list[1] != "OK")
                        {
                            serialPort.WriteLine("AT+CMGF=1");
                            toConsoleSend("AT+CMGF=1");
                        }
                        else
                        {
                            break;
                        }
                    }
                } while (true);




                tmp.Clear();
                i = 0;
                serialPort.WriteLine("AT+CSCS=\"GSM\"");
                toConsoleSend("AT+CSCS=\"GSM\"");


                do
                {
                    try
                    {
                        char c = (char)serialPort.ReadChar();

                        tmp.Append(c);

                        toConsoleReceive(c);

                        string[] list = tmp.ToString().Replace("\r\n", "#").Split('#');

                        if (list.Length == 3)
                        {
                            if (list[1] == "OK")
                            {
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string[] list = tmp.ToString().Replace("\r\n", "#").Split('#');

                        Console.Write(tmp.ToString());
                        tmp.Clear();

                        if (list[1] != "OK")
                        {
                            serialPort.WriteLine("AT+CSCS=\"GSM\"");
                            toConsoleSend("AT+CSCS=\"GSM\"");
                        }
                        else
                        {
                            break;
                        }
                    }
                } while (true);



                tmp.Clear();
                i = 0;


                this.Dispatcher.Invoke((Action)(() =>
                {
                    serialPort.WriteLine("AT+CMGS=\"" + PhoneNumber.Text + "\"");
                    toConsoleSend("AT+CMGS=\"" + PhoneNumber.Text+"\"");
                }));
               


                do
                {
                    try
                    {
                        char c = (char)serialPort.ReadChar();

                        tmp.Append(c);

                        toConsoleReceive(c);

                        string[] list = tmp.ToString().Replace("\r\n", "#").Split('#');

                        if (c=='>')
                        {
                            serialPort.WriteLine(SmsText.Text.ToString() + "\x1A");
                            toConsoleSend(SmsText.Text.ToString());
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        string[] list = tmp.ToString().Replace("\r\n", "#").Split('#');

                        Console.Write(tmp.ToString());
                        tmp.Clear();

                        if (list.Length > 4)
                        {
                            if (list[4] != "OK")
                            {
                                serialPort.WriteLine(SmsText.Text.ToString() + "\x1A");
                                toConsoleSend(SmsText.Text.ToString());
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                } while (true);


            }
            catch (Exception ex)
            {
                setLabelText(ex.Message);
            }
             * */
        }

        private void toConsoleSend(string s)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                ConsoleSend.Text +=s ;
                ConsoleSend.Text += "\r\n";
                ConsoleSend.ScrollToEnd();
            }));
        }

        private void setLabelText(string s)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                LblMessage.Content = s;
            }));
        }

        private void toConsoleReceive(string s)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                ConsoleReceive.Text += s;
                ConsoleReceive.ScrollToEnd();
            }));
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!serialPort.IsOpen)
                {
                    serialPort.Open();
                }
            }
            catch (Exception ex)
            {
                setLabelText(ex.Message);
                return;
            }

            workActive = true;

            backgroundWorker_StartSendSms = new BackgroundWorker();
            backgroundWorker_StartSendSms.DoWork += backgroundWorker_StartSendSms_DoWork;
            backgroundWorker_StartSendSms.RunWorkerAsync();
        }

        private void ClearAllConsoleReceive(object sender, RoutedEventArgs e)
        {
            ConsoleReceive.Clear();
        }

        private void ClearAllConsoleSend(object sender, RoutedEventArgs e)
        {
            ConsoleSend.Clear();
        }
    }
}
