using App.Network;
using Sirenix.OdinInspector;
using UnityEngine;

namespace App
{
    public class HttpConfigWindow
    {
        [ReadOnly]
        public string filePath = "/DynamicAssets/Configs/HttpConfig.json";
        
        /// <summary>
        /// 域名
        /// </summary>
        [LabelText("域名")]
        public string domain;

        [LabelText("客户端")]
        public string client;
        
        /// <summary>
        /// 登陆
        /// </summary>
        [LabelText("【登陆】URL")]
        public string login;

        [LabelText("【公钥】URL")]
        public string publicKey;

        [LabelText("【用户信息】URL")]
        public string userInfo;

        /// <summary>
        /// 实验列表
        /// </summary>
        [Space(10)]
        [LabelText("【实验列表】URL")]
        public string ExperimentList;
        /// <summary>
        /// 实验步骤列表
        /// </summary>
        [LabelText("【实验步骤列表】URL")]
        public string ExperimentStepList;
        
        /// <summary>
        /// 上传所有实验
        /// </summary>
        [LabelText("【上传所有实验】URL")]
        public string UploadAllExperiments;
        
        /// <summary>
        /// 考试列表
        /// </summary>
        [Space(10)]
        [LabelText("【考试列表】URL")]
        public string ExamList;
        /// <summary>
        /// 考试步骤列表
        /// </summary>
        [LabelText("【考试步骤列表】URL")]
        public string ExamStepList;
        /// <summary>
        /// 上传考试结果
        /// </summary>
        [LabelText("【上传考试结果】URL")]
        public string UploadExamResults;

        /// <summary>
        /// 练习列表
        /// </summary>
        [Space(10)]
        [LabelText("【练习列表】URL")]
        public string ExerciseList;
        /// <summary>
        /// 练习步骤列表
        /// </summary>
        [LabelText("【练习步骤列表】URL")]
        public string ExerciseStepList;
        /// <summary>
        /// 上传练习结果
        /// </summary>
        [LabelText("【上传练习结果】URL")]
        public string UploadExerciseResults;

        [LabelText("【上传考试文件】URL")]
        public string UploadExamFile;
        
        public void LoadData()
        {
            string fullPath = Application.dataPath + filePath;

            if (!System.IO.File.Exists(fullPath)) return;
            
            string jsonData = JsonHelper.ReadJsonString(fullPath);

            var httpConfig = JsonHelper.JsonToStruct<HttpConfig>(jsonData);
            domain = httpConfig.domain;
            login = httpConfig.login;
            client = httpConfig.client;
            publicKey = httpConfig.publicKey;
            userInfo = httpConfig.userInfo;

            ExperimentList = httpConfig.ExperimentList;
            ExperimentStepList = httpConfig.ExperimentStepList;
            UploadAllExperiments = httpConfig.UploadAllExperiments;

            ExerciseList = httpConfig.ExerciseList;
            ExerciseStepList = httpConfig.ExerciseStepList;
            UploadExerciseResults = httpConfig.UploadExerciseResults;

            ExamList = httpConfig.ExamList;
            ExamStepList = httpConfig.ExamStepList;
            UploadExamResults = httpConfig.UploadExamResults;

            UploadExamFile = httpConfig.UploadExamFile;
        }
        
        [HorizontalGroup]
        [Button(ButtonSizes.Large,Name = "保存更改")]
        public void SaveData()
        {
            var httpConfig = new HttpConfig()
            {
                domain = domain,
                login = login,
                client = client,
                publicKey = publicKey,
                userInfo=userInfo,

                ExperimentList = ExperimentList,
                ExperimentStepList = ExperimentStepList,
                UploadAllExperiments = UploadAllExperiments,

                ExerciseList = ExerciseList,
                ExerciseStepList = ExerciseStepList,
                UploadExerciseResults = UploadExerciseResults,

                ExamList = ExamList,
                ExamStepList = ExamStepList,
                UploadExamResults = UploadExamResults,
                UploadExamFile = UploadExamFile
            };
            
            string jsonData = JsonHelper.ObjectToJsonString(httpConfig);
            JsonHelper.SaveJson(jsonData, Application.dataPath + filePath);
        }
    }
}