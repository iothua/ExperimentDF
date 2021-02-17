using ExperimentFramework.Equipments;
using HighlightPlus;
using UnityEngine;

namespace ExperimentFramework.Interactive
{

    /// <summary>
    /// 物体操作状态
    /// </summary>
    public enum OperateStatus
    {
        
        /// <summary>
        /// 空闲
        /// </summary>
        Idle,
        /// <summary>
        /// 被抓取
        /// </summary>
        Grab,
        InteractionEnter,
        /// <summary>
        /// 交互中
        /// </summary>
        Interaction,
        /// <summary>
        /// 禁止中
        /// </summary>
        Prohibit
    }

    public interface IFeaturesOperate
    {
        GameObject InteractionObject { get; }

        OperateStatus Status { get; set; }

        EquipmentLocation Location { get; set; }

        string Guid { get; set; }

        Features Features { get; }

    }
}