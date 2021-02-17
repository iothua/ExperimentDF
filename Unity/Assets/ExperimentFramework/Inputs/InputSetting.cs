using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ExperimentFramework.Inputs
{
    [Serializable]
    public class InputSetting
    {
        /// <summary>
        /// 抓取识别的Tag
        /// </summary>
        [ReadOnly]
        public string tagObject = "grab";

        /// <summary>
        /// 玩家离相机的最大距离
        /// </summary>
        [LabelText("玩家离相机最大距离")]
        [Range(5.0f, 100.0f)]
        public float maxDistance = 10;

        /// <summary>
        /// 物体最大掉落距离，超出这个距离则不掉下
        /// </summary>
        [LabelText("物体最大掉落距离")]
        [Tooltip("超出这个距离则不掉下")]
        public float maxDropDistance = 0.2f;

        /// <summary>
        /// 镜头旋转速度
        /// </summary>
        [Title("摄像机设置")]
        [LabelText("镜头旋转速度")]
        public float rotateSpeed = 40f;

        /// <summary>
        /// 镜头旋转阻尼
        /// </summary>
        [LabelText("镜头旋转阻尼")]
        public float rotateLerp = 4;

        /// <summary>
        /// 镜头移动速度
        /// </summary>
        [LabelText("镜头平移速度")]
        public float panSpeed = 0.1f;

        /// <summary>
        /// 镜头移动阻尼
        /// </summary>
        [LabelText("镜头平移阻尼")]
        public float panLerp = 5f;

        /// <summary>
        /// 镜头缩放速度
        /// </summary>
        [LabelText("镜头缩放速度")]
        public float zoomSpeed = 1f;

        /// <summary>
        /// 镜头缩放阻尼
        /// </summary>
        [LabelText("镜头缩放阻尼")]
        public float zoomLerp = 1f;

        /// <summary>
        /// 默认离中心点的距离
        /// </summary>
        [LabelText("默认离中心点的距离")]
        public float defaultDistance = 1f;

        /// <summary>
        /// 镜头Y轴旋转范围
        /// </summary>
        [LabelText("镜头Y轴最小旋转范围")]
        public float minAngleY = -10f;
        [LabelText("镜头Y轴最大旋转范围")]
        public float maxAngleY = 60f;

        /// <summary>
        /// 镜头缩放范围
        /// </summary>
        [LabelText("镜头缩放最小值")]
        public float minZoom = -2f;
        [LabelText("镜头缩放最大值")]
        public float maxZoom = 2f;

        /// <summary>
        /// 镜头平移范围
        /// </summary>
        [LabelText("镜头平移最小范围")]
        public Vector3 minPan = new Vector3(-3, 0.5f, -3);
        [LabelText("镜头平移最大范围")]
        public Vector3 maxPan = new Vector3(3, 4, 3);

        /// <summary>
        /// 移动范围限制
        /// </summary>
        [LabelText("物体移动最小范围")]
        public Vector3 minObjectTranslation = new Vector3(-2, 1.1624f, -0.55f);

        /// <summary>
        /// 桌面操作位置
        /// </summary>
        [LabelText("桌面操作位置")]
        public float DesktopOperationLocation = -0.11f;

        /// <summary>
        /// 仪器Z轴在摄像机前的位置
        /// </summary>
        [LabelText("仪器Z轴在摄像机前的位置")]
        public float CameraFrontLocation = 0.15f;

        /// <summary>
        /// 仪器Z轴移动的速度
        /// </summary>
        [LabelText("仪器Z轴移动的速度")]
        public float DesktopOperationSpeed = 10f;
    }
}