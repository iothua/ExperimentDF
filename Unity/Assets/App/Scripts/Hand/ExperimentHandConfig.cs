using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.Hand
{
    [Serializable]
    public class ExperimentHandConfig
    {
        public List<ExperimentHandData> ExperimentHandDatas;
        
        public List<ExperimentModelHandData> ModelHandDatas;
    }

    [Serializable]
    public struct HandModelData
    {
        /// <summary>
        /// 左手模型Key
        /// </summary>
        public string ModelKey;
        
        /// <summary>
        /// 左手坐标
        /// </summary>
        public Vector3 HandPosition;

        public Vector3 HandRotation;
    }

    /// <summary>
    /// 实验手数据
    /// </summary>
    [Serializable]
    public struct ExperimentHandData
    {
        /// <summary>
        /// 仪器Key
        /// </summary>
        public string experimentKey;

        public HandModelData LeftHand;
        public HandModelData RightHand;
    }

    /// <summary>
    /// 手模型数据
    /// </summary>
    [Serializable]
    public struct ExperimentModelHandData
    {
        /// <summary>
        /// 手 Key
        /// </summary>
        public string handKey;
        
        /// <summary>
        /// 手模型路径
        /// </summary>
        public string handPath;
    }
}

