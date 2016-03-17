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
using System.IO;

namespace TCPKonekcija
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        public StreamReader STR;
        public StreamWriter STW;
        public string recieve;
        public String text_to_send;
        public Form1()
        {
            InitializeComponent();
            IPAddress[] localIP = Dns.GetHostAddresses(Dns.GetHostName());
            foreach(IPAddress address in localIP)
            {
                if(address.AddressFamily == AddressFamily.InterNetwork)
                {
                    textBoxIPs.Text = address.ToString();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)//Start server
        {
            
                TcpListener listener = new TcpListener(IPAddress.Any, int.Parse(textBoxPs.Text));
                listener.Start();
              textBoxStatus.AppendText("Server started!");
                client = listener.AcceptTcpClient();
                STR = new StreamReader(client.GetStream());
                STW = new StreamWriter(client.GetStream());
                STW.AutoFlush = true;
                backgroundWorker1.RunWorkerAsync(); //pocinje slusati podatke
                backgroundWorker2.WorkerSupportsCancellation = true; //omogucava cancelanje threada
                textBoxStatus.AppendText("Client connected!\n");
          

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)  //primanje
        {
            while(client.Connected)
            {
                try
                {
                    recieve = STR.ReadLine();
                    this.textBoxStatus.Invoke(new MethodInvoker(delegate() { textBoxStatus.AppendText("You: " + recieve + "\n"); } ));
                    recieve = "";
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.Message.ToString());
                }
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e) //slanje
        {
            if(client.Connected)
            {
                STW.WriteLine(text_to_send);
                this.textBoxStatus.Invoke(new MethodInvoker(delegate () { textBoxStatus.AppendText("Me: " + text_to_send + "\n"); }));
            }
            else
            {
                MessageBox.Show("Neuspješno slanje");
            }
            backgroundWorker2.CancelAsync();
        }

        private void button3_Click(object sender, EventArgs e) //Connect
        {
            client = new TcpClient();
            IPEndPoint IP_End = new IPEndPoint (IPAddress.Parse(textBoxIPc.Text), int.Parse(textBox5.Text));

            try
            {
                client.Connect(IP_End);
                if (client.Connected)
                {
                    textBoxStatus.AppendText("Connected!\n");
                    STR = new StreamReader(client.GetStream());
                    STW = new StreamWriter(client.GetStream());
                    STW.AutoFlush = true;
                    backgroundWorker1.RunWorkerAsync(); //pocinje slusati podatke
                    backgroundWorker2.WorkerSupportsCancellation = true; //omogucava cancelanje threada
                }
            }
            catch (Exception x)
            {

                MessageBox.Show(x.Message.ToString());
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if(textBox1.Text!="")
            {
                text_to_send = textBox1.Text;
                backgroundWorker2.RunWorkerAsync();
            }
            textBox1.Text = "";
        }
    }
}
