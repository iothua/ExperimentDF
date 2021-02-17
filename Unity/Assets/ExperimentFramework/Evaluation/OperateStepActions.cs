using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace ExperimentFramework.Evaluation
{
    /// <summary>
    /// 操作步骤动作集(完成)
    /// 1、一种是文本
    /// 2、是实时添加错误节点
    /// 3、是最后结算时，来判断错误节点
    /// </summary>
    [Serializable]
    public class OperateStepActions
    {
        /// <summary>
        /// 动作集
        /// </summary>
        public Dictionary<string, OperateStepSettlement> StepOperates = new Dictionary<string, OperateStepSettlement>();
        
        public OperateStepSettlement GetSettlement(string stepID)
        {
            StepOperates.TryGetValue(stepID, out var settlement);

            return settlement;
        }

        public bool ContainsFlag(string stepID,string description)
        {
            if (!StepOperates.ContainsKey(stepID)) return false;

            StepOperates.TryGetValue(stepID, out var stepSettlement);

            return stepSettlement != null && stepSettlement.ContainsFlag(description);
        }

        public bool AddReplayFlag(string stepID,string replayName, ReplayFlag replayFlag)
        {
            if (StepOperates.ContainsKey(stepID))
            {
                var settlement = StepOperates[stepID];

                settlement.AddReplayFlag(replayFlag);

                return false;
            }
            else
            {
                var settlement = AddStepSettlement(stepID, replayName);

                settlement.StepDatas = new List<ReplayFlag>();
                
                settlement.AddReplayFlag(replayFlag);
                
                StepOperates.Add(stepID, settlement);
                return true;
            }
        }

        public OperateStepSettlement AddStepSettlement(string stepID,string replayName)
        {
            if (StepOperates.ContainsKey(stepID)) return StepOperates[stepID];
            
            var settlement = new OperateStepSettlement();

            settlement.stepID = stepID;
            settlement.replayFile = replayName + ".mp4";
            settlement.StepDatas = new List<ReplayFlag>();
            
            StepOperates.Add(stepID, settlement);

            return settlement;
        }
    }

    /// <summary>
    /// 操作步骤结算(每一个步骤都需要)
    /// </summary>
    [Serializable]
    public class OperateStepSettlement
    {
        /// <summary>
        /// 步骤ID
        /// </summary>
        public string stepID;
        
        /// <summary>
        /// 录制文件名
        /// </summary>
        public string replayFile;
        
        /// <summary>
        /// 总的时长
        /// </summary>
        public float duration;

        /// <summary>
        /// 错误操作集合
        /// </summary>
        public List<ReplayFlag> StepDatas = new List<ReplayFlag>();

        public List<string> FlagDescription = new List<string>();

        public int FlagCount => StepDatas.Count;

        public bool ContainsFlag(string description)
        {
            return StepDatas.Any(stepData => stepData.Description.Equals(description));
        }

        public void AddReplayFlag(ReplayFlag flag)
        {
            if (FlagDescription.Contains(flag.Description)) return;

            FlagDescription.Add(flag.Description);
            StepDatas.Add(flag);
        }
    }
    
    /// <summary>
    /// 操作步骤数据(针对错误操作)
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public struct ReplayFlag
    {
        /// <summary>
        /// 坐标
        /// </summary>
        [JsonIgnore]
        public Transform Transform { get; set; }

        public EVector3 Position;

        public int width;
        public int height;

        /// <summary>
        /// 错误类型
        /// </summary>
        public FlagErrorType flagError;
        
        /// <summary>
        /// 录制时长
        /// </summary>
        public float RecordTime;

        /// <summary>
        /// 偏移量
        /// </summary>
        public float VerticalPositionOffset;
        
        /// <summary>
        /// 描述
        /// </summary>
        public string Description;
    }

    /// <summary>
    /// 错误类型
    /// </summary>
    public enum FlagErrorType
    {
        /// <summary>
        /// 操作错误
        /// </summary>
        OperateError = 0,
        /// <summary>
        /// 顺序错误
        /// </summary>
        OrderError,
        /// <summary>
        /// 操作遗漏
        /// </summary>
        OperateOmission
    }
}