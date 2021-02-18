using UnityEngine;

namespace ExperimentFramework.Interactive
{
    /// <summary>
    /// 交互基类
    /// </summary>
    public class InteractiveBase : MonoBehaviour
    {
        /// <summary>
        /// 交互物体
        /// </summary>
        public InteractiveBehavior Behavior;

        public virtual bool IsCanInteraction(InteractiveBase interactive)
        {
            return true;
        }

        /// <summary>
        /// 移入
        /// </summary>
        public virtual void OnInteractiveEntern(InteractiveBase interactive)
        {
            
        }

        /// <summary>
        /// 离开
        /// </summary>
        public virtual void OnInteractiveExit(InteractiveBase interactive)
        {

        }

        /// <summary>
        /// 停留
        /// </summary>
        public virtual void OnInteractiveStay(InteractiveBase interactive)
        {

        }

        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="distanceInteraction"></param>
        /// <param name="status"></param>
        public virtual void OnInteractiveRelease(InteractiveBase interactive, InteractiveStatus status)
        {

        }
    }
}