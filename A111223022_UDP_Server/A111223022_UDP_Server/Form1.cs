using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Diagnostics.Eventing.Reader;

namespace A111223022_UDP_Server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Thread Th;

        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            Th = new Thread(Listen);
            Th.IsBackground = true;
            Th.Start();
            this.Text += MyIP();
        }
        private void Listen()
        {
            UdpClient U = new UdpClient(2019);
            while (true)
            {
                IPEndPoint EP = new IPEndPoint(IPAddress.Any, 2019);
                byte[] B = U.Receive(ref EP);
                string A = Encoding.Default.GetString(B);
                string M = "Unknown Command";
                if (A == "Times?") M = DateTime.Now.ToString();
                else if (A == "School?") M = "Shih Hsin University";
                else if (A == "Name?") M = "I-TING";
                else if (A == "Phone number?") M = "0909123456";
                else M = "I don't know the answer of your question!";
                B = Encoding.Default.GetBytes(M);
                U.Send(B, B.Length, EP);
                listBox1.Items.Add(A + ":" + M);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            MessageBox.Show("關閉伺服器");
            Th.Abort();

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
