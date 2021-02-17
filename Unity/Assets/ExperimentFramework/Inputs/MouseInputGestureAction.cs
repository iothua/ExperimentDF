using System;
using UnityEngine;
using Loxodon.Framework.Execution;

namespace ExperimentFramework.Inputs
{
    /// <summary>
    /// 鼠标手势
    /// </summary>
    public class MouseInputGestureAction : InputGestureAction
    {
        public override Vector2 ScreenAxis => new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        //时间计时
        private readonly System.Timers.Timer clickTimer=new System.Timers.Timer();

        private bool isDoubleClick; //是否双击
        private bool isPressed; //是否光标按下
        private bool isLongPressed;

        public MouseInputGestureAction(InputSystem system) : base(system)
        {
            clickTimer.Interval = 400;
            clickTimer.Elapsed += SingleClick;
        }

        void SingleClick(object o, EventArgs e)
        {
            clickTimer.Stop();
            isDoubleClick = false;

            //在主线程执行
            Executors.RunOnMainThread(() =>
            {
                if (isPressed)
                {
                    isLongPressed = true;
                    inputSystem.LongTap();
                    return;
                }

                inputSystem.Tap();

            });
        }

        public override void OnUpdate()
        {
            if (!IsEnable) return;
            
            if (Input.GetMouseButtonDown(0))
            {
                isPressed = true;
                if (!clickTimer.Enabled)
                {
                    clickTimer.Start();
                }
                else
                {
                    clickTimer.Stop();
                    isDoubleClick = true;
                    
                    //执行双击，在判定双击前，需要进行其他判断……
                    inputSystem.DoubleTap();
                }
                
                inputSystem.CursorDown(0);

            }

            if (Input.GetMouseButtonUp(0))
            {
                if (!clickTimer.Enabled && !isDoubleClick && !isLongPressed)
                {
                    //单击判定
                    isDoubleClick = false;
                    inputSystem.Tap();
                }

                isPressed = false;
                isLongPressed = false;

                inputSystem.CursorUp(0);
            }

            float zoomAxis = Input.GetAxis("Mouse ScrollWheel");

            if (inputSystem.IsDisableUI) zoomAxis = 0;
            
            if (zoomAxis!= 0)
            {
                inputSystem.Zoom(zoomAxis);
            }
            else
            {
                if (inputSystem.IsStatusSelf(CursorStatus.Zoom))
                {
                    inputSystem.Status = CursorStatus.None;
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                inputSystem.CursorDown(1);
                inputSystem.Rotate(ScreenAxis);
            }

            if (Input.GetMouseButtonUp(1))
            {
                inputSystem.CursorUp(1);
            }

            if (Input.GetMouseButtonDown(2))
            {
                inputSystem.CursorDown(2);
                inputSystem.Pan(ScreenAxis);
            }

            if (Input.GetMouseButtonUp(2))
            {
                inputSystem.CursorUp(2);
            }

            // if (inputSystem.Status == CursorStatus.Pan)
            // {
            //     inputSystem.Pan(ScreenAxis);
            // }
        }

        public override Vector2 HandPoint => Input.mousePosition;

    }
}