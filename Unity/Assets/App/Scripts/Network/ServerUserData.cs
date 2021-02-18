using System;

namespace App.Network
{
    [Serializable]
    public class ServerUserData
    {
        /// <summary>
        /// 学号
        /// </summary>
        public string username;
        /// <summary>
        /// 姓名
        /// </summary>
        public string nickname;
        /// <summary>
        /// 头像
        /// </summary>
        public string avatarPath;
        /// <summary>
        /// 学校
        /// </summary>
        public string schoolName;
        /// <summary>
        /// 最后登陆设备
        /// </summary>
        public string lastDeviceName;
    }
}