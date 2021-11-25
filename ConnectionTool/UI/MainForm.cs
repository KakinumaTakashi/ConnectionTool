using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConnectionTool.UI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        public class UdpState
        {
            public UdpClient client;
            public IPEndPoint localEndPoint;
        }

        private bool ReceiveFlag = false;

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.ReceiveFlag)
            {
                this.ReceiveFlag = false;
                return;
            }

            Task.Run(() =>
            {
                IPEndPoint _localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);
                UdpClient _client = new UdpClient(_localEndPoint);

                UdpState _state = new UdpState()
                {
                    client = _client,
                    localEndPoint = _localEndPoint
                };

                _client.BeginReceive(new AsyncCallback(ReceiveCallback), _state);

                this.ReceiveFlag = true;
                while (this.ReceiveFlag)
                {
                    Thread.Sleep(100);
                }

                _client.Close();
                Debug.WriteLine("Receive thread end");
            });

            Debug.WriteLine("Receive thread start");
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            UdpClient _client = ((UdpState)ar.AsyncState).client;
            IPEndPoint _localEndPoint = ((UdpState)ar.AsyncState).localEndPoint;

            byte[] _data = _client.EndReceive(ar, ref _localEndPoint);
            string _dataText = Encoding.UTF8.GetString(_data);

            Debug.WriteLine($"Receive Data : {_dataText}");

            this.ReceiveFlag = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UdpClient _client = new UdpClient();

            byte[] _data = Encoding.UTF8.GetBytes("Test Test!!");
            _client.Send(_data, _data.Length, "localhost", 8888);

            _client.Close();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.ReceiveFlag = false;
        }
    }
}
