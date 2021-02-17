using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ExperimentFramework.Evaluation.Config
{
    /// <summary>
    /// 步骤配置，用于读取步骤数据
    /// </summary>
    [Serializable]
    public class EvaluationConfig
    {
        /// <summary>
        /// 实验名称
        /// </summary>
        public string ExperimentName;

        /// <summary>
        /// 实验编号
        /// </summary>
        public string ExperimentNumber;

        /// <summary>
        /// 实验目的
        /// </summary>
        public string ExperimentPurpose;

        /// <summary>
        /// 实验器材
        /// </summary>
        public string ExperimentEquipment;

        /// <summary>
        /// 考试时长
        /// </summary>
        public float ExamDuration = 10;

        /// <summary>
        /// 考试分数
        /// </summary>
        public int ExamFraction = 15;
        
        [Header("使用化学库")]
        public bool useChemicalLibrary;

        [Header("使用放大镜")]
        public bool useMagnifier;

        /// <summary>
        /// 表格公式计算类(用于计算数据对错)
        /// </summary>
        //public string Formula = "HuaCore.Evaluation.FormulaBase";

        [Title("2、摄像机配置")]
        public TransformData CameraNormal;
        
        public float CameraNormalTargetFOV = 60;
        
        public TransformData PlayerNormal;
        
        /// <summary>
        /// 步骤数据
        /// </summary>
        [Title("3、实验步骤配置")]
        public List<StepData> Steps;
        
        /// <summary>
        /// 系统数据
        /// </summary>
        [Title("4、实验报告配置")]
        public List<Answer> Automatics = new List<Answer>();

    }
}