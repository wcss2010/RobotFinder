using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFinderLibrary
{
    /// <summary>
    /// 端口扫描后台监听器
    /// </summary>
    public class UDPPortScanListener
    {
        private UDPListener _udpClient = new UDPListener();
        /// <summary>
        /// UDP客户端
        /// </summary>
        public UDPListener UdpClient
        {
            get { return _udpClient; }
        }

        public UDPPortScanListener()
        {
            UdpClient.UDPReceivedEvent += UdpClient_UDPReceivedEvent;
        }

        /// <summary>
        /// 当收到数据时返回的文本
        /// </summary>
        public string ResponseText { get; set; }

        void UdpClient_UDPReceivedEvent(object sender, ReceivedEventArgs args)
        {
            string response = CommandConst.ROBOT_ONLINE + ResponseText;
            byte[] responseBytes = Encoding.UTF8.GetBytes(response);
            UdpClient.UdpClient.Send(responseBytes, responseBytes.Length, args.Remote);
        }
    }
}