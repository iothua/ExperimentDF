using System;
using System.Collections.Generic;
using ExperimentFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
namespace App.Config
{
    [Serializable]
    public class ExperimentEquipmentData
    {
        public string EquipmentPrefabPath;

        public string BuildLabel;
        
        /// <summary>
        /// 仪器key
        /// </summary>
        public string EquipmentName;

        /// <summary>
        /// 仪器数量
        /// </summary>
        public int EquipmentNumber = 1;

        /// <summary>
        /// 位置
        /// </summary>
        public List<TransformData> InitEquipment;
        
    }

    [Serializable]
    public class ExperimentEquipmentConfig
    {
        /// <summary>
        /// 架子上的仪器
        /// </summary>
        public List<ExperimentEquipmentData> ShelfEquipments;
        
        /// <summary>
        /// 桌子上的仪器
        /// </summary>
        public List<ExperimentEquipmentData> DesktopEquipments;
        
        /// <summary>
        /// 其他上的仪器
        /// </summary>
        public List<ExperimentEquipmentData> OtherEquipments;
    }
}