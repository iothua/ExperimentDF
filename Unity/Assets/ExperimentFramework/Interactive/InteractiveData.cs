using UnityEngine.Events;
using System;

namespace ExperimentFramework.Interactive
{
    /// <summary>
    /// 交互数据
    /// </summary>
    [Serializable]
    public class InteractiveData
    {
        public string TagID;
        public InteractiveType Type;
    }

    /// <summary>
    /// 交互类型
    /// </summary>
    public enum InteractiveType
    {
        Distance,
        Raycast,
        Collision
    }

    /// <summary>
    /// 交互状态
    /// </summary>
    public enum InteractiveStatus
    {
        /// <summary>
        /// 无交互释放
        /// </summary>
        None,
        /// <summary>
        /// 在距离内第一次交互释放
        /// </summary>
        Once,
        /// <summary>
        /// 内部释放
        /// </summary>
        Inside,
        /// <summary>
        /// 是自动交互
        /// </summary>
        IsAuto
    }

     [Serializable]
    public class EventInteractive : UnityEvent<InteractiveBehavior, InteractiveBase>
    {

    }

    [Serializable]
    public class EventInteractiveStatus : UnityEvent<InteractiveBehavior, InteractiveBase, InteractiveStatus>
    {

    }
}