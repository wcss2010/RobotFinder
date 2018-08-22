using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RobotFinderLibrary
{
    public delegate void UDPReceivedEventDelegate(object sender, ReceivedEventArgs args);

    public class ReceivedEventArgs : EventArgs
    {
        public byte[] Content { get; set; }

        public IPEndPoint Remote { get; set; }
    }

    /// <summary>
    /// UDPListener
    /// </summary>
    public class UDPListener
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

        private int _udpFreePortMax = 60100;
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

        public event UDPReceivedEventDelegate UDPReceivedEvent;

        protected void OnUDPReceivedEvent(IPEndPoint remote, byte[] content)
        {
            if (UDPReceivedEvent != null)
            {
                ReceivedEventArgs rea = new ReceivedEventArgs();
                rea.Remote = remote;
                rea.Content = content;

                UDPReceivedEvent(this, rea);
            }
        }

        private BackgroundWorker udpWorker = null;

        public void OpenListener()
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
            IPEndPoint remotePoint = new IPEndPoint(IPAddress.Any, 0);
            while (!((BackgroundWorker)sender).CancellationPending)
            {
                try
                {
                    //接收消息                        
                    byte[] content = UdpClient.Receive(ref remotePoint);

                    //投递事件
                    OnUDPReceivedEvent(remotePoint, content);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.ToString());
                }


                try
                {
                    Thread.Sleep(2);
                }
                catch (Exception ex) { }
            }
        }

        public void CloseListener()
        {
            if (udpWorker != null)
            {
                udpWorker.CancelAsync();
                udpWorker = null;
            }

            if (_udpClient != null)
            {
                try
                {
                    _udpClient.Close();
                }
                catch (Exception ex) { }
                _udpClient = null;
            }
        }
    }
}