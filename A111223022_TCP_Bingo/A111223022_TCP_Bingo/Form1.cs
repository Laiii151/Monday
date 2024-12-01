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

namespace A111223022_TCP_Bingo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Socket T;
        Thread Th;
        string User;

        private void label2_Click(object sender, EventArgs e)
        {

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

            original = B0.BackColor;
            this.Text += MyIP();
            for (int i = 0; i < 25; i++)
            {
                this.Controls["B" + i.ToString()].Click += new System.EventHandler(this.B0_Click);
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
            button25.Enabled = false;
            button26.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            User = textBox3.Text;
            string IP = textBox1.Text;
            int Port = int.Parse(textBox2.Text);
            try
            {
                IPEndPoint EP = new IPEndPoint(IPAddress.Parse(IP), Port);
                T = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                T.Connect(EP);
                Th = new Thread(Listen);
                Th.IsBackground = true;
                Th.Start();
                textBox4.Text = "已連線伺服器!" + "\r\n"; Send("0" + User);
                button1.Enabled = false; 
                button26.Enabled = true;
            }
            catch
            {
                textBox4.Text = "無法連線伺服器!" + "\r\n";
            }
        }

        private void button26_Click(object sender, EventArgs e)
        {
            if(listBox1.SelectedIndex != -1)
            {
                if(listBox1.SelectedItem.ToString() != User)
                {
                    Send("I" + User + "," + comboBox1.Text + "|" + listBox1.SelectedItem);
                }
                else
                {
                    MessageBox.Show("不可以邀請自己!");
                }
            }
            else
            {
                MessageBox.Show("沒有選取邀請的對象!");
            }
        }
        string my;
        bool Turn = true;

        private void button25_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                Send("5" + comboBox1.Text + "|" + listBox1.SelectedItem);
            }
        }
        private void Send(string Str)
        {
            byte[] B = Encoding.Default.GetBytes(Str);
            T.Send(B, 0, B.Length, SocketFlags.None);
        }
        private void Listen()
        {
            EndPoint ServerEP = (EndPoint)T.RemoteEndPoint;
            byte[] B = new byte[1023];
            int inLen = 0;
            string Msg;
            string St;
            string Str;
            while (true)
            {
                try
                {
                    inLen = T.ReceiveFrom(B, ref ServerEP);
                }
                catch (Exception)
                {
                    T.Close();
                    listBox1.Items.Clear();
                    MessageBox.Show("伺服器斷線了!");
                    button1.Enabled = true;
                    Th.Abort();
                }
                Msg = Encoding.Default.GetString(B, 0 , inLen);
                St = Msg.Substring(0, 1 );
                Str = Msg.Substring(1);
                switch (St)
                {
                    case "L":
                        listBox1.Items.Clear ();
                        string[] M = Str.Split(',');
                        for (int i = 0; i < M.Length; i++) listBox1.Items.Add (M[i]);
                        break;
                    case "5":
                        DialogResult result = MessageBox.Show("是否重玩賓果遊戲(連" + Str+ "排)?","重玩訊息",MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {
                            comboBox1.Text = Str;
                            comboBox1.Enabled = false;
                            Send("P" + "Y" + "|" + listBox1.SelectedItem);
                            Random rnd = new Random();
                            int[] mark = new int[25];
                            int num;

                            for (int i = 0; i <= 24 ; i++)
                            {
                                Button D = (Button)this.Controls["B" + i.ToString()];

                                D.Enabled = true;
                                D.BackColor = original;
                                D.Tag = "_";
                                Turn = true;
                            }
                            for (int i = 0; i < 25; i++) mark[i] = 0;
                            for (int i = 0; i < 25 ; i++)
                            {
                                do
                                {
                                    num = rnd.Next(0,25);
                                }while (mark[num] != 0);
                                mark[num] = 1;
                                this.Controls["B" + i.ToString()].Text = (num + 1).ToString();
                            }
                            textBox4.Text = "輪到我下";
                        }
                        else
                        {
                            Send("P" + "N" + "|" + listBox1.SelectedItem);
                        }
                        break;
                    case "P":
                        if (Str == "Y")
                        {
                            MessageBox.Show(listBox1.SelectedItem.ToString() + "接受你的邀請，可以開始重玩遊戲");
                            Random rnd = new Random();
                            int[] mark = new int[25];
                            int num;

                            for (int i = 0; i <= 24; i++)
                            {
                                Button D = (Button)this.Controls["B" + i.ToString()];

                                D.Enabled = true;
                                D.BackColor = original;
                                D.Tag = "_";
                                Turn = true;
                            }
                            for (int i = 0; i < 25; i++) mark[i] = 0;
                            for (int i = 0; i < 25; i++)
                            {
                                do
                                {
                                    num = rnd.Next(0, 25);
                                } while (mark[num] != 0);
                                mark[num] = 1;
                                this.Controls["B" + i.ToString()].Text = (num + 1).ToString();
                            }
                            textBox4.Text = "輪到我下";
                            comboBox1.Enabled = false;
                            button26.Enabled = false;
                            button25.Enabled = true;
                        }
                        else
                        {
                            MessageBox.Show("抱歉"+listBox1.SelectedItem.ToString() + "拒絕你的邀請");
                        }
                        break;
                    case "6":
                        string[] A = Str.Split(':');
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
                            byte[] K = Encoding.Default.GetBytes(my + ":" + "-1");
                            Send("6" + my + ":" + "-1" + "|" + listBox1.SelectedItem);
                         }
                        textBox4.Text = "";
                        bool iwin = chk(my);
                        bool youwin = chk(A[0]);
                        if (!iwin && !youwin)
                        {
                            if (A[1] != "-1")
                            {
                                Turn = true;
                                textBox4.Text = "輪到我下";
                            }
                            else
                            {
                                Turn = false;
                                textBox4.Text = "輪到對手下";
                            }
                        }
                        else
                        {
                            comboBox1.Enabled = true;
                            Turn = false;
                            textBox4.Text = "已分出勝負";
                            if (iwin && !youwin)
                            {
                                textBox4.Text += "我贏了";
                            }
                            else if (!iwin && youwin)
                            {
                                textBox4.Text += "你贏了";
                            }
                            else
                            {
                                textBox4.Text += "雙方平手";
                            }
                        }
                        break;
                    case "D":
                        textBox4.Text = Str;
                        button1.Enabled = true;
                        button26.Enabled = false;
                        T.Close();
                        Th.Abort();
                        break;
                    case "I":
                        string[] F = Str.Split(',');
                        DialogResult res = MessageBox.Show(F[0] + "邀請玩賓果遊戲(連" + F[1] + "排)，是否接受?", "邀請訊息", MessageBoxButtons.YesNo);
                        if (res == DialogResult.Yes) 
                        { 
                            int i = listBox1.Items.IndexOf(F[0]);
                            listBox1.SetSelected(i, true);
                            listBox1.Enabled = false;
                            comboBox1.Text = F[1];
                            comboBox1.Enabled = false;
                            button26.Enabled = false;
                            button25.Enabled = true;
                            Send("R" + "Y" + "|" + F[0]);
                        }
                        else
                        {
                            Send("R" + "N" + "|" + F[0]);
                        }
                        break;
                    case "R":
                        if(Str == "Y")
                        {
                            MessageBox.Show(listBox1.SelectedItem.ToString() + "接受你的邀請，可以開始遊戲");
                            listBox1.Enabled = false;
                            comboBox1.Enabled = false;
                            button26.Enabled = false;
                            button25.Enabled = true;
                        }
                        else
                        {
                            MessageBox.Show("抱歉" + listBox1.SelectedItem.ToString() + "拒絕你的邀請");
                        }
                        break;
                }

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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(button1.Enabled == false)
            {
                Send("9" + User);
                T.Close();
            }
        }

        private void B0_Click(object sender, EventArgs e)
        {
            if (Turn == false) return;
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
            textBox4.Text = "";
            byte[] K = Encoding.Default.GetBytes(my + ":" + B.Text);
            Send("6" + my + ":" + B.Text + "|" + listBox1.SelectedItem);
            Turn = false;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
