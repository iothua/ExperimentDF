using UnityEngine;
using System;
using System.Collections.Generic;
using ExperimentFramework;
using ExperimentFramework.Evaluation;
using ExperimentFramework.Evaluation.Config;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;

namespace App
{
    /// <summary>
    /// 实验步骤窗体
    /// </summary>
    [Serializable]
    public class ExperimentStepWindow
    {
        
        
        /// <summary>
        /// 实验名称
        /// </summary>
        [LabelText("实验名称")]
        public string ExperimentName;

        /// <summary>
        /// 实验编号
        /// </summary>
        [ LabelText("实验编号")] public string ExperimentNumber;

        /// <summary>
        /// 实验目的
        /// </summary>
        [LabelText("实验目的"),TextArea]
        public string ExperimentPurpose;

        /// <summary>
        /// 实验器材
        /// </summary>
        [LabelText("实验器材"),TextArea]
        public string ExperimentEquipment;

        /// <summary>
        /// 考试时长
        /// </summary>
        [LabelText("考试时长"),Tooltip("以分为单位")]
        public float ExamDuration = 10;

        [LabelText("考试分数")]
        public int ExamFraction = 10;
        
        [ LabelText("激活化学库")] 
        public bool useChemicalLibrary;

        [LabelText("激活放大镜")]
        public bool useMagnifier;

        /// <summary>
        /// 表格公式计算类(用于计算数据对错)
        /// </summary>
        //[LabelText("自定义表格反射类")]
        //public string Formula = "HuaCore.Evaluation.FormulaBase";
        
        [Space(10)]
        [LabelText("摄像机初始位置")]
        public TransformData CameraNormal;
        
        [LabelText("摄像机初始FOV")]
        public float CameraNormalTargetFOV = 60;
        
        [Space(10)]
        [LabelText("Player初始位置"),Tooltip("摄像机初始朝向位置,只需要设置Y轴即可")]
        public TransformData PlayerNormal;

        [Space(10)]
        [LabelText("实验步骤数据配置")]
        [TableList(ShowIndexLabels = true)]
        public List<ExperimentStepDataWindow> Steps;
        
        [Space(10)]
        [LabelText("实验报告答案配置")]
        [TableList(DrawScrollView = false)]
        public List<AnswerDataWindow> Automatics;

        private EvaluationConfig _config;
        private string filePath;
        
        public void GetStepData(EvaluationConfig config,string filePath)
        {
            this._config = config;
            this.filePath = filePath;
            
            if (Steps != null && Steps.Count > 0)
                Steps.Clear();
            
            if(Automatics!=null&& Automatics.Count>0)
                Automatics.Clear();
            
            ExperimentName = config.ExperimentName;
            ExperimentNumber = config.ExperimentNumber;
            ExperimentPurpose = config.ExperimentPurpose;
            ExperimentEquipment = config.ExperimentEquipment;
            ExamDuration = config.ExamDuration;
            ExamFraction = config.ExamFraction;
            
            useChemicalLibrary = config.useChemicalLibrary;
            useMagnifier = config.useMagnifier;

            //Formula = config.Formula;
            CameraNormal = config.CameraNormal;
            CameraNormalTargetFOV = config.CameraNormalTargetFOV;
            PlayerNormal = config.PlayerNormal;

            foreach (var step in config.Steps)
            {
                if (Steps == null)
                {
                    Steps=new List<ExperimentStepDataWindow>();
                }
                
                var stepDataWindow = new ExperimentStepDataWindow();
                stepDataWindow.SetStepData(step);
                
                Steps.Add(stepDataWindow);
            }

            //添加自动数据
            foreach (var answer in config.Automatics)
            {
                if (Automatics == null)
                {
                    Automatics = new List<AnswerDataWindow>();
                }

                var automatics = new AnswerDataWindow()
                {
                    Value = answer.Value,
                    Range = answer.Range,
                    Fraction = answer.Fraction
                };

                Automatics.Add(automatics);
            }
        }
        
        [Button("保存配置数据")]
        public void SaveSetpData()
        {
            //保存数据
            _config.ExperimentName = ExperimentName;
            _config.ExperimentNumber = ExperimentNumber;
            _config.ExperimentPurpose = ExperimentPurpose;
            _config.ExperimentEquipment = ExperimentEquipment;
            _config.ExamDuration = ExamDuration;
            _config.ExamFraction = ExamFraction;

            _config.useChemicalLibrary = useChemicalLibrary;
            _config.useMagnifier = useMagnifier;

            //_config.Formula = Formula;
            _config.CameraNormal = CameraNormal;
            _config.CameraNormalTargetFOV = CameraNormalTargetFOV;
            _config.PlayerNormal = PlayerNormal;

            _config.Steps.Clear();
            
            foreach (var step in Steps)
            {
                _config.Steps.Add(step.GetStepData());
            }

            _config.Automatics.Clear();
            foreach (var automatic in Automatics)
            {
                _config.Automatics.Add(new Answer()
                {
                    Value = automatic.Value,
                    Range = automatic.Range,
                    Fraction = automatic.Fraction
                });
            }
            
            string jsonData = JsonHelper.ObjectToJsonString(_config);
            JsonHelper.SaveJson(jsonData, filePath);

            //重新加载一次
            GetStepData(_config, filePath);
        }
        
    }

    /// <summary>
    /// 实验步骤数据窗体
    /// </summary>
    [Serializable]
    public class ExperimentStepDataWindow
    {
        /// <summary>
        /// 步骤标识
        /// </summary>
        [LabelWidth(150),TextArea]
        public string TagID;

        /// <summary>
        /// 步骤显示名称
        /// </summary>
        [LabelWidth(200),TextArea]
        public string DisplayName;
        
        /// <summary>
        /// 完成时间(秒为单位)
        /// </summary>
        [VerticalGroup("设置"),LabelWidth(80),LabelText("完成时间"),Tooltip("基于总时间百分比")]
        [Range(0,1)]
        public float FinishTime;
        
        /// <summary>
        /// 是否需要录制
        /// </summary>
        [VerticalGroup("设置"),LabelWidth(80),LabelText("开启录制")]
        public bool IsRecord;
        
        /// <summary>
        /// 分数
        /// </summary>
        [VerticalGroup("设置"),LabelWidth(80),LabelText("步骤分数"),Tooltip("基于总时间百分比")]
        [Range(0,1)]
        public float Fraction;

        /// <summary>
        /// 错误说明
        /// </summary>
        [LabelWidth(200),TextArea]
        public string Error;
        
        /// <summary>
        /// 点评
        /// </summary>
        [LabelWidth(200),TextArea]
        public string Reviews;

        [VerticalGroup("操作"),Button("动作设置")]
        public void ActionWindow()
        {
            var actionDataWindow = new ExperimentActionWindow();
            actionDataWindow.SetActionData(_stepData.Actions);
            
            var window = OdinEditorWindow.InspectObject(actionDataWindow);
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1366, 500);
            window.titleContent=new GUIContent($"{TagID}步骤动作配置");
            // window.OnBeginGUI += () =>
            // {
            //     actionDataWindow.SetActionData(_stepData.Actions);
            // };
            window.OnClose += () =>
            {
                _stepData.Actions = actionDataWindow.GetActionDatas();
            };
        }
        
        private StepData _stepData;

        public void SetStepData(StepData stepData)
        {
            _stepData = stepData;
            
            TagID = stepData.TagID;
            DisplayName = stepData.DisplayName;
            FinishTime = stepData.FinishTime;
            IsRecord = stepData.IsRecord;
            Fraction = stepData.Fraction;
            Error = stepData.Error;
            Reviews = stepData.Reviews;
        }

        public StepData GetStepData()
        {
            _stepData.TagID = TagID;
            _stepData.DisplayName = DisplayName;
            _stepData.FinishTime = FinishTime;
            _stepData.IsRecord = IsRecord;
            _stepData.Fraction = Fraction;
            _stepData.Error = Error;
            _stepData.Reviews = Reviews;
            
            return _stepData;
        }
    }

    [Serializable]
    public class AnswerDataWindow
    {
        [HideLabel,LabelText("答案值")]        
        public string Value;
        [HideLabel, LabelText("±范围"), Tooltip("范围内视为正确")]
        public float Range;
        [HideLabel,LabelText("分数"),Tooltip("该空所占分数")]
        public float Fraction;
    }
}