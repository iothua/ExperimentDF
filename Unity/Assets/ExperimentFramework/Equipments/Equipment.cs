using System.Collections;
using UnityEngine;
using ExperimentFramework.Interactive;

namespace ExperimentFramework.Equipments
{
    public class Equipment : MonoBehaviour
    {
        public InteractiveBehavior Behavior;

        /// <summary>
        /// 仪器数据
        /// </summary>
        public EquipmentData equipmentData;

        public IEnumerator Build(TextAsset asset)
        {
            //创建碰撞体、模型、特效等
            //创建距离检测点

            //实例化距离检测、碰撞体、模型、lua脚本

            equipmentData = JsonHelper.JsonToObject<EquipmentData>(asset.text);

            //初始化仪器
            yield return equipmentData.LoadEquipment();
            
            //实例化lua脚本
            
            InitializeEquipment();
        }

        public virtual void InitializeEquipment()
        {
            Behavior.OnCanInteractive = IsCanInteractive;
            Behavior.OnInteractiveEnter.AddListener(OnEnter);
            Behavior.OnInteractiveExit.AddListener(OnExit);
            Behavior.OnInteractiveStay.AddListener(OnStay);
            Behavior.OnInteractiveRelease.AddListener(OnRelease);
        }

        public virtual void DestroyEquipment()
        {
            Behavior.OnCanInteractive = null;
            Behavior.OnInteractiveEnter.RemoveListener(OnEnter);
            Behavior.OnInteractiveExit.RemoveListener(OnExit);
            Behavior.OnInteractiveStay.RemoveListener(OnStay);
            Behavior.OnInteractiveRelease.RemoveListener(OnRelease);

            //卸载仪器
            equipmentData.UnAsyncLoadEquipment();
        }

        public virtual bool IsCanInteractive(InteractiveBehavior interaction)
        {
            return true;
        }
        
        public virtual void OnEnter(InteractiveBehavior interaction,InteractiveBase interactiveBase)
        {
            
        }

        public virtual void OnExit(InteractiveBehavior interaction,InteractiveBase interactiveBase)
        {
            
        }

        public virtual void OnStay(InteractiveBehavior interaction,InteractiveBase interactiveBase)
        {
            
        }

        public virtual void OnRelease(InteractiveBehavior interaction,InteractiveBase interactiveBase, InteractiveStatus status)
        {
            
        }
    }
}

