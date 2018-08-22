using RobotFinderLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RobotFinder
{
    public partial class MainForm : Form
    {
        UDPPortScan portScan = new UDPPortScan();

        public MainForm()
        {
            InitializeComponent();

            portScan.StartEvent += portScan_StartEvent;
            portScan.StopEvent += portScan_StopEvent;
            portScan.RobotResponseEvent += portScan_RobotResponseEvent;
        }

        void portScan_RobotResponseEvent(object sender, ProgressEventArgs args)
        {
            System.Console.WriteLine("IP:" + args.Remote + ",Txt:" + args.ResponseText);
        }

        void portScan_StopEvent(object sender, EventArgs args)
        {
            
        }

        void portScan_StartEvent(object sender, EventArgs args)
        {
            
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            portScan.UdpClient.OpenListener();

            int portStart = 50000;
            for (int k = 0; k < 10; k++)
            {
                TestForm tf = new TestForm();

                portStart += 1;
                tf.Listener.UdpClient.UdpFreePortMin = portStart;

                portStart += 10;
                tf.Listener.UdpClient.UdpFreePortMax = portStart;

                tf.Show();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            portScan.UdpClient.CloseListener();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            portScan.InitQueues("192.168.8.111", "192.168.8.112", 50000, 50100);
            portScan.StartScan();
        }
    }
}