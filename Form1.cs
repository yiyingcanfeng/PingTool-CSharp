using System;
using System.Drawing;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace Ping
{
    public partial class Form1 : Form
    {
        private const string XmlConfigName = "pingTool.config.xml";
        private readonly string _xmlConfigFilePath;

        public Form1()
        {
            InitializeComponent();
            var currentDirectory = Environment.CurrentDirectory;
            _xmlConfigFilePath = Path.Combine(currentDirectory, XmlConfigName);
            // 判断应用程序启动路径下是否有配置文件，如果有则读取其中的配置，如果没有则创建并写入默认配置
            if (!File.Exists(_xmlConfigFilePath))
            {
                SaveConfig();
            }
            else
            {
                LoadConfig();
            }

            CheckForIllegalCrossThreadCalls = false;
            // 渲染表格内的控件
            for (var i = 0; i < 15; i++)
            for (var j = 0; j < 17; j++)
            {
                var label = new Label();
                label.Text = (j * 15 + i + 1).ToString();
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Click += (sender, args) =>
                {
                    textBox1.Text = $"{textBox2.Text}.{textBox3.Text}.{textBox4.Text}.{label.Text}";
                };
                tableLayoutPanel1.Controls.Add(label, i, j);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 开始按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            StartPing();
        }

        /// <summary>
        /// 单独ping按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            SaveConfig();
            PingStandalone();
        }

        /// <summary>
        /// ping独立ip的textBox增加enter事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) 13)
            {
                PingStandalone();
            }
        }

        /// <summary>
        /// 绑定窗体全局快捷键，实现按Ctrl+W关闭程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.W)
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// 给上方的6个textBox增加enter事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterStartPing(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                StartPing();
            }
        }

        /// <summary>
        /// 创建多个线程ping指定范围的ip
        /// </summary>
        private void StartPing()
        {
            if (int.Parse(textBox2.Text) > 255 || int.Parse(textBox2.Text) <= 0 ||
                int.Parse(textBox3.Text) > 255 || int.Parse(textBox3.Text) <= 0 ||
                int.Parse(textBox4.Text) > 255 || int.Parse(textBox4.Text) <= 0 ||
                int.Parse(textBox5.Text) > 255 || int.Parse(textBox5.Text) <= 0 ||
                int.Parse(textBox6.Text) > 255 || int.Parse(textBox6.Text) <= 0)
            {
                MessageBox.Show("IP输入不正确!", "错误");
                return;
            }

            SaveConfig();

            for (int i = int.Parse(textBox5.Text) - 1; i <= int.Parse(textBox6.Text) - 1; i++)
            {
                var tableLabel = SearchLabelFromTableLayoutPanel((i + 1).ToString());
                tableLabel.BackColor = DefaultBackColor;

                var thread = new Thread(() =>
                {
                    System.Net.NetworkInformation.Ping ping = null;
                    var ipAddress = $"{textBox2.Text}.{textBox3.Text}.{textBox4.Text}.{tableLabel.Text}";
                    try
                    {
                        ping = new System.Net.NetworkInformation.Ping();

                        PingReply pingReply = ping.Send(ipAddress, int.Parse(textBox7.Text));
                        var pingable = pingReply.Status == IPStatus.Success;
                        if (pingable)
                        {
                            tableLabel.BackColor = Color.Green;
                        }
                        else
                        {
                            tableLabel.BackColor = Color.Red;
                        }
                    }
                    catch (Exception exception)
                    {
                        ping = new System.Net.NetworkInformation.Ping();

                        PingReply pingReply = ping.Send(ipAddress);
                        var pingable = pingReply?.Status == IPStatus.Success;
                        tableLabel.BackColor = pingable ? Color.Green : Color.Red;

                        Console.WriteLine(ipAddress);
                        Console.WriteLine(exception);
                    }
                    finally
                    {
                        ping?.Dispose();
                    }
                });
                try
                {
                    thread.Start();
                }
                catch (Exception exception)
                {
                    thread.Start();
                }
            }
        }

        /// <summary>
        /// 根据控件的文本搜索表格中的控件
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private Control SearchLabelFromTableLayoutPanel(string text)
        {
            Control control = null;
            for (int i = 0; i < tableLayoutPanel1.Controls.Count; i++)
            {
                if (tableLayoutPanel1.Controls[i].Text == text)
                {
                    control = tableLayoutPanel1.Controls[i];
                }
            }

            return control;
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        private void SaveConfig()
        {
            try
            {
                XmlDocument xd = new XmlDocument();
                XmlDeclaration xmlDeclaration = xd.CreateXmlDeclaration("1.0", "utf-8", "yes");
                xd.AppendChild(xmlDeclaration);

                XmlElement config = xd.CreateElement("config");

                XmlElement ipPart1 = xd.CreateElement("ipPart1");
                ipPart1.InnerText = textBox2.Text;

                XmlElement ipPart2 = xd.CreateElement("ipPart2");
                ipPart2.InnerText = textBox3.Text;

                XmlElement ipPart3 = xd.CreateElement("ipPart3");
                ipPart3.InnerText = textBox4.Text;

                XmlElement ipPart4 = xd.CreateElement("ipPart4");
                ipPart4.InnerText = textBox5.Text;

                XmlElement ipPart5 = xd.CreateElement("ipPart5");
                ipPart5.InnerText = textBox6.Text;

                XmlElement delay = xd.CreateElement("delay");
                delay.InnerText = textBox7.Text;

                XmlElement standaloneIp = xd.CreateElement("standaloneIp");
                standaloneIp.InnerText = textBox1.Text;

                xd.AppendChild(config);
                config.AppendChild(ipPart1);
                config.AppendChild(ipPart2);
                config.AppendChild(ipPart3);
                config.AppendChild(ipPart4);
                config.AppendChild(ipPart5);
                config.AppendChild(delay);
                config.AppendChild(standaloneIp);

                xd.Save(_xmlConfigFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        private void LoadConfig()
        {
            try
            {
                XmlDocument xd = new XmlDocument();
                xd.Load(_xmlConfigFilePath);
                if (xd.DocumentElement != null)
                {
                    XmlNode ipPart1Node = xd.DocumentElement.SelectSingleNode("/config/ipPart1");
                    XmlNode ipPart2Node = xd.DocumentElement.SelectSingleNode("/config/ipPart2");
                    XmlNode ipPart3Node = xd.DocumentElement.SelectSingleNode("/config/ipPart3");
                    XmlNode ipPart4Node = xd.DocumentElement.SelectSingleNode("/config/ipPart4");
                    XmlNode ipPart5Node = xd.DocumentElement.SelectSingleNode("/config/ipPart5");
                    XmlNode delayNode = xd.DocumentElement.SelectSingleNode("/config/delay");
                    XmlNode standaloneIpNode = xd.DocumentElement.SelectSingleNode("/config/standaloneIp");

                    textBox2.Text = ipPart1Node?.InnerText;
                    textBox3.Text = ipPart2Node?.InnerText;
                    textBox4.Text = ipPart3Node?.InnerText;
                    textBox5.Text = ipPart4Node?.InnerText;
                    textBox6.Text = ipPart5Node?.InnerText;
                    textBox7.Text = delayNode?.InnerText;
                    textBox1.Text = standaloneIpNode?.InnerText;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// 单独ping一个ip
        /// </summary>
        private void PingStandalone()
        {
            label8.Text = "正在Ping";
            var ipText = textBox1.Text;
            Thread thread = new Thread(() =>
            {
                System.Net.NetworkInformation.Ping ping = null;
                try
                {
                    ping = new System.Net.NetworkInformation.Ping();
                    PingReply pingReply = ping.Send(ipText, int.Parse(textBox7.Text));
                    var pingable = pingReply?.Status == IPStatus.Success;
                    if (pingable)
                    {
                        label8.Text = "通过";
                        label8.ForeColor = Color.Green;
                    }
                    else
                    {
                        label8.Text = "不通";
                        label8.ForeColor = Color.Red;
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    Console.WriteLine(ipText);
                    label8.Text = "";
                    MessageBox.Show($"{exception.Message}{exception.GetBaseException().Message}", "错误");
                }
                finally
                {
                    ping?.Dispose();
                }
            });
            thread.Start();
        }
    }
}