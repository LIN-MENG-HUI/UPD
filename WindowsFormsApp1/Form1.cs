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
using Microsoft.VisualBasic.PowerPacks;


namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        UdpClient U;
        Thread Th;
        ShapeContainer C;
        ShapeContainer D;
        Point stP;
        string p;

        public Form1()
        {
            InitializeComponent();
        }
        private void Listen()
        {
            int Port = int.Parse(textBox3.Text);
            U = new UdpClient(Port);
            IPEndPoint EP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Port);
            while (true)
            {
                byte[] B = U.Receive(ref EP); //訊息到達時讀取到B陣列
                string A = Encoding.Default.GetString(B); //翻譯C陣列到字串A
                string[] Z = A.Split('_');
                string[] Q = Z[1].Split('/');//切割座標點資訊
                Point[] R = new Point[Q.Length];//宣告座標陣列
                for (int i = 0; i < Q.Length; i++)
                {
                    string[] K = Q[i].Split(',');//切割X、Y座標
                    R[i].X = int.Parse(K[0]);//定義第i個x座標
                    R[i].Y = int.Parse(K[1]);//定義第i個y座標
                }
                for(int i = 0; i < Q.Length - 1; i++)
                {
                    LineShape L = new LineShape(); //建立線段物件
                    L.StartPoint = R[i];    //線段起點
                    L.EndPoint = R[i + 1];  //線段終點
                    switch (Z[0])
                    {
                        case "1":
                            L.BorderColor = Color.Red;
                            break;
                        case "2":
                            L.BorderColor = Color.Green;
                            break;
                        case "3":
                            L.BorderColor = Color.Blue;
                            break;
                        case "4":
                            L.BorderColor = Color.Black;
                            break;
                    }
                    L.Parent = D;   //線段L加入畫布D
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Th.Abort();//關閉監聽執行續
                U.Close();//關閉程式碼
            }
            catch
            {

            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            Th = new Thread(Listen);
            Th.Start();
            button1.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            C = new ShapeContainer(); //建立畫布
            this.Controls.Add(C);   //加入畫布C到Form1
            D = new ShapeContainer();
            this.Controls.Add(D);
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            stP = e.Location; //起點
            p = stP.X.ToString() + "," + stP.Y.ToString(); //起點座標紀錄
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                LineShape L = new LineShape();
                L.StartPoint = stP;
                L.EndPoint = e.Location;
                L.Parent = C;
                stP = e.Location;
                p += "/" + stP.X.ToString() + "," + stP.Y.ToString();
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                LineShape L = new LineShape();
                L.StartPoint = stP;
                L.EndPoint = e.Location;
                if (radioButton1.Checked) { L.BorderColor = Color.Red; }
                if (radioButton2.Checked) { L.BorderColor = Color.Green; }
                if (radioButton3.Checked) { L.BorderColor = Color.Blue; }
                if (radioButton4.Checked) { L.BorderColor = Color.Black; }
                L.Parent = C;
                stP = e.Location;
                p += "/" + stP.X.ToString() + "," + stP.Y.ToString();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            int Port = int.Parse(textBox2.Text);
            UdpClient S = new UdpClient(textBox1.Text, Port);
           
            if (radioButton1.Checked) { p = "1_" + p; }
            if (radioButton2.Checked) { p = "2_" + p; }
            if (radioButton3.Checked) { p = "3_" + p; }
            if (radioButton4.Checked) { p = "4_" + p; }
            byte[] B = Encoding.Default.GetBytes(p);
            S.Send(B, B.Length);
            S.Close();
        }
    }
}
