using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.Remoting.Contexts;

namespace A111223022_MyPainter_Ex2
{
    public partial class Form1 : Form
    {
        Color mycolor = Color.Black;
        bool ShouldPaint;
        Point P, YSP, YEP;
        int tool = 1;
        public Form1()
        {
            InitializeComponent();
        }
        UdpClient U;
        Thread Th;
        bool connect = false;

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            ShouldPaint = true;
            P.X = e.X;
            P.Y = e.Y;
        }
        private string MyIP()
        {
            string hn = Dns.GetHostName();
            IPAddress[] ip = Dns.GetHostEntry(hn).AddressList;
            foreach (IPAddress it in ip)
            {
                if (it.AddressFamily == AddressFamily.InterNetwork)
                {
                    return it.ToString();
                }
            }
            return "";
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            ShouldPaint = false;
            int length = 0 , width = 0 ;
            using (Graphics graphics = CreateGraphics())
            {
                if (radioButton3.Checked) //畫直線
                {
                    int size = (int)numericUpDown1.Value;
                    Pen linepen = new Pen(Brushes.Black);
                    linepen.Color = mycolor;
                    linepen.Width = size;
                    graphics.DrawLine(linepen, P.X, P.Y, e.X, e.Y);
                    tool = 3;

                }
                else if (radioButton6.Checked) //畫方形填滿
                {
                    Point start = new Point();
                    if (P.X < e.X) start.X = P.X;
                    else start.X = e.X;
                    if (P.Y < e.Y) start.Y = P.Y;
                    else start.Y = e.Y;
                    length = Math.Abs(e.X - P.X);
                    width = Math.Abs(e.Y - P.Y);
                    graphics.FillRectangle(new SolidBrush(mycolor), start.X, start.Y, length, width);
                    P.X = start.X; 
                    P.Y = start.Y;
                    tool = 6;

                }
                else if (radioButton5.Checked) //畫圓形
                {
                    int size = (int)numericUpDown1.Value;
                    Pen linepen = new Pen(mycolor);
                    linepen.Color = mycolor;
                    linepen.Width = size;
                    length = e.X - P.X;
                    width = e.Y - P.Y;
                    graphics.DrawEllipse(linepen, P.X, P.Y, length, width);
                    tool = 5;
                }
                else if (radioButton7.Checked) //畫圓形填滿
                {
                    int size = (int)numericUpDown1.Value;
                    Pen linepen = new Pen(mycolor);
                    linepen.Color = mycolor;
                    linepen.Width = size;
                    length = e.X - P.X;
                    width = e.Y - P.Y;
                    graphics.FillEllipse(new SolidBrush(mycolor), P.X, P.Y, length, width);
                    tool = 7;

                }
                else if (radioButton4.Checked) //畫方形
                {
                    Point start = new Point();
                    if (P.X < e.X) start.X = P.X;
                    else start.X = e.X;
                    if (P.Y < e.Y) start.Y = P.Y;
                    else start.Y = e.Y;
                    int size = (int)numericUpDown1.Value;
                    Pen linepen = new Pen(mycolor);
                    linepen.Color = mycolor;
                    linepen.Width = size;
                    length = Math.Abs(e.X - P.X);
                    width = Math.Abs(e.Y - P.Y);
                    graphics.DrawRectangle(linepen, start.X, start.Y, length, width);
                    P.X = start.X;
                    P.Y = start.Y;
                    tool = 4;
                }
                if(radioButton3.Checked || radioButton6.Checked || radioButton5.Checked || radioButton4.Checked || radioButton7.Checked)
                {
                    string IP = textBox1.Text;
                    int Port = int.Parse(textBox2.Text);
                    string message = tool + "_" + ColorTranslator.ToHtml(mycolor) + "_" + numericUpDown1.Value.ToString() + "_" + P.X + "," + P.Y + "|" + e.X + "," + e.Y + "_" + length + "_" + width;
                    byte[] B = Encoding.UTF8.GetBytes(message);
                    UdpClient S = new UdpClient();
                    S.Send(B, B.Length, IP, Port);
                    listBox1.Items.Add(("<Send to>") + textBox1.Text + ":" + textBox2.Text + ":" + message);
                    S.Close();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(colorDialog1.ShowDialog() == DialogResult.OK)
            {
                mycolor = colorDialog1.Color;
            }
            else
            {
                MessageBox.Show("取消選擇顏色!");
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (ShouldPaint)
            {
                using (Graphics graphics = CreateGraphics())
                {
                    if (radioButton1.Checked) //畫筆
                    {
                        int size = (int)numericUpDown1.Value;
                        graphics.FillEllipse(new SolidBrush(mycolor), e.X, e.Y, size, size);
                        tool = 1;
                    }
                    else if (radioButton2.Checked)
                    {
                        int size = (int)numericUpDown1.Value;
                        graphics.FillEllipse(new SolidBrush(this.BackColor), e.X, e.Y, size, size);
                        tool = 2;
                    }
                    if (radioButton1.Checked|| radioButton2.Checked)
                    {
                        string IP = textBox1.Text;
                        int Port = int.Parse(textBox2.Text);
                        string message = tool + "_" + ColorTranslator.ToHtml(mycolor) + "_" + numericUpDown1.Value.ToString() + "_" + e.X + "," + e.Y;
                        byte[] B = Encoding.Default.GetBytes(message);
                        UdpClient S = new UdpClient();
                        S.Send(B, B.Length, IP, Port);
                        listBox1.Items.Add(("<Send to>") + textBox1.Text + ":" + textBox2.Text + ":" + message);
                        S.Close();
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            Th = new Thread(Listen);
            Th.Start();
            connect = true;
            button2.Enabled = false;
        }

        private void Listen()
        {
            int Port = int.Parse(textBox3.Text);
            U = new UdpClient(Port);
            IPEndPoint EP = new IPEndPoint(IPAddress.Parse(MyIP()), Port);
            while (true)
            {
                byte[] B = U.Receive(ref EP);
                string DM = Encoding.Default.GetString(B);
                listBox1.Items.Add("<Receivefrom>" + EP.Address.ToString() + ":" + EP.Port.ToString() + ":" + DM);
                string[] OP = DM.Split('_');
                int yourtool= int.Parse(OP[0]);
                Color yourcolor = ColorTranslator.FromHtml(OP[1]);
                int yoursize = int.Parse(OP[2]);
                string[] coodinate = OP[3].Split('|');
                if (yourtool == 1 || yourtool == 2)
                {
                    string[] xy = coodinate[0].Split(',');
                    YSP.X = int.Parse(xy[0]);
                    YSP.Y = int.Parse(xy[1]);
                    using (Graphics graphics = CreateGraphics())
                    {
                        if (yourtool == 1)
                        {
                            graphics.FillEllipse(new SolidBrush(yourcolor), YSP.X, YSP.Y, yoursize, yoursize);
                        }
                        else if (yourtool == 2)
                        {
                            graphics.FillEllipse(new SolidBrush(this.BackColor), YSP.X, YSP.Y, yoursize, yoursize);
                            tool = 2;
                        }
                    }
                }
                else
                {
                    string[] sxy = coodinate[0].Split(',');
                    YSP.X = int.Parse(sxy[0]);
                    YSP.Y = int.Parse(sxy[1]);
                    string[] exy = coodinate[1].Split(',');
                    YEP.X = int.Parse(exy[0]);
                    YEP.Y = int.Parse(exy[1]);
                    int ylen = int.Parse(OP[4]);
                    int ywid = int.Parse(OP[5]);
                    using (Graphics graphics = CreateGraphics())
                    {
                        if (yourtool == 3)
                        {
                            Pen linepen = new Pen(Brushes.Gold);
                            linepen.Color = yourcolor;
                            linepen.Width = yoursize;
                            graphics.DrawLine(linepen, YSP.X, YSP.Y, YEP.X, YEP.Y);
                            tool = 3;
                        }
                        else if (yourtool == 6)
                        {
                            graphics.FillRectangle(new SolidBrush(yourcolor), YSP.X, YSP.Y, ylen, ywid);
                        }
                        else if (yourtool == 5)
                        {
                            Pen linepen = new Pen(yourcolor);
                            linepen.Color = yourcolor;
                            linepen.Width = yoursize;
                            graphics.DrawEllipse(linepen, YSP.X, YSP.Y, ylen, ywid);
                            tool = 5;
                        }
                        else if (yourtool == 4)
                        {
                            Pen linepen = new Pen(yourcolor);
                            linepen.Color = yourcolor;
                            linepen.Width = yoursize;
                            graphics.DrawRectangle(linepen, YSP.X, YSP.Y, ylen, ywid);
                            tool = 4;
                        }
                        else if(yourtool == 7)
                        {
                            graphics.FillEllipse(new SolidBrush(yourcolor), YSP.X, YSP.Y, ylen, ywid);
                        }
                    }
                }

            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (connect)
                {
                    Th.Abort();
                    U.Close();
                    MessageBox.Show("關閉監聽執行緒與監視器");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text += MyIP();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    } 
}
