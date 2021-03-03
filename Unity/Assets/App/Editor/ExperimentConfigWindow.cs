using Sirenix.OdinInspector.Editor;
using System.IO;
using App.Config;
using ExperimentFramework.Evaluation.Config;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace App
{
    /// <summary>
    /// 实验配置窗口
    /// </summary>
    public class ExperimentConfigWindow : OdinMenuEditorWindow
    {
        [MenuItem("实验帮/实验数据配置窗口 _F11")]
        private static void OpenWindow()
        {
            var window = GetWindow<ExperimentConfigWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
        }

        [SerializeField]
        private StepDataWindow stepData = new StepDataWindow();

        private ExperimentEquipmentWindow _equipmentWindow = new ExperimentEquipmentWindow();
        private EquipmentTransformWindow updateEquipmentWindow = new EquipmentTransformWindow();
        private ExperimentStepChangeWindow _stepChangeWindow = new ExperimentStepChangeWindow();

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(supportsMultiSelect: true);

            tree.Add("步骤配置/创建新文件", stepData);

            //加载步骤配置
            GenerateStepsList(tree, StepResourcePath("EC"), "化学");
            GenerateStepsList(tree, StepResourcePath("EP"), "物理");

            tree.Add("步骤配置/全局修改步骤配置", _stepChangeWindow);
            
            // //加载仪器配置
            // GenerateBackpackList(tree, "/_Configs/Backpacks/Chemistry/", "化学");
            // GenerateBackpackList(tree, "/_Configs/Backpacks/Physical/", "物理");

            tree.Add("实验仪器配置/创建新文件", _equipmentWindow);
            tree.Add("实验仪器配置/修改全局仪器坐标", updateEquipmentWindow);
            
            GenerateEquipmentConfigList(tree, EquipmentResourcePath("EC"), "化学");
            GenerateEquipmentConfigList(tree, EquipmentResourcePath("EP"), "物理");

            GenerateEquipmentShadow(tree);
            GenerateExperimentHand(tree);
            GenerateHttp(tree);

            return tree;
        }

        private void GenerateBackpackList(OdinMenuTree tree,string path, string subject)
        {
            string[] backpackFiles = Directory.GetFiles(Application.dataPath + path);
            if (backpackFiles.Length != 0)
            {
                for (int i = 0; i < backpackFiles.Length; i++)
                {
                    if (!Path.GetExtension(backpackFiles[i]).Equals(".asset"))
                    {
                        continue;
                    }
                    string fileName = Path.GetFileNameWithoutExtension(backpackFiles[i]);
                    tree.AddAssetAtPath($"仪器配置/{subject}/{fileName}",
                        path + Path.GetFileName(backpackFiles[i]));
                }
            }
        }

        private void GenerateEquipmentConfigList(OdinMenuTree tree, string path, string subject)
        {
            string[] stepsFiles = Directory.GetFiles(Application.dataPath + path);
            if (stepsFiles.Length != 0)
            {
                for (int i = 0; i < stepsFiles.Length; i++)
                {
                    if (!Path.GetExtension(stepsFiles[i]).Equals(".json"))
                    {
                        continue;
                    }

                    string fileName = Path.GetFileNameWithoutExtension(stepsFiles[i]);
                    string jsonData = JsonHelper.ReadJsonString(stepsFiles[i]);
                    
                    ExperimentEquipmentConfig config= JsonHelper.JsonToObject<ExperimentEquipmentConfig>(jsonData);

                    ExperimentEquipmentWindow stepWindow = new ExperimentEquipmentWindow();
                    stepWindow.GetEquipmentData(config, stepsFiles[i]);
                    
                    tree.Add($"实验仪器配置/{subject}/{fileName}", stepWindow);
                }          
            }
        }
        

        private void GenerateStepsList(OdinMenuTree tree, string path, string subject)
        {
            string[] stepsFiles = Directory.GetFiles(Application.dataPath + path);
            if (stepsFiles.Length != 0)
            {
                for (int i = 0; i < stepsFiles.Length; i++)
                {
                    if (!Path.GetExtension(stepsFiles[i]).Equals(".json"))
                    {
                        continue;
                    }

                    string fileName = Path.GetFileNameWithoutExtension(stepsFiles[i]);
                    string jsonData = JsonHelper.ReadJsonString(stepsFiles[i]);
                    
                    EvaluationConfig config= JsonHelper.JsonToObject<EvaluationConfig>(jsonData);

                    ExperimentStepWindow stepWindow = new ExperimentStepWindow();
                    stepWindow.GetStepData(config, stepsFiles[i]);
                    
                    tree.Add($"步骤配置/{subject}/{fileName}", stepWindow);
                }          
            }
        }

        private void GenerateEquipmentShadow(OdinMenuTree tree)
        {
            EquipmentShadowWindow window = new EquipmentShadowWindow();

            window.LoadEquipmentData();
            
            tree.Add($"虚影配置", window);

        }

        private void GenerateExperimentHand(OdinMenuTree tree)
        {
            EquipmentHandWindow window = new EquipmentHandWindow();

            window.LoadData();
            
            tree.Add($"实验手配置", window);
        }

        private void GenerateHttp(OdinMenuTree tree)
        {
            HttpConfigWindow window = new HttpConfigWindow();
            window.LoadData();
            tree.Add("Http路径配置", window);
        }

        private static string resourcePath = "/DynamicAssets/Config/ExperimentSteps/";

        public static string StepResourcePath(string fileName)
        {
            string number = fileName.Substring(0, 2);

            switch (number)
            {
                case "EC":
                    resourcePath = "/_Configs/ExperimentSteps/Chemistry/";
                    break;
                case "EP":
                    resourcePath = "/_Configs/ExperimentSteps/Physical/";
                    break;
            }

            return resourcePath;
        }

        public static string EquipmentResourcePath(string fileName)
        {
            string number = fileName.Substring(0, 2);

            switch (number)
            {
                case "EC":
                    return "/_Configs/ExperimentEquipments/Chemistry/";
                case "EP":
                    return "/_Configs/ExperimentEquipments/Physical/";
                    break;
            }

            return "/_Configs/ExperimentEquipments/";
        }

    }
}