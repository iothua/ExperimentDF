using System;
using System.Collections.Generic;
using System.Linq;
using ExperimentFramework.Evaluation.Config;
using Loxodon.Framework.Contexts;
using UnityEngine;

namespace ExperimentFramework.Evaluation
{
    public class EvaluationSystem
    {
        public enum EndStepStatus
        {
            None,
            Break,
            End
        }

        public List<StepData> Steps;

        public ExperimentalReport Report;

        /// <summary>
        /// 最终结果
        /// </summary>
        public UserExperimentData userExperimentData;

        //当前步骤
        private OperateStepSettlement currentSettlement;//当前步骤对象

        /// <summary>
        /// 当前步骤对象
        /// </summary>
        public string CurrentStepID => currentSettlement == null ? "" : currentSettlement.stepID;

        /// <summary>
        /// 当前步骤索引
        /// </summary>
        public int CurrentStepIndex { get; set; }

        public string ExperimentName { get; private set; }

        /// <summary>
        /// 动作集(外部添加)
        /// </summary>
        public OperateStepActions StepActions;

        public EvaluationConfig Config { get; private set; }

        public FormulaBase formula;

        private string storagePath;

        //实验开始时间
        public DateTime startTime;

        //步骤开始时间
        public DateTime stepStartTime;
        
        /// <summary>
        /// true:考试模型
        /// false:练习模式
        /// </summary>
        public bool IsExam;

        /// <summary>
        /// 是否开始考试
        /// </summary>
        public bool IsExaming { get; set; }
        
        //临时命令集,用于当前没办法确定动作时的处理
        private Dictionary<ActionData, int> tempActionDatas = new Dictionary<ActionData, int>();

        private Dictionary<ActionData, ActionDataIdentity> tempActionIdentitys =
            new Dictionary<ActionData, ActionDataIdentity>();
        
        /// <summary>
        /// 命令库
        /// </summary>
        public Dictionary<string, List<Vector2Int>> CommandLibrary = new Dictionary<string, List<Vector2Int>>();

        public void Init(EvaluationConfig config)
        {
            Config = config;
            
            Steps = Config.Steps;

            ExperimentName = Config.ExperimentName;

        }

        public string GetStepNumber(string stepID)
        {
            var index = userExperimentData.FindIndex(stepID);
            return Config.ExperimentNumber + index;
        }

        public int FindIndex(string stepID)
        {
            return Steps.FindIndex(obj => obj.TagID.Equals(stepID));
        }

        /// <summary>
        /// 启动系统
        /// </summary>
        public void StartSystem(bool isExam)
        {
            this.IsExam = isExam;

            Report = new ExperimentalReport();
            Report.LoadAutomatics(Config.Automatics);

            StepActions = new OperateStepActions();

            userExperimentData = new UserExperimentData();
            userExperimentData.ExperimentNumber = Config.ExperimentNumber;
            userExperimentData.ExperimentName = Config.ExperimentName;

            for (int i = 0; i < Steps.Count; i++)
            {
                var report = new StepReport(Steps[i], isExam, Config.ExamDuration * 60, Config.ExamFraction);
                userExperimentData.StepReports.Add(report);

                StuffCommand(i, Steps[i].Actions);
            }

            //formula = HuaUtilitys.CreateType<FormulaBase>("HuaCore.Evaluation.FormulaBase");
            formula = new FormulaBase();
            formula.Report = Report;
            

            startTime = DateTime.Now;

            IsExaming = true;

            // var userData = Context.GetApplicationContext().GetService<UserManager>();
            // userExperimentData.ExperimentFolder = userExperimentData.ExperimentNumber + DateTime.Now.ToFileTime();
            // userExperimentData.Folder =
            //     $"{userData.CurrentUser.studentID}/{userExperimentData.ExperimentFolder}/";
            //
            // storagePath = userData.StoragePath;

            CurrentStepIndex = -1;

            //默认显示第一位
            if (!IsExam)
            {
                SetTipsData(0);
            }
        }

        void StuffCommand(int stepIndex,List<ActionData> actionDatas)
        {
            for (int i = 0; i < actionDatas.Count; i++)
            {
                var actionData = actionDatas[i];
                if(string.IsNullOrEmpty(actionData.ActionCommand())) continue;
                
                if (CommandLibrary.ContainsKey(actionData.ActionCommand()))
                {
                    var list = CommandLibrary[actionData.ActionCommand()];
                    
                    //定义一个二维数组
                    list.Add(new Vector2Int(stepIndex,i));
                }
                else
                {
                    //定义一个二维数组
                    CommandLibrary.Add(actionData.ActionCommand(), new List<Vector2Int> {new Vector2Int(stepIndex,i)});
                }
            }
        }
        
        /// <summary>
        /// 设置提示数据
        /// </summary>
        /// <param name="index"></param>
        void SetTipsData(int index)
        {
            if (index >= Steps.Count) {

                //隐藏UI
                // MessageDispatcher.SendMessage("HideStep");
                return;
            }
            
            StepData stepData = Steps[index];
            
            StartStep(stepData.TagID);//手动

            var actionData = GetStepReport().GetCurrentAction();
            actionData.StepID = stepData.TagID;

            SetTipsData(stepData.DisplayName, actionData);

        }

        void SetTipsData(string stepName,ActionData actionData)
        {
            //
            
            StepTipsData tipsData = new StepTipsData();

            string actionText = string.Empty;

            string[] results = actionData.Description.Split('-');
            
            for (int i = 0; i < results.Length; i++)
            {
                if (i % 2 != 0)
                {
                    //奇数
                    actionText += string.Format("{0}{1}{2}", "[color=#EDAA02]", results[i],
                        "[/color]");
                }
                else
                {
                    actionText += results[i];
                }
            }

            tipsData.stepName = stepName;
            tipsData.actionName = actionText;
            
            // MessageDispatcher.SendMessageData(EventMessage._INIT_EQUIPMENT_TIPS, actionData);
            // MessageDispatcher.SendMessageData(EventMessage._EQUIPMENT_TIPS, actionData);
            //
            // var actionCommandParameter = actionData.ActionCommandParameter();
            // if (!string.IsNullOrEmpty(actionCommandParameter))
            //     MessageDispatcher.SendMessageData(EventMessage._COMMAND_PARMES, actionCommandParameter);
            //
            // MessageDispatcher.SendMessageData("SetStep", tipsData);
        }

        /// <summary>
        /// 获取指定步骤的步骤报告
        /// </summary>
        /// <param name="stepId"></param>
        /// <returns></returns>
        public StepReport GetStepReport(string stepId)
        {
            return userExperimentData.GetStepReport(stepId);
        }

        public StepReport GetStepReport()
        {
            return GetStepReport(CurrentStepID);
        }

        /// <summary>
        /// 结束系统
        /// </summary>
        public void EndSystem()
        {
            var errors = formula.GetErrorValue(Report);

            List<Answer> answers = new List<Answer>();

            for (int i = 0; i < Report.Manuals.Count; i++)
            {
                var item = new Answer();

                item.Fraction = Report.Automatics[i].Fraction;
                item.Value = Report.Manuals[i];
                item.IsError = errors.ContainsKey(i);

                answers.Add(item);
            }

            //结束当前步骤
            EndStep(CurrentStepID, EndStepStatus.End);

            IsExaming = false;
            
            userExperimentData.Report.Answers = answers;
            userExperimentData.IsExam = IsExam;
            
            CurrentStepIndex = 0;
            //结算 
            userExperimentData.Settlement();
            
            // //结束实验，将系统保存到当前用户
            // var userData = Context.GetApplicationContext().GetService<UserManager>();
            //
            // if (userExperimentData.IsExam)
            // {
            //     userData.CurrentUser.ExamDatas.Insert(0, userExperimentData);
            // }
            // else
            // {
            //     userData.CurrentUser.ExerciseDatas.Insert(0, userExperimentData);
            // }
            //
            // currentSettlement = null;
            // userData.WriteText();//保存文件
        }

        /// <summary>
        /// 中途考试时，应将录制数据清空
        /// </summary>
        public void BreakSystem()
        {
            if (!IsExaming) return;
            
            // var userData = Context.GetApplicationContext().GetService<UserManager>();
            // userData.DeletePath(userExperimentData.Folder);//删除此文件

            //结束当前步骤
            EndStep(CurrentStepID, EndStepStatus.Break);

            IsExaming = false;

            currentSettlement = null;
        }

        /// <summary>
        /// 启动步骤
        /// </summary>
        /// <param name="stepID"></param>
        /// <param name="isRecord"></param>
        public void StartStep(string stepID)
        {
            //需要检测此步骤是否已经存在，存在则不让其操作
            if (StepActions.GetSettlement(stepID) != null) return;
            
            int index = FindIndex(stepID);
            if (index == -1) return;

            if (CurrentStepIndex == index) return;

            StepData stepData = Steps[index];

            if (stepData.Actions.Count == 0)
            {
                AddStep(stepID); //直接完成
                return;
            }

            // if (currentSettlement == null)
            // {
            //     currentSettlement = StepActions.AddStepSettlement(stepID, GetStepNumber(stepID));
            //
            //     if (stepData.IsRecord && IsExam)
            //     {
            //         ExperimentMain.StartRecording(storagePath + userExperimentData.Folder,
            //             currentSettlement.replayFile);
            //     }
            //
            //     CurrentStepIndex = index;
            //
            //     stepStartTime = DateTime.Now;
            //
            //     //刷新步骤
            //     MessageDispatcher.SendMessageData("UpdateStep", index);
            // }
            // else
            // {
            //
            //     if (FindIndex(currentSettlement.stepID) > FindIndex(stepID)) return;
            //
            //     EndStep(currentSettlement.stepID);
            //
            //     if (CurrentStepIndex == -1)
            //     {
            //         StartStep(stepID);
            //         return;
            //     }
            //     var tempStep = Steps[CurrentStepIndex];
            //     //获取到当前的数据
            //     if (!IsStepDone(tempStep.TagID)&& tempStep.IsRecord)
            //     {
            //         //添加录屏信息
            //         var actionDatas = userExperimentData.GetStepReport(tempStep.TagID).GetUndoneAchieved();
            //
            //         if (actionDatas.Count > 0)
            //         {
            //             // string description = string.Empty;
            //
            //             for (int i = 0; i < actionDatas.Count; i++)
            //             {
            //                 if (StepActions.ContainsFlag(tempStep.TagID, actionDatas[i].Error))
            //                     continue;
            //
            //                 ReplayFlag flag = new ReplayFlag();
            //                 flag.Position = new HVector3(Input.mousePosition.x, Input.mousePosition.y);
            //                 flag.flagError = FlagErrorType.OperateOmission;
            //                 
            //                 flag.Description = $"{actionDatas[i].Error};";
            //                 flag.RecordTime = 0;
            //                 
            //                 AutoStepAddStepReplay(tempStep.TagID, flag);
            //             }
            //         }
            //     }
            //
            //     StartStep(stepID);
            // }
        }
        
        /// <summary>
        /// 结束步骤（包含录制）
        /// </summary>
        /// <param name="stepID"></param>
        public void EndStep(string stepID,EndStepStatus status = EndStepStatus.None)
        {

            //加入到步骤报告中 todo
            if (currentSettlement == null)
                return;
            if (!currentSettlement.stepID.Equals(stepID)) return;

            //Debug.Log("结束步骤：" + stepID);
            var index = FindIndex(stepID);
            if (index == -1) return;

            StepData stepData = Steps[index];
            //判断动作是否都完成，如果不完成，则设置状态

            var stepReport = userExperimentData.GetStepReport(stepID);
            
            if (stepReport == null)
                throw new Exception(string.Format("步骤:{0}不存在", stepID));

            //在练习模式下，只有当所有动作都完成，才可以进行下一步，如果中断则数据清空
            if (status == EndStepStatus.None && !IsExam && !stepReport.IsDone())
            {
                return;
            }

            if (stepData.IsRecord && IsExam)
                stepReport.StepSettlement = currentSettlement;


            stepReport.ActualTime = (int)(DateTime.Now - stepStartTime).TotalSeconds;

            if (status == EndStepStatus.None && stepReport.IsDone() && !IsExam)
            {
                currentSettlement = null;
                SetTipsData(index + 1);
                return;
            }

            // if (stepData.IsRecord&& IsExam)
            // {
            //     currentSettlement.duration = ExperimentMain.GetRecordingDuration();
            //
            //     ExperimentMain.StopRecording();
            // }
            
            currentSettlement = null;

        }

        /// <summary>
        /// 直接添加步骤信息
        /// </summary>
        /// <param name="stepID"></param>
        private void AddStep(string stepID)
        {

            if (currentSettlement != null)
            {
                if (currentSettlement.stepID.Equals(stepID)) return;

                //停止当前步骤
                EndStep(currentSettlement.stepID);
            }

            var stepReport = userExperimentData.GetStepReport(stepID);
            
            if (stepReport == null) throw new Exception("步骤ID不存在");
            stepReport.StepSettlement = null;
            stepReport.Unlock();
        }

        /// <summary>
        /// 往步骤内添加动作完成度
        /// </summary>
        /// <param name="stepID"></param>
        /// <param name="actionID"></param>
        /// <exception cref="Exception"></exception>
        public void AddStepAction(string stepID, string actionID)
        {
            if (currentSettlement == null)
            {
                //直接启动动作，进行录制
                StartStep(stepID);
            }

            if (currentSettlement == null) return;
            
            if (!currentSettlement.stepID.Equals(stepID))
            {
                //如果当前步骤已经超过，现有步骤，则直接跳过。否则则自动进行
                if (FindIndex(currentSettlement.stepID) > FindIndex(stepID)) return;
                StartStep(stepID);
            }

            var stepReport = userExperimentData.GetStepReport(stepID);
            if (stepReport == null) throw new Exception("步骤ID不存在");
            
            stepReport.ActionUnlock(actionID);

            AutoUpdateStepOrAction(stepID, actionID);

        }
        
        public void AddStepAction(string stepID, string actionID, float progress)
        {
            if (currentSettlement == null)
            {
                StartStep(stepID);
            }
            if (currentSettlement == null) return;
            
            if (!currentSettlement.stepID.Equals(stepID))
            {
                //如果当前步骤已经超过，现有步骤，则直接跳过。否则则自动进行
                if (FindIndex(currentSettlement.stepID) > FindIndex(stepID)) return;
                StartStep(stepID);
            }
            
            var stepReport = userExperimentData.GetStepReport(stepID);
            if (stepReport == null) throw new Exception("步骤ID不存在");
            
            stepReport.StepSettlement = currentSettlement;
            stepReport.AddActionProgress(actionID, progress);
            
            AutoUpdateStepOrAction(stepID, actionID);
        }

        /// <summary>
        /// 自动更新步骤或动作
        /// </summary>
        /// <param name="stepID"></param>
        /// <param name="actionID"></param>
        void AutoUpdateStepOrAction(string stepID, string actionID)
        {
            var stepReport = GetStepReport(stepID);

            bool achieved = stepReport.GetActionDone(actionID,out var tempActionData);

            if (!achieved) return;
            
            //发送地址命令

            tempActionIdentitys.TryGetValue(tempActionData, out var actionDataIdentity);

            if (actionDataIdentity == null) return;
            
            //MessageDispatcher.SendMessageData(EventMessage._ACTION_COMMAND_FINISHING, actionDataIdentity);
            tempActionIdentitys.Remove(tempActionData);

            if (IsExam) {
                if (stepReport.IsExamDone()) {
                    EndStep(stepID);
                }

                return;
            }

            if (stepReport.IsDone())
            {
                EndStep(stepID);
            }
            else
            {
                var actionData = stepReport.GetCurrentAction();

                if (string.IsNullOrEmpty(actionData.TagID))
                {
                    EndStep(stepID);
                    return;
                }

                actionData.StepID = stepID;
                
                if (!actionData.TagID.Equals(actionID))
                {
                    SetTipsData(stepReport.stepData.DisplayName, actionData);
                }
            }

            //先判断这个步骤有没有完成，如果完成，直接结束当前步骤，并且进行结算
        }

        /// <summary>
        /// 往步骤中添加录制节点信息
        /// </summary>
        /// <param name="stepID">步骤标识</param>
        /// <param name="flag">节点数据</param>
        public void AddStepReplay(string stepID, ReplayFlag flag)
        {
            if (currentSettlement == null)return;
            if (!currentSettlement.stepID.Equals(stepID)) return;

            AutoStepAddStepReplay(stepID, flag);
        }

        public void AddStepReplay(ReplayFlag flag)
        {
            if (currentSettlement == null) return;
            if (string.IsNullOrEmpty(flag.Description)) return;

            GetReplayPosition(ref flag);

            StepActions.AddReplayFlag(currentSettlement.stepID, GetStepNumber(currentSettlement.stepID), flag);
        }

        public void AutoStepAddStepReplay(string stepID, ReplayFlag flag)
        {
            GetReplayPosition(ref flag);

            StepActions.AddReplayFlag(stepID,GetStepNumber(stepID), flag);
        }

        public void AutoStepAddStepReplay(string stepID,int actionId, ReplayFlag flag)
        {
            GetReplayPosition(ref flag);

            //如果描述不清楚，则采用配置上的说明
            if (string.IsNullOrEmpty(flag.Description))
            {
                var stepReport = userExperimentData.GetStepReport(stepID);
                if (stepReport == null) throw new Exception("步骤ID不存在");

                var action = stepReport.stepData.Actions[actionId];
                flag.Description = action.Error;
            }

            StepActions.AddReplayFlag(stepID, GetStepNumber(stepID), flag);
        }

        void GetReplayPosition(ref ReplayFlag flag)
        {
            //flag.RecordTime = flag.RecordTime == 0 ? ExperimentMain.GetRecordingDuration() : flag.RecordTime;
            flag.width = Screen.width;
            flag.height = Screen.height;

            if (flag.Transform != null)
            {
                var boxCollider = flag.Transform.GetComponent<BoxCollider>();

                Vector3 position = boxCollider == null ? flag.Transform.position : ExperimentUtilitys.GetBoxColliderCenter(boxCollider)[0];
                position.y += flag.VerticalPositionOffset;
                flag.Position = new EVector3(Camera.main.WorldToScreenPoint(position));
            }
        }

        /// <summary>
        /// 根据命令添加录制信息
        /// </summary>
        /// <param name="command"></param>
        /// <param name="flag"></param>
        public void AddCommandStepReplay(string command, ReplayFlag flag)
        {
            var lists = FindActionList(command);

            foreach (var item in lists)
            {
                if (item.x.Equals(CurrentStepIndex))
                {
                    AutoStepAddStepReplay(Steps[item.x].TagID, item.y, flag);
                }
            }
            
        }

        /// <summary>
        /// 判断该步骤是否完成
        /// </summary>
        /// <param name="stepID"></param>
        /// <returns></returns>
        public bool IsStepDone(string stepID)
        {
            var step = userExperimentData.GetStepReport(stepID);
            if (step == null) throw new Exception("步骤ID不存在");

            return step.IsDone();
        }

        public Dictionary<int,int> FindAction(string command)
        {
            
            Dictionary<int, int> dics = new Dictionary<int, int>();
            //从集合中寻找

            CommandLibrary.TryGetValue(command, out var actions);

            if (actions == null) return dics;
            
            for (int i = 0; i < actions.Count; i++)
            {
                if(dics.ContainsKey(actions[i].y)) continue;
                dics.Add(actions[i].y, actions[i].x);
            }

            return dics;
        }

        public List<Vector2Int> FindActionList(string command)
        {
            CommandLibrary.TryGetValue(command, out var actions);

            return actions ?? new List<Vector2Int>();
        }

        /// <summary>
        /// 获取当前动作命令
        /// </summary>
        /// <returns></returns>
        public string GetCurrentActionCommand()
        {
            if (currentSettlement == null) return "";

            return GetStepReport().GetCurrentAction().ActionCommand();
        }

        /// <summary>
        /// 添加动作身份,用于跟踪是哪一个器材进行的
        /// </summary>
        /// <param name="actionData"></param>
        /// <param name="identities"></param>
        void AddActionDataIdentity(ActionData actionData, ObjectIdentity[] identities)
        {
            var actionDataIdentity = new ActionDataIdentity(actionData, identities);

            if (tempActionIdentitys.ContainsKey(actionData))
            {
                tempActionIdentitys[actionData] = actionDataIdentity;
            }
            else
            {
                tempActionIdentitys.Add(actionData, actionDataIdentity);
            }
        }

        /// <summary>
        /// 操作命令
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public bool OperateCommand(string command,params ObjectIdentity[] identities)
        {
            //根据情况，传输命令
            //解析命令，匹配处理

            //如果是练习模式，则判断传输过来的命令所属当前动作，不属于则返回false.
            //处于则完成该动作

            //如果是考试模式，则判断传输过来的命令，是否属于当前开启的步骤，不属于则关闭退出，但是仍然返回true。

            //如果没有开启步骤，说明没有触发，但是如果该步骤属于关联步骤，则不进行处理，直接跳过。
            //如果没有开启步骤，传输过来的是非关键动作，那么将信息记录到临时动作集中。
            //如果没有开启步骤，传输过来的是关键动作，那么将信息转变为当前步骤，如果当前录制信息已经启动，则把录制信息转化为其他该信息。
            //如果没有开启步骤，传输过来的是录制动作，但是非关键动作，则开启录制，但是作为临时数据存储，当关键帧到来时，将录制信息传递给当前关键动作所属步骤

            //但是如果当前录制动作，不是当前步骤，那么直接舍弃掉，从而来开启新的步骤。
            //需要提供一个方法，用来更改当前动作的信息，但是注意，如果是非当前步骤，则无效。
            //仍然可以添加录制错误信息，但是不建议这么做了，只需要让他不传输命令即可.
            //需要添加一个步骤进度信息
            
            if (!IsExam)
            {
                if (currentSettlement == null) return false;

                var stepReport = GetStepReport();

                //练习模式,获取到当前步骤的动作
                var actionData = stepReport.GetCurrentAction();
                
                var ids = command.Split('|');

                var actionIds = actionData.ActionCommand().Split('|');

                var results = ids.Intersect(actionIds);

                var result = results.Count() == actionIds.Length;

                if (result)
                {

                    //能操作，但是动作不完成
                    if (!LabelEquals(actionData.RelationLabels, identities)) return true;
                    
                    //添加动作标识
                    AddActionDataIdentity(actionData, identities);
                    
                    //动作完成
                    if (actionData.IsProgress)
                    {
                        AddStepAction(stepReport.stepData.TagID,actionData.TagID, 1);
                    }
                    else
                    {
                        AddStepAction(stepReport.stepData.TagID, actionData.TagID);
                    }
                }

                return true;
            }

            //注：考试情况下，不管做什么事情，都是返回true,只是在于他的情况是否带动了步骤的开启
            //如果动作顺序不对，仍然让其完成，但是记录顺序错误。并且根据描述文字找到仪器，添加错误信息。

            //没有开启步骤
            if (currentSettlement == null)
            {
                //先找到所属动作，需要一个优先级。

                var dicActions = FindActionList(command);

                if (dicActions.Count == 0) return true;

                //遍历动作
                foreach (var item in dicActions)
                {

                    //如果小于上一个步骤的索引，则直接跳过
                    if (item.x<CurrentStepIndex) continue;

                    var stepReport = userExperimentData.GetStepReport(item.x);

                    if (stepReport.GetActionDone(item.y)) continue;   
                    
                    var actionData = stepReport.GetAction(item.y);

                    if (TempActionContains(actionData)) continue;

                    //判断关联动作有没有完成

                    if (!string.IsNullOrEmpty(actionData.RelationActions))
                    {
                        bool result = stepReport.GetActionDone(actionData.RelationActions.Split(','));
                        if (!result) return true;
                    }
                    
                    //能操作，但是动作不完成
                    if (!LabelEquals(actionData.RelationLabels, identities)) continue;
                    
                    //如果是关键动作，则进行处理
                    if (actionData.KeyAction)
                    {
                        //开启步骤，并且开启录屏
                        StartStep(stepReport.stepData.TagID);

                        AddTempAction(actionData, item.x, identities);

                        ExecuteTempAction(item.x, stepReport);
                        
                    }
                    else
                    {
                        AddTempAction(actionData, item.x, identities);
                    }
                    
                    break;
                }
            }
            else
            {
                //如果开启步骤，判断是不是当前步骤
                var actions = FindActionList(command);
                
                if (actions.Count == 0) return true;

                foreach (var item in actions)
                {

                    if(CurrentStepIndex>item.x) continue;
                    
                    var stepReport = userExperimentData.GetStepReport(item.x);
                    
                    if (stepReport.GetActionDone(item.y)) continue;   
                    
                    //判断关联动作有没有完成，没有完成，则直接跳过
                    var actionData = stepReport.GetAction(item.y);

                    if (TempActionContains(actionData)) continue;

                    //能操作，但是动作不完成
                    if (!LabelEquals(actionData.RelationLabels, identities)) continue;
                    
                    if (CurrentStepIndex < item.x)
                    {
                        
                        //关键动作才进行处理
                        if (!actionData.KeyAction) continue;

                        if (!string.IsNullOrEmpty( actionData.RelationActions))
                        {
                            bool result = stepReport.GetActionDone(actionData.RelationActions.Split(','));
                            if (!result) return true;
                        }
                        
                        //开启新的步骤
                        StartStep(stepReport.stepData.TagID);

                        AddTempAction(actionData, item.x, identities);

                        ExecuteTempAction(item.x, stepReport);

                    }
                    else
                    {

                        if (!string.IsNullOrEmpty(actionData.RelationActions))
                        {
                            bool result = stepReport.GetActionDone(actionData.RelationActions.Split(','));
                            if (!result) return true;
                        }

                        AddOrderError(actionData, stepReport);

                        AddTempAction(actionData, item.x, identities);

                        ExecuteTempAction(item.x, stepReport);
                        
                        break;
                    }
                }
            }
            
            return true;
        }

        void AddOrderError(ActionData actionData,StepReport stepReport)
        {
            if (!actionData.KeyAction)
            {
                return;
            }

            //如果是关键动作，需要查看上一个动作是否完成，如果没完成，则添加顺序错误问题
            var result = stepReport.PreviousActionDone(actionData, out var previousAction);

            // if (!result)
            // {
            //
            //     //添加错误信息，顺序不对
            //     var equipments = ExperimentMain.GetEquipments(actionData);
            //
            //     string equipmentStr = string.Empty;
            //
            //     for (int i = 0; i < equipments.Count; i++)
            //     {
            //         equipmentStr += equipments[i].CommonName;
            //     }
            //
            //     if (equipments.Count > 0)
            //     {
            //         ReplayFlag flag = new ReplayFlag
            //         {
            //             Transform = equipments[0].transform,
            //             flagError = FlagErrorType.OrderError,
            //             Description = $"应先操作【{previousAction.TagID}】,再操作【{actionData.TagID}】;",
            //             RecordTime = ExperimentMain.GetRecordingDuration()
            //         };
            //         AutoStepAddStepReplay(stepReport.stepData.TagID, flag);
            //     }
            //
            //     Debug.Log($"顺序错误：{actionData.TagID}--{equipmentStr}");
            //
            // }
        }

        void AddTempAction(ActionData actionData, int stepId, ObjectIdentity[] identities)
        {
            if (TempActionContains(actionData)) return;
            AddActionDataIdentity(actionData, identities);
            tempActionDatas.Add(actionData, stepId);
        }

        bool TempActionContains(ActionData actionData)
        {
            return tempActionDatas.ContainsKey(actionData);
        }

        void ExecuteTempAction(int stepId,StepReport stepReport)
        {
            Dictionary<ActionData,int> actionDatas = new Dictionary<ActionData, int>();

            foreach (var tempActionData in tempActionDatas)
            {
                if(!tempActionData.Value.Equals(stepId)) continue;

                int index = stepReport.FindIndex(tempActionData.Key.TagID);

                if (index == -1) continue;
                
                actionDatas.Add(tempActionData.Key, index);
            }

            if (actionDatas.Count > 1)
            {
                //进行升序排
                var results = from actionData in actionDatas 
                    orderby actionData.Value descending select actionData;
            
                foreach (var tempAction in results)
                {
                    ExecuteAction(tempAction.Key, stepReport.stepData.TagID);

                    //Debug.Log($"步骤{stepReport.stepData.TagID}：动作：{tempAction.Key.TagID}");

                }
            }
            else
            {
                foreach (var actionData in actionDatas)
                {
                    ExecuteAction(actionData.Key,stepReport.stepData.TagID);
                    
                    //Debug.Log($"步骤{stepReport.stepData.TagID}：动作：{actionData.Key.TagID}");
                }
            }

            tempActionDatas.Clear();
        }

        void ExecuteAction(ActionData actionData,string stepId)
        {
            //Debug.Log($"步骤：{stepId} 动作：{actionData.Description}");

            if (actionData.IsProgress)
            {
                AddStepAction(stepId, actionData.TagID, 1);
            }
            else
            {
                AddStepAction(stepId, actionData.TagID);
            }
        }

        bool LabelEquals(List<LabelData> labelDatas, ObjectIdentity[] identities)
        {
            if (identities.Length == 0) return true;

            for (int i = 0; i < identities.Length; i++)
            {
                if (LabelContains(labelDatas, identities[i].Name, identities[i].Label))
                {
                    continue;
                }

                return false;
            }
            
            return true;
        }

        /// <summary>
        /// 标签判断
        /// </summary>
        /// <param name="labelDatas"></param>
        /// <param name="labelData"></param>
        /// <returns></returns>
        bool LabelContains(List<LabelData> labelDatas, string Name,string Label)
        {

            if (labelDatas == null || labelDatas.Count == 0)
            {
                return string.IsNullOrEmpty(Label);
            }

            for (var index = 0; index < labelDatas.Count; index++)
            {
                var data = labelDatas[index];
                
                if (!data.Name.Equals(Name)) continue;
                
                if (string.IsNullOrEmpty(data.Label) && string.IsNullOrEmpty(Label)) return true;

                if (data.Label != null && !data.Label.Equals(Label)) return false;
            }

            return true;
        }
    }
}