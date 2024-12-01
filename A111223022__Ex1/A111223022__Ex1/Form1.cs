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

namespace A111223022__Ex1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        UdpClient U;
        Thread Th;
        bool startlisten = false;

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text += MyIP();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                MessageBox.Show("關閉程式!");
                if (startlisten)
                {
                    Th.Abort();
                    U.Close();
                }
            }
            catch
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            startlisten = true;
            Control.CheckForIllegalCrossThreadCalls = false;
            Th = new Thread(Listen);
            Th.Start();
            button1.Enabled = false;
        }
        private void Listen()
        {
            int Port  = int.Parse(textBox1.Text);
            U = new UdpClient(Port);
            IPEndPoint EP = new IPEndPoint(IPAddress.Parse(MyIP()),Port);
            while (true)
            {
                byte[] B = U.Receive(ref EP);
                textBox2.Text = EP.Address.ToString() + ":" + Encoding.Default.GetString(B);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string IP = textBox3.Text;
            int Port = int.Parse(textBox4.Text);
            byte[] B = Encoding.Default.GetBytes(textBox5.Text);
            UdpClient S = new UdpClient();
            S.Send(B, B.Length, IP, Port);
            S.Close();
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
    }
}
