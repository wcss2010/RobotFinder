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
        UDPPortScanListener listener = new UDPPortScanListener();
        
        public TestForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            listener.UdpClient.OpenListener();
            listener.ResponseText = "哪个剧比较好看?";

            this.Text = "本地端口：" + listener.UdpClient.LocalUdpPort;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            listener.UdpClient.CloseListener();
        }
    }
}