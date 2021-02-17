using ExperimentFramework.Interactive;
using System;

using UnityEngine;

namespace ExperimentFramework.Inputs
{

    /// <summary>
    /// 输入类型
    /// </summary>
    public enum InputType
    {
        Mouse,
        Touch
    }

    /// <summary>
    /// 手势输入
    /// </summary>
    public class InputSystem
    {
        // /// <summary>
        // /// 手势动作
        // /// </summary>
        // private Dictionary<InputType,GestureAction> GestureActions = new Dictionary<InputType, GestureAction>();

        public MouseInputGestureAction MouseInputGesture;
        public TouchInputGestureAction TouchInputGesture;
        
        /// <summary>
        /// 摄像机状态
        /// </summary>
        public CursorStatus Status = CursorStatus.None;
        
        /// <summary>
        /// 光标按下（支持鼠标/触摸，等其他输入设备）
        /// </summary>
        public event Action<int> OnCursorDown;

        /// <summary>
        /// 光标抬起（支持鼠标/触摸，等其他输入设备）
        /// </summary>
        public event Action<int> OnCursorUp;

        /// <summary>
        /// 缩放
        /// </summary>
        public event Action<float> OnZoom;

        /// <summary>
        /// 旋转
        /// </summary>
        public event Action<Vector2> OnRotate;

        /// <summary>
        /// 平移
        /// </summary>
        public event Action<Vector2> OnPan;

        //public GestureAction CurrentGesture;
        
        //功能操作脚本
        private IFeaturesOperate _featuresOperate;

        public Vector2 ScreenAxis => TouchInputGesture.TouchCount > 0 ? TouchInputGesture.ScreenAxis : MouseInputGesture.ScreenAxis;

        public Vector2 HandPoint => TouchInputGesture.TouchCount > 0 ? TouchInputGesture.HandPoint : MouseInputGesture.HandPoint;

        public bool CanInteractive = true;

        public bool IsTouch => TouchInputGesture.TouchCount > 0;

        public InputSystem()
        {
            TouchInputGesture = new TouchInputGestureAction(this);
            MouseInputGesture = new MouseInputGestureAction(this);
        }

        public IFeaturesOperate FeaturesOperate
        {
            get => _featuresOperate;
            set => _featuresOperate = value;
        }

        /// <summary>
        /// 射线检测
        /// </summary>
        public Func<GameObject> OnRaycastHit;
        
        /// <summary>
        /// 操作对象移入
        /// </summary>
        public event Action<IFeaturesOperate> OnFeaturesEnter;
        
        /// <summary>
        /// 操作对象移除
        /// </summary>
        public event Action<IFeaturesOperate> OnFeaturesExit;
        
        /// <summary>
        /// 操作对象被抓取
        /// </summary>
        public event Action<IFeaturesOperate> OnFeaturesGrab;

        /// <summary>
        /// 操作对象抓取中...
        /// </summary>
        public event Action<IFeaturesOperate> OnFeaturesCrawling; 
        
        /// <summary>
        /// 操作对象抓取释放
        /// </summary>
        public event Action<IFeaturesOperate> OnFeaturesRelease;

        /// <summary>
        /// 操作对象单击
        /// </summary>
        public event Action<IFeaturesOperate> OnFeaturesTap;

        /// <summary>
        /// 长按
        /// </summary>
        public event Action<IFeaturesOperate> OnFeaturesLongTap; 
        
        /// <summary>
        /// 操作对象双击
        /// </summary>
        public event Action<IFeaturesOperate> OnFeaturesDoubleTap;

        public Func<Ray,RaycastHit> OnIdle;

        /// <summary>
        /// 碰撞筛选
        /// </summary>
        public Func<RaycastHit, bool> OnIdleFilter;
        
        /*
         * 1、双击：光标不动，0.3s内双击两次，有/无照射物体
         * 2、单击：光标不动，0.3s内单击一次，有/无照射物体
         * 3、抓取：按下，光标移动，照射物体
         * 4、释放：grab执行释放操作
         * 5、旋转：右键+差值
         * 6、缩放：滚轮
         * 7、平移：按下鼠标2键，移动
         */

        private Ray tempRay;

        public virtual void OnUpdate(Ray ray)
        {
            // foreach (var gestureAction in GestureActions)
            // {
            //     gestureAction.Value.OnUpdate();
            // }
            tempRay = ray;
            
            TouchInputGesture.OnUpdate();

#if UNITY_EDITOR_WIN|| UNITY_STANDALONE_WIN
            if (TouchInputGesture.TouchCount == 0)
                MouseInputGesture.OnUpdate();
#endif
            

            switch (Status)
            {
                case CursorStatus.None:

                    //if (!Stage.isTouchOnUI && CanInteractive)
                        Idle(ray);
                    
                    break;
                case CursorStatus.Pressed:

                    if (FeaturesOperate == null &&
                        (Mathf.Abs(ScreenAxis.x) > 0.1f || Mathf.Abs(ScreenAxis.y) > 0.1f))
                    {
                        Status = CursorStatus.Pan;
                        
                        //MessageDispatcher.SendMessageData(EventMessage._MOUSE_ICON, MouseIcon.Pan);
                        //MessageDispatcher.SendMessage(EventMessage._CAMERA_TRANSLATION_START);
                    }

                    break;
                case CursorStatus.PressedGrab:
                    
                    if (FeaturesOperate != null && (Mathf.Abs(ScreenAxis.x) > 0.1f || Mathf.Abs(ScreenAxis.y) > 0.1f))
                    {
                        Status = CursorStatus.Grab;
                    }

                    break;
                case CursorStatus.Grab:
                    //抓取时的处理，紧接着进入到Grabing中
                    Grab();
                    break;
                case CursorStatus.Release:
                    Release();
                    Status = CursorStatus.None;
                    break;
                case CursorStatus.Crawling:
                    Crawling();
                    break;
                case CursorStatus.Pan:
                    Pan(ScreenAxis);
                    break;
                case CursorStatus.Rotate:
                    Rotate(ScreenAxis);
                    break;
                case CursorStatus.Zoom:
                    break;
            }
        }

        public virtual void OnFixedUpdate()
        {
            TouchInputGesture.OnFixedUpdate();
#if UNITY_EDITOR_WIN|| UNITY_STANDALONE_WIN
            if (TouchInputGesture.TouchCount == 0)
                MouseInputGesture.OnFixedUpdate();
            #endif
        }

        public void OnLateUpdate()
        {
            TouchInputGesture.OnLateUpdate();
#if UNITY_EDITOR_WIN|| UNITY_STANDALONE_WIN
            if (TouchInputGesture.TouchCount == 0)
                MouseInputGesture.OnLateUpdate();
#endif
            
        }

        public void Dispose()
        {
            TouchInputGesture.Dispose();
#if UNITY_EDITOR_WIN|| UNITY_STANDALONE_WIN
                MouseInputGesture.Dispose();
#endif
        }

        /// <summary>
        /// 状态是否等于 None
        /// </summary>
        /// <returns></returns>
        public bool IsStatusNone()
        {
            return Status == CursorStatus.None;
        }

        /// <summary>
        /// 状态是否等于指定状态
        /// </summary>
        /// <param name="cursorStatus"></param>
        /// <returns></returns>
        public bool IsStatusSelf(CursorStatus cursorStatus)
        {
            return Status.Equals(cursorStatus);
        }

        /// <summary>
        /// 状态等于自身，或者为None
        /// </summary>
        /// <param name="cursorStatus"></param>
        /// <returns></returns>
        public bool IsSelfOrNone(CursorStatus cursorStatus)
        {
            return IsStatusNone() || IsStatusSelf(cursorStatus);
        }

        public bool IsDisableUI
        {
            get
            {
                //if (Stage.isTouchOnUI)
                //{
                //    return Stage.inst.touchTarget == null || Stage.inst.touchTarget.gOwner.name != "mask";
                //}

                return false;
            }

        }

        public void CursorDown(int cursorId)
        {
            switch (cursorId)
            {
                case 0:
                    Status = CursorStatus.Pressed;
                    //MessageDispatcher.SendMessageData(EventMessage._HAND_DOWN, 0);

                    //if (Stage.isTouchOnUI)
                    //{
                    //    if (!IsDisableUI) return;

                    //    Status = CursorStatus.PressedUI;
                    //    return;
                    //}

                    if (!CanInteractive) return;

                    var hitObject = OnRaycastHit();
                    if (hitObject != null)
                    {
                        Status = CursorStatus.PressedGrab;
                    }
                    
                    break;
                case 1:
                    //需要移动才可以
                    //Rotate(ScreenAxis); //但是需要加一个条件判断 待定
                    break;
                case 2:
                    // Pan(ScreenAxis);
                    break;
                default:
                    break;
            }            
            
            OnCursorDown?.Invoke(cursorId);
        }

        public void CursorUp(int cursorId)
        {
            switch (cursorId)
            {
                case 0:
                    switch (Status)
                    {
                        case CursorStatus.Crawling:
                            Release();
                            Status = CursorStatus.None;
                            
                            break;
                        case CursorStatus.Rotate:
                            Rotate(Vector2.zero);
                            Status = CursorStatus.None;
                            //MessageDispatcher.SendMessageData(EventMessage._MOUSE_ICON, MouseIcon.None);
                            break;
                        case CursorStatus.Pan:
                            Pan(Vector2.zero);
                            Status = CursorStatus.None;
                            //MessageDispatcher.SendMessageData(EventMessage._MOUSE_ICON, MouseIcon.None);
                            break;
                        case CursorStatus.Zoom:
                            //Zoom(0);
                            Status = CursorStatus.None;
                            //MessageDispatcher.SendMessageData(EventMessage._MOUSE_ICON, MouseIcon.None);
                            break;
                        default:
                            Status = CursorStatus.None;
                            break;
                    }
                    
                    //MessageDispatcher.SendMessageData(EventMessage._HAND_UP, 0);
                    
                    break;
                case 1:
                    Status = CursorStatus.None;
                    //MessageDispatcher.SendMessageData(EventMessage._MOUSE_ICON, MouseIcon.None);
                    break;
                case 2:
                    Status = CursorStatus.None;
                    //MessageDispatcher.SendMessageData(EventMessage._MOUSE_ICON,MouseIcon.None);
                    break;
                default:
                    break;
            }
            
            OnCursorUp?.Invoke(cursorId);
        }

        void Idle(Ray ray)
        {
            //处理
            var tempHit = OnIdle(ray);
            if (tempHit.collider == null)
            {
                if (FeaturesOperate != null)
                {
                    FeaturesExit(FeaturesOperate);
                }

                return;
            }


            if (OnIdleFilter(tempHit))
            {
                if (FeaturesOperate == null)
                {

                    FeaturesOperate = tempHit.collider.GetComponent<IFeaturesOperate>();
                    GetExtendFeaturesOperate(tempHit.collider.gameObject);
                    
                    FeaturesEnter(FeaturesOperate);


                }
                else
                {
                    if (FeaturesOperate.InteractionObject.Equals(tempHit.collider.gameObject))
                        return;
                    
                    FeaturesExit(FeaturesOperate);

                    FeaturesOperate = tempHit.collider.GetComponent<IFeaturesOperate>();
                    GetExtendFeaturesOperate(tempHit.collider.gameObject);
                    FeaturesEnter(FeaturesOperate);

                }
            }
            else
            {
                if (FeaturesOperate != null)
                {
                    FeaturesExit(FeaturesOperate);
                }
            }

            
        }

        void GetExtendFeaturesOperate(GameObject hitObject)
        {
            if (FeaturesOperate != null) return;

            var extendFeatures = hitObject.GetComponent<ExtendFeaturesOperate>();
            if (extendFeatures == null) return;
            FeaturesOperate = extendFeatures.featuresOperate;
        }

        public void IdleTouchBegin()
        {
            Idle(tempRay);
        }

        public void IdleTouchEnd()
        {
            //if (FeaturesOperate != null)
            //{
            //    FeaturesExit(FeaturesOperate);
            //}
        }

        void Grab()
        {
            FeaturesGrab(FeaturesOperate);
        }

        void Release()
        {
            FeaturesRelease(FeaturesOperate);
            //FeaturesOperate = null;
        }

        void Crawling()
        {
            FeaturesCrawling(FeaturesOperate);
        }

        public void Rotate(Vector2 rotate)
        {
            OnRotate?.Invoke(rotate);
        }

        public void Zoom(float zoom)
        {
            OnZoom?.Invoke(zoom);
        }

        public void Pan(Vector2 pan)
        {
            OnPan?.Invoke(pan);
        }
        
        public void FeaturesEnter(IFeaturesOperate featuresOperate)
        {
            OnFeaturesEnter?.Invoke(featuresOperate);
        }

        public void FeaturesExit(IFeaturesOperate featuresOperate)
        {
            OnFeaturesExit?.Invoke(featuresOperate);
        }

        public void FeaturesGrab(IFeaturesOperate featuresOperate)
        {
            OnFeaturesGrab?.Invoke(featuresOperate);
        }

        public void FeaturesCrawling(IFeaturesOperate featuresOperate)
        {
            OnFeaturesCrawling?.Invoke(featuresOperate);
        }

        public void FeaturesRelease(IFeaturesOperate featuresOperate)
        {
            OnFeaturesRelease?.Invoke(featuresOperate);
        }

        /// <summary>
        /// 双击操作
        /// </summary>
        public void DoubleTap()
        {
            if (Status != CursorStatus.PressedGrab && Status != CursorStatus.None &&
                Status != CursorStatus.Pressed) return;
            var hitObject = OnRaycastHit();
            if (hitObject == null) return;

            var features = hitObject.GetComponent<IFeaturesOperate>();
            FeaturesDoubleTap(features);
        }

        /// <summary>
        /// 单击操作
        /// </summary>
        public void Tap()
        {

            if (Status != CursorStatus.PressedGrab && Status != CursorStatus.None &&
                Status != CursorStatus.Pressed) return;

            var hitObject = OnRaycastHit();
            if (hitObject == null) return;

            var features = hitObject.GetComponent<IFeaturesOperate>();
            FeaturesTap(features);
        }

        /// <summary>
        /// 长按
        /// </summary>
        public void LongTap()
        {
            if (Status != CursorStatus.PressedGrab) return;
            
            var hitObject = OnRaycastHit();
            if (hitObject == null) return;
            
            var features = hitObject.GetComponent<IFeaturesOperate>();
            FeaturesLongTap(features);
        }

        public void FeaturesDoubleTap(IFeaturesOperate featuresOperate)
        {
            OnFeaturesDoubleTap?.Invoke(featuresOperate);
        }

        public void FeaturesTap(IFeaturesOperate featuresOperate)
        {
            OnFeaturesTap?.Invoke(featuresOperate);
        }

        public void FeaturesLongTap(IFeaturesOperate featuresOperate)
        {
            OnFeaturesLongTap?.Invoke(featuresOperate);
        }
    }
}