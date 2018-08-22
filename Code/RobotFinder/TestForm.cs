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
        UDPListener listener = new UDPListener();


        public TestForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            listener.UDPReceivedEvent += listener_UDPReceivedEvent;
            listener.OpenListener();
        }

        void listener_UDPReceivedEvent(object sender, ReceivedEventArgs args)
        {
            byte[] bytes = Encoding.UTF8.GetBytes("hello");
            listener.UdpClient.Send(bytes, bytes.Length, args.Remote);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            listener.CloseListener();
        }
    }
}