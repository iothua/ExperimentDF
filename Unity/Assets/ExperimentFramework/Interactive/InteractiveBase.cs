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
            if (interactive == null) return true;
            if (Behavior == null) return true;
            return Behavior.OnCanInteractive == null || Behavior.OnCanInteractive(interactive.Behavior);
        }

        /// <summary>
        /// 移入
        /// </summary>
        public virtual void OnInteractiveEnter(InteractiveBase interactive)
        {
            if (interactive == null) return;
            if (Behavior == null) return;

            Behavior.OnInteractiveEnter?.Invoke(interactive.Behavior, interactive);
        }

        /// <summary>
        /// 离开
        /// </summary>
        public virtual void OnInteractiveExit(InteractiveBase interactive)
        {
            if (interactive == null) return;
            if (Behavior == null) return;

            Behavior.OnInteractiveExit?.Invoke(interactive.Behavior, interactive);
        }

        /// <summary>
        /// 停留
        /// </summary>
        public virtual void OnInteractiveStay(InteractiveBase interactive)
        {
            if (interactive == null) return;
            if (Behavior == null) return;

            Behavior.OnInteractiveStay?.Invoke(interactive.Behavior, interactive);
        }

        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="interactive"></param>
        /// <param name="status"></param>
        public virtual void OnInteractiveRelease(InteractiveBase interactive, InteractiveStatus status)
        {
            if (interactive == null) return;
            if (Behavior == null) return;

            Behavior.OnInteractiveRelease?.Invoke(interactive.Behavior, interactive, status);
        }
    }
}