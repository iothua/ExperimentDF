using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace ExperimentFramework.Interactive.Distance
{
    /// <summary>
    /// 距离交互信息
    /// </summary>
    public class InteractionDistanceInfo
    {
        public DistanceInteraction sendKey;
        public DistanceInteraction receiveValue;

        //距离状态
        public DistanceStatus distanceStatus = DistanceStatus.Exit;

        public InteractionDistanceInfo(DistanceInteraction key, DistanceInteraction value, DistanceStatus distanceStatus)
        {
            this.sendKey = key;
            this.receiveValue = value;
            this.distanceStatus = distanceStatus;
        }

        /// <summary>
        /// 设置距离状态
        /// </summary>
        /// <param name="distanceStatus"></param>
        public void SetDistanceStatus(DistanceStatus distanceStatus)
        {
            this.distanceStatus = distanceStatus;
        }

    }


    /// <summary>
    /// 交互距离控制端
    /// </summary>
    public static class InteractionDistanceController
    {
        private static List<InteractionDistanceInfo> DistanceInfos = new List<InteractionDistanceInfo>();

        /// <summary>
        /// 已经存在交互的距离检测点
        /// 当执行OnEnter时，就需要往执行该方法。
        /// 当抓取该物体的Send对象时，该集合不进行处理。
        /// 只有抓取Receive对象时，或者每隔多长时间，进行一次筛选
        /// </summary>
        public static Dictionary<DistanceInteraction, DistanceInteraction> AlreadyDatas = new Dictionary<DistanceInteraction, DistanceInteraction>();

        //是否对字典进行操作
        private static bool isDicing = false;

        /// <summary>
        /// 移入
        /// </summary>
        /// <param name="send"></param>
        /// <param name="receive"></param>
        public static void OnEnter(DistanceInteraction send, DistanceInteraction receive)
        {
            //if (send.distanceData.IsOnly && send.OnlyDistance != receive) return;

            //在次之前，还需要判断一次，否则一直执行循环查找，会很费资源，这么处理不恰当
            if (send.OnlyDistance != null || !receive.InteractionCheck()) return;

            //Debug.Log("进来-发送端：" + send.Interaction.FeaturesObjectController + " 接收端：" + receive.Interaction.FeaturesObjectController);

            InteractionDistanceInfo distanceInfo;

            if (IsContains(send, receive, out distanceInfo))
            {
                distanceInfo.SetDistanceStatus(DistanceStatus.Enter);
            }
            else
            {
                distanceInfo = new InteractionDistanceInfo(send, receive, DistanceStatus.Enter);
                DistanceInfos.Add(distanceInfo);
            }

            receive.OnInteractionEnter(send);
            send.OnInteractionEnter(receive);

            //存入到集合中

            //var alreadyInteractive = new AlreadyInteractiveData()
            //{
            //    Send = send,
            //    Receive = receive,
            //    TagID = send.distanceData.TagID
            //};
            isDicing = true;

            if (!AlreadyDatas.ContainsKey(send))
            {
                AlreadyDatas.Add(send, receive);
            }

            isDicing = false;
        }

        /// <summary>
        /// 移出
        /// </summary>
        /// <param name="send"></param>
        /// <param name="receive"></param>
        public static void OnExit(DistanceInteraction send, DistanceInteraction receive)
        {
            //在次之前，还需要判断一次，否则一直执行循环查找，会很费资源，这么处理不恰当

            if (send == null || receive == null) return;
            
            //To…Do

            InteractionDistanceInfo distanceInfo;

            if (IsContains(send, receive, out distanceInfo))
            {
                DistanceInfos.Remove(distanceInfo);

                receive.OnInteractionExit(send);
                send.OnInteractionExit(receive);

                //Debug.Log("离开了");
            }

            isDicing = true;

            if (AlreadyDatas.ContainsKey(send) && AlreadyDatas[send].Equals(receive))
            {
                AlreadyDatas.Remove(send);
            }


            isDicing = false;

            //AlreadyDatas.Remove(new AlreadyInteractiveData()
            //{
            //    Send = send,
            //    Receive = receive,
            //    TagID = send.distanceData.TagID
            //});
        }

        /// <summary>
        /// 停留
        /// </summary>
        /// <param name="send"></param>
        /// <param name="receive"></param>
        public static void OnStay(DistanceInteraction send, DistanceInteraction receive)
        {
            receive.OnInteractionStay(send);
            send.OnInteractionStay(receive);
        }

        /// <summary>
        /// 放下，完成
        /// </summary>
        /// <param name="send"></param>
        /// <param name="receive"></param>
        public static void OnRelease(DistanceInteraction send, DistanceInteraction receive,bool isAuto=false )
        {
            InteractionDistanceInfo distanceInfo;

            if (IsContains(send, receive, out distanceInfo))
            {
                distanceInfo.SetDistanceStatus(DistanceStatus.Complete);

                receive.OnInteractionRelease(send,isAuto);
                send.OnInteractionRelease(receive, isAuto);
            }
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="send"></param>
        /// <param name="receive"></param>
        /// <returns></returns>
        static bool IsContains(DistanceInteraction send, DistanceInteraction receive,out InteractionDistanceInfo distanceInfo)
        {
            for (int i = 0; i < DistanceInfos.Count; i++)
            {
                var distance = DistanceInfos[i];
                if (!distance.sendKey.Equals(send) || !distance.receiveValue.Equals(receive)) continue;
                
                distanceInfo = distance;
                return true;
            }

            distanceInfo = null;
            return false;
            
            // var distance = DistanceInfos.Find(obj => obj.sendKey.Equals(send) && obj.receiveValue.Equals(receive));
            //
            // distanceInfo = distance;
            //
            // return distance != null;
        }
        
        public static bool DistanceCalculation(DistanceInteraction receiveDistance,DistanceInteraction sendDistance,out float distanceValue)
        {
            switch (receiveDistance.distanceData.distanceShape)
            {
                case DistanceShape.Sphere:
                    //距离检测
                    return InteractionUtility.Distance(receiveDistance.Position, sendDistance.Position,
                               receiveDistance.distanceData.distanceType, out distanceValue) <=
                           (receiveDistance.distanceData.distanceType == DistanceType.D2D
                               ? receiveDistance.distanceData.distanceValue *
                                 (1/Vector3.Distance(InteractionUtility.InteractionCamera.transform.position,receiveDistance.Position)) * InteractiveController.screenInteractiveCoefficient
                                 //InteractiveController.screenInteractiveCoefficient
                               : receiveDistance.distanceData.distanceValue);
                case DistanceShape.Cube:
                    return InteractionUtility.CubeDistance(receiveDistance.Position,receiveDistance.distanceData.Size,sendDistance.Position,receiveDistance.distanceData.distanceType,out distanceValue);
                default:
                    distanceValue = -1;
                    return false;
            }
        }

        /// <summary>
        /// 检测现有距离交互
        /// </summary>
        public static void UpdateExit()
        {
            //实时检测已交互的
            if (AlreadyDatas.Count != 0&& !isDicing)
            {
                List<DistanceInteraction> interactions = new List<DistanceInteraction>();

                var keys = AlreadyDatas.Keys.ToArray();

                for (int i = 0; i < keys.Length; i++)
                {
                    if (keys[i] == null) {

                        interactions.Add(keys[i]);

                        continue;
                    }

                    if (i >= AlreadyDatas.Count) break;

                    AlreadyDatas.TryGetValue(keys[i], out var value);

                    if (value == null) continue;

                    if (!keys[i].ActiveAutoExit) continue;

                    if (RealtimeDetectionExit(keys[i], value))
                    {
                        interactions.Add(keys[i]);
                    }
                }

                if (interactions.Count > 0)
                {
                    for (int i = 0; i < interactions.Count; i++)
                    {
                        AlreadyDatas.Remove(interactions[i]);
                    }
                }

                interactions.Clear();
            }
        }

        /// <summary>
        /// 交互退出
        /// </summary>
        public static void InteractiveQuit()
        {
            if (AlreadyDatas.Count == 0) return;
            
            Dictionary<DistanceInteraction,DistanceInteraction> tempAlreadys = new Dictionary<DistanceInteraction, DistanceInteraction>(AlreadyDatas);
            
            foreach (var already in tempAlreadys)
            {
                OnExit(already.Key, already.Value);
            }
        }

        public static bool InteractiveQuit(DistanceInteraction send)
        {
            if (AlreadyDatas.Count == 0) return false;

            AlreadyDatas.TryGetValue(send, out var receive);

            if (receive == null) return false;

            OnExit(send, receive);

            return true;
        }

        /// <summary>
        /// 实时检测是否在距离检测外
        /// </summary>
        /// <param name="alreadyInteractiveData"></param>
        public static bool RealtimeDetectionExit(DistanceInteraction send,DistanceInteraction receive)
        {
            //不在距离外
            if (!DistanceCalculation(receive, send, out var distance))
            {
                OnExit(send, receive);

                return true;
            }

            return false;
        }

        /// <summary>
        /// 两者之间是否已经靠近
        /// </summary>
        /// <param name="send"></param>
        /// <param name="receive"></param>
        /// <returns></returns>
        public static bool IsEnter(DistanceInteraction send, DistanceInteraction receive)
        {
            InteractionDistanceInfo distanceInfo;

            IsContains(send, receive, out distanceInfo);

            if (distanceInfo == null) return false;

            return distanceInfo.distanceStatus == DistanceStatus.Enter
                || distanceInfo.distanceStatus == DistanceStatus.Complete;
        }
    }
}
