using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using ExperimentFramework.Interactive.Distance;

namespace ExperimentFramework.Interactive
{
    /// <summary>
    /// 距离检索
    /// </summary>
    public class InteractiveSearch
    {
        public Dictionary<GameObject,List<DistanceDataManager>> dataManagers = new Dictionary<GameObject,List<DistanceDataManager>>();

        ///// <summary>
        ///// 已经存在交互的距离检测点
        ///// 当执行OnEnter时，就需要往执行该方法。
        ///// 当抓取该物体的Send对象时，该集合不进行处理。
        ///// 只有抓取Receive对象时，或者每隔多长时间，进行一次筛选
        ///// </summary>
        //public List<AlreadyInteractiveData> AlreadyDatas = new List<AlreadyInteractiveData>();

        public bool IsStartInteraction;

        /// <summary>
        /// 启动交互
        /// </summary>
        public void StartInteraction(GameObject target,bool isGrab,int handIndex,bool defaultInteraction = false)
        {
            IsStartInteraction = true;

            var interactions = target.GetComponentsInChildren<DistanceInteraction>();

            if (interactions.Length == 0) return;

            GetSendInteraction(target,isGrab,handIndex,interactions,defaultInteraction);

            //OnPourAll(target,isGrab,handIndex,interactions,InteractionType.Pour);
            GetAllInteraction(target,isGrab,handIndex,interactions,InteractionType.All);

            //接受端要去找已经交互的对象
            
        }

        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="target">物体</param>
        /// <param name="isGrab">是否抓取</param>
        /// <param name="interactions">距离互动集合</param>
        void GetSendInteraction(GameObject target,bool isGrab,int handIndex,DistanceInteraction[] interactions,bool defaultInteraction = false)
        {
            //获取到所有的发送信息
            //var interactionSends = interactions.Where(_ => _.distanceData.interactionType == InteractionType.Send);

            List<DistanceInteraction> interactionSends = new List<DistanceInteraction>();

            for (int i = 0; i < interactions.Length; i++)
            {
                if(interactions[i]==null) continue;

                if (interactions[i].distanceData.interactionType == InteractionType.Send)
                {
                    interactionSends.Add(interactions[i]);
                }
            }

            //获取到发送集合
            interactionSends = DistanceSearch(target,interactionSends,isGrab);

            foreach (var interaction in interactionSends)
            {
                interaction.IsGrab = true;
                interaction.HandIndex = handIndex;

                //根据指定key获取到接收点
                var managers = DistanceStorage.GetSendDistaceDataKey(interaction);

                List<DistanceDataManager> distanceManagers;
                dataManagers.TryGetValue(target,out distanceManagers);

                if (distanceManagers == null)
                    distanceManagers = new List<DistanceDataManager>();

                foreach (var item in managers)
                {
                    if (distanceManagers.Contains(item))
                        continue;

                    distanceManagers.Add(item);

                }

                //如果是初始交互，则对接收端筛选一次
                if (defaultInteraction)
                {
                    //var distances = new List<DistanceDataManager>();
                    //distances.CopyTo(distanceManagers.ToArray());

                    //DistanceDataManager[] distances = new DistanceDataManager[distanceManagers.Count];
                    List<DistanceDataManager> distances = new List<DistanceDataManager>();

                    for (int i = 0; i < distanceManagers.Count; i++)
                    {
                        //distances[i] = new DistanceDataManager
                        //{
                        //    sendData = distanceManagers[i].sendData,
                        //    Distances = new List<DistanceInteraction>()
                        //};

                        var distanceDataManager = new DistanceDataManager
                        {
                            sendData = distanceManagers[i].sendData,
                            Distances = new List<DistanceInteraction>()
                        };

                        distances.Add(distanceDataManager);

                        for (int j = 0; j < distanceManagers[i].Distances.Count; j++)
                        {
                            var distance = distanceManagers[i].Distances[j];
                            if (distance.AutoDetection)
                            {
                                //distances[i].AddDistance(distance);
                                distanceDataManager.AddDistance(distance);
                                
                            }
                        }
                    }

                    if (!dataManagers.ContainsKey(target))
                        dataManagers.Add(target,distances);
                    else
                        dataManagers[target] = distances;
                }
                else
                {

                    if (!dataManagers.ContainsKey(target))
                        dataManagers.Add(target,distanceManagers);
                    else
                        dataManagers[target] = distanceManagers;
                }
            }
        }

        
         
        
        /// <summary>
        /// 根据不同的距离类型，赛选合适的距离检测信息
        /// </summary>
        /// <param name="target"></param>
        /// <param name="isGrab"></param>
        /// <param name="interactions"></param>
        /// <param name="interactionType"></param>
        void GetAllInteraction(GameObject target,bool isGrab,int handIndex,DistanceInteraction[] interactions,
            InteractionType interactionType,bool defaultInteraction = false)
        {
            List<DistanceInteraction> interactionPours = new List<DistanceInteraction>();

            for (int i = 0; i < interactions.Length; i++)
            {
                if(interactions[i]==null) continue;

                if (interactions[i].distanceData.interactionType == interactionType)
                {
                    interactionPours.Add(interactions[i]);
                }
            }
            
            interactionPours = DistanceSearch(target,interactionPours,isGrab);

            foreach (var interaction in interactionPours)
            {
                //获取到其他的接收点（All、Pour）
                var managers = DistanceStorage.GetSendDistaceDataAll(interaction,interactionType);

                //如果没有获取到其他的，则跳过
                if (managers.Count == 0) continue;

                //获取到当前已经存在的距离管理端
                List<DistanceDataManager> distanceManagers;
                dataManagers.TryGetValue(target,out distanceManagers);

                //如果不存在，则实例化新的
                if (distanceManagers == null)
                    distanceManagers = new List<DistanceDataManager>();

                //在当前的距离管理端中，找是否存在的，
                DistanceDataManager distanceManager = distanceManagers.Count == 0 ? new DistanceDataManager() :
                    distanceManagers.Find(obj => obj.sendData.Equals(interaction)) ?? new DistanceDataManager();

                distanceManager.sendData = interaction;

                interaction.IsGrab = true;
                interaction.HandIndex = handIndex;

                foreach (var item in managers)
                {
                    distanceManager.AddDistance(interaction,item.sendData);
                }

                distanceManagers.Add(distanceManager);

                if (!dataManagers.ContainsKey(target))
                    dataManagers.Add(target,distanceManagers);
                else
                    dataManagers[target] = distanceManagers;
            }
        }

        private List<DistanceInteraction> DistanceSearch(GameObject target, List<DistanceInteraction> interactions,
            bool isGrab)
        {
            List<DistanceInteraction> results = new List<DistanceInteraction>();

            for (int i = 0; i < interactions.Count; i++)
            {
                var obj = interactions[i];
                
                if (isGrab && !obj.distanceData.IsGrabOwn)
                {
                    results.Add(obj);
                }
                else
                {
                    if (obj.InteractionObject == null) continue;
                    if (obj.InteractionObject.InteractionObject != target)
                        continue;
                    
                    results.Add(obj);
                }
            }
            
            return results;
        }

        /// <summary>
        /// 停止交互
        /// </summary>
        public void StopInteraction(GameObject target,bool isAuto = false)
        {
            IsStartInteraction = false;

            if (target == null)
            {
                return;
            }

            List<DistanceDataManager> managers;
            dataManagers.TryGetValue(target,out managers);

            if (managers == null || managers.Count == 0) return;

            foreach (var send in managers)
            {
                //遍历距离检测，并且触发相应的事件
                send.OnComputeRelesae(isAuto);
            }

            managers.Clear();

            dataManagers.Remove(target);
        }

        public void OnUpdate()
        {

            if (dataManagers.Count > 0)
            {
                foreach (var dataManager in dataManagers)
                {
                    for (int i = 0; i < dataManager.Value.Count; i++)
                    {
                        //遍历距离检测，并且触发相应的事件
                        dataManager.Value[i].ComputeDistance();
                    }
                }
            }

            if (IsStartInteraction)
                InteractionDistanceController.UpdateExit();
        }
    }
}
