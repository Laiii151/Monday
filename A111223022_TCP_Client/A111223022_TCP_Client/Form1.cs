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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace A111223022_TCP_Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Socket T;       //通訊物件
        Thread Th;      //網路監聽執行緒
        string User;

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (button1.Enabled == false)
            {
                Send("9" + User);
                T.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;                        //忽略跨執行緒錯誤
            string IP = textBox1.Text;                                      //伺服器IP
            int Port = int.Parse(textBox3.Text);                            //伺服器Port
            IPEndPoint EP = new IPEndPoint(IPAddress.Parse(IP), Port);      //伺服器的連線端點資訊
                                                                            //建立可以雙向通訊的TCP連線
            T = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            User = textBox2.Text;      //使用者名稱
            if (User != "")
            {
                try
                {
                    T.Connect(EP);
                    Th = new Thread(Listen);
                    Th.IsBackground = true;
                    Th.Start();
                    listBox2.Items.Add("已連線伺服器!");
                    Send("0" + User);
                }
                catch (Exception)
                {
                    listBox2.Items.Add("無法連上伺服器!");     //連上失敗時顯示訊息
                    return;
                }
                button1.Enabled = false;    //讓連線按鍵失敗，避免重複連線
                button2.Enabled = true;     //如連線成功可以開始發送訊息
                button3.Enabled = true;
            }
            else
            {
                //顯示警告訊息
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox4.Text == "") return;     //未輸入訊息不傳送資料
            if (listBox1.SelectedIndex >= 0)    //為選取傳送對象(廣播)，命令碼:1
                                                //有選取傳送對象(私密訊息)，命令碼:2
            {
                Send("2" + "來自" + User + ":" + textBox4.Text + "|" + listBox1.SelectedItem);
                listBox2.Items.Add("告訴" + listBox1.SelectedItem + ":" + textBox4.Text);
            }
            else
            {
                MessageBox.Show("請選取傳送的對象!");
            }
            textBox4.Text = "";      //清除發言框
        }
        //傳送訊息給Server
        private void Send(string Str)
        {
            byte[] B = Encoding.Default.GetBytes(Str);      //翻譯字串Str為Byte陣列B
            T.Send(B, 0, B.Length, SocketFlags.None);       //使用連線物件傳送資料
        }
        //監聽Server訊息
        private void Listen()
        {
            EndPoint ServerEP = (EndPoint)T.RemoteEndPoint; //Server 的 EndPoint
            byte[] B = new byte[1023];                      //接收用的 Byte 陣列
            int inLin = 0;                                  //接收的位元組數目
            string Msg;                                     //接收到的完整訊息
            string St;                                      //命令碼
            string Str;                                     //訊息內容(不含命令碼)
            while (true)                                    //無限次監聽迴圈
            {
                try
                {
                    inLin = T.ReceiveFrom(B, ref ServerEP); //收聽資訊並取得位元組數
                }
                catch (Exception)                           //產生錯誤時
                {
                    T.Close();                              //關閉通訊器
                    listBox1.Items.Clear();
                    MessageBox.Show("伺服器斷線了!");
                    button1.Enabled = true;                 //連線按鍵恢復可用
                    Th.Abort();                             //刪除執行緒
                }
                Msg = Encoding.Default.GetString(B, 0, inLin);
                St = Msg.Substring(0, 1);
                Str = Msg.Substring(1);
                switch (St)
                {
                    case "L":
                        listBox1.Items.Clear();             //接收線上名單
                        string[] M = Str.Split(',');        //清除名單
                        for (int i = 0; i < M.Length; i++)  //拆解名單成陣列
                        {
                            listBox1.Items.Add(M[i]);
                        }
                        
                        break;
                    case "1":                               //接收廣播訊息
                        listBox2.Items.Add("(公開)" + Str);   //顯示訊息並換行
                        break;
                    case "2":                               //接收私密訊息
                        listBox2.Items.Add("(私密)" + Str);   //顯示訊息並換行
                        break;
                    case "3":
                        textBox2.Text = "";
                        listBox2.Items.Add(Str + "此名稱已有人使用，請重新建立名稱");
                        button1.Enabled = true;    
                        button2.Enabled = false;     
                        button3.Enabled = false;
                        T.Close();
                        Th.Abort();
                        break;
                }
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (textBox4.Text == "") return;     //未輸入訊息不傳送資料
            Send("1" + User + "公告:" + textBox4.Text);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
