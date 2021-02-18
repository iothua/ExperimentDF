using System;

namespace App.Network
{
    [Serializable]
    public class HttpMsgCallback
    {
        public int code;
        public bool success;
        public string msg;
        public string errorMessage;
        public string data;
        public string timestamp;
    }
}