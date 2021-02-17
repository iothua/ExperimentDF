using System;
using System.Collections.Generic;
using Loxodon.Framework.Contexts;
using UnityEngine;

namespace ExperimentFramework.Evaluation
{
    
    /// <summary>
    /// 用户实验数据
    /// </summary>
    [Serializable]
    public class UserExperimentData
    {

        /// <summary>
        /// 实验编号
        /// </summary>
        public string ExperimentNumber;

        /// <summary>
        /// 实验名称
        /// </summary>
        public string ExperimentName;

        /// <summary>
        /// 总分数
        /// </summary>
        public float Fraction => StepFraction + Report.GetFraction();

        /// <summary>
        /// 文件夹
        /// </summary>
        public string Folder;

        /// <summary>
        /// 实验文件夹
        /// </summary>
        public string ExperimentFolder;
        
        /// <summary>
        /// 步骤分数
        /// </summary>
        public float StepFraction => GetCurrentScore();

        /// <summary>
        /// 时长(秒)
        /// </summary>
        public int Duration;

        // /// <summary>
        // /// 系统推荐时间
        // /// </summary>
        // public int RecomTime;

        /// <summary>
        /// 级别
        /// </summary>
        public int Level;
        
        /// <summary>
        /// 是考试还是练习
        /// </summary>
        public bool IsExam;
        
        /// <summary>
        /// 步骤报告
        /// </summary>
        public List<StepReport> StepReports = new List<StepReport>();

        /// <summary>
        /// 报告数据
        /// </summary>
        public ReportData Report = new ReportData();

        public StepReport GetStepReport(string stepId)
        {
            var index = FindIndex(stepId);

            if (index == -1) return null;
            
            return StepReports[index];
        }

        public StepReport GetStepReport(int index)
        {
            if (index == -1) return null;

            return StepReports[index];
        }

        public int FindIndex(string stepId)
        {
            return StepReports.FindIndex(obj => obj.stepData.TagID.Equals(stepId));
        }
        
        /// <summary>
        /// 获取到分数
        /// </summary>
        /// <returns></returns>
        public float GetCurrentScore()
        {
            float score = 0;

            foreach (var stepReport in StepReports)
            {
                score += stepReport.GetFraction();
            }

            return score;
        }

        public void Settlement()
        {
            int level1 = 0;
            int level2 = 0;
            int level3 = 0;

            Duration = 0;
            
            foreach (var stepReport in StepReports)
            {
                //RecomTime += stepReport.stepData.FinishTime;
                
                stepReport.Settlement();

                Duration += stepReport.ActualTime;
                
                switch (stepReport.Level)
                {
                    case 1:
                        level1++;
                        break;
                    case 2:
                        level2++;
                        break;
                    case 0:
                    case 3:
                        level3++;
                        break;
                }
            }

            if (level3 > 0)
            {
                Level = 3;
            }
            else if (level2 > 0)
            {
                Level = 2;
            }
            else
            {
                Level = 1;
            }

            //UploadData();
        }
    
        public bool IsReportNoInit()
        {
            for (int i = 0; i < StepReports.Count; i++)
            {
                if (string.IsNullOrEmpty(StepReports[i].stepData.TagID))
                    return true;
            }

            return false;
        }

        /*
         *
         
        
        void UploadData()
        {
            if (HuaUtilitys.Client == ClientType.Standalone) return;
            if (!HuaUtilitys.IsConnectNetwork) return;
            
            var httpRequest = Context.GetApplicationContext().GetService<HttpRequestManager>();
            if (httpRequest == null) return;
            
            var userManager = Context.GetApplicationContext().GetService<ExamSystem.Users.UserManager>();
            if (userManager == null) return;

            var recordManager = Context.GetApplicationContext().GetService<ExperimentFileRecordManager>();
            
            if (IsExam)
            {
                ExamData examData = new ExamData();
                examData.number = ExperimentNumber;
                examData.costTime = Duration;
                examData.reportScore = Report.Fraction.ToString();
                examData.stepList = new List<ExamStepData>();

                examData.reportList = new List<ExamReportData>();

                for (int i = 0; i < Report.Answers.Count; i++)
                {
                    var reportAnswers = Report.Answers[i];

                    var reportData = new ExamReportData()
                    {
                        answer = reportAnswers.Value,
                        status = reportAnswers.IsError ? 0 : 1,
                        score = reportAnswers.Fraction.ToString()
                    };

                    examData.reportList.Add(reportData);
                }

                for (int i = 0; i < StepReports.Count; i++)
                {
                    var stepReport = StepReports[i];

                    
                    ExamStepData stepData = new ExamStepData();
                    stepData.sortNo = i;
                    stepData.score = stepReport.GetFraction().ToString();
                    stepData.status = stepReport.Achieved ? 1 : 0;
                    stepData.isVideo = stepReport.StepSettlement != null ? 1 : 0;

                    stepData.screenX = Screen.width;
                    stepData.screenY = Screen.height;
                    stepData.errorReason = stepReport.ErrorSummarize;
                    stepData.comment = stepReport.Reviews;
                    
                    examData.stepList.Add(stepData);

                    if (stepReport.StepSettlement == null ) continue;

                    stepData.clientFileId = ExperimentFolder + i;
                    
                    ExperimentFileRecord fileRecord = new ExperimentFileRecord();

                    fileRecord.Guid = stepData.clientFileId;
                    fileRecord.FilePath =  userManager.StoragePath + Folder + stepReport.StepSettlement.replayFile;

                    recordManager.AddRecord(fileRecord);

                    if (stepReport.StepSettlement.FlagCount==0) continue;

                    stepData.actionList = new List<ExamErrorData>();
                    for (int j = 0; j < stepReport.StepSettlement.StepDatas.Count; j++)
                    {
                        var flag = stepReport.StepSettlement.StepDatas[j]; 
                        
                        ExamErrorData errorData = new ExamErrorData();
                        errorData.type = (int) flag.flagError;
                        errorData.description = flag.Description;
                        errorData.timeNode = (int) flag.RecordTime;
                        errorData.positionX = (int) flag.Position.X;
                        errorData.positionY = (int) flag.Position.Y;

                        stepData.actionList.Add(errorData);
                    }
                }

                httpRequest.PostExamStep(examData, null);
            }
            else
            {
                ExerciseData exerciseData = new ExerciseData();
                exerciseData.level = Level;
                exerciseData.number = ExperimentNumber;
                exerciseData.stepList = new List<ExerciseStepData>();
                for (int i = 0; i < StepReports.Count; i++)
                {
                    var stepReport = StepReports[i];
                    exerciseData.stepList.Add(new ExerciseStepData()
                    {
                        sortNo = i,
                        costTime = stepReport.ActualTime,
                        level = stepReport.Level
                    });
                }

                httpRequest.PostExerciseStep(exerciseData);
            }
        }
         */
    }
}