using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RobotFinder
{
    public partial class TestForm : Form
    {
        private UdpClient _udpClient = null;
        /// <summary>
        /// UDPClient
        /// </summary>
        public UdpClient UdpClient
        {
            get { return _udpClient; }
        }

        private int _udpFreePortMin = 60000;
        /// <summary>
        /// UDP Listen Port Min
        /// </summary>
        public int UdpFreePortMin
        {
            get { return _udpFreePortMin; }
            set { _udpFreePortMin = value; }
        }

        private int _udpFreePortMax = 60600;
        /// <summary>
        /// UDP Listen Port Max
        /// </summary>
        public int UdpFreePortMax
        {
            get { return _udpFreePortMax; }
            set { _udpFreePortMax = value; }
        }

        /// <summary>
        /// Current UDP Port
        /// </summary>
        public int LocalUdpPort { get; set; }

        private BackgroundWorker udpWorker = null;

        public TestForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            OpenListener();
        }

        private void OpenListener()
        {
            for (int portVal = _udpFreePortMin; portVal <= _udpFreePortMax; portVal++)
            {
                try
                {
                    _udpClient = new UdpClient(portVal);
                    LocalUdpPort = portVal;
                    break;
                }
                catch (Exception ex)
                {
                    _udpClient = null;
                }
            }

            if (_udpClient != null)
            {
                udpWorker = new BackgroundWorker();
                udpWorker.WorkerSupportsCancellation = true;
                udpWorker.DoWork += udpWorker_DoWork;
                udpWorker.RunWorkerAsync();
            }
        }

        void udpWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!((BackgroundWorker)sender).CancellationPending)
            {
                try
                {
                    IPEndPoint remotePoint = new IPEndPoint(IPAddress.Any,LocalUdpPort);
                    byte[] content = UdpClient.Receive(ref remotePoint);


                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.ToString());
                }


                try
                {
                    Thread.Sleep(5);
                }
                catch (Exception ex) { }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            CloseListener();
        }

        private void CloseListener()
        {
            if (udpWorker != null)
            {
                udpWorker.CancelAsync();
                udpWorker = null;
            }

            if (udpClient != null)
            {
                try
                {
                    udpClient.Close();
                }
                catch (Exception ex) { }
                udpClient = null;
            }
        }
    }
}