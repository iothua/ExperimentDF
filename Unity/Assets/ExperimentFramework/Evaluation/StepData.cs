using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ExperimentFramework.Evaluation
{

    /// <summary>
    /// 标签数据
    /// </summary>
    [Serializable]
    public struct LabelData
    {
        /// <summary>
        /// 仪器名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 标签数据
        /// </summary>
        public string Label;

        /// <summary>
        /// 是否全局
        /// </summary>
        public bool IsGlobal;//是否影响全局
    }

    /// <summary>
    /// 步骤数据(不应改变，读取配置文件生成)
    /// </summary>
    [Serializable]
    public struct StepData
    {
        /// <summary>
        /// 步骤标识
        /// </summary>
        public string TagID;

        /// <summary>
        /// 步骤显示名称
        /// </summary>
        public string DisplayName;
        
        /// <summary>
        /// 完成时间(秒为单位)
        /// </summary>
        public float FinishTime;
        
        /// <summary>
        /// 是否需要录制
        /// </summary>
        public bool IsRecord;
        
        /// <summary>
        /// 分数
        /// </summary>
        public float Fraction;

        /// <summary>
        /// 错误说明
        /// </summary>
        public string Error;
        
        /// <summary>
        /// 点评
        /// </summary>
        public string Reviews;

        /// <summary>
        /// 动作集
        /// </summary>
        public List<ActionData> Actions;
    }

    /// <summary>
    /// 动作数据
    /// </summary>
    [Serializable]
    public struct ActionData
    {
        /// <summary>
        /// 步骤标识
        /// </summary>
        [JsonIgnore]
        public string StepID { get; set; }

        /// <summary>
        /// 动作标识
        /// </summary>
        public string TagID;
        
        /// <summary>
        /// 说明
        /// </summary>
        public string Description;

        /// <summary>
        /// 关键动作(用于触发步骤)
        /// </summary>
        public bool KeyAction;

        /// <summary>
        /// 关联动作节点，当这些关联动作都完成时，该动作才会被启动
        /// </summary>
        public string RelationActions;

        /// <summary>
        /// 设置标签
        /// 当该动作启动时，需要给哪些仪器标签，如果没有设置，则默认指定第0个。
        /// </summary>
        public List<LabelData> SetLabels;

        public List<LabelData> DeleteLabels;
        
        /// <summary>
        /// 关联标签(同一个仪器，多个标签可用逗号分开)
        /// </summary>
        public List<LabelData> RelationLabels;
        
        /// <summary>
        /// 分数
        /// </summary>
        public float Fraction;

        /// <summary>
        /// 错误说明
        /// </summary>
        public string Error;

        /// <summary>
        /// 是否属于进度值(适用于记录数据操作)
        /// </summary>
        public bool IsProgress;
        
        /// <summary>
        /// 达到的进度目标
        /// </summary>
        public float ProgressGoal;

        /// <summary>
        /// 操作命令
        /// </summary>
        public string Command;

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (!(obj is ActionData)) return false;

            var actionData = (ActionData) obj;

            return TagID.Equals(actionData.TagID) && StepID.Equals(actionData.StepID) &&
                   Command.Equals(actionData.Command);
        }

        public string ActionCommand() {

            return Command.Split('&')[0];
        }

        public string ActionCommandParameter()
        {
            var parames = Command.Split('&');
            if (parames.Length >= 2)
            {
                return parames[1];
            }

            return string.Empty;
        }

    }
}