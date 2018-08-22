using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RobotFinderLibrary
{
    /// <summary>
    /// 开始停止事件委托
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void StartAndStopNotifyDelegate(object sender,EventArgs args);

    public delegate void ProgressEventDelegate(object sender,ProgressEventArgs args);

    public delegate void RobotResponseEvenDelegate(object sender, ProgressEventArgs args);

    public class ProgressEventArgs : EventArgs
    {
        public IPEndPoint Remote { get; set; }

        public string ResponseText { get; set; }
    }

    public class UDPPortScan
    {
        System.Collections.Concurrent.ConcurrentQueue<IPEndPoint> _scanQueues = new System.Collections.Concurrent.ConcurrentQueue<IPEndPoint>();
        /// <summary>
        /// 等待扫描地址队列
        /// </summary>
        public System.Collections.Concurrent.ConcurrentQueue<IPEndPoint> ScanQueues
        {
            get { return _scanQueues; }
        }

        System.Collections.Concurrent.ConcurrentDictionary<string, string> _resultDict = new System.Collections.Concurrent.ConcurrentDictionary<string, string>();
        /// <summary>
        /// 结果字典
        /// </summary>
        public System.Collections.Concurrent.ConcurrentDictionary<string, string> ResultDict
        {
            get { return _resultDict; }
        }

        public event StartAndStopNotifyDelegate StartEvent;
        public event StartAndStopNotifyDelegate StopEvent;
        public event ProgressEventDelegate ProgressEvent;
        public event RobotResponseEvenDelegate RobotResponseEvent;

        private UDPListener _udpClient = new UDPListener();
        /// <summary>
        /// UDP客户端
        /// </summary>
        public UDPListener UdpClient
        {
            get { return _udpClient; }
        }

        public UDPPortScan()
        {
            UdpClient.UDPReceivedEvent += UdpClient_UDPReceivedEvent;
        }

        void UdpClient_UDPReceivedEvent(object sender, ReceivedEventArgs args)
        {
            if (args.Content != null)
            {
                string result = Encoding.UTF8.GetString(args.Content);
                if (result != null && result.StartsWith(CommandConst.ROBOT_ONLINE))
                {
                    if (ResultDict.ContainsKey(args.Remote.ToString()))
                    {
                        return;
                    }

                    string txt = result.Replace(CommandConst.ROBOT_ONLINE, string.Empty).Trim();
                    ResultDict.TryAdd(args.Remote.ToString(), txt);

                    if (RobotResponseEvent != null)
                    {
                        ProgressEventArgs pea = new ProgressEventArgs();
                        pea.Remote = args.Remote;
                        pea.ResponseText = txt;
                        RobotResponseEvent(this, pea);
                    }
                }
            }
        }

        /// <summary>
        /// 清理队列
        /// </summary>
        public void ClearQueues()
        {
            _scanQueues = new System.Collections.Concurrent.ConcurrentQueue<IPEndPoint>();
        }

        /// <summary>
        /// 初始化扫描队列(仅支持IPv4,不支持跨网段)
        /// </summary>
        /// <param name="ipStart">IP段开始,0.0.0.1</param>
        /// <param name="ipEnd">IP段结束,0.0.0.255</param>
        /// <param name="portStart">端口开始</param>
        /// <param name="portEnd">端口结束</param>
        public void InitQueues(string ipStart, string ipEnd, int portStart, int portEnd)
        {
            string[] ipStartTeam = ipStart.Split('.');
            string[] ipEndTeam = ipEnd.Split('.');
            if (ipStartTeam != null && ipEndTeam != null && ipStartTeam.Length == ipEndTeam.Length && ipStartTeam.Length >= 4)
            {
                int ipStartIndex = int.Parse(ipStartTeam[3]);
                int ipEndIndex = int.Parse(ipEndTeam[3]);
                int ipCount = ipEndIndex - ipStartIndex;
                if (ipCount >= 1 && portEnd > portStart)
                {
                    //生成IP和端口信息
                    for (int ipNum = ipStartIndex; ipNum <= ipEndIndex; ipNum++)
                    {
                        //IP地址
                        string ipAddress = ipStartTeam[0] + "." + ipStartTeam[1] + "." + ipStartTeam[2] + "." + ipNum;

                        //生成端口
                        for (int portNum = portStart; portNum <= portEnd; portNum++)
                        {
                            try
                            {
                                ScanQueues.Enqueue(new IPEndPoint(IPAddress.Parse(ipAddress), portNum));
                            }
                            catch (Exception ex) { }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 开始扫描
        /// </summary>
        public void StartScan()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object stateObj)
                {
                    //事件开始
                    if (StartEvent != null)
                    {
                        StartEvent(this, new EventArgs());
                    }

                    while (ScanQueues.Count > 0)
                    {
                        ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object stateobj2)
                            {
                                try
                                {
                                    //取IP地址
                                    IPEndPoint ipe = null;
                                    ScanQueues.TryDequeue(out ipe);

                                    //发送查询指令
                                    if (ipe != null)
                                    {
                                        byte[] queryCmd = Encoding.UTF8.GetBytes(CommandConst.QUERY_ROBOT_STATUS);
                                        UdpClient.UdpClient.Send(queryCmd, queryCmd.Length, ipe);

                                        //投递事件
                                        if (ProgressEvent != null)
                                        {
                                            ProgressEventArgs pea = new ProgressEventArgs();
                                            pea.Remote = ipe;
                                            ProgressEvent(this, pea);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    System.Console.WriteLine(ex.ToString());
                                }

                                //结束事件
                                if (ScanQueues.Count == 0)
                                {
                                    if (StopEvent != null)
                                    {
                                        StopEvent(this, new EventArgs());
                                    }
                                }
                            }));

                        try
                        {
                            Thread.Sleep(5);
                        }
                        catch (Exception ex) { }
                    }
                }));
        }

        /// <summary>
        /// 结束扫描
        /// </summary>
        public void StopScan()
        {
            ClearQueues();
        }
    }
}