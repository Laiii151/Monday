using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Security.Authentication.ExtendedProtection;

namespace A111223022_Bingo_Ex3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        UdpClient U;
        Thread Th;
        bool connect = false;

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            Th = new Thread(Listen);
            Th.Start();
            connect = true;
            button1.Enabled = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (connect)
                {
                    Th.Abort();
                    U.Close();
                }
                MessageBox.Show("謝謝使用本系統");
            }
            catch
            {
                //
            }
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
        Color original;
        private void Form1_Load(object sender, EventArgs e)
        {
            Random rnd = new Random();
            int[] mark = new int[25];
            int num;
            
            original = B1.BackColor;
            this.Text += MyIP();
            for (int i = 0; i < 25; i++)
            {
                this.Controls["B" + i.ToString()].Click += new EventHandler(this.B0_Click);
            }
            for (int i = 0; i < 25; i++)
            {
                mark[i] = 0;
            }
            for (int i = 0; i < 25; i++)
            {
                do
                {
                    num = rnd.Next(0, 25);
                } while (mark[num] != 0);
                mark[num] = 1;
                this.Controls["B" + i.ToString()].Text = (num + 1).ToString();
            }
        }

        string my;
        private void Listen()
        {
            int Port = int.Parse(textBox1.Text);
            U = new UdpClient(Port);
            IPEndPoint EP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Port);
            while (true)
            {
                byte[] B = U.Receive(ref EP);
                string D = Encoding.Default.GetString(B);
                string[] A = D.Split(':');
                if (A[1] != "-1")
                {
                    for (int i = 0; i < 25; i++)
                    {
                        if (this.Controls["B" + i.ToString()].Text == A[1])
                        {
                            this.Controls["B" + i.ToString()].Tag = "0";
                            this.Controls["B" + i.ToString()].Enabled = false;
                            this.Controls["B" + i.ToString()].BackColor = Color.Red;
                            break;
                        }
                    }
                    my = "";
                    for (int i = 0; i < 25; i++)
                    {
                        my += this.Controls["B" + i.ToString()].Tag;
                    }
                    int Port2 = int.Parse(textBox3.Text);
                    UdpClient S = new UdpClient(textBox2.Text, Port2);
                    byte[] K = Encoding.Default.GetBytes(my + ":" + "-1");
                    S.Send(K, K.Length);
                    S.Close();
                }
                label4.Text = "";
                bool iwin = chk(my);
                bool youwin = chk(A[0]);
                if (!iwin && !youwin)
                {
                    if (A[1] != "-1")
                    {
                        T = true;
                        label4.Text = "輪我下";
                    }
                    else
                    {
                        T = false;
                        label4.Text = "輪對手下";
                    }
                }
                else
                {
                    T = false;
                    label4.Text = "已分出勝負";
                    if (iwin && !youwin)
                    {
                        label4.Text += "我贏了";
                    }
                    else if (!iwin && youwin)
                    {
                        label4.Text += "你贏了";
                    }
                    else
                    {
                        label4.Text += "雙方平手";
                    }
                }
            }
        }

        private void button4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (connect)
                {
                    Th.Abort();
                    U.Close();
                    MessageBox.Show("謝謝使用本系統");
                }
            }
            catch
            {
                //
            }
        }
        
        private bool chk(string A)
        {
            int lines = 0;
            char[] C = A.ToCharArray();
            if (C[0] == '0' && C[1] == '0' && C[2] == '0' && C[3] == '0' && C[4] == '0') lines++;
            if (C[5] == '0' && C[6] == '0' && C[7] == '0' && C[8] == '0' && C[9] == '0') lines++;
            if (C[10] == '0' && C[11] == '0' && C[12] == '0' && C[13] == '0' && C[14] == '0') lines++;
            if (C[15] == '0' && C[16] == '0' && C[17] == '0' && C[18] == '0' && C[19] == '0') lines++;
            if (C[20] == '0' && C[21] == '0' && C[22] == '0' && C[23] == '0' && C[24] == '0') lines++;
            if (C[0] == '0' && C[5] == '0' && C[10] == '0' && C[15] == '0' && C[20] == '0') lines++;
            if (C[1] == '0' && C[6] == '0' && C[11] == '0' && C[16] == '0' && C[21] == '0') lines++;
            if (C[2] == '0' && C[7] == '0' && C[12] == '0' && C[17] == '0' && C[22] == '0') lines++;
            if (C[3] == '0' && C[8] == '0' && C[13] == '0' && C[18] == '0' && C[23] == '0') lines++;
            if (C[4] == '0' && C[9] == '0' && C[14] == '0' && C[19] == '0' && C[24] == '0') lines++;
            if (C[0] == '0' && C[6] == '0' && C[12] == '0' && C[18] == '0' && C[24] == '0') lines++;
            if (C[4] == '0' && C[6] == '0' && C[12] == '0' && C[16] == '0' && C[20] == '0') lines++;
            if (lines >= int.Parse(comboBox1.Text)) return true;
            else return false;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            int[] mark = new int[25];
            int num;
            for (int i = 0; i <= 24; i++)
            {
                Button D = (Button)this.Controls["B" + i.ToString()];
                D.Enabled = true;
                D.BackColor = original;
                D.Tag = "_";
                T = true;
            }
            for(int i = 0; i < 25; i++) mark[i] = 0;
            for (int i = 0; i < 25; i++)
            {
                do
                {
                    num = rnd.Next(0, 25);
                }while(mark[num] != 0);
                mark[num] = 1;
                this.Controls["B" + i.ToString()].Text = (num + 1).ToString();  
            }
            label4.Text = "輪我下";
        }
        bool T = true;

        private void B0_Click(object sender, EventArgs e)
        {
            if (T == false) return;
            Button B = (Button)sender;
            if (B.Tag.ToString() != "_") return;

            B.Tag = "0";
            B.Enabled = false;
            B.BackColor = Color.Red;
            my = "";
            for (int i = 0; i < 25; i++)
            {
                my += this.Controls["B" + i.ToString()].Tag;
            }
            label4.Text = "";
            int Port = int.Parse(textBox3.Text);
            UdpClient S = new UdpClient();
            byte[] K = Encoding.Default.GetBytes(my + ":" + B.Text);
            S.Send(K, K.Length, textBox2.Text, Port);
            S.Close();
            T = false;
        }

        private void B1_Click(object sender, EventArgs e)
        {
            B0_Click(sender, e);
        }

        private void B6_Click(object sender, EventArgs e)
        {
            B0_Click(sender, e);
        }

        private void B1_Click_1(object sender, EventArgs e)
        {
            B0_Click(sender, e);
        }

        private void B2_Click(object sender, EventArgs e)
        {
            B0_Click(sender, e);
        }
    }
}
