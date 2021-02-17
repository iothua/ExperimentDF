using System;
using UnityEngine;

namespace ExperimentFramework.Inputs
{
    /// <summary>
    /// 手势动作
    /// </summary>
    public class InputGestureAction: IDisposable
    {

        public InputSystem inputSystem;
        
        /// <summary>
        /// 屏幕向量
        /// </summary>
        public virtual Vector2 ScreenAxis => new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        public bool IsEnable { get; set; }

        public InputGestureAction(InputSystem system)
        {
            inputSystem = system;
            IsEnable = true;
        }

        public virtual void OnUpdate()
        {
            
        }

        public virtual void OnFixedUpdate()
        {
            
        }

        public virtual void OnLateUpdate()
        {
            
        }

        /// <summary>
        /// 获取当单指坐标点（鼠标或者单个手指）
        /// </summary>
        /// <returns></returns>
        public virtual Vector2 HandPoint=>Vector2.zero;

        public virtual void Dispose()
        {
            
        }
    }
}