using System;
using System.Collections.Generic;
using App.Hand;
using Sirenix.OdinInspector;
using UnityEngine;

namespace App
{
    public class EquipmentHandWindow
    {
        public string filePath = "/DynamicAssets/Configs/ExperimentHandConfig.json";
        
        [HorizontalGroup]
        [Button(ButtonSizes.Large,Name = "加载资源")]
        public void LoadData()
        {
            string fullPath = Application.dataPath + filePath;
            
            if (System.IO.File.Exists(fullPath))
            {
                string jsonData = JsonHelper.ReadJsonString(fullPath);
                var shadowWindow = JsonHelper.JsonToObject<ExperimentHandConfig>(jsonData);
                if (shadowWindow != null)
                {
                    GetData(shadowWindow);
                }
            }
        }
        
        [HorizontalGroup]
        [Button(ButtonSizes.Large,Name = "保存更改")]
        public void SaveData()
        {
            var window = new ExperimentHandConfig()
            {
                ExperimentHandDatas = ExperimentHandDatas,
                ModelHandDatas = ModelHandDatas
            };
            
            string jsonData = JsonHelper.ObjectToJsonString(window);

            if (string.IsNullOrEmpty(filePath))
            {
                string fileName = "临时" + DateTime.Now.ToString("yy-MM-dd-hh-mm-ss");
                filePath = ExperimentConfigWindow.EquipmentResourcePath(fileName) + fileName;
            }

            JsonHelper.SaveJson(jsonData, Application.dataPath + filePath);
            GetData(window);
        }

        [LabelText("实验手数据")]
        [TableList(ShowIndexLabels = true)]
        public List<ExperimentHandData> ExperimentHandDatas;
        
        [LabelText("模型手数据")]
        [TableList(ShowIndexLabels = true)]
        public List<ExperimentModelHandData> ModelHandDatas;

        public void GetData(ExperimentHandConfig config)
        {
            if (config == null)
            {
                config = new ExperimentHandConfig();
            }

            ExperimentHandDatas = config.ExperimentHandDatas;
            ModelHandDatas = config.ModelHandDatas;
        }

    }
}