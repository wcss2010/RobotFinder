using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFinderLibrary
{
    public class CommandConst
    {
        /// <summary>
        /// 查询设备状态
        /// </summary>
        public const string QUERY_ROBOT_STATUS = "QUERY_ROBOT_STATUS{{%%}}";

        /// <summary>
        /// 设备在线 
        /// </summary>
        public const string ROBOT_ONLINE = "ROBOT_ONLINE{{%%}}";
    }
}