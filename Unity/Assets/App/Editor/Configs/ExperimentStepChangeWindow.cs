using System.Collections.Generic;
using System.IO;
using ExperimentFramework;
using ExperimentFramework.Evaluation.Config;
using Sirenix.OdinInspector;
using UnityEngine;

namespace App
{
    public class ExperimentStepChangeWindow
    {

        [Title("需要修改的数据")] 
        [LabelText("修改摄像机默认位置")]
        public bool editCameraTransform;
        
        [ShowIf("editCameraTransform")]
        [LabelText("修改摄像机位置")]
        public TransformData NormalCamera;

        [Button(ButtonSizes.Large, Name = "获取相机坐标并转化")]
        public void GetCameraData()
        {
            TransformData CurrentCamera = new TransformData(); 
            
            CurrentCamera.Position = Camera.main.transform.position;
            CurrentCamera.Rotate = new Vector3(Mathf.Repeat(Camera.main.transform.eulerAngles.x+180f,360f)-180f, Mathf.Repeat(Camera.main.transform.eulerAngles.y+180f,360f)-180f, Mathf.Repeat(Camera.main.transform.eulerAngles.z+180f,360f)-180f);
            CurrentCamera.Scale = Camera.main.transform.localScale;

            var tempPos = new Vector3(CurrentCamera.Position.x, CurrentCamera.Position.y, 0) +(Quaternion.Euler(CurrentCamera.Rotate) * Vector3.forward * (0 - CurrentCamera.Position.z));
            NormalCamera.Position = new Vector3(tempPos.x, tempPos.y, CurrentCamera.Position.z);
            NormalCamera.Rotate = CurrentCamera.Rotate;
            NormalCamera.Scale = CurrentCamera.Scale;
        }
        
        [LabelText("修改摄像机描点位置")]
        public bool editPlayer;

        [ShowIf("editPlayer")]
        [LabelText("描点Y轴值位置")]
        public float normalPlayer;

        [LabelText("修改总时间")]
        public bool editDuration;

        [ShowIf("editDuration")]
        [LabelText("总时间(分钟)")]
        public float examDuration;
        
        [LabelText("修改分数")]
        public bool editFraction;
        [ShowIf("editFraction")]
        [LabelText("总分数")]
        public int examFraction;
        
        [Title("筛选条件")]
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
            if (!editCameraTransform && !editDuration && !editFraction) return;

            var dicChanges = new Dictionary<string, EvaluationConfig>();
            
            if (IsGlobal)
            {
                var dicChemistrys = GetExperimentStepConfigs("EC");
                var dicPhysical = GetExperimentStepConfigs("EP");

                AddNewChangeStepConfigs(dicChemistrys,ref dicChanges);
                AddNewChangeStepConfigs(dicPhysical,ref dicChanges);
            }
            else
            {
                switch (subjectType)
                {
                    case SubjectType.All:
                        var dicChemistrys = GetExperimentStepConfigs("EC", ExperimentNumbers);
                        var dicPhysical = GetExperimentStepConfigs("EP", ExperimentNumbers);

                        AddNewChangeStepConfigs(dicChemistrys, ref dicChanges);
                        AddNewChangeStepConfigs(dicPhysical, ref dicChanges);

                        break;
                    case SubjectType.Chemistry:
                        var dicChemistrysOnly = GetExperimentStepConfigs("EC", ExperimentNumbers);

                        AddNewChangeStepConfigs(dicChemistrysOnly, ref dicChanges);
                        break;
                    case SubjectType.Physics:
                        var dicPhysicalOnly = GetExperimentStepConfigs("EP", ExperimentNumbers);

                        AddNewChangeStepConfigs(dicPhysicalOnly, ref dicChanges);
                        break;
                }
            }

            SaveJson(dicChanges);
        }
        
        
        void AddNewChangeStepConfigs(Dictionary<string, EvaluationConfig> normals,
            ref Dictionary<string, EvaluationConfig> dicChanges)
        {
            
            foreach (var item in normals)
            {

                bool haveEdit = false;
                
                if (editCameraTransform)
                {
                    item.Value.CameraNormal = NormalCamera;
                    haveEdit = true;
                }

                if (editPlayer)
                {
                    item.Value.PlayerNormal.Position.y = normalPlayer;
                    haveEdit = true;
                }

                if (editDuration)
                {
                    item.Value.ExamDuration = examDuration;
                    haveEdit = true;
                }

                if (editFraction)
                {
                    item.Value.ExamFraction = examFraction;
                    haveEdit = true;
                }

                if (haveEdit && !dicChanges.ContainsKey(item.Key))
                {
                    dicChanges.Add(item.Key, item.Value);
                }
            }
        }

        public Dictionary<string, EvaluationConfig> GetExperimentStepConfigs(string subject,List<string> numbers=null)
        {
            Dictionary<string, EvaluationConfig> configs = new Dictionary<string, EvaluationConfig>();
            
            string[] stepsFiles = Directory.GetFiles(Application.dataPath + ExperimentConfigWindow.StepResourcePath(subject));
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
                    
                    var config= JsonHelper.JsonToObject<EvaluationConfig>(jsonData);
                    
                    if(configs.ContainsKey(filePath)) continue;

                    configs.Add(filePath, config);
                }          
            }

            return configs;
        }

        public void SaveJson(Dictionary<string, EvaluationConfig> configs)
        {
            foreach (var config in configs)
            {
                var jsonData = JsonHelper.ObjectToJsonString(config.Value);
                JsonHelper.SaveJson(jsonData, config.Key);
            }
        }
    }
}