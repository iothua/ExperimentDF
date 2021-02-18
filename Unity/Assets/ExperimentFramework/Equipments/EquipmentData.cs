using System;
using System.Collections;
using System.Collections.Generic;
using ExperimentFramework.Interactive;
using Loxodon.Framework.Asynchronous;

namespace ExperimentFramework.Equipments
{

    [Serializable]
    public class EquipmentData
    {
        /// <summary>
        /// 模型节点
        /// </summary>
        public List<ModelNode> ModelNodes;
        
        /// <summary>
        /// 特效节点
        /// </summary>
        public List<EffectNode> EffectNodes;
        
        /// <summary>
        /// 交互节点
        /// </summary>
        public List<InteractiveData> InteractiveDatas;

        public IEnumerator LoadEquipment()
        {
            yield break;
        }

        public IEnumerator UnLoadEquipment()
        {
            yield break;
        }

        public async void UnAsyncLoadEquipment()
        {
            await UnLoadEquipment();
        }
    }

    /// <summary>
    /// 模型节点
    /// </summary>
    [Serializable]
    public class ModelNode
    {
        public string assetPath;
        public TransformData Transform;
    }

    [Serializable]
    public class EffectNode
    {
        public string key;
        public string assetPath;
        public TransformData Transform;
        public string ClassPath;
    }

    /// <summary>
    /// 仪器位置状态
    /// </summary>
    public enum LocationStatus
    {
        /// <summary>
        /// 架子上
        /// </summary>
        Shelf,
        /// <summary>
        /// 桌面上
        /// </summary>
        Desktop,
        /// <summary>
        /// 其他
        /// </summary>
        Other
    }
    
    /// <summary>
    /// 仪器位置
    /// </summary>
    public class EquipmentLocation
    {
        public LocationStatus InitialLocationStatus;
        public LocationStatus CurrentLocationStatus;
        public TransformData InitialTransform;
    }

}
