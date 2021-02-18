using System;
using System.Collections.Generic;
using App.Config;
using ExperimentFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace App
{
    [Serializable]
    public class EquipmentShadowWindow
    {

        public string filePath = "/Laboratory/Ghost/Config/EquipmentShadow.json";

        [HorizontalGroup]
        [Button(ButtonSizes.Large,Name = "加载资源")]
        public void LoadEquipmentData()
        {
            string fullPath = Application.dataPath + filePath;
            
            if (System.IO.File.Exists(fullPath))
            {
                string jsonData = JsonHelper.ReadJsonString(fullPath);
                var shadowWindow = JsonHelper.JsonToObject<EquipmentShadowConfig>(jsonData);
                if (shadowWindow != null)
                {
                    GetEquipmentData(shadowWindow);
                }
            }
        }
        
        [HorizontalGroup]
        [Button(ButtonSizes.Large,Name = "保存更改")]
        public void SaveEquipmentData()
        {
            var window = new EquipmentShadowConfig()
            {
                ShadowDatas = ShadowDatas
            };
            string jsonData = JsonHelper.ObjectToJsonString(window);

            if (string.IsNullOrEmpty(filePath))
            {
                string fileName = "临时" + DateTime.Now.ToString("yy-MM-dd-hh-mm-ss");
                filePath = ExperimentConfigWindow.EquipmentResourcePath(fileName) + fileName;
            }

            JsonHelper.SaveJson(jsonData, Application.dataPath + filePath);
            GetEquipmentData(window);
        }
        
        [LabelText("仪器虚影配置")]
        [TableList(ShowIndexLabels = true)]
        public List<KeyValueData> ShadowDatas;
        
        public void GetEquipmentData(EquipmentShadowConfig window)
        {
            if (window == null)
            {
                ShadowDatas = new List<KeyValueData>();
                return;
            }

            ShadowDatas = window.ShadowDatas;
        }
    }
}