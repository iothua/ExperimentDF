using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ExperimentFramework.Evaluation
{

    /// <summary>
    /// 步骤动作数据的状态
    /// </summary>
    [System.Serializable]
    public class StepActionStatus
    {

        public StepActionStatus(float progress, bool achieved)
        {
            this.Progress = progress;
            this.Achieved = achieved;
        }

        /// <summary>
        /// 进度值（属于进度值才有用）
        /// </summary>
        public float Progress;
    
        /// <summary>
        /// 步骤是否已经完成
        /// </summary>
        public bool Achieved;

        public bool IsError;
        public string Error;
    }
    
    /// <summary>
    /// 步骤报告
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class StepReport
    {
        /// <summary>
        /// 步骤数据
        /// </summary>
        [JsonIgnore]
        public StepData stepData;

        /// <summary>
        /// 步骤动作集(录制数据)
        /// </summary>
        public OperateStepSettlement StepSettlement;

        /// <summary>
        /// 实际操作时间(秒为单位)
        /// </summary>
        public int ActualTime;

        /// <summary>
        /// 明确推荐时间
        /// </summary>
        public float DefiniteTime;
        /// <summary>
        /// 明确步骤分数
        /// </summary>
        public float DefiniteFraction;
        
        public int Level;
        
        /// <summary>
        /// 进度值
        /// </summary>
        public float Progress;
        
        /// <summary>
        /// 完成度
        /// </summary>
        public bool Achieved;

        private List<StepActionStatus> list;
        //判断步骤有没有完成
        
        private string[] errors;

        [JsonIgnore]
        public string[] Errors
        {
            get
            {
                if (errors == null || errors.Length == 0)
                {
                    errors = stepData.Error.Split(';');
                }

                return errors;
            }
        }
        
        /// <summary>
        /// 错误总结
        /// </summary>
        public string ErrorSummarize;

        /// <summary>
        /// 点评
        /// </summary>
        public string Reviews;

        public bool IsStepStart;

        public bool IsExam;
        
        public bool IsDone()
        {
            if (IsExam && StepSettlement != null && StepSettlement.FlagCount > 0) return false;

            return Achieved;
        }

        public bool IsExamDone()
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].Achieved) return false;
            }

            return true;
        }

        public StepReport()
        { 
        
        }

        public StepReport(StepData stepData,bool isExam,float sumDuration,float sumFraction)
        {
            InitStepReport(stepData, isExam);
            this.DefiniteTime = sumDuration * stepData.FinishTime;
            this.DefiniteFraction = sumFraction * stepData.Fraction;
        }

        public void InitStepReport(StepData stepData, bool isExam)
        {
            this.IsExam = isExam;
            this.stepData = stepData;

            list = new List<StepActionStatus>();

            errors = stepData.Error.Split(';');

            for (var i = 0; i < stepData.Actions.Count; i++)
            {
                var action = stepData.Actions[i];
                action.StepID = stepData.TagID;
                stepData.Actions[i] = action;

                var status = new StepActionStatus(0, false);
                list.Add(status);
            }
        }

        public void Unlock()
        {
            Achieved = true;
            ErrorSummarize = string.Empty;
            IsStepStart = true;
        }

        public void ActionUnlock(string actionID)
        {
            int index = FindIndex(actionID);

            ActionUnlock(index);
        }

        public void ActionUnlock(int index)
        {
            if (index == -1)
            {
                return;
            }
            list[index].Achieved = true;
            IsStepStart = true;
            
            Done();
        }

        public ActionData GetCurrentAction()
        {
            var actionId = new ActionData();

            if (list == null) return actionId;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Achieved) continue;
                
                actionId = stepData.Actions[i];

#if UNITY_ANDROID
                if (actionId.Command.Contains("实验报告"))
                {
                    ActionUnlock(i);

                    if (i == list.Count - 1)
                    {
                        return new ActionData();
                    }

                    continue;
                }
#endif
                break;
            }
            
            return actionId;
        }

        /// <summary>
        /// 获取动作数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ActionData GetAction(int index)
        {
            return stepData.Actions[index];
        }

        public ActionData GetAction(string actionId)
        {
            var index = FindIndex(actionId);

            if (index == -1)
            {
                throw new Exception(actionId + " 动作不存在");
            }

            return GetAction(index);
        }

        /// <summary>
        /// 获取动作完成情况
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool GetActionDone(int index)
        {
            if (index == -1 || index >= list.Count)
                throw new Exception("索引超出范围");
            
            return list[index].Achieved;
        }

        public bool PreviousActionDone(ActionData actionData,out ActionData previous)
        {
            int index = FindIndex(actionData.TagID);
            previous = new ActionData();
            if (index == 0 || index == -1)
            {
                return true;
            }

            for (int i = index - 1; i > 0; i--)
            {
                //判断上一个关键动作是否完成
                var tempAction = GetAction(i);
                if (!tempAction.KeyAction)
                {
                    continue;
                }

                previous = tempAction;

                return GetActionDone(i);
            }
            
            return true;
        }

        /// <summary>
        /// 获取动作完成情况，并且返回动作数据
        /// </summary>
        /// <param name="actionId"></param>
        /// <param name="actionData"></param>
        /// <returns></returns>
        public bool GetActionDone(string actionId, out ActionData actionData)
        {
            int index = FindIndex(actionId);

            actionData = GetAction(index);
            
            bool result = GetActionDone(index);

            return result;
        }

        public bool GetActionDone(string[] nums)
        {
            bool result = true;

            for (int i = 0; i < nums.Length; i++)
            {
                result = GetActionDone(int.Parse(nums[i]));

                if (!result) break;
            }

            return result;
        }

        /// <summary>
        /// 手动设置动作完成状态
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="result">结果值</param>
        /// <exception cref="Exception">异常处理</exception>
        public void SetActionDone(int index, bool result)
        {
            if (index == -1 || index >= list.Count)
                throw new Exception("索引超出范围");

            list[index].Achieved = result;
        }

        public void AddActionProgress(string actionID, float progress)
        {
            int index = FindIndex(actionID);

            list[index].Progress += progress;
            
            list[index].Achieved = list[index].Progress >= stepData.Actions[index].ProgressGoal;

            if (list[index].Achieved)
            {
                Done();
            }
        }
         
        public void AddError(string error,string actionId)
        {
            int index = FindIndex(actionId);
            if (index == -1) return;
            list[index].IsError = true;
            list[index].Error = error;
        }

        public void AddError(int index,string actionId)
        {
            if (index >= Errors.Length) return;

            AddError(Errors[index],actionId);
        }

        /// <summary>
        /// 判断是否需要设置完成设置完成
        /// </summary>
        void Done()
        {
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];


                if (IsExam)
                {
                    if (stepData.Actions[i].Fraction == 0) continue;

                    if (!item.Achieved || item.IsError || (StepSettlement != null && StepSettlement.FlagCount > 0))
                    {
                        Achieved = false;
                        return;
                    }
                }
                else
                {
                    if (!item.Achieved)
                    {
                        Achieved = false;
                        return;
                    }
                }
            }
            
            Achieved = true;
        }

        public int FindIndex(string key)
        {
            return stepData.Actions.FindIndex(obj => obj.TagID.Equals(key));
        }

        /// <summary>
        /// 获取到未完成的集合对象
        /// </summary>
        /// <returns></returns>
        public List<ActionData> GetUndone()
        {
            //TODO
            
            List<ActionData> actionDatas=new List<ActionData>();

            for (int i = 0; i < list.Count; i++)
            {
                if (IsExam && stepData.Actions[i].Fraction == 0) continue;
                
                if (!list[i].Achieved || list[i].IsError)
                {
                    actionDatas.Add(stepData.Actions[i]);
                }
            }
            
            return actionDatas;
        }

        public List<ActionData> GetUndoneAchieved()
        {
            List<ActionData> actionDatas = new List<ActionData>();

            for (int i = 0; i < list.Count; i++)
            {
                if (IsExam && stepData.Actions[i].Fraction == 0) continue;
                
                if (!list[i].Achieved)
                {
                    actionDatas.Add(stepData.Actions[i]);
                }
            }

            return actionDatas;
        }

        public List<ActionData> GetActionError()
        {
            List<ActionData> actionDatas = new List<ActionData>();

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].IsError)
                {
                    actionDatas.Add(stepData.Actions[i]);
                }
            }

            return actionDatas;
        }

        /// <summary>
        /// 获取此步骤的分数
        /// </summary>
        /// <returns></returns>
        public float GetFraction()
        {
            if (Achieved) return DefiniteFraction;

            float fraction = 0;

            if (list == null) return 0;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Achieved)
                {
                    //累加动作集分数
                    fraction += stepData.Actions[i].Fraction;
                }
            }

            if (StepSettlement != null && StepSettlement.FlagCount > 0)
            {
                fraction = 0;
            }

            return fraction * DefiniteFraction;
        }

        /// <summary>
        /// 最后结算
        /// </summary>
        public void Settlement()
        {
            
            //如果某些动作没完成，则错误原因是某些动作提示，并且点评是如此
            int number = 1;

            int reviewNumber = 1;
            if (StepSettlement != null)
            {
                for (int i = 0; i < StepSettlement.StepDatas.Count; i++)
                {
                    ErrorSummarize += $"{number}:{StepSettlement.StepDatas[i].Description}\r\n";
                    number++;

                    Level = 3;
                }
            }
            else
            {
                var actions = GetUndoneAchieved();

                for (int i = 0; i < actions.Count; i++)
                {
                    ErrorSummarize += $"{number}:{actions[i].Error}\r\n";
                    number++;
                }

                if (actions.Count > 0)
                    Level = 3;
                
                if (actions.Count > 0)
                {
                    Reviews += $"{reviewNumber}、按照正确步骤进行操作\r\n";
                    reviewNumber++;
                }
            }

            string reviewNumberStr = reviewNumber == 1 ? "" : "2、";

            Reviews += $" {reviewNumberStr}{stepData.Reviews}";

            if (Level == 3) return;
            
            //计算等级
            var result = ActualTime / DefiniteTime;
            
            if (result < 0.95f)
            {
                Level = 1;
            }
            else if (result < 1.2f)
            {
                Level = 2;
            }
            else
            {
                Level = 3;
            }
        }
    }
}