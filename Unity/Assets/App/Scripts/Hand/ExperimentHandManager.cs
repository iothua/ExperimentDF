using System;
using System.Collections.Generic;
using ExperimentFramework;
using UnityEngine;
using Loxodon.Framework.Asynchronous;

namespace App.Hand
{

    public class ExperimentIdentity
    {
        public Transform target;
        public string equipmentKey;
        public string identity;//唯一ID
        public bool IsRight;

        public ExperimentIdentity(Transform target, string key,string identity,bool isRight)
        {
            this.target = target;
            this.equipmentKey = key;
            this.identity = identity;
            this.IsRight = isRight;
        }
    }

    
    
    /// <summary>
    /// 实验手管理端
    /// </summary>
    public class ExperimentHandManager : IDisposable
    {
        
        
        public class HandObjectData
        {
            public string identity;
            public string equipmentKey;
            public string handKey;
            public GameObject handObject;
        }
        
        private ExperimentHandConfig _handConfig;

        /// <summary>
        /// 手数据
        /// </summary>
        private Dictionary<string, ExperimentHandData> _handDatas = new Dictionary<string, ExperimentHandData>();

        /// <summary>
        /// 手源模型引用
        /// </summary>
        private Dictionary<string, GameObject> experimentSourceHands = new Dictionary<string, GameObject>();

        //当前显示的手记录信息
        private Dictionary<string, HandObjectData> experimentHandDatas = new Dictionary<string, HandObjectData>();
        
        /// <summary>
        /// 手模型池
        /// </summary>
        private Dictionary<string, List<GameObject>> handModelPools = new Dictionary<string, List<GameObject>>();
        
        public ExperimentHandManager()
        {
            LoadConfig();
        }

        /// <summary>
        /// 加载Config
        /// </summary>
        async void LoadConfig()
        {
            await AddressableManager.LoadSourceCoroutine<TextAsset>("DynamicAssets/Configs/ExperimentHandConfig.json", result =>
            {
                if (result == null)
                {
                    Debug.Log("加载ExperimentHandConfig文件出错...");
                    return;
                }

                _handConfig = JsonHelper.JsonToObject<ExperimentHandConfig>(result.text);

                foreach (var experimentHandData in _handConfig.ExperimentHandDatas)
                {
                    if (_handDatas.ContainsKey(experimentHandData.experimentKey))
                    {
                        _handDatas[experimentHandData.experimentKey] = experimentHandData;
                        continue;
                    }

                    _handDatas.Add(experimentHandData.experimentKey, experimentHandData);
                }
            });


            if (_handConfig == null)
            {
                return;
            }

            foreach (var modelHandData in _handConfig.ModelHandDatas)
            {
                await AddressableManager.LoadSourceGameObjectCoroutine(modelHandData.handPath, result =>
                {
                    if (result == null) return;
                    experimentSourceHands.Add(modelHandData.handKey, result);
                });
            }
        }

        public void Dispose()
        {
            foreach (var item in experimentHandDatas)
            {
                GameObject.Destroy(item.Value.handObject);
            }

            foreach (var item in handModelPools)
            {
                for (int i = 0; i < item.Value.Count; i++)
                {
                    GameObject.Destroy(item.Value[i]);
                }
            }

            _handDatas.Clear();
            experimentHandDatas.Clear();
            experimentSourceHands.Clear();
            
            // MessageDispatcher.RemoveListener(EventMessage._SHOW_EXPERIMENT_HAND,ShowExperimentHand);
            // MessageDispatcher.RemoveListener(EventMessage._HIDE_EXPERIMENT_HAND, HideExperimentHand);
        }

        /// <summary>
        /// 显示实验手
        /// </summary>
        /// <param name="message"></param>
        void ShowExperimentHand()
        {
            /*
            var experimentIdentity = (ExperimentIdentity) ;
            if (experimentIdentity == null) return;
            
            _handDatas.TryGetValue(experimentIdentity.equipmentKey, out var handData);
            
            experimentHandDatas.TryGetValue(experimentIdentity.identity, out var handObjectData);
            if (handObjectData != null)
            {
                //SetHandModelTransform(handObjectData.handObject, experimentIdentity, handData);
                return;
            }

            var handKey = experimentIdentity.IsRight
                ? handData.RightHand.ModelKey
                : handData.LeftHand.ModelKey;

            if (string.IsNullOrEmpty(handKey)) return;
            
            //创建新的模型，根据表格生成新的坐标
            handModelPools.TryGetValue(handKey, out var handObjects);
            
            if (handObjects == null|| handObjects.Count==0)
            {
                //做其他处理
                //生成新的手
                var handModel = CreateObject(experimentIdentity.IsRight
                    ? handData.RightHand.ModelKey
                    : handData.LeftHand.ModelKey);

                if (handModel == null) return;

                SetHandModelTransform(handModel, experimentIdentity, handData);
            }
            else
            {
                var handModel = handObjects[0];

                SetHandModelTransform(handModel, experimentIdentity, handData);

                handObjects.Remove(handModel);

                if (handObjects.Count == 0)
                {
                    handModelPools.Remove(handKey);
                }
                else
                {
                    handModelPools[handKey] = handObjects;
                }
            }
            */
        }

        /// <summary>
        /// 设置显示手Transform信息
        /// </summary>
        /// <param name="handModel"></param>
        /// <param name="experimentIdentity"></param>
        /// <param name="handData"></param>
        void SetHandModelTransform(GameObject handModel,ExperimentIdentity experimentIdentity,ExperimentHandData handData)
        {
            handModel.SetActive(true);
            
            handModel.transform.SetParent(experimentIdentity.target);
            handModel.transform.localPosition = experimentIdentity.IsRight
                ? handData.RightHand.HandPosition
                : handData.LeftHand.HandPosition;

            handModel.transform.localRotation = Quaternion.Euler(experimentIdentity.IsRight
                ? handData.RightHand.HandRotation
                : handData.LeftHand.HandRotation);

            var handKey = experimentIdentity.IsRight
                ? handData.RightHand.ModelKey
                : handData.LeftHand.ModelKey;
            
            if (experimentHandDatas.ContainsKey(experimentIdentity.identity)) return;
            
            experimentHandDatas.Add(experimentIdentity.identity, new HandObjectData()
            {
                identity = experimentIdentity.identity,
                handKey = handKey,
                equipmentKey = experimentIdentity.equipmentKey,
                handObject = handModel
            });
        }

        GameObject CreateObject(string handKey)
        {
            if (string.IsNullOrEmpty(handKey)) return null;

            experimentSourceHands.TryGetValue(handKey, out var sourceObject);

            return sourceObject == null ? null : GameObject.Instantiate(sourceObject);
        }

        /// <summary>
        /// 隐藏实验手
        /// </summary>
        /// <param name="message"></param>
        void HideExperimentHand()
        {
            var identity = string.Empty;

            experimentHandDatas.TryGetValue(identity, out var handObjectData);
            
            if (handObjectData == null) return;
            
            handObjectData.handObject.transform.SetParent(null);
            handObjectData.handObject.SetActive(false);

            experimentHandDatas.Remove(identity);
            
            handModelPools.TryGetValue(handObjectData.handKey, out var models);
            if (models == null)
            {
                models = new List<GameObject>();
                handModelPools.Add(handObjectData.handKey, models);
            }

            models.Add(handObjectData.handObject);
            handModelPools[handObjectData.handKey] = models;
        }
    }
}