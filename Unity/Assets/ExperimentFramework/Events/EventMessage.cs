using UnityEngine;

namespace ExperimentFramework.Events
{
    public partial class EventMessage
    {
        
        /// <summary>
        /// 射线命令
        /// </summary>
        public const string _RAY_TARGET = "RayTarget";

        public const string _RAY_OBJECT_TARGET = "RayObject";
        
        /// <summary>
        /// 抓取物体命令
        /// </summary>
        public const string _GRAB_OBJECT = "GrabObject";
        
        /// <summary>
        /// 抓取指定物体命令
        /// </summary>
        public const string _GRAB_OBJECT_KEY = "GrabObjectKey";
        
        /// <summary>
        /// 释放物体命令
        /// </summary>
        public const string _RELEASE_OBJECT = "ReleaseObject";
        
        /// <summary>
        /// 释放指定物体命令
        /// </summary>
        public const string _RELEASE_OBJECT_KEY = "ReleaseObjectKey";

        /// <summary>
        /// 摄像机缩放命令-开始
        /// </summary>
        public const string _CAMERA_ZOOM_START = "CameraZoomStart";
        
        /// <summary>
        /// 摄像机缩放命令
        /// </summary>
        public const string _CAMERA_ZOOM = "CameraZoom";

        /// <summary>
        /// 摄像机缩放命令-结束
        /// </summary>
        public const string _CAMERA_ZOOM_END = "CameraZoomEnd";

        /// <summary>
        /// 摄像机旋转命令-开始
        /// </summary>
        public const string _CAMERA_ROTATE_START = "CameraRotateStart";

        /// <summary>
        /// 摄像机旋转命令
        /// </summary>
        public const string _CAMERA_ROTATE = "CameraRotate";

        /// <summary>
        /// 摄像机旋转命令-结束
        /// </summary>
        public const string _CAMERA_ROTATE_END = "CameraRotateEnd";

        /// <summary>
        /// 摄像机平移命令-开始
        /// </summary>
        public const string _CAMERA_TRANSLATION_START = "CameraTranslationStart";

        /// <summary>
        /// 摄像机平移命令
        /// </summary>
        public const string _CAMERA_TRANSLATION = "CameraTranslation";

        /// <summary>
        /// 摄像机平移命令-结束
        /// </summary>
        public const string _CAMERA_TRANSLATION_END = "CameraTranslationEnd";

        /// <summary>
        /// 摄像机旋转阻尼命令
        /// </summary>
        public const string _CAMERA_ROTATE_DAMP = "CameraRotateDamp";

        /// <summary>
        /// 摄像机移动阻尼命令
        /// </summary>
        public const string _CAMERA_TRANSLATION_DAMP = "CameraTranslationDamp";

        /// <summary>
        /// 摄像机缩放阻尼命令
        /// </summary>
        public const string _CAMERA_ZOOM_DAMP = "CameraZoomDamp";

        /// <summary>
        /// 摄像机重置命令
        /// </summary>
        public const string _CAMERA_RESET = "CameraReset";

        /// <summary>
        /// 手势按下
        /// </summary>
        public const string _HAND_DOWN = "HandDown";
        
        /// <summary>
        /// 手势抬起
        /// </summary>
        public const string _HAND_UP = "HandUp";

        public const string _MOUSE_ICON = "mouseIcon";

        /// <summary>
        /// 物体放置桌面
        /// </summary>
        public const string _OBJECT_PUTDOWN = "ObjcectPutDown";
        /// <summary>
        /// 物体离开桌面
        /// </summary>
        public const string _OBJECT_PICKUP = "ObjectPickUp";

        public const string _UPDATE_SCENE = "UpdateScene";

        public const string _ACTIVE_SHELF = "ActiveShelf";

        /// <summary>
        /// 仪器移入架子命令
        /// </summary>
        public const string _SHELF_ENTER = "Shelf_Enter";

        /// <summary>
        /// 仪器离开架子命令
        /// </summary>
        public const string _SHELF_EXIT = "Shelf_Exit";

        #region 实验部分事件
        /// <summary>
        /// 整理器材命令
        /// </summary>
        public const string _FINISHING_EQUIPMENT = "FinishingEquipment";

        /// <summary>
        /// 实验开始
        /// </summary>
        public const string _EXPERIMENT_START = "ExperimentStart";
        
        /// <summary>
        /// 实验重做命令
        /// </summary>
        public const string _EXPERIMENT_REDO = "ExperimentRedo";

        /// <summary>
        /// 实验结束命令
        /// </summary>
        public const string _EXPERIMENT_END = "ExperimentEnd";

        /// <summary>
        /// 实验计时
        /// </summary>
        public const string _EXPERIMENT_TIMING = "ExperimentTiming";
        
        /// <summary>
        /// 提交实验命令
        /// </summary>
        public const string _EXPERIMENT_SUBMIT = "ExperimentSubmit";

        /// <summary>
        /// 删除仪器
        /// </summary>
        public const string _DELETE_EQUIPMENT = "DeleteEquipment";
        
        /// <summary>
        /// 操作确定
        /// </summary>
        public const string _NORMAL_OPERATE = "OperateVerify";
        /// <summary>
        /// 器材确定
        /// </summary>
        public const string _EQUIPMENT_VERIFY = "EquipmentVerify";

        /// <summary>
        /// 初始化仪器提示
        /// </summary>
        public const string _INIT_EQUIPMENT_TIPS = "InitEquipmentTips";

        /// <summary>
        /// 命令参数
        /// </summary>
        public const string _COMMAND_PARMES = "CommandParmes";

        /// <summary>
        /// 仪器提示
        /// </summary>
        public const string _EQUIPMENT_TIPS = "EquipmentTips";

        /// <summary>
        /// 仪器创建
        /// </summary>
        public const string _EQUIPMENT_CREATE = "EquipmentCreate";

        /// <summary>
        /// 仪器释放
        /// </summary>
        public const string _EQUIPMENT_RELEASE = "EquipmentRelease";

        /// <summary>
        /// 动作命令完成
        /// </summary>
        public const string _ACTION_COMMAND_FINISHING = "ActionCommandFinish";

        /// <summary>
        /// 显示实验手
        /// </summary>
        public const string _SHOW_EXPERIMENT_HAND = "ShowExperimentHand";
        /// <summary>
        /// 隐藏实验手
        /// </summary>
        public const string _HIDE_EXPERIMENT_HAND = "HideExperimentHand";
        
        #endregion
    }
}
