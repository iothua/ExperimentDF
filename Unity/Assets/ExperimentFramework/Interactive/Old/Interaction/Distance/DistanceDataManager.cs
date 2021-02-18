using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace ExperimentFramework.Interactive.Distance
{
    /// <summary>
    /// 距离数据管理
    /// </summary>
    public class DistanceDataManager
    {
        public DistanceInteraction sendData; //主动点对应的距离对象信息

        public List<DistanceInteraction> Distances;//被动点的所有集合,包括主动点对象

        public List<DistanceInteraction> Distanceing;

        private DistanceInteraction tempDistance = null;
        private float tempValue = 0;

        public DistanceDataManager()
        {
            //sendData = new DistanceInteraction();
            Distances = new List<DistanceInteraction>();
            Distanceing = new List<DistanceInteraction>();
        }

        /// <summary>
        /// 加入距离
        /// </summary>
        /// <param name="distance"></param>
        public void AddDistance(DistanceInteraction distance)
        {
            switch (distance.distanceData.interactionType)
            {
                case InteractionType.Receive:

                    if (sendData.InteractionObject.Equals(distance.InteractionObject))
                        return;

                    for (int i = 0; i < Distances.Count; i++)
                    {
                        if (Distances[i].Equals(distance)) return;
                    }

                    Distances.Add(distance);

                    break;
                case InteractionType.Send:
                case InteractionType.Pour:
                case InteractionType.All:
                    sendData = distance;
                    break;
            }
        }

        /// <summary>
        /// 加入距离
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="target"></param>
        public void AddDistance(DistanceInteraction distance,DistanceInteraction target)
        {
            if (sendData != distance) return;

            if (distance.distanceData.interactionType != target.distanceData.interactionType) return;

            if (Distances.Contains(target)) return;

            Distances.Add(target);
        }

        /// <summary>
        /// 移除距离
        /// </summary>
        /// <param name="distance"></param>
        public void RemoveDistance(DistanceInteraction distance)
        {
            for (int i = 0; i < Distances.Count; i++)
            {
                if (!Distances[i].Equals(distance)) continue;
                Distances.RemoveAt(i);
                return;
            }
            // var data = Distances.Find(obj => obj.Equals(distance));
            // Distances.Remove(data);
        }

        /// <summary>
        /// 根据传递过来的具体距离对象，来获取到从内存读取距离信息的对象
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public DistanceInteraction GetDistanceData(DistanceInteraction data)
        {
            if (sendData.Equals(data)) return sendData;

            DistanceInteraction distanceInteraction = null;

            for (int i = 0; i < Distances.Count; i++)
            {
                if (!Distances[i].Equals(data)) continue;
                
                distanceInteraction = Distances[i];
                break;
            }

            return distanceInteraction;
            
            //return sendData.Equals(data) ? sendData : Distances.Find(obj => obj.Equals(data));
        }

        /// <summary>
        /// 计算距离
        /// </summary>
        public void ComputeDistance()
        {

            if (!sendData.IsEnable || sendData.InteractionObject == null || !sendData.InteractionObject.IsEnable)
                return;

            if (sendData.distanceData.IsShort)
            {
                for (var index = 0; index < Distances.Count; index++)
                {
                    var receive = Distances[index];
                    if (receive == null || !receive.IsEnable || sendData.InteractionObject == null) continue;

                    if (!receive.InteractionObject.IsEnable) continue;

                    float distanceValue;

                    if (InteractionDistanceController.DistanceCalculation(receive, sendData, out distanceValue))
                    {
                        if (tempDistance == null)
                        {
                            tempDistance = receive;
                            tempValue = distanceValue;

                            OnEnter(tempDistance);
                        }
                        else
                        {
                            if (tempValue >= distanceValue && tempDistance != receive)
                            {
                                //先执行退出，然后在执行移入
                                OnExit(tempDistance);

                                tempDistance = receive;
                                tempValue = distanceValue;

                                OnEnter(tempDistance);
                                continue;
                            }

                            OnEnter(tempDistance);
                        }
                    }
                    else
                    {
                        OnExit(receive);
                        if (tempDistance == receive)
                        {
                            tempDistance = null;
                            distanceValue = 0;
                        }
                    }
                }
            }
            else
            {
                for (var index = 0; index < Distances.Count; index++)
                {
                    var receive = Distances[index];
                    if (receive == null) continue;

                    if (!receive.InteractionObject.IsEnable) continue;

                    float distanceValue;

                    //如果在距离范围内
                    if (InteractionDistanceController.DistanceCalculation(receive, sendData, out distanceValue))
                    {
                        OnEnter(receive);
                    }
                    else
                    {
                        OnExit(receive);
                    }
                }
            }

        }

        void OnEnter(DistanceInteraction receive)
        {
            // //判断能否进行交互
            // var result = OperateVerifySystem.CanOperate(sendData.InteractionObject.DisplayName + "|" + receive.InteractionObject.DisplayName);
            //
            // if (!result) return;

            switch (sendData.distanceData.detectType)
            {
                //并且关系
                case InteractionDetectType.And:

                    //如果都没有移入，则
                    if (!InteractionDistanceController.IsEnter(sendData, receive))
                    {
                        //判断两者的条件是否都可以进行交互。
                        if (receive.IsCanInteraction(sendData) &&
                            sendData.IsCanInteraction(receive))
                        {
                            InteractionDistanceController.OnEnter(sendData, receive);

                            if (!Distanceing.Contains(receive))
                                Distanceing.Add(receive);
                        }
                    }
                    else
                    {
                        if (!Distanceing.Contains(receive))
                            Distanceing.Add(receive);
                        InteractionDistanceController.OnStay(sendData, receive);
                    }

                    // if (InteractionDistanceController.IsEnter(sendData, receive))
                    // {
                    //     if (!Distanceing.Contains(receive))
                    //         Distanceing.Add(receive);
                    //     InteractionDistanceController.OnStay(sendData, receive);
                    // }

                    break;
                case InteractionDetectType.Receive:

                    //如果是接收端，那么只需要计算接收端的IsEnter和receiveData看是否可以进行交互。
                    if (!InteractionDistanceController.IsEnter(sendData, receive)
                        /*&& receive.IsCanInteraction(sendData)*/)
                    {
                        if (receive.IsCanInteraction(sendData))
                        {
                            InteractionDistanceController.OnEnter(sendData, receive);

                            if (!Distanceing.Contains(receive))
                                Distanceing.Add(receive);
                        }
                    }
                    else
                    {
                        if (!Distanceing.Contains(receive))
                            Distanceing.Add(receive);
                        InteractionDistanceController.OnStay(sendData, receive);
                    }

                    // if (InteractionDistanceController.IsEnter(sendData, receive))
                    // {
                    //     if (!Distanceing.Contains(receive))
                    //         Distanceing.Add(receive);
                    //     InteractionDistanceController.OnStay(sendData, receive);
                    // }

                    break;

                case InteractionDetectType.Send:

                    //如果是发送端为主，那么只需要计算发射端的IsEnter和sendData看是否可以进行交互
                    if (!InteractionDistanceController.IsEnter(sendData, receive)
                        && sendData.IsCanInteraction(receive))
                    {

                        if (sendData.IsCanInteraction(receive))
                        {
                            InteractionDistanceController.OnEnter(sendData, receive);

                            if (!Distanceing.Contains(receive))
                                Distanceing.Add(receive);
                        }

                        // InteractionDistanceController.OnEnter(sendData, receive);
                        //
                        // if (!Distanceing.Contains(receive))
                        //     Distanceing.Add(receive);
                    }
                    else
                    {
                        if (!Distanceing.Contains(receive))
                            Distanceing.Add(receive);
                        InteractionDistanceController.OnStay(sendData, receive);
                    }

                    // if (InteractionDistanceController.IsEnter(sendData, receive))
                    // {
                    //     if (!Distanceing.Contains(receive))
                    //         Distanceing.Add(receive);
                    //     InteractionDistanceController.OnStay(sendData, receive);
                    // }

                    break;

                default:
                    break;
            }
        }

        void OnExit(DistanceInteraction receive)
        {
            //要加一层判断，否则一直执行是不好的
            InteractionDistanceController.OnExit(sendData, receive);

            if (Distanceing.Count == 0) return;

            if (Distanceing.Contains(receive))
                Distanceing.Remove(receive);
        }

        /// <summary>
        /// 计算释放
        /// </summary>
        public void OnComputeRelesae(bool isAuto=false )
        {
            if (Distanceing.Count == 0)
            {
                sendData.OnInteractionNotRelease();
                return;
            }

            bool isNotRelease = false;

            for (var index = 0; index < Distanceing.Count; index++)
            {
                var receive = Distanceing[index];
                if (receive == null) continue;

                //检测是否有正在交互中，如果没有，则执行notRelease释放。
                if (!InteractionDistanceController.IsEnter(sendData, receive))
                {
                    isNotRelease = true;
                    continue;
                }

                isNotRelease = false;

                InteractionDistanceController.OnRelease(sendData, receive, isAuto);

                //switch (sendData.distanceData.detectType)
                //{
                //    case InteractionDetectType.And:

                //        if (receive.IsCanInteraction(sendData) &&
                //            sendData.IsCanInteraction(receive))
                //        {
                //            InteractionDistanceController.OnRelease(sendData, receive, isAuto);
                //        }

                //        break;
                //    case InteractionDetectType.Receive:

                //        Debug.Log("松手处理 DistanceDataManager Receive");

                //        if (receive.IsCanInteraction(sendData))
                //        {
                //            InteractionDistanceController.OnRelease(sendData, receive, isAuto);
                //        }

                //        break;
                //    case InteractionDetectType.Send:
                //        Debug.Log("松手处理 DistanceDataManager Send");

                //        if (sendData.IsCanInteraction(receive))
                //        {
                //            InteractionDistanceController.OnRelease(sendData, receive, isAuto);
                //        }

                //        break;
                //    default:
                //        break;
                //}
            }

            //Debug.Log("SendData IsGrab");
            sendData.IsGrab = false;
            sendData.HandIndex = -1;

            //如果不存在有交互的，就进行无释放。
            if (isNotRelease)
                sendData.OnInteractionNotRelease();
        }
    }
}
