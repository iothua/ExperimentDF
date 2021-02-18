using System;
using System.Collections.Generic;
using System.IO;
using App.Config;
using ExperimentFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace App
{
    
    public class EquipmentTransformWindow
    {

        [InfoBox("更改规则：只会更改【架子上仪器】与【桌面上仪器】\r\n")]
        [LabelText("仪器名称"),Tooltip("对应配置文件中的实验名称")]
        public string EquipmentName;
        
        [LabelText("新的坐标"),Tooltip("直接修改仪器配置中的InitEquipment属性")]
        public List<TransformData> UpdateDtas;
        
        [LabelText("全局更改"),Tooltip("遍历所有实验文件，存在同一个名称的都会进行更改")]
        public bool IsGlobal;

        [HideIf("IsGlobal")]
        [LabelText("筛选学科")]
        public SubjectType subjectType = SubjectType.All;

        [HideIf("IsGlobal")]
        [LabelText("实验编号指定")]
        public List<string> ExperimentNumbers;

        [LabelText("实验编号过滤"),Tooltip("过滤掉指定的实验编号文件")]
        public List<string> ExperimentFilter;

        [Button(ButtonSizes.Large,Name = "开始修改")]
        public void UpdateEquipment()
        {
            //检索到所有的文件，实例化并且遍历出来

            if (string.IsNullOrEmpty(EquipmentName) || UpdateDtas.Count == 0) return;

            var dicChanges = new Dictionary<string, ExperimentEquipmentConfig>();
            
            if (IsGlobal)
            {
                var dicChemistrys = GetExperimentEquipmentConfigs("EC");
                var dicPhysical = GetExperimentEquipmentConfigs("EP");

                AddNewChangeEquipmentConfigs(dicChemistrys,ref dicChanges);
                AddNewChangeEquipmentConfigs(dicPhysical,ref dicChanges);
            }
            else
            {
                switch (subjectType)
                {
                    case SubjectType.All:
                        var dicChemistrys = GetExperimentEquipmentConfigs("EC", ExperimentNumbers);
                        var dicPhysical = GetExperimentEquipmentConfigs("EP", ExperimentNumbers);

                        AddNewChangeEquipmentConfigs(dicChemistrys, ref dicChanges);
                        AddNewChangeEquipmentConfigs(dicPhysical, ref dicChanges);

                        break;
                    case SubjectType.Chemistry:
                        var dicChemistrysOnly = GetExperimentEquipmentConfigs("EC", ExperimentNumbers);

                        AddNewChangeEquipmentConfigs(dicChemistrysOnly, ref dicChanges);
                        break;
                    case SubjectType.Physics:
                        var dicPhysicalOnly = GetExperimentEquipmentConfigs("EP", ExperimentNumbers);

                        AddNewChangeEquipmentConfigs(dicPhysicalOnly, ref dicChanges);
                        break;
                }
            }

            SaveJson(dicChanges);
        }

        void AddNewChangeEquipmentConfigs(Dictionary<string, ExperimentEquipmentConfig> normals,
            ref Dictionary<string, ExperimentEquipmentConfig> dicChanges)
        {
            foreach (var item in normals)
            {
                var result = ChangeEquipmentAll(item.Value, EquipmentName, UpdateDtas);
                if (!result) continue;
                if(dicChanges.ContainsKey(item.Key)) continue;
                dicChanges.Add(item.Key, item.Value);
            }
        }

        public bool ChangeEquipmentAll(ExperimentEquipmentConfig equipmentConfig, string equipmentKey, List<TransformData> datas)
        {
            bool isShelf = ChangeEquipments(equipmentConfig.ShelfEquipments, equipmentKey, datas);
            bool isDesktop = ChangeEquipments(equipmentConfig.DesktopEquipments, equipmentKey, datas);

            return isShelf|| isDesktop;
        }

        bool ChangeEquipments(List<ExperimentEquipmentData> equipmentDatas, string equipmentKey,
            List<TransformData> datas)
        {
            if (equipmentDatas == null) return false;
            
            foreach (var item in equipmentDatas)
            {
                if (item.EquipmentName.Equals(equipmentKey))
                {
                    ChangeEquipmentData(item, datas);
                    return true;
                }
            }

            return false;
        }

        void ChangeEquipmentData(ExperimentEquipmentData equipmentData, List<TransformData> datas)
        {
            for (int i = 0; i < datas.Count; i++)
            {
                if ( i<equipmentData.InitEquipment.Count)
                {
                    equipmentData.InitEquipment[i] = datas[i];
                }
                else
                {
                    return;
                }
            }
        }

        public Dictionary<string, ExperimentEquipmentConfig> GetExperimentEquipmentConfigs(string subject,List<string> numbers=null)
        {
            Dictionary<string, ExperimentEquipmentConfig> configs = new Dictionary<string, ExperimentEquipmentConfig>();
            
            string[] stepsFiles = Directory.GetFiles(Application.dataPath + ExperimentConfigWindow.EquipmentResourcePath(subject));
            if (stepsFiles.Length != 0)
            {
                for (int i = 0; i < stepsFiles.Length; i++)
                {
                    var filePath = stepsFiles[i];
                    
                    if (!Path.GetExtension(filePath).Equals(".json"))
                    {
                        continue;
                    }

                    string fileName = Path.GetFileNameWithoutExtension(filePath);

                    if (numbers != null && numbers.Count>0 && !numbers.Contains(fileName)) continue;

                    if (ExperimentFilter != null && numbers.Count > 0 && ExperimentFilter.Contains(fileName)) continue;

                    string jsonData = JsonHelper.ReadJsonString(filePath);
                    
                    ExperimentEquipmentConfig config= JsonHelper.JsonToObject<ExperimentEquipmentConfig>(jsonData);
                    
                    if(configs.ContainsKey(filePath)) continue;

                    configs.Add(filePath, config);
                }          
            }

            return configs;
        }

        public void SaveJson(Dictionary<string, ExperimentEquipmentConfig> configs)
        {
            foreach (var config in configs)
            {
                var jsonData = JsonHelper.ObjectToJsonString(config.Value);
                JsonHelper.SaveJson(jsonData, config.Key);
            }
        }
    }
}