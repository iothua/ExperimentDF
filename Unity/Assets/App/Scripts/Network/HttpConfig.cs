using System;

namespace App.Network
{
    [Serializable]
    public struct HttpConfig
    {
        /// <summary>
        /// 域名
        /// </summary>
        public string domain;

        public string client;
        
        /// <summary>
        /// 登陆
        /// </summary>
        public string login;

        public string publicKey;

        /// <summary>
        /// 用户信息
        /// </summary>
        public string userInfo;

        /// <summary>
        /// 实验列表
        /// </summary>
        public string ExperimentList;
        /// <summary>
        /// 实验步骤列表
        /// </summary>
        public string ExperimentStepList;
        /// <summary>
        /// 上传所有实验
        /// </summary>
        public string UploadAllExperiments;

        /// <summary>
        /// 练习列表
        /// </summary>
        public string ExerciseList;
        /// <summary>
        /// 练习步骤列表
        /// </summary>
        public string ExerciseStepList;
        
        /// <summary>
        /// 上传练习结果
        /// </summary>
        public string UploadExerciseResults;

        /// <summary>
        /// 考试列表
        /// </summary>
        public string ExamList;
        /// <summary>
        /// 考试步骤列表
        /// </summary>
        public string ExamStepList;
        
        /// <summary>
        /// 上传考试结果
        /// </summary>
        public string UploadExamResults;

        /// <summary>
        /// 上传视频文件
        /// </summary>
        public string UploadExamFile;
    }
}

