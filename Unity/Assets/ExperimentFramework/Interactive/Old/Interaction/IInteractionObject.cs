using UnityEngine;

namespace ExperimentFramework.Interactive
{
    /// <summary>
    /// 用于距离检测对外的交互接口
    /// </summary>
    public interface IInteractionObject
    {
        GameObject InteractionObject { get;}
        bool IsEnable { get; set; }
        
        /// <summary>
        /// 外部识别的字符串Key
        /// </summary>
        string DisplayName { get; }
    }
}