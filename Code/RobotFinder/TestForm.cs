using RobotFinderLibrary;
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
        UDPPortScanListener _listener = new UDPPortScanListener();
        public UDPPortScanListener Listener
        {
            get { return _listener; }
        }
        
        public TestForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _listener.UdpClient.OpenListener();
            _listener.ResponseText = "哪个剧比较好看?";

            this.Text = "本地端口：" + _listener.UdpClient.LocalUdpPort;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            _listener.UdpClient.CloseListener();
        }
    }
}