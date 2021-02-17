using System;
using DigitalRubyShared;
using UnityEngine;
using TouchPhase = UnityEngine.TouchPhase;

namespace ExperimentFramework.Inputs
{
    /// <summary>
    /// 触发手势动作
    /// </summary>
    public class TouchInputGestureAction : InputGestureAction
    {
       public override Vector2 ScreenAxis => TouchCount > 0 ? new Vector2(FingersScript.Instance.CurrentTouches[0].DeltaX/50.0f, FingersScript.Instance.CurrentTouches[0].DeltaY/50.0f) : Vector2.zero;

        public int TouchCount => FingersScript.Instance.CurrentTouches.Count;
        
        private TapGestureRecognizer tapGesture;
        private TapGestureRecognizer doubleTapGesture;
        private PanGestureRecognizer panGesture;
        private ScaleGestureRecognizer scaleGesture;
        private LongPressGestureRecognizer longPressGesture;

        public TouchInputGestureAction(InputSystem system) : base(system)
        {
            CreatePanGesture();
            CreateScaleGesture();
            CreateLongPressGesture();
            
            CreateDoubleTapGesture();
            CreateTapGesture();
        }

        private void TapGestureCallback(DigitalRubyShared.GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                inputSystem.Tap();
            }
        }

        private void CreateTapGesture()
        {
            tapGesture = new TapGestureRecognizer();
            tapGesture.SequentialTapThresholdSeconds = 0.2f;
            tapGesture.ThresholdSeconds = 0.2f;
            tapGesture.StateUpdated += TapGestureCallback;
            tapGesture.RequireGestureRecognizerToFail = doubleTapGesture;
            FingersScript.Instance.AddGesture(tapGesture);
        }

        private void DoubleTapGestureCallback(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                inputSystem.DoubleTap();
            }
        }

        private void CreateDoubleTapGesture()
        {
            doubleTapGesture = new TapGestureRecognizer();
            doubleTapGesture.NumberOfTapsRequired = 2;
                 
            doubleTapGesture.StateUpdated += DoubleTapGestureCallback;
            
            FingersScript.Instance.AddGesture(doubleTapGesture);
        }
        
        private void PanGestureCallback(GestureRecognizer gesture)
        {

            if (inputSystem.Status == CursorStatus.Grab || inputSystem.Status == CursorStatus.Crawling)
            {
                return;
            }
            if (gesture.State == GestureRecognizerState.Began)
            {
                
                inputSystem.Status = CursorStatus.Rotate;
            }

            if (gesture.State == GestureRecognizerState.Ended)
            {
                inputSystem.Status = CursorStatus.None;
            }
        }

        private void CreatePanGesture()
        {
            panGesture = new PanGestureRecognizer();
            panGesture.MinimumNumberOfTouchesToTrack = 2;
            panGesture.StateUpdated += PanGestureCallback;
            FingersScript.Instance.AddGesture(panGesture);
        }

        private void ScaleGestureCallback(GestureRecognizer gesture)
        {
            if (inputSystem.Status == CursorStatus.Grab || inputSystem.Status == CursorStatus.Crawling)
            {
                return;
            }

            if (gesture.State == GestureRecognizerState.Executing)
            {
                
                inputSystem.Status = CursorStatus.Zoom;

                float zoomSpeed = Mathf.Clamp(scaleGesture.ScaleMultiplier - 1, -0.2f, 0.2f);

                inputSystem.Zoom(zoomSpeed);
            }

            if (gesture.State == GestureRecognizerState.Ended)
            {
                inputSystem.Status = CursorStatus.None;
            }
        }

        private void CreateScaleGesture()
        {
            scaleGesture = new ScaleGestureRecognizer();
            scaleGesture.StateUpdated += ScaleGestureCallback;
            FingersScript.Instance.AddGesture(scaleGesture);
        }
        public override void OnUpdate()
        {
            if (!IsEnable) return;

            base.OnUpdate();
            
            if (FingersScript.Instance.CurrentTouches.Count ==1)
            {
                switch (FingersScript.Instance.CurrentTouches[0].TouchPhase)
                {
                    case DigitalRubyShared.TouchPhase.Began:
                        inputSystem.IdleTouchBegin();
                        inputSystem.CursorDown(0);
                        break;
                    case DigitalRubyShared.TouchPhase.Cancelled:
                    case DigitalRubyShared.TouchPhase.Ended:
                    case DigitalRubyShared.TouchPhase.Unknown:
                        inputSystem.CursorUp(0);
                        inputSystem.IdleTouchEnd();
                        break;
                }
            }
        }
        
        private void LongPressGestureCallback(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Began)
            {
                inputSystem.LongTap();
            }
        }

        private void CreateLongPressGesture()
        {
            longPressGesture = new LongPressGestureRecognizer();
            longPressGesture.MaximumNumberOfTouchesToTrack = 1;
            longPressGesture.StateUpdated += LongPressGestureCallback;
            FingersScript.Instance.AddGesture(longPressGesture);
        }

        public override void Dispose()
        {
            base.Dispose();
            FingersScript.Instance.RemoveGesture(longPressGesture);
            FingersScript.Instance.RemoveGesture(scaleGesture);
            FingersScript.Instance.RemoveGesture(panGesture);
            FingersScript.Instance.RemoveGesture(doubleTapGesture);
            FingersScript.Instance.RemoveGesture(tapGesture);
        }

        public override Vector2 HandPoint => TouchCount > 0 ? new Vector2(FingersScript.Instance.CurrentTouches[0].ScreenX, FingersScript.Instance.CurrentTouches[0].ScreenY) : Vector2.zero;
    }
}