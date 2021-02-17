﻿namespace ExperimentFramework.Interactive
{
    public enum InteractionReleaseStatus
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
}
