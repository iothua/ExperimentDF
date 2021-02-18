using ExperimentFramework.Evaluation.Config;
using Sirenix.OdinInspector;
using UnityEngine;

namespace App
{
    public class StepDataWindow
    {

        [PropertyOrder(-10)]
        [HorizontalGroup]
        [Button(ButtonSizes.Large,Name = "加载文件")]
        public void LoadConfig()
        {
            //并将配置信息，加入到Building中
            string fullPath = Application.dataPath + ExperimentConfigWindow.StepResourcePath(fileName) + fileName + ".json";

            string jsonData = JsonHelper.ReadJsonString(fullPath);
            Config = JsonHelper.JsonToObject<EvaluationConfig>(jsonData);
        }
        [HorizontalGroup]
        [Button(ButtonSizes.Large,Name = "保存文件")]
        public void SaveConfig()
        {
            //并将配置信息，加入到Building中
            string fullPath = Application.dataPath + ExperimentConfigWindow.StepResourcePath(fileName) + fileName + ".json";
            
            string jsonData = JsonHelper.ObjectToJsonString(Config);
            
            JsonHelper.SaveJson(jsonData, fullPath);
        }
        
        [Title("数据配置")]
        public string fileName;

        
        public EvaluationConfig Config;
    }
}