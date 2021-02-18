using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using Loxodon.Framework.Contexts;
using ExperimentFramework.Interactive.Actions;

namespace ExperimentFramework.Interactive.Distance
{
    [System.Serializable]
    public class EventDistanceInteraction :UnityEvent<DistanceInteraction> { }

    public class EventDistanceInteractionRelease :UnityEvent<DistanceInteraction,InteractiveStatus> { }

    /// <summary>
    /// 多仪器数据
    /// </summary>
    [Serializable]
    public class MultiObjectData
    {
        public string key;
        public TransformData Data;
    }

    /// <summary>
    /// 距离交互(挂在物体中)
    /// </summary>
    [ExecuteInEditMode]
    public class DistanceInteraction :MonoBehaviour
    {
        public DistanceData distanceData;

        #region 事件

        //靠近、停留、离开、放下、中断(OnBreak)
        public EventDistanceInteraction OnEnter, OnStay, OnExit;

        public EventDistanceInteraction OnRelease; //交互距离后交互释放
        public EventDistanceInteractionRelease OnStatusRelease; //交互后，第二次释放
        public UnityEvent OnNotRelease;//没有交互时的释放
       
        protected IInteraction_Limit[] Limits;

        public bool IsEnable
        {
            get; private set;
        }

        #endregion
        
        private IInteractionObject interactionObject;
        /// <summary>
        /// 功能对象
        /// </summary>
        public IInteractionObject InteractionObject
        {
            get
            {
                if (interactionObject == null)
                {
                    interactionObject = gameObject.GetComponentInParent<IInteractionObject>();
                }

                return interactionObject;
            }
        }

        public IExternalInteraction ExternalInteraction { get; set; }

        /// <summary>
        /// 初始距离检测，在距离内则进行交互
        /// </summary>
        public bool AutoDetection = true;
        public bool HasDeteced { get; set; }

        /// <summary>
        /// 激活多仪器种类
        /// </summary>
        public bool ActiveMultiEquipment;

        /// <summary>
        /// 仪器释放复位
        /// </summary>
        public bool IsEquipmentReleaseReset = true;
        
        public List<MultiObjectData> MultiData;

        public Dictionary<string, TransformData> MultiDataDic;
        
        //加入子父物体,虚影的位置与子父物体一致
        public bool ActiveParent;
        
        // //显示虚影
        // public bool ActiveShadow;
        
        public TransformData ChildData;
        
        public bool IsSelf;
        
        /// <summary>
        /// 是否被抓取
        /// </summary>
        public bool IsGrab { get; set; }

        public int HandIndex = -1;

        public bool ActiveAutoExit = true;
        
        public Vector3 Position => transform.position;

        protected virtual void Awake()
        {

            if (OnEnter == null)
                OnEnter = new EventDistanceInteraction();

            if (OnStay == null)
                OnStay = new EventDistanceInteraction();

            if (OnExit == null)
                OnExit = new EventDistanceInteraction();

            if (OnRelease == null)
                OnRelease = new EventDistanceInteraction();

            if (OnStatusRelease == null)
                OnStatusRelease = new EventDistanceInteractionRelease();

            if (OnNotRelease == null)
                OnNotRelease = new UnityEvent();

            //加入时，检索一次
            if (distanceData == null)
            {
                distanceData = new DistanceData();
            }

            if (Application.isPlaying)
            {
                if (MultiData.Count <= 0) return;
                MultiDataDic=new Dictionary<string, TransformData>();
                for (var index = 0; index < MultiData.Count; index++)
                {
                    var multi = MultiData[index];
                    if (MultiDataDic.ContainsKey(multi.key)) continue;

                    MultiDataDic.Add(multi.key, multi.Data);
                }
            }
        }

        protected virtual void OnEnable()
        {
            //统一调用，去匹配数据，还需要一个数据，每隔一段时间校验一次，用于匹配执行顺序等情况
            DistanceStorage.AddDistanceData(this);
            IsEnable = true;
        }

        protected virtual void Start()
        {
            if (Application.isPlaying)
            {
                //目前只支持send端初始交互
                if (distanceData.interactionType == InteractionType.Send && !IsGrab)
                    StartCoroutine(AutoInteraction(0.1f));
            }
        }

        /// <summary>
        /// 自动交互
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator AutoInteraction(float delay)
        {

            var context = Context.GetApplicationContext();
            var interactive = context.GetService<InteractiveController>();
            
            if (interactive == null) yield break;

            if (distanceData.interactionType == InteractionType.Send)
            {
                yield return new WaitForSeconds(delay);
                //初始交互
                interactive.Search.StartInteraction(InteractionObject.InteractionObject,false,0,true);
                yield return new WaitForSeconds(0.15f);
                interactive.Search.StopInteraction(InteractionObject.InteractionObject,true);

                AutoDetection = false;
            }
        }

        protected virtual void OnDisable()
        {
            DistanceStorage.DeleteDistanceData(this);
            IsEnable = false;
        }

        public virtual bool IsCanInteraction(DistanceInteraction distanceInteraction)
        {
            return true;
        }

        /// <summary>
        /// 移入
        /// </summary>
        public virtual void OnDistanceEnter(DistanceInteraction distanceInteraction)
        {

            if (OnEnter != null)
                OnEnter.Invoke(distanceInteraction);
        }

        /// <summary>
        /// 离开
        /// </summary>
        public virtual void OnDistanceExit(DistanceInteraction distanceInteraction)
        {
            
            if (OnExit != null)
            {
                OnExit.Invoke(distanceInteraction);
            }
            
        }

        /// <summary>
        /// 停留
        /// </summary>
        public virtual void OnDistanceStay(DistanceInteraction distanceInteraction)
        {

            if (OnStay != null)
            {
                OnStay.Invoke(distanceInteraction);
            }
        }

        /// <summary>
        /// 释放
        /// </summary>
        public virtual void OnDistanceRelesae(DistanceInteraction distanceInteraction)
        {

            

            if (OnRelease != null)
            {
                OnRelease.Invoke(distanceInteraction);
            }
            
        }

        /// <summary>
        /// 带状态释放
        /// </summary>
        /// <param name="distanceInteraction"></param>
        /// <param name="status"></param>
        public virtual void OnDistanceRelease(DistanceInteraction distanceInteraction,InteractiveStatus status)
        {
            //Debug.Log("OnDistanceRelease IsGrab");
            IsGrab = false;

            if (OnStatusRelease != null)
            {
                OnStatusRelease.Invoke(distanceInteraction,status);
            }
        }

        /// <summary>
        /// 没有物体与它进行交互时，会进行释放
        /// </summary>
        public virtual void OnDistanceNotInteractionRelease()
        {

            if (OnNotRelease != null)
            {
                OnNotRelease.Invoke();
            }
        }

        #region 编辑器使用

        
        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (UnityEditor.Selection.activeObject == gameObject)
            {
                switch (distanceData.distanceShape)
                {
                    case DistanceShape.Sphere:

                        Gizmos.color = Color.yellow;
                        Gizmos.DrawSphere(transform.position,distanceData.distanceValue);

                        break;
                    case DistanceShape.Cube:

                        Gizmos.color = Color.yellow;
                        Gizmos.DrawCube(transform.position,distanceData.Size);

                        break;
                }


            }
#endif

        }

        #endregion

        #region 距离互动处理

        public DistanceInteraction OnlyDistance { get; set; }
        public List<DistanceInteraction> Distanced { get; set; }
        public List<DistanceInteraction> Distancing { get; set; }

        /// <summary>
        /// 交互移入处理
        /// </summary>
        /// <param name="interaction">Interaction.</param>
        public void OnInteractionEnter(DistanceInteraction interaction)
        {
            switch (distanceData.interactionType)
            {
                case InteractionType.Receive:
                case InteractionType.All:
                case InteractionType.Pour:
                    AddReceiveDistancing(interaction);
                    OnDistanceEnter(interaction);

                    break;
                case InteractionType.Send:
                    OnDistanceEnter(interaction);
                    break;
            }
        }

        /// <summary>
        /// 交互离开处理
        /// </summary>
        /// <param name="interaction">Interaction.</param>
        public void OnInteractionExit(DistanceInteraction interaction)
        {
            switch (distanceData.interactionType)
            {
                case InteractionType.Receive:
                case InteractionType.All:
                case InteractionType.Pour:

                    if (distanceData.IsOnly)
                    {
                        OnlyDistance = null;
                    }
                    else
                    {
                        RemoveReceiveDistancing(interaction);
                        RemoveReceiveDistanced(interaction);
                    }

                    OnDistanceExit(interaction);

                    break;
                case InteractionType.Send:

                    OnlyDistance = null;

                    OnDistanceExit(interaction);

                    break;
            }
        }

        /// <summary>
        /// 交互停留处理
        /// </summary>
        /// <param name="interaction">Interaction.</param>
        public void OnInteractionStay(DistanceInteraction interaction)
        {
            OnDistanceStay(interaction);
        }

        /// <summary>
        /// 交互松手处理
        /// </summary>
        /// <param name="interaction">Target.</param>
        public void OnInteractionRelease(DistanceInteraction interaction,bool isAuto = false)
        {

            switch (distanceData.interactionType)
            {
                case InteractionType.Receive:
                case InteractionType.All:
                case InteractionType.Pour:

                    if (distanceData.IsOnly)
                    {
                        if (OnlyDistance == interaction)
                        {
                            OnDistanceRelesae(interaction);
                            if (isAuto)
                                OnDistanceRelease(interaction,InteractiveStatus.IsAuto);
                            else
                                OnDistanceRelease(interaction,InteractiveStatus.Inside);
                            return;
                        }
                    }
                    else
                    {
                        if (Distanced != null && Distanced.Contains(interaction))
                        {
                            OnDistanceRelesae(interaction);
                            if (isAuto)
                                OnDistanceRelease(interaction,InteractiveStatus.IsAuto);
                            else
                                OnDistanceRelease(interaction,InteractiveStatus.Inside);
                            return;
                        }
                    }

                    if (!InteractionCheck()) return;

                    OnDistanceRelesae(interaction);
                    if (isAuto)
                        OnDistanceRelease(interaction,InteractiveStatus.IsAuto);
                    else
                        OnDistanceRelease(interaction,InteractiveStatus.Once);

                    if (distanceData.IsOnly)
                    {
                        OnlyDistance = interaction;

                        return;
                    }
                    else
                    {
                        AddReceiveDistanced(interaction);
                    }

                    break;
                case InteractionType.Send:

                    if (!InteractionCheck())
                    {
                        OnDistanceRelesae(interaction);
                        if (isAuto)
                            OnDistanceRelease(interaction,InteractiveStatus.IsAuto);
                        else
                            OnDistanceRelease(interaction,InteractiveStatus.Inside);

                        return;
                    }

                    OnDistanceRelesae(interaction);
                    if (isAuto)
                        OnDistanceRelease(interaction,InteractiveStatus.IsAuto);
                    else
                        OnDistanceRelease(interaction,InteractiveStatus.Once);

                    AddSendDistance(interaction);

                    break;
            }
        }

        /// <summary>
        /// 没有执行交互时释放
        /// </summary>
        public void OnInteractionNotRelease()
        {
            OnDistanceNotInteractionRelease();
            OnDistanceRelease(null,InteractiveStatus.None);
        }

        /// <summary>
        /// 校验是否可以进行交互
        /// </summary>
        /// <returns></returns>
        public bool InteractionCheck()
        {
            switch (distanceData.interactionType)
            {
                case InteractionType.Receive:
                case InteractionType.All:
                case InteractionType.Pour:

                    if (distanceData.IsOnly)
                    {
                        if (OnlyDistance != null) return false;
                    }
                    else
                    {
                        if (distanceData.maxCount == 0) return false;
                    }

                    break;
                case InteractionType.Send:

                    if (OnlyDistance != null) return false;

                    break;
            }


            return true;
        }

        /// <summary>
        /// 往接收端中添加发送端信息
        /// </summary>
        /// <param name="send"></param>
        public void AddReceiveDistanced(DistanceInteraction send)
        {
            if (Distanced == null)
                Distanced = new List<DistanceInteraction>();

            //移除正在交互的
            if (Distancing.Contains(send))
                Distancing.Remove(send);

            if (Distanced.Contains(send)) return;

            Distanced.Add(send);

            if (distanceData.maxCount == -1)
                return;

            distanceData.maxCount--;
        }

        /// <summary>
        /// 添加接收端信息
        /// </summary>
        /// <param name="send">Send.</param>
        public void AddReceiveDistancing(DistanceInteraction send)
        {
            if (Distancing == null)
                Distancing = new List<DistanceInteraction>();

            if (Distancing.Contains(send))
                return;

            Distancing.Add(send);
        }

        /// <summary>
        /// 往发送端中添加接收信息，
        /// </summary>
        /// <param name="receive">Receive.</param>
        public void AddSendDistance(DistanceInteraction receive)
        {
            OnlyDistance = receive;
        }

        /// <summary>
        /// 移除接收距离交互信息
        /// </summary>
        /// <param name="send">Send.</param>
        public void RemoveReceiveDistancing(DistanceInteraction send)
        {
            if (Distancing == null) return;
            if (!Distancing.Contains(send)) return;
            Distancing.Remove(send);
        }

        /// <summary>
        /// 移除已经交互的距离信息
        /// </summary>
        /// <param name="send">Send.</param>
        public void RemoveReceiveDistanced(DistanceInteraction send)
        {
            if (Distanced == null) return;
            if (!Distanced.Contains(send)) return;

            Distanced.Remove(send);

            if (distanceData.maxCount != -1)
                distanceData.maxCount++;
        }



        #endregion

        public override bool Equals(object other)
        {
            if (other == null) return false;
            var distanceInteraction = (DistanceInteraction)other;
            if (distanceInteraction == null) return false;

            return distanceData.TagID.Equals(distanceInteraction.distanceData.TagID) && this == distanceInteraction;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}

