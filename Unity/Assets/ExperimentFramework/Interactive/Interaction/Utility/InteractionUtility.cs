using ExperimentFramework.Interactive.Distance;
using System;
using UnityEngine;

namespace ExperimentFramework.Interactive
{
    /// <summary>
    /// 距离公用
    /// </summary>
    public static class InteractionUtility
    {
        private static Camera interactionCamera;
        public static Camera InteractionCamera
        {
            get
            {
                if (interactionCamera == null)
                {
                    //interactionCamera = GameObject.FindWithTag("InteractionCamera").GetComponent<Camera>();
                    
                    if (interactionCamera == null)
                    
                        interactionCamera = Camera.main;
                    //interactionCamera = Camera.main;

                }


                return interactionCamera;
            }
            set { interactionCamera = value; }
        }

        /// <summary>
        /// 距离判断
        /// </summary>
        /// <param name="v1">距离检测1</param>
        /// <param name="v2">距离检测2</param>
        /// <param name="distanceType">类型</param>
        /// <returns></returns>
        public static float Distance(Vector3 v1, Vector3 v2, DistanceType distanceType,out float distanceValue)
        {
            
            switch (distanceType)
            {
                case DistanceType.D3D:
                    return distanceValue= Vector3.Distance(v1, v2);
                case DistanceType.D2D:
                    // distanceValue = Vector2.Distance(v1, v2);
                    //
                    // //Debug.Log("距离检测值：" + distanceValue);
                    //
                    // return distanceValue;

                    // Vector2 v21 = Camera.main.WorldToScreenPoint(v1);
                    // Vector2 v22 = Camera.main.WorldToScreenPoint(v2);
                    // float zoom = 750 / Mathf.Sqrt(Mathf.Pow(1920, 2) + Mathf.Pow(1080, 2));
                    // zoom = Mathf.Sqrt(Mathf.Pow(Screen.height, 2) + Mathf.Pow(Screen.width, 2)) * zoom;
                    //
                    // return distanceValue = Vector3.Distance(v21, v22) / zoom;
                case DistanceType.DScreen:
                    Vector3 screen1 = InteractionCamera.WorldToScreenPoint(v1);
                    Vector3 screen2 = InteractionCamera.WorldToScreenPoint(v2);

                    distanceValue = Vector2.Distance(screen1, screen2);

                    //Debug.LogFormat("screen:{0}-- screen1:{1} -- distance: {2}", screen1, screen2, distanceValue);
                    
                    return distanceValue;

                default:
                    return distanceValue = -1;
            }
        }

        /// <summary>
        /// 判断是否在立方体内
        /// </summary>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <param name="targetPosition"></param>
        /// <param name="distanceType"></param>
        /// <returns></returns>
        public static bool CubeDistance(Vector3 position, Vector3 size,Vector3 targetPosition, DistanceType distanceType,out float distanceValue)
        {
            Distance(position, targetPosition, distanceType, out distanceValue);

            switch (distanceType)
            {
                case DistanceType.D3D:

                    float d3d_xMin = position.x - size.x / 2;
                    float d3d_xMax = position.x + size.x / 2;

                    float d3d_yMin = position.y - size.y / 2;
                    float d3d_yMax = position.y + size.y / 2;

                    float d3d_zMin = position.z - size.z / 2f;
                    float d3d_zMax = position.z + size.z / 2f;

                    return targetPosition.x.FloatContains(d3d_xMin, d3d_xMax) && targetPosition.y.FloatContains(d3d_yMin, d3d_yMax)
                        && targetPosition.z.FloatContains(d3d_zMin, d3d_zMax);

                case DistanceType.D2D:
                    float xMin = position.x - size.x / 2;
                    float xMax = position.x + size.x / 2;
                    
                    float yMin = position.y - size.y / 2;
                    float yMax = position.y + size.y / 2;
                    
                    return targetPosition.x.FloatContains(xMin, xMax) && targetPosition.y.FloatContains(yMin, yMax);

                case DistanceType.DScreen:

                    //计算两个坐标的屏幕坐标
                    Vector3 screen1 = InteractionCamera.WorldToScreenPoint(position);
                    Vector3 screen2 = InteractionCamera.WorldToScreenPoint(targetPosition);

                    //进行放大
                    //var screenSize = size * InteractiveController.screenInteractiveCoefficient;
                    
                    //在计算范围
                    float sizeScreenX = Mathf.Abs(
                        InteractionCamera
                            .WorldToScreenPoint(new Vector3(position.x + size.x / 2.0f, position.y, position.z)).x
                        - InteractionCamera
                            .WorldToScreenPoint(new Vector3(position.x - size.x / 2.0f, position.y, position.z))
                            .x);
                    
                    float sizeScreenY = Mathf.Abs(
                        InteractionCamera
                            .WorldToScreenPoint(new Vector3(position.x, position.y + size.y / 2.0f, position.z)).y
                        - InteractionCamera
                            .WorldToScreenPoint(new Vector3(position.x, position.y - size.y / 2.0f, position.z))
                            .y);

                    return ScreenPointContains(screen1, new Vector2(sizeScreenX, sizeScreenY), screen2);

                default:
                    return false;
            }
        }
        
        
        /// <summary>
        /// 判断坐标是否在指定物体内
        /// </summary>
        /// <param name="position">物体屏幕坐标</param>
        /// <param name="size">物体的大小</param>
        /// <param name="handPotion">手屏幕坐标</param>
        /// <returns></returns>
        public static bool ScreenPointContains(Vector2 position,Vector2 size,Vector2 handPotion)
        {
            //X轴比较
            float xMin = position.x - size.x / 2;
            float xMax = position.x + size.x / 2;

            float yMin = position.y - size.y / 2;
            float yMax = position.y + size.y / 2;

            return handPotion.x.FloatContains(xMin,xMax) && handPotion.y.FloatContains(yMin,yMax);
        }

        /// <summary>
        /// 比较float值是否在指定范围内
        /// </summary>
        /// <param name="value">指定值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>在范围内，返回True，否则返回false</returns>
        public static bool FloatContains(this float value,float min,float max)
        {
            return value >= min && value <= max;

        }

        /// <summary>
        /// 是否存在区域内
        /// </summary>
        /// <param name="transform">指定物体</param>
        /// <param name="handIndex"></param>
        /// <returns></returns>
        public static bool IsAreaContains(Transform transform,Vector3 screenHandPoint)
        {
            //if (!KinectTransfer.IsHandActive(handIndex)) return false;

            try
            {

                Vector3 screenPoint = InteractionCamera.WorldToScreenPoint(transform.position);

                //根据自身此时的屏幕坐标，去算区域

                RectTransform rectTransform = transform.GetComponent<RectTransform>();

                return ScreenPointContains(screenPoint,rectTransform.sizeDelta,screenHandPoint);
            }
            catch (Exception)
            {
                return false;
                //throw new Exception("手势可能没激活，如果是在编辑器上遇到此问题，不用理会");
            }
        }

        /// <summary>
        /// 是否存在区域内
        /// </summary>
        /// <param name="screenPoint">屏幕坐标</param>
        /// <param name="size">大小</param>
        /// <param name="handIndex">手势</param>
        /// <returns></returns>
        public static bool IsAreaContains(Vector2 screenPoint,Vector2 size,Vector3 screenHandPoint)
        {
            //if (!KinectTransfer.IsHandActive(handIndex)) return false;

            //获取到此时手的屏幕坐标屏幕坐标

            //根据自身此时的屏幕坐标，去算区域

            return ScreenPointContains(screenPoint,size,screenHandPoint);
        }

        /// <summary>
        /// 返回是否在屏幕内
        /// </summary>
        /// <param name="thingAttach"></param>
        /// <returns></returns>
        public static bool AttachThingPosInCamera(Transform thingAttach,Vector2 xlimits,Vector2 ylimits)
        {
            Transform camTransform =InteractionCamera.transform;

            Vector3 dir = (thingAttach.position - camTransform.position).normalized;

            float dot = Vector3.Dot(camTransform.forward,dir);     //判断物体是否在相机前面  

            Vector2 screenPos = InteractionCamera.WorldToScreenPoint(thingAttach.position);

            if (screenPos.x < xlimits.y &&
                screenPos.x > xlimits.x &&
                screenPos.y < ylimits.y &&
                screenPos.y > ylimits.x
               && dot > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
    }
}
