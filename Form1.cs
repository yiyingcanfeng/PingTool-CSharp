using System;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Forms;

namespace Ping
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
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

        private void button1_Click(object sender, EventArgs e)
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
                        var pingable = pingReply.Status == IPStatus.Success;
                        if (pingable)
                        {
                            tableLabel.BackColor = Color.Green;
                        }
                        else
                        {
                            tableLabel.BackColor = Color.Red;
                        }

                        Console.WriteLine(ipAddress);
                        Console.WriteLine(exception);
                    }
                    finally
                    {
                        if (ping != null)
                        {
                            ping.Dispose();
                        }
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

        private void button3_Click(object sender, EventArgs e)
        {
            PingStandalone();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) 13)
            {
                PingStandalone();
            }
        }

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
                    var pingable = pingReply.Status == IPStatus.Success;
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

                    Console.WriteLine(pingable);
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
                    if (ping != null)
                    {
                        ping.Dispose();
                    }
                }
            });
            thread.Start();
        }
    }
}