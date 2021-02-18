using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace App.Network
{
    [Serializable]
    public class LoginData
    {
        public string username;
        public string encryptPass;
        public DeviceInfo deviceInfo;
    }

    [Serializable]
    public class DeviceInfo {
        /// <summary>
        /// 设备唯一标识符
        /// </summary>
        public string deviceId;
        /// <summary>
        /// 设备名称
        /// </summary>
        public string name;
        /// <summary>
        /// 设备类型
        /// </summary>
        public string type;
        
        /// <summary>
        /// 显示器
        /// </summary>
        public string monitor;

        /// <summary>
        /// 显卡
        /// </summary>
        public string graphicsCardsName;
        /// <summary>
        /// 显卡标识符
        /// </summary>
        public string graphicsCardsId;
        /// <summary>
        /// 显卡标类型
        /// </summary>
        public string graphicsCardsType;
        /// <summary>
        /// 显存
        /// </summary>
        public string graphicsCardsSize;
        
        /// <summary>
        /// 内存
        /// </summary>
        public string memory;
        /// <summary>
        /// 操作系统
        /// </summary>
        public string os;
        /// <summary>
        /// cpu核数
        /// </summary>
        public string cpuCount;
        /// <summary>
        /// cpu类型
        /// </summary>
        public string cupType;
        
        public DeviceInfo()
        {
            deviceId = SystemInfo.deviceUniqueIdentifier;
            name = SystemInfo.deviceName;
            type = SystemInfo.deviceType.ToString();
            monitor = SystemInfo.deviceModel;

            graphicsCardsName = SystemInfo.graphicsDeviceName;
            graphicsCardsId = SystemInfo.graphicsDeviceID.ToString();
            graphicsCardsType = SystemInfo.graphicsDeviceType.ToString();
            graphicsCardsSize = SystemInfo.graphicsMemorySize.ToString();

            memory = SystemInfo.systemMemorySize.ToString();
            os = SystemInfo.operatingSystem;
            cpuCount = SystemInfo.processorCount.ToString();
            cupType = SystemInfo.processorType;

        }
    }

    public class TokenUserData
    {
        public string token;
        public ServerUserData userInfo;
    }
}
