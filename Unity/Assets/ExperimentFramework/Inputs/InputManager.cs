using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace ExperimentFramework.Inputs
{
    
     
    //public class InputManager : MonoBehaviour
    //{
         

    //    private Collider _colliderDesk;

    //    public TouchOffsetType touchOffset = TouchOffsetType.OffsetRange;

    //    public Collider colliderDesk
    //    {
    //        get
    //        {
    //            if (_colliderDesk == null)
    //            {
    //                var desks = GameObject.FindGameObjectsWithTag("desktop");
    //                if (desks.Length > 0)
    //                {
    //                    _colliderDesk = desks[0].GetComponent<BoxCollider>();
    //                }
    //                else
    //                {
    //                    Debug.LogError("场景不存在桌面限制物体...");
    //                }
    //            }

    //            return _colliderDesk;
    //        }
    //    }

    //    public static InputManager Instance;

    //    public bool IsEnable = true;

    //    /// <summary>
    //    /// 手势动作
    //    /// </summary>
    //    public InputSystem inputSystem;

    //    /// <summary>
    //    /// 摄像机锁
    //    /// </summary>
    //    public bool IsCameraLock { get; set; }

    //    public bool IsTouch => inputSystem.IsTouch;

    //    /// <summary>
    //    /// 默认摄像机Look点
    //    /// </summary>
    //    public Transform Anchor;
    //    /// <summary>
    //    /// 设置
    //    /// </summary>
    //    public InputSetting setting;

    //    //计算镜头移动
    //    private Vector3 position, targetPosition;

    //    //计算镜头旋转
    //    private Quaternion rotation, targetRotation;

    //    //计算镜头缩放距离
    //    private float distanceZoom, targetDistance;

    //    //更改的距离接收点集合
    //    private Dictionary<DistanceInteraction, Dictionary<float, Vector3>> CurrentChangeDatas = new Dictionary<DistanceInteraction, Dictionary<float, Vector3>>();

    //    //当前物体碰撞大小
    //    private Vector3 CurrentColliderSize =Vector3.zero;

    //    private float idleDelay = 0;
    //    #region 一些虚影、标签、高亮等字段

    //    public GameObject shadowObject;
    //    private float fadeDis = 0.7f;
    //    private MeshRenderer shadowRender;
    //    private bool canShadow = true;

    //    private bool isShowLabel;

    //    #endregion

    //    #region 其他

    //    //移动中
    //    private bool isMoving;

    //    //当前操作的物体的最上级父物体
    //    private Transform operateParent;
    //    private Vector3 idelPos;

    //    public float ghostWaitTime;

    //    private Vector3 _offset;

    //    #endregion

    //    private Camera _mainCamera;

    //    public Camera mainCamera
    //    {
    //        get
    //        {

    //            if (_mainCamera == null)
    //            {
    //                _mainCamera = Camera.main;
    //            }

    //            return _mainCamera;
    //        }

    //    }
    //    public Transform MainCameraTransform => mainCamera.transform;

    //    private bool isinitialize; //是否已经初始化

    //    private float relativePositionY; //Y轴相对位置

    //    private bool applicationFocus = true; //程序焦距，当系统光标不位于该程序时，停止相应操作

    //    private List<UnityEngine.Object> rList;
    //    private Bounds currentObjBound;
    //    private TransformData normalCamera;

    //    /// <summary>
    //    /// 屏幕向量
    //    /// </summary>
    //    public static Vector2 ScreenAxis
    //    {
    //        get
    //        {
    //            if (Instance == null) return Vector2.zero;

    //            return Instance.inputSystem.ScreenAxis;
    //        }
    //    }

    //    #region MonoBehaviour方法
    //    public LayerMask layerObject;
    //    private void Awake()
    //    {
    //        HuaUtilitys.LAYER_OBJECT = layerObject;

    //        inputSystem = new InputSystem();

    //        Instance = this;

    //        if (shadowObject != null)
    //            shadowRender = shadowObject.GetComponent<MeshRenderer>();
    //        else
    //        {
    //            Debug.Log("虚影对象shadowObject等于Null");
    //        }

    //        normalCamera = new TransformData(MainCameraTransform);

    //    }

    //    void Start()
    //    {
    //        Refresh();
    //        inputSystem.OnPan += OnPan;
    //        inputSystem.OnZoom += OnZoom;
    //        inputSystem.OnRotate += OnRotate;
            
    //        inputSystem.OnFeaturesDoubleTap += OnFeaturesDoubleTap;

    //        inputSystem.OnFeaturesEnter += OnFeaturesEnter;
    //        inputSystem.OnFeaturesExit += OnFeaturesExit;
    //        inputSystem.OnFeaturesGrab += OnFeaturesGrab;
    //        inputSystem.OnFeaturesRelease += OnFeaturesRelease;
    //        inputSystem.OnFeaturesCrawling += FeaturesCrawling;

    //        inputSystem.OnRaycastHit = GetRaycastHit;
    //        inputSystem.OnIdle = GetRaycastHit;
    //        inputSystem.OnIdleFilter = IdleFilter;

            
    //    }

    //    private void Update()
    //    {

    //        if (!applicationFocus) return;

    //        if (!IsEnable) return;
            
    //        Ray ray = mainCamera.ScreenPointToRay(inputSystem.HandPoint);

    //        //if (Stage.isTouchOnUI) return;
    //        if (idleDelay == 0)
    //            inputSystem.OnUpdate(ray);
    //        else
    //        {
    //            idleDelay -= Time.deltaTime;
    //            if (idleDelay <= 0)
    //            {
    //                idleDelay = 0;
    //            }
    //        }

    //        //发送射线
    //        MessageDispatcher.SendMessageData(EventMessage._RAY_TARGET, ray);
    //    }

    //    private void OnApplicationFocus(bool hasFocus)
    //    {
    //        applicationFocus = hasFocus;

    //    }
    //    //抓取前物体是否在桌面上
    //    bool deskT;
    //    //执行自动下落的物体
    //    IFeaturesOperate dropObj = null;
    //    //物体位置缓存
    //    Vector3 posT;

    //    //放回架子临时存储物体
    //    IFeaturesOperate ReleaseShelfObj = null;

    //    Transform ShelfObj = null;

    //    bool IsShowGhost = false;
    //    float time = 0;
    //    private void FixedUpdate()
    //    {
    //        inputSystem.OnFixedUpdate();
    //        //架子高亮颜色
    //        var color = new Color();
    //        //移动
    //        if (inputSystem.Status == CursorStatus.Crawling && inputSystem.FeaturesOperate != null)
    //        {
    //            Vector3 worldPosition = GetMovePosition(inputSystem.FeaturesOperate, offset);

    //            //抓取物体移入桌面
    //            if (inputSystem.FeaturesOperate.Location != null)
    //            {
    //                if (inputSystem.FeaturesOperate.Location.CurrentLocationStatus == Experiment.EquipmentLocationStatus.Shelf)
    //                {
    //                    time += Time.deltaTime;
    //                    if (mainCamera.transform.forward.z > 0.8f)
    //                    {
    //                        var Z  = Mathf.Clamp(setting.DesktopOperationLocation, MainCameraTransform.position.z+setting.CameraFrontLocation, worldPosition.z);
    //                        worldPosition.z = Mathf.Lerp(worldPosition.z, Z, Time.deltaTime * setting.DesktopOperationSpeed);
    //                        if (Mathf.Abs(worldPosition.z - setting.DesktopOperationLocation) < 0.01f&&time > ghostWaitTime)
    //                        {
    //                            if (!inputSystem.FeaturesOperate.Location.InitialTransform.IsLocal)
    //                            {
    //                                IsShowGhost = true;
    //                            }
    //                            inputSystem.FeaturesOperate.Location.CurrentLocationStatus = Experiment.EquipmentLocationStatus.Desktop;
    //                            MessageDispatcher.SendMessageData(EventMessage._SHELF_EXIT, inputSystem.FeaturesOperate);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        if (time > ghostWaitTime)
    //                        {
    //                            if (!inputSystem.FeaturesOperate.Location.InitialTransform.IsLocal)
    //                            {
    //                                IsShowGhost = true;
    //                            }
    //                            inputSystem.FeaturesOperate.Location.CurrentLocationStatus = Experiment.EquipmentLocationStatus.Desktop;
    //                        }
    //                    }
    //                }
    //                //当抓取仪器进入架子触发区域显示架子高亮
    //                if (inputSystem.FeaturesOperate.Location.InitialLocationStatus == Experiment.EquipmentLocationStatus.Shelf)
    //                {
    //                    Ray ray = mainCamera.ScreenPointToRay(inputSystem.HandPoint);
    //                    if (Physics.Raycast(ray, out var tempHit, (setting.maxDistance + 1), 1 << LayerMask.NameToLayer("Shelf")) && !inputSystem.FeaturesOperate.Location.InitialTransform.IsLocal && inputSystem.FeaturesOperate.Status == OperateStatus.Grab)
    //                    {
    //                        if (tempHit.collider != null && ShelfObj != tempHit.transform&&IsShowGhost)
    //                        {
    //                            if (tempHit.transform.GetComponent<MeshRenderer>() != null)
    //                            {
    //                                color = tempHit.transform.GetComponent<MeshRenderer>().material.GetColor("_Color");
    //                                tempHit.transform.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(color.r, color.g, color.b, 1));
    //                                ShelfObj = tempHit.transform;
    //                                IsShowGhost = false;
    //                                time = 0;
    //                            }
    //                        }
    //                    }
    //                    //当抓取仪器离开架子触发区域隐藏架子高亮
    //                    else if (ShelfObj != null)
    //                    {
    //                        color = ShelfObj.transform.GetComponent<MeshRenderer>().material.GetColor("_Color");
    //                        ShelfObj.transform.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(color.r, color.g, color.b, 0));
    //                        IsShowGhost = true;
    //                        ShelfObj = null;
    //                    }

    //                }

                   

    //            }

    //            EventUpdateObject.SendListener(inputSystem.FeaturesOperate.InteractionObject, worldPosition);

    //            //抓取前物体在桌面，抓取离开桌面时，发送事件
    //            if (!IsDesktop(inputSystem.FeaturesOperate) && deskT)
    //            {
    //                MessageDispatcher.SendMessageData(EventMessage._OBJECT_PICKUP, dropObj);
    //                deskT = false;
    //                //抓取物体后旋转到合适得角度
    //                if (inputSystem.FeaturesOperate.Features.IsRotate)
    //                {
    //                    inputSystem.FeaturesOperate.InteractionObject.transform.rotation = Quaternion.Euler(inputSystem.FeaturesOperate.Features.RotateValue);
    //                }
    //            }
    //            if (canShadow)
    //            {
    //               UpdateShowdowPosition(worldPosition);
    //            }
    //        }

            
    //        //释放移入架子
    //        if(ReleaseShelfObj != null)
    //        {
    //            ReleaseShelfObj.InteractionObject.transform.position =Vector3.Lerp(ReleaseShelfObj.InteractionObject.transform.position, ReleaseShelfObj.Location.InitialTransform.Position.UnityVector(),Time.deltaTime*setting.DesktopOperationSpeed);
    //            ReleaseShelfObj.InteractionObject.transform.rotation = Quaternion.Lerp(ReleaseShelfObj.InteractionObject.transform.rotation, Quaternion.Euler(ReleaseShelfObj.Location.InitialTransform.Rotate.UnityVector()), Time.deltaTime * setting.DesktopOperationSpeed);
    //            ReleaseShelfObj.InteractionObject.transform.localScale = Vector3.Lerp(ReleaseShelfObj.InteractionObject.transform.localScale, ReleaseShelfObj.Location.InitialTransform.Scale.UnityVector(), Time.deltaTime * setting.DesktopOperationSpeed);
    //            ReleaseShelfObj.Location.CurrentLocationStatus = Experiment.EquipmentLocationStatus.Shelf;

    //            //放回架子关闭高亮
    //            if (ShelfObj != null)
    //            {
    //                color = ShelfObj.transform.GetComponent<MeshRenderer>().material.GetColor("_Color");
    //                ShelfObj.transform.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(color.r, color.g, color.b, 0));
    //                ShelfObj = null;
    //            }

    //            if (Vector3.Distance(ReleaseShelfObj.InteractionObject.transform.position, ReleaseShelfObj.Location.InitialTransform.Position.UnityVector())<0.01f)
    //            {   
    //                ReleaseShelfObj.InteractionObject.transform.position = ReleaseShelfObj.Location.InitialTransform.Position.UnityVector();
    //                ReleaseShelfObj.InteractionObject.transform.rotation = Quaternion.Euler(ReleaseShelfObj.Location.InitialTransform.Rotate.UnityVector());
    //                ReleaseShelfObj.InteractionObject.transform.localScale = ReleaseShelfObj.Location.InitialTransform.Scale.UnityVector();
    //                MessageDispatcher.SendMessageData(EventMessage._SHELF_ENTER, ReleaseShelfObj);
    //                ReleaseShelfObj = null;
    //            }
                
    //        }

    //        //释放后自动下落
    //        if(dropObj != null)
    //        {
    //            dropObj.InteractionObject.transform.position = GetDropPosition(dropObj);
    //            if (IsDesktop(dropObj))
    //            {
    //                MessageDispatcher.SendMessageData(EventMessage._OBJECT_PUTDOWN, dropObj);
    //                //物体掉落桌面自动回复初始旋转
    //                if (dropObj.Features.IsRotate&&dropObj.Location!=null)
    //                {
    //                    if (dropObj.Location.InitialLocationStatus == Experiment.EquipmentLocationStatus.Shelf)
    //                    {
    //                        dropObj.InteractionObject.transform.position = new Vector3(dropObj.InteractionObject.transform.position.x, dropObj.Location.InitialTransform.Position.UnityVector().y - (dropObj.Location.InitialTransform.Position.UnityVector().y - colliderDesk.bounds.min.y), dropObj.InteractionObject.transform.position.z);
    //                    }
    //                    else
    //                    {
    //                        dropObj.InteractionObject.transform.position = new Vector3(dropObj.InteractionObject.transform.position.x, dropObj.Location.InitialTransform.Position.UnityVector().y, dropObj.InteractionObject.transform.position.z);
    //                    }
    //                    dropObj.InteractionObject.transform.rotation = Quaternion.Euler(dropObj.Location.InitialTransform.Rotate.UnityVector());
    //                }
    //                dropObj = null;
    //            }
    //            else if (dropObj.InteractionObject.transform.position == posT)
    //                dropObj = null;
    //            else
    //                posT = dropObj.InteractionObject.transform.position;

    //        }
    //        Smoothness();


            
    //    }

    //    private void LateUpdate()
    //    {
    //        inputSystem.OnLateUpdate();
    //    }

    //    private void OnDestroy()
    //    {
    //        if (mainCamera != null)
    //        {
    //            MainCameraTransform.SetTransform(normalCamera, false);
    //        }

    //        inputSystem.Dispose();
            
    //        inputSystem.OnZoom -= OnZoom;
    //        inputSystem.OnRotate -= OnRotate;
    //        inputSystem.OnPan -= OnPan;
    //        inputSystem.OnFeaturesDoubleTap -= OnFeaturesDoubleTap;
    //        inputSystem.OnFeaturesEnter -= OnFeaturesEnter;
    //        inputSystem.OnFeaturesExit -= OnFeaturesExit;
    //        inputSystem.OnFeaturesGrab -= OnFeaturesGrab;
    //        inputSystem.OnFeaturesRelease -= OnFeaturesRelease;
    //        inputSystem.OnFeaturesCrawling -= FeaturesCrawling;

    //        inputSystem.OnRaycastHit = null;
    //        inputSystem.OnIdle = null;
    //        inputSystem.OnIdleFilter = null;
    //    }

    //    #endregion

    //    #region 私有方法

    //    GameObject GetRaycastHit()
    //    {
    //        Ray ray = mainCamera.ScreenPointToRay(inputSystem.HandPoint);

    //        var tempHit = GetRaycastHit(ray);

    //        if (Stage.isTouchOnUI)
    //            return null;
            
    //        return tempHit.collider == null ? null : tempHit.collider.gameObject;
    //    }

    //    RaycastHit GetRaycastHit(Ray ray)
    //    {
    //        Physics.Raycast(ray, out var tempHit, (setting.maxDistance + 1), HuaUtilitys.LAYER_OBJECT);

    //        return tempHit;
    //    }

    //    /// <summary>
    //    /// 平滑镜头移动、旋转、缩放
    //    /// </summary>
    //    private void Smoothness()
    //    {
    //        //Anchor.position = new Vector3(targetPosition.x, targetPosition.y, targetDistance);

            

    //        position = Vector3.Lerp(position, targetPosition, Time.deltaTime *  (setting.panLerp +
    //            setting.panLerp *
    //            (Mathf.Clamp(HuaUtilitys.SystemConfig.PanSpeed, 1, 100) / 100.0f)));
            
    //        distanceZoom = Mathf.Lerp(distanceZoom, targetDistance, Time.deltaTime *  (setting.zoomLerp +
    //            setting.zoomLerp *
    //            (Mathf.Clamp(HuaUtilitys.SystemConfig.ZoomSpeed, 1, 100) / 100.0f)));

    //        rotation = Quaternion.Lerp(rotation, targetRotation, Time.deltaTime * (setting.rotateLerp +
    //            setting.rotateLerp *
    //            (Mathf.Clamp(HuaUtilitys.SystemConfig.RotateSpeed, 1, 100) / 100.0f)));

    //        // 设置摄像头位置
    //        MainCameraTransform.position = position - rotation * Vector3.forward*distanceZoom;

    //        // 设置摄像头旋转
    //        MainCameraTransform.rotation = rotation;



    //        if(Mathf.Abs(targetDistance - distanceZoom)>0.01f)
    //        {
    //            MessageDispatcher.SendMessage(EventMessage._CAMERA_ZOOM_DAMP);
    //        }

    //        if(position!=targetPosition)
    //        {
    //            MessageDispatcher.SendMessage(EventMessage._CAMERA_TRANSLATION_DAMP);
    //        }

    //        if((Mathf.Abs(rotation.eulerAngles.x-targetRotation.eulerAngles.x)>0.1f)|| (Mathf.Abs(rotation.eulerAngles.y - targetRotation.eulerAngles.y) > 0.1f) || (Mathf.Abs(rotation.eulerAngles.z - targetRotation.eulerAngles.z) > 0.1f))
    //        {
    //            MessageDispatcher.SendMessage(EventMessage._CAMERA_ROTATE_DAMP);
    //        }
            
    //    }

    //    /// <summary>
    //    /// 重置摄像头
    //    /// </summary>
    //    private void ResetCam()
    //    {
    //        targetRotation = MainCameraTransform.rotation;
    //        // 摄像机位置初始化
    //        targetPosition = new Vector3(MainCameraTransform.position.x, MainCameraTransform.position.y, Anchor.position.z);
    //        // 摄像机缩放初始化
    //        targetDistance = Anchor.position.z - MainCameraTransform.position.z;
    //        Smoothness();
    //    }

    //    /// <summary>
    //    /// 刷新位置
    //    /// </summary>
    //    private void Refresh()
    //    {

    //        // 摄像机旋转初始化
    //        rotation = targetRotation = MainCameraTransform.rotation;
    //        // 摄像机位置初始化
    //        position = targetPosition = new Vector3(MainCameraTransform.position.x, MainCameraTransform.position.y,Anchor.position.z);
    //        // 摄像机缩放初始化
    //        distanceZoom = targetDistance = Anchor.position.z - MainCameraTransform.position.z;
    //       // Smoothness();
    //    }


    //    private void ShowLabel(string labelName)
    //    {
    //        if (string.IsNullOrEmpty(labelName)) return;
    //        if (isShowLabel) return;

    //        MessageDispatcher.SendMessageData(EventMessage._SHOW_LABEL, labelName);
    //        isShowLabel = true;
    //    }

    //    private void HideLabel()
    //    {
    //        if (!isShowLabel) return;

    //        MessageDispatcher.SendMessage(EventMessage._HIDE_LABEL);
    //        isShowLabel = false;
    //    }

    //    private void UpdateShowdowPosition(Vector3 worldPos)
    //    {
    //        if (!shadowObject.activeSelf)
    //        {
    //            shadowObject.SetActive(true);
    //        }

    //        shadowObject.transform.position = new Vector3(worldPos.x,
    //            setting.minObjectTranslation.y, worldPos.z);

    //        shadowRender.material.SetColor("_BaseColor",
    //            new Color(1, 1, 1,
    //                Mathf.Clamp((fadeDis - (worldPos.y - setting.minObjectTranslation.y)) / fadeDis, 0, 0.3f)));
    //    }
        
    //    /// <summary>
    //    /// 查找最上层父物体
    //    /// </summary>
    //    /// <param name="zi"></param>
    //    /// <returns></returns>
    //    Transform FindUpParent(Transform zi)
    //    {
    //        if (zi.parent == null)
    //            return zi;
    //        else
    //            return FindUpParent(zi.parent);
    //    }

    //    #endregion

    //    #region 状态枚举方法

    //    bool IdleFilter(RaycastHit tempHit)
    //    {
    //        return Vector3.Distance(MainCameraTransform.position, tempHit.point) <= setting.maxDistance &&
    //               tempHit.transform.CompareTag(setting.tagObject);
    //    }

    //    /// <summary>
    //    /// 抓取中
    //    /// </summary>
    //    void FeaturesCrawling(IFeaturesOperate featuresOperate)
    //    {
    //        if (featuresOperate == null) return;

    //        if (!isMoving) return;

    //        if (featuresOperate.Features.ActiveLabel)
    //            HideLabel();
    //    }

    //    /// <summary>
    //    /// 抓取处理
    //    /// </summary>
        
    //    //鼠标与物体位置的偏移量
    //    Vector3 offset;

    //    LayerMask templayer;
    //    void FeaturesGrab(IFeaturesOperate featuresOperate)
    //    {
    //        if (!featuresOperate.Features.ActiveGrabMove) return;

    //        if (featuresOperate.Features.GrabIcon == MouseIcon.None)
    //        {
    //            featuresOperate.Features.GrabIcon = MouseIcon.Grab;
    //        }
            
    //        if (featuresOperate.Status != OperateStatus.Interaction)
    //            featuresOperate.Status = OperateStatus.Grab;

    //        //修改接收碰撞体的层
    //        if (featuresOperate.Features.ReceiveCollider != null)
    //        {
    //            templayer = featuresOperate.Features.ReceiveCollider.layer;
    //            featuresOperate.Features.ReceiveCollider.layer = 21;
    //        }
                
    //        //保存抓取物体是否在桌面上的状态
    //        deskT = IsDesktop(inputSystem.FeaturesOperate);
    //        if(featuresOperate.Location!=null)
    //        {
    //            if(featuresOperate.Location.CurrentLocationStatus==Experiment.EquipmentLocationStatus.Shelf)
    //            {
    //                deskT = true;
    //            }
    //        }
    //        //记录鼠标与物体位置的偏移量
    //        offset = GetOffset(featuresOperate);
    //        if (HuaUtilitys.OperationConfig != null)
    //        {
    //            switch ((TouchOffsetType)HuaUtilitys.OperationConfig.TouchOffsetType)
    //            {
    //                case TouchOffsetType.OffsetRange:
    //                    if (offset.x > featuresOperate.Features.Collider.size.x || offset.y > featuresOperate.Features.Collider.size.y)
    //                    {
    //                        offset = Vector3.zero;
    //                    }
    //                    break;
    //                case TouchOffsetType.NotOffset:
    //                    if (IsTouch)
    //                    {
    //                        offset = Vector3.zero;
    //                    }
    //                    break;
    //            }
    //        }
            
    //        MessageDispatcher.SendMessageData(EventMessage._MOUSE_ICON, featuresOperate.Features.GrabIcon);
    //        MessageDispatcher.SendMessageData(EventMessage._GRAB_OBJECT, featuresOperate.InteractionObject);

            

    //        var lMessage = new Message
    //        {
    //            Type = EventMessage._GRAB_OBJECT_KEY,
    //            Sender = this,
    //            Recipient = featuresOperate.InteractionObject,
    //            Delay = 0
    //        };
    //        //下一帧执行
    //        MessageDispatcher.SendMessage(lMessage);

            

    //        inputSystem.Status = CursorStatus.Crawling;
    //    }
    //    /// <summary>
    //    /// 释放处理
    //    /// </summary>
    //    void FeaturesRelease(IFeaturesOperate featuresOperate)
    //    {
    //        if (featuresOperate == null) return;
    //        if (featuresOperate.Features.ReceiveCollider != null)
    //            featuresOperate.Features.ReceiveCollider.gameObject.SetActive(true);

    //        //先发事件去处理
    //        MessageDispatcher.SendMessageData(EventMessage._RELEASE_OBJECT, featuresOperate.InteractionObject);

    //        Message lMessage = new Message();
    //        lMessage.Type = EventMessage._RELEASE_OBJECT_KEY;
    //        lMessage.Sender = this;
    //        lMessage.Recipient = featuresOperate.InteractionObject;
    //        lMessage.Delay = 0; //下一帧执行

    //        MessageDispatcher.SendMessage(lMessage);

    //        //修改回接收碰撞体的层
    //        if (featuresOperate.Features.ReceiveCollider != null)
    //        {
    //            featuresOperate.Features.ReceiveCollider.layer = templayer;
    //        }

    //        if (featuresOperate.Status == OperateStatus.Grab)
    //        {
    //            featuresOperate.Status = OperateStatus.Idle;
    //        }

    //        //释放的时候判断释放要将仪器放回架子
    //        Ray ray = mainCamera.ScreenPointToRay(inputSystem.HandPoint);
    //        if(featuresOperate.Location!=null)
    //        {
    //            if (featuresOperate.Location.InitialLocationStatus == Experiment.EquipmentLocationStatus.Shelf)
    //            {
    //                if (Physics.Raycast(ray, out var tempHit, (setting.maxDistance + 1), 1 << LayerMask.NameToLayer("Shelf"))&& featuresOperate.Status!= OperateStatus.Interaction&&!featuresOperate.Location.InitialTransform.IsLocal)
    //                {
    //                    if (tempHit.collider != null)
    //                    {
    //                        if(tempHit.collider.transform.GetComponent<MeshRenderer>()!=null)
    //                        {
    //                            tempHit.collider.transform.GetComponent<MeshRenderer>().enabled = true;
    //                        }
    //                        ReleaseShelfObj = featuresOperate;
    //                    }
    //                }
    //                else
    //                {
    //                    featuresOperate.Location.CurrentLocationStatus = Experiment.EquipmentLocationStatus.Shelf;
    //                }
    //            }
    //            //放到桌面时回复成默认状态
    //            if (featuresOperate.Features.IsRotate&& IsDesktop(featuresOperate)&& featuresOperate.Status != OperateStatus.Interaction)
    //            {
    //                if(featuresOperate.Location.InitialLocationStatus == Experiment.EquipmentLocationStatus.Shelf)
    //                {
    //                    featuresOperate.InteractionObject.transform.position = new Vector3(featuresOperate.InteractionObject.transform.position.x, featuresOperate.Location.InitialTransform.Position.UnityVector().y-(featuresOperate.Location.InitialTransform.Position.UnityVector().y- colliderDesk.bounds.min.y), featuresOperate.InteractionObject.transform.position.z);
    //                }
    //                else
    //                {
    //                    featuresOperate.InteractionObject.transform.position = new Vector3(featuresOperate.InteractionObject.transform.position.x, featuresOperate.Location.InitialTransform.Position.UnityVector().y, featuresOperate.InteractionObject.transform.position.z);
    //                }
    //                featuresOperate.InteractionObject.transform.rotation = Quaternion.Euler(featuresOperate.Location.InitialTransform.Rotate.UnityVector());
    //            }
    //        }
            

    //        //释放时如果在桌面，发送事件
    //        if (IsDesktop(featuresOperate))
    //        {
    //            MessageDispatcher.SendMessageData(EventMessage._OBJECT_PUTDOWN, featuresOperate);
    //        }
    //        //释放时如果不在桌面，且有发送碰撞体，且高于桌面一定高度，且该物体激活自动下落，则开始自动下落
    //        else if (featuresOperate.Features.SendCollider != null && featuresOperate.Features.SendBounds.min.y - colliderDesk.bounds.min.y < featuresOperate.Features.DropOutHeight && featuresOperate.Features.ActiveDropOut && featuresOperate.Status != OperateStatus.Interaction)
    //            dropObj = featuresOperate;
    //    }

    //    #endregion

    //    #region 事件方法

    //    /// <summary>
    //    /// 缩放
    //    /// </summary>
    //    /// <param name="zoomAxis"></param>
    //    void OnZoom(float zoomAxis)
    //    {
    //        if (IsCameraLock) return;

    //        //Debug.Log("缩放值：" + zoomAxis);

    //        // if (zoomAxis == 0)
    //        // {
    //        //     if (inputSystem.IsStatusSelf(CursorStatus.Zoom))
    //        //     {
    //        //         inputSystem.Status = CursorStatus.None;
    //        //         
    //        //         MessageDispatcher.SendMessage(EventMessage._CAMERA_ZOOM_END);
    //        //     }
    //        //
    //        //     return;
    //        // }

    //        if (!inputSystem.IsSelfOrNone(CursorStatus.Zoom)) return;

    //        //鼠标滚轮拉伸
    //        targetDistance -= zoomAxis * (setting.zoomSpeed +
    //                                      setting.zoomSpeed * (Mathf.Clamp(HuaUtilitys.SystemConfig.ZoomSpeed, 1, 100) / 100.0f)) ;

    //        //限制缩放范围
    //        targetDistance = Mathf.Clamp(targetDistance, setting.minZoom, setting.maxZoom);

    //        if (inputSystem.IsStatusNone())
    //        {
    //            inputSystem.Status = CursorStatus.Zoom;

    //            //发送开始事件
    //            MessageDispatcher.SendMessage(EventMessage._CAMERA_ZOOM_START);
    //        }
    //    }

    //    /// <summary>
    //    /// 旋转
    //    /// </summary>
    //    /// <param name="rotateAxis"></param>
    //    void OnRotate(Vector2 rotateAxis)
    //    {
    //        if (IsCameraLock) return;
    //        if (!inputSystem.IsSelfOrNone(CursorStatus.Rotate)) return;

    //        //旋转视角
    //        if (inputSystem.IsStatusNone())
    //        {
    //            inputSystem.Status = CursorStatus.Rotate;
    //            MessageDispatcher.SendMessageData(EventMessage._MOUSE_ICON, MouseIcon.Rotate);

    //            //MessageDispatcher.SendMessage(EventMessage._CAMERA_ROTATE);
    //        }

    //        float xAxis = rotateAxis.x;
    //        float yAxis = rotateAxis.y;

    //        ////异常波动
    //        //if (Mathf.Abs(xAxis) > 5f || Mathf.Abs(yAxis) > 5f)
    //        //{
    //        //    return;
    //        //}

    //        if (Mathf.Abs(xAxis) > Mathf.Abs(yAxis))
    //        {
    //            xAxis *= (setting.rotateSpeed + setting.rotateSpeed *
    //                (Mathf.Clamp(HuaUtilitys.SystemConfig.RotateSpeed, 1, 100) / 100.0f)) * Time.deltaTime;
    //        }
    //        if (Mathf.Abs(xAxis) < Mathf.Abs(yAxis))
    //        {
    //            yAxis *= (setting.rotateSpeed + setting.rotateSpeed *
    //                (Mathf.Clamp(HuaUtilitys.SystemConfig.RotateSpeed, 1, 100) / 100.0f)) * Time.deltaTime;
    //        }
    //        if (Mathf.Abs(xAxis) == Mathf.Abs(yAxis))
    //        {
    //            return;
    //        }
    //        //
    //        if (Mathf.Abs(xAxis) > 0 || Mathf.Abs(yAxis) > 0)
    //        {
    //            // 获取摄像机欧拉角
    //            Vector3 angles = MainCameraTransform.rotation.eulerAngles;
    //            // 欧拉角表示按照坐标顺序旋转，比如angles.x=30，表示按x轴旋转30°，dy改变引起x轴的变化
    //            angles.x = Mathf.Repeat(angles.x + 180f, 360f) - 180f;
    //            angles.y += xAxis;
    //            angles.x -= yAxis;
    //            angles.x = HuaUtilitys.ClampAngle(angles.x, setting.minAngleY, setting.maxAngleY);
    //            // 计算摄像头旋转
    //            targetRotation.eulerAngles = new Vector3(angles.x, angles.y, 0);
    //            //// 随着旋转，摄像头位置自动恢复
    //            //Vector3 temp_position =
    //            //    Vector3.Lerp(targetPosition, Anchor.position, Time.deltaTime * setting.moveLerp);
    //            //targetPosition = Vector3.Lerp(targetPosition, temp_position, Time.deltaTime * setting.moveLerp);
    //        }
    //        MessageDispatcher.SendMessageData(EventMessage._CAMERA_ROTATE, new Vector2(xAxis, yAxis));
    //    }

    //    /// <summary>
    //    /// 平移
    //    /// </summary>
    //    /// <param name="panAxis"></param>
    //    void OnPan(Vector2 panAxis)
    //    {
    //        float xAxis = panAxis.x;
    //        float yAxis = panAxis.y;

    //        if (!inputSystem.IsSelfOrNone(CursorStatus.Pan)) return;
    //        if (IsCameraLock) return;

    //        //旋转视角
    //        if (inputSystem.IsStatusNone())
    //        {
    //            inputSystem.Status = CursorStatus.Pan;
    //            MessageDispatcher.SendMessageData(EventMessage._MOUSE_ICON, MouseIcon.Pan);
    //            MessageDispatcher.SendMessage(EventMessage._CAMERA_TRANSLATION_START);
    //        }

    //        xAxis *= (setting.panSpeed +
    //                  setting.panSpeed * (Mathf.Clamp(HuaUtilitys.SystemConfig.PanSpeed, 1, 100) / 100.0f))/* *
    //                 Mathf.Abs(targetDistance / setting.defaultDistance)*/;
            
    //        yAxis *= (setting.panSpeed +
    //                  setting.panSpeed * (Mathf.Clamp(HuaUtilitys.SystemConfig.PanSpeed, 1, 100) / 100.0f)) /** Mathf.Abs(targetDistance / setting.defaultDistance)*/;
            
    //        targetPosition -= MainCameraTransform.up * yAxis + MainCameraTransform.right * xAxis;
    //        //限制移动范围
    //        float x = Mathf.Clamp(targetPosition.x, setting.minPan.x, setting.maxPan.x);
    //        float y = Mathf.Clamp(targetPosition.y, setting.minPan.y, setting.maxPan.y);
    //        float z = Mathf.Clamp(targetPosition.z, setting.minPan.z, setting.maxPan.z);
    //        targetPosition = new Vector3(x, y, z);

    //    }

    //    /// <summary>
    //    /// 聚焦
    //    /// </summary>
    //    /// <param name="focusValue"></param>
    //    void OnFeaturesDoubleTap(IFeaturesOperate featuresOperate)
    //    {
    //        if (featuresOperate == null) return;

    //        targetPosition = featuresOperate.InteractionObject.transform.position + new Vector3(0, featuresOperate.Features.FocusDistance.y, 0);

    //        targetDistance = featuresOperate.Features.FocusDistance.x;

    //        Anchor.position = new Vector3(targetPosition.x, targetPosition.y, targetDistance);

    //        //聚焦处理
    //        inputSystem.Status = CursorStatus.None;
    //    }

    //    //移入
    //    void OnFeaturesEnter(IFeaturesOperate featuresOperate)
    //    {
            
    //        inputSystem.FeaturesOperate = featuresOperate;

    //        //判断是否是触摸操作，触摸则放大碰撞体的大小
    //        if (IsTouch)
    //        {
    //            if (featuresOperate.Features.Collider != null)
    //            {
    //                CurrentColliderSize = featuresOperate.Features.Collider.size;
    //                featuresOperate.Features.Collider.size = featuresOperate.Features.Collider.size * HuaUtilitys.OperationConfig.colliderMultiple;
    //            }
    //        }

    //        HuaUtilitys.ShowHighlight(inputSystem.FeaturesOperate);

    //        if (inputSystem.FeaturesOperate.Features.ActiveLabel)
    //            //显示标签
    //            ShowLabel(inputSystem.FeaturesOperate.Features.DisplayName);

    //        if (inputSystem.FeaturesOperate.Features.EnterIcon == MouseIcon.None)
    //        {
    //            inputSystem.FeaturesOperate.Features.EnterIcon = MouseIcon.Enter;
    //        }

    //        MessageDispatcher.SendMessageData(EventMessage._MOUSE_ICON, inputSystem.FeaturesOperate.Features.EnterIcon);
    //    }

    //    //移出
    //    void OnFeaturesExit(IFeaturesOperate featuresOperate)
    //    {
    //        //如果是提示的话，则不隐藏就行
    //        if (featuresOperate.Features.TipsStatus == HighlightTipsStatus.None)
    //        {
    //            //隐藏高亮，隐藏标签
    //            HuaUtilitys.HideHighlight(featuresOperate);
    //        }

    //        //判断碰撞体是否放大了，放大了则变回原来大小
    //        if (featuresOperate.Features.Collider != null&&CurrentColliderSize!=Vector3.zero)
    //        {
    //            featuresOperate.Features.Collider.size = CurrentColliderSize;
    //            CurrentColliderSize = Vector3.zero;
    //        }


    //        MessageDispatcher.SendMessageData(EventMessage._MOUSE_ICON, inputSystem.FeaturesOperate.Features.IdleIcon);

    //        if (inputSystem.FeaturesOperate.Features.ActiveLabel)
    //        {
    //            HideLabel();
    //        }

    //        inputSystem.FeaturesOperate = null;
    //    }

    //    //抓取
    //    void OnFeaturesGrab(IFeaturesOperate featuresOperate)
    //    {
    //        if (featuresOperate == null) return;
            
    //        FeaturesGrab(featuresOperate);

    //        //判断是否是触摸抓取，是则增加距离检测点的范围
    //        if(IsTouch)
    //        {
    //            //遍历抓取物体的所有距离点
    //            for (int i = 0; i < featuresOperate.Features.Distance.interactions.Count; i++)
    //            {
    //                //判断是否是发送端
    //                if(featuresOperate.Features.Distance.interactions[i].distanceData.interactionType==InteractionType.Send)
    //                {
    //                    //获取所有发送端对应的接收端
    //                    List<DistanceInteraction> receives = DistanceStorage.GetReceiveDistanceData(featuresOperate.Features.Distance.interactions[i].distanceData.TagID);
    //                    //循环所有接收端，记录接收端的距离值
    //                    foreach (var item in receives)
    //                    {
    //                        if(!CurrentChangeDatas.ContainsKey(item))
    //                        {
    //                            CurrentChangeDatas.Add(item, new Dictionary<float, Vector3>());
    //                            //判断接收端是球形检测点还是方形检测点
    //                            if (item.distanceData.distanceShape==DistanceShape.Sphere)
    //                            {
    //                                CurrentChangeDatas[item].Add(item.distanceData.distanceValue, item.distanceData.Size);
    //                                item.distanceData.distanceValue = item.distanceData.distanceValue + item.distanceData.distanceValue * HuaUtilitys.OperationConfig.distanceSphereMulitiple;
    //                            }
    //                            else
    //                            {
    //                                CurrentChangeDatas[item].Add(item.distanceData.distanceValue, item.distanceData.Size);
    //                                item.distanceData.Size = item.distanceData.Size + item.distanceData.Size * HuaUtilitys.OperationConfig.distanceCubeMulitiple;
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }

    //        //获取当前操作的物体的最上层父物体
    //        operateParent = FindUpParent(featuresOperate.InteractionObject.transform);

    //        if (operateParent.transform.position.y < setting.minObjectTranslation.y + setting.maxDropDistance)
    //        {
    //            idelPos = featuresOperate.InteractionObject.transform.position;
    //        }

    //        //用另外的方式去移动
    //        _offset = HuaUtilitys.GetOffsetPosition(inputSystem.HandPoint,
    //            featuresOperate.InteractionObject);

    //        isMoving = true;

    //        rList = HuaUtilitys.CalculateBounds(featuresOperate.InteractionObject.transform);

    //        canShadow = !EventUpdateObject.IsHandlerKey(featuresOperate.InteractionObject);

    //        if (canShadow)
    //        {
    //            canShadow = featuresOperate.Features.ActiveShadow;

    //            if (shadowObject != null && canShadow)
    //            {
    //                shadowObject.transform.localScale = featuresOperate.Features.ShadowObjectScale;
    //            }
    //        }

    //        HuaUtilitys.HideHighlight(featuresOperate);
    //    }

    //    //释放
    //    void OnFeaturesRelease(IFeaturesOperate featuresOperate)
    //    {
    //        if (featuresOperate == null) return;
            
    //        FeaturesRelease(featuresOperate);

    //        //在处理该物体的一些状态
    //        isMoving = false;

    //        //释放的时候判断释放有改变的距离点，有则缩小为原来的大小
    //        if(CurrentChangeDatas.Count!=0)
    //        {
    //            foreach (var item in CurrentChangeDatas)
    //            {
    //                if(item.Key.distanceData.distanceShape == DistanceShape.Sphere)
    //                {
    //                    foreach (var temp in item.Value)
    //                    {
    //                        item.Key.distanceData.distanceValue = temp.Key;
    //                    }
    //                }
    //                else
    //                {
    //                    foreach (var temp in item.Value)
    //                    {
    //                        item.Key.distanceData.Size = temp.Value;
    //                    }
    //                }
    //            }
    //            CurrentChangeDatas.Clear();
    //        }


    //        //判断碰撞体是否放大了，放大了则变回原来大小
    //        if (featuresOperate.Features.Collider != null && CurrentColliderSize != Vector3.zero)
    //        {
    //            featuresOperate.Features.Collider.size = CurrentColliderSize;
    //            CurrentColliderSize = Vector3.zero;
    //        }

    //        //继续显示高亮
    //        if (featuresOperate.Features.TipsStatus == HighlightTipsStatus.HighlightTips)
    //            HuaUtilitys.ShowHighlight(featuresOperate);
    //        else
    //        {
    //            HuaUtilitys.HideHighlight(featuresOperate);
    //        }
            
    //        HideLabel();
    //        MessageDispatcher.SendMessageData(EventMessage._MOUSE_ICON, inputSystem.FeaturesOperate.Features.IdleIcon);

    //        rList.Clear();

    //        shadowObject.SetActive(false);
    //        idleDelay = 0.05f;
    //    }

    //    #endregion

    //    #region 静态方法

    //    public static void RotateCamera(Vector3 rot, Vector3 pos, float dis)
    //    {
    //        if (Instance == null) return;
    //        Instance.targetPosition = pos;
    //        Instance.targetRotation.eulerAngles = rot;
    //        Instance.targetDistance = dis;

    //        Instance.Anchor.position = new Vector3(Instance.targetPosition.x, Instance.targetPosition.y, Instance.targetDistance);
    //        // Instance.Smoothness();
    //    }

    //    public static void RotateCamera(Vector3 rot)
    //    {
    //        if (Instance == null) return;
    //        Instance.targetRotation.eulerAngles = rot;
 
    //        // Instance.Smoothness();
    //    }

    //    public static void FoucsFun(Vector3 rot,Vector3 pos,Vector2 XY)
    //    {
    //        Instance.targetRotation.eulerAngles = rot;

    //        Instance.targetPosition = pos + new Vector3(0, XY.y, 0);

    //        Instance.targetDistance = XY.x;

    //        Instance.Anchor.position = new Vector3(Instance.targetPosition.x, Instance.targetPosition.y, Instance.targetDistance);

    //        //聚焦处理
    //        Instance.inputSystem.Status = CursorStatus.None;
    //    }

    //    public static void MoveCamera(Vector3 pos)
    //    {
    //        if (Instance == null) return;
    //        Instance.targetPosition = pos;
    //       // Instance.Smoothness();
    //    }

    //    /// <summary>
    //    /// 重置摄像机位置
    //    /// </summary>
    //    public static void ResetCamera()
    //    {
    //        if (Instance == null) return;

    //        Instance.ResetCam();
    //    }

    //    public static void SetCameraLock(bool isLock)
    //    {
    //        if (Instance == null) return;

    //        Instance.IsCameraLock = isLock;
    //    }

    //    public static float CheckAngle(float value) // 将大于180度角进行以负数形式输出
    //    {
    //        float angle = value - 180;

    //        if (angle > 0)
    //        {
    //            return angle - 180;
    //        }

    //        if (value == 0)
    //        {
    //            return 0;
    //        }

    //        return angle + 180;
    //    }

    //    #endregion

    //   #region 移动模块
    //    //鼠标与物体偏移
    //    Vector3 GetOffset(IFeaturesOperate featuresOperate)
    //    {
    //        switch (featuresOperate.Features.moveMod)
    //        {
    //            case MoveModel.Screen:
    //                return featuresOperate.InteractionObject.transform.position - ScreenTargetPosition(featuresOperate.InteractionObject.transform.position);
    //            case MoveModel.World:
    //                return featuresOperate.InteractionObject.transform.position - WorldTargetPosition(featuresOperate.InteractionObject.transform.position);
    //            case MoveModel.Mixed:
    //            default:
    //                return featuresOperate.InteractionObject.transform.position - ScreenAndWorldTargetPosition(featuresOperate.InteractionObject.transform.position);
    //        }
    //    }
    //    //鼠标移动
    //    Vector3 GetMovePosition(IFeaturesOperate featuresOperate, Vector3 offset)
    //    {
    //        switch (featuresOperate.Features.moveMod)
    //        {
    //            case MoveModel.Screen:
    //                return LimitPosition(ScreenCollision(ScreenTargetPosition(featuresOperate.InteractionObject.transform.position) + offset, featuresOperate), featuresOperate);
    //            case MoveModel.World:
    //                return LimitPosition(WorldCollision(WorldTargetPosition(featuresOperate.InteractionObject.transform.position) + offset, featuresOperate), featuresOperate);
    //            case MoveModel.Mixed:
    //            default:
    //                return LimitPosition(ScreenAndWorldCollision(ScreenAndWorldTargetPosition(featuresOperate.InteractionObject.transform.position) + offset, featuresOperate), featuresOperate);
    //        }
    //    }
    //    //自动下落物体
    //    Vector3 GetDropPosition(IFeaturesOperate featuresOperate)
    //    {
    //        return LimitPosition(WorldCollision(featuresOperate.InteractionObject.transform.position - new Vector3(0, dropSpeed * Time.fixedDeltaTime, 0), featuresOperate), featuresOperate);
    //    }
    //    //移动物体，混合模式
    //    Vector3 ScreenAndWorldTargetPosition(Vector3 pos)
    //    {
    //        Vector3 cv3 = mainCamera.transform.forward;
    //        if (Mathf.Abs(cv3.y) > 0.8f || Mathf.Abs(cv3.z) > 0.8f || Mathf.Abs(cv3.x) > 0.8f)
    //        {
    //            return WorldTargetPosition(pos);
    //        }
    //        else
    //        {
    //            return ScreenTargetPosition(pos);
    //        }
    //    }
    //    //移动物体，物体距相机的深度保持不变
    //    Vector3 ScreenTargetPosition(Vector3 pos)
    //    {
    //        return mainCamera.ScreenToWorldPoint(new Vector3(inputSystem.HandPoint.x, inputSystem.HandPoint.y, mainCamera.WorldToScreenPoint(pos).z));
    //    }
    //    //移动物体，根据相机角度锁定轴
    //    Vector3 WorldTargetPosition(Vector3 pos)
    //    {
    //        Vector3 cv3 = mainCamera.transform.forward;
    //        Vector3 v = (mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1)) - mainCamera.transform.position).normalized;
    //        if (Mathf.Abs(cv3.y) >= Mathf.Abs(cv3.z) && Mathf.Abs(cv3.y) >= Mathf.Abs(cv3.x))
    //        {
    //            pos = Camera.main.transform.position + v * (pos.y - Camera.main.transform.position.y) / v.y;
    //        }
    //        else if (Mathf.Abs(cv3.z) >= Mathf.Abs(cv3.x) && Mathf.Abs(cv3.z) > Mathf.Abs(cv3.y))
    //        {
    //            pos = Camera.main.transform.position + v * (pos.z - Camera.main.transform.position.z) / v.z;
    //        }
    //        //else if (Mathf.Abs(cv3.x) > Mathf.Abs(cv3.y) && Mathf.Abs(cv3.x) > Mathf.Abs(cv3.z))
    //        else
    //        {
    //            pos = Camera.main.transform.position + v * (pos.x - Camera.main.transform.position.x) / v.x;
    //        }
    //        return pos;
    //    }
        
    //    public Vector3 LimitPosition(Vector3 target, IFeaturesOperate featuresOperate)
    //    {
    //        Bounds boundsDesk = colliderDesk.bounds;
    //        Bounds sendBounds = featuresOperate.Features.SendCollider == null ? featuresOperate.Features.Collider.bounds : featuresOperate.Features.SendBounds;
    //        return new Vector3(
    //            Mathf.Clamp(target.x, boundsDesk.min.x + (featuresOperate.InteractionObject.transform.position.x - sendBounds.min.x), boundsDesk.max.x + (featuresOperate.InteractionObject.transform.position.x - sendBounds.max.x)),
    //            Mathf.Clamp(target.y, boundsDesk.min.y + (featuresOperate.InteractionObject.transform.position.y - sendBounds.min.y), boundsDesk.max.y + (featuresOperate.InteractionObject.transform.position.y - sendBounds.max.y)),
    //            Mathf.Clamp(target.z, boundsDesk.min.z + (featuresOperate.InteractionObject.transform.position.z - sendBounds.min.z), boundsDesk.max.z + (featuresOperate.InteractionObject.transform.position.z - sendBounds.max.z))
    //        );
    //    }
    //    //混合碰撞
    //    Vector3 ScreenAndWorldCollision(Vector3 target, IFeaturesOperate featuresOperate)
    //    {
    //        Vector3 cv3 = mainCamera.transform.forward;
    //        if (Mathf.Abs(cv3.y) > 0.8f || Mathf.Abs(cv3.z) > 0.8f || Mathf.Abs(cv3.x) > 0.8f)
    //        {
    //            return WorldCollision(target, featuresOperate);
    //        }
    //        else
    //        {
    //            return ScreenCollision(target, featuresOperate);
    //        }
    //    }
    //    //屏幕空间物体碰撞
    //    Vector3 ScreenCollision(Vector3 target, IFeaturesOperate featuresOperate)
    //    {
    //        if (featuresOperate.Features.SendCollider == null) return target;
    //        Vector3 dis = target - featuresOperate.InteractionObject.transform.position;
    //        dis = MainCameraTransform.InverseTransformVector(dis);
    //        if (dis.x != 0)
    //        {
    //            dis.x = RayTest(featuresOperate.Features.SendBounds, MainCameraTransform.right, dis.x, featuresOperate.Features.GetSendLayerMask());
    //        }
    //        if (dis.y != 0)
    //        {
    //            dis.y = RayTest(featuresOperate.Features.SendBounds, MainCameraTransform.up, dis.y, featuresOperate.Features.GetSendLayerMask());
    //        }
    //        if (dis.z != 0)
    //        {
    //            dis.z = RayTest(featuresOperate.Features.SendBounds, MainCameraTransform.forward, dis.z, featuresOperate.Features.GetSendLayerMask());
    //        }
    //        if (Physics.BoxCast(featuresOperate.Features.SendBounds.center, featuresOperate.Features.SendBounds.extents, dis, out RaycastHit info, Quaternion.identity, dis.magnitude, featuresOperate.Features.GetSendLayerMask()))
    //            dis = dis.normalized * (info.distance - padding);

    //        return featuresOperate.InteractionObject.transform.position + MainCameraTransform.TransformVector(dis);
    //    }

    //    //世界空间物体碰撞
    //    Vector3 WorldCollision(Vector3 target, IFeaturesOperate featuresOperate)
    //    {
    //        if (featuresOperate.Features.SendCollider == null) return target;
    //        Vector3 dis = target - featuresOperate.InteractionObject.transform.position;
    //        if (dis.x != 0)
    //        {
    //            dis.x = RayTest(featuresOperate.Features.SendBounds, MainCameraTransform.right, dis.x, featuresOperate.Features.GetSendLayerMask());
    //        }
    //        if (dis.y != 0)
    //        {
    //            dis.y = RayTest(featuresOperate.Features.SendBounds, MainCameraTransform.up, dis.y, featuresOperate.Features.GetSendLayerMask());
    //        }
    //        if (dis.z != 0)
    //        {
    //            dis.z = RayTest(featuresOperate.Features.SendBounds, MainCameraTransform.forward, dis.z, featuresOperate.Features.GetSendLayerMask());
    //        }
    //        if (Physics.BoxCast(featuresOperate.Features.SendBounds.center, featuresOperate.Features.SendBounds.extents, dis, out RaycastHit info, Quaternion.identity, dis.magnitude, featuresOperate.Features.GetSendLayerMask()))
    //            dis = dis.normalized * (info.distance - padding);

    //        return featuresOperate.InteractionObject.transform.position + dis;
    //    }
    //    float RayTest(Bounds bounds, Vector3 direction, float distance, int layerMask)
    //    {
    //        float value = Mathf.Abs(distance);
    //        float sign = Mathf.Sign(distance);
    //        if (Physics.BoxCast(bounds.center, bounds.extents, direction * sign, out RaycastHit info, Quaternion.identity, value, layerMask))
    //            value = Mathf.Min(info.distance - padding, value);
    //        return value * sign;
    //    }

    //    //检测物体是否放置桌面
    //    public bool IsDesktop(IFeaturesOperate featuresOperate)
    //    {
    //        Bounds sendBounds = featuresOperate.Features.SendCollider == null ? featuresOperate.Features.Collider.bounds : featuresOperate.Features.SendBounds;
    //        return Mathf.Abs(sendBounds.min.y - colliderDesk.bounds.min.y) < 0.01f;
    //    }
    //    [Tooltip("碰撞缓冲距离")]
    //    public float padding = 0.01f;
    //    [Tooltip("下落速度")]
    //    public float dropSpeed = 2f;

    //    #endregion
    //}
}