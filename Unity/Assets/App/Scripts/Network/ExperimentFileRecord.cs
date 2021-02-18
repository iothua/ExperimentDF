using System;
using System.Collections.Generic;
using System.IO;
using Loxodon.Framework.Contexts;
using UnityEngine;
using System.Collections;
using Loxodon.Framework.Asynchronous;

namespace App.Network
{
    /// <summary>
    /// 实验文件记录
    /// </summary>
    [Serializable]
    public class ExperimentFileRecord
    {
        /// <summary>
        /// 文件唯一ID
        /// </summary>
        public string Guid;
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath;
        /// <summary>
        /// 上传进度
        /// </summary>
        public float Progress;

        /// <summary>
        /// 0:未开始
        /// 1：上传中
        /// 2：完成
        /// </summary>
        public int status;

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            var record = (ExperimentFileRecord) obj;
            return FilePath.Equals(record.FilePath);
        }
    }
    
    [Serializable]
    public class ExperimentFileData
    {
        public byte[] file;
        /// <summary>
        /// 业务类型
        /// </summary>
        public int type;
        /// <summary>
        /// 是否支持续传
        /// </summary>
        public int resume;

        /// <summary>
        /// 文件名
        /// </summary>
        public string fileName;
        
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string clientFileId;

    }

    public class ExperimentFileRecordManager : IDisposable
    {
        private List<ExperimentFileRecord> fileRecords = new List<ExperimentFileRecord>();

        private string fullPath;
        
        private HttpRequestManager httpReques;

        private bool startUpload;
        
        public ExperimentFileRecordManager()
        {
            fullPath = Application.streamingAssetsPath + "/fileRecord.json";
            httpReques = Context.GetApplicationContext().GetService<HttpRequestManager>();

            LoadRecord();
        }

        //加载文件
        public void LoadRecord()
        {
            var jsonData = JsonHelper.ReadJsonString(fullPath);
            if (string.IsNullOrEmpty(jsonData)) return;

            fileRecords = JsonHelper.JsonToList<ExperimentFileRecord>(jsonData);
        }

        void SaveRecord()
        {
            if (fileRecords.Count == 0) return;

            var jsonData = JsonHelper.ObjectToJsonString(fileRecords);
            JsonHelper.SaveJson(jsonData, fullPath);
        }

        public void AddRecord(ExperimentFileRecord fileRecord)
        {
            fileRecords.Insert(0, fileRecord);

            Debug.Log("添加视频：" + fileRecord.FilePath + "   视频ID：" + fileRecord.Guid);
            
            StartUpload();
        }

        /// <summary>
        /// 开始上传
        /// </summary>
        void StartUpload()
        {
            if (startUpload || fileRecords.Count == 0) return;

            startUpload = true;

            RunUpload();
        }

        async void RunUpload()
        {
            if (fileRecords.Count == 0 || !startUpload) return;

            await OnUpload();
        }

        IEnumerator OnUpload()
        {
            while (fileRecords.Count > 0 && startUpload)
            {
                var fileRecord = fileRecords[0];

                var fileData = new ExperimentFileData();
                fileData.file = File.ReadAllBytes(fileRecord.FilePath);
                fileData.fileName = Path.GetFileName(fileRecord.FilePath);

                fileData.clientFileId = fileRecord.Guid;
                fileData.type = 1;
                fileData.resume = 0;

                bool success = false;

                yield return httpReques.DoExperimentFileUpload(fileData, result =>
                {
                    success = result.success;

                    Debug.LogFormat("{0}状态，{1}", fileRecord.FilePath, result.success);
                });

                // if (!HuaUtilitys.IsConnectNetwork)
                // {
                //     EndUpload();
                // }

                if (success)
                {
                    fileRecords.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// 结束上传
        /// </summary>
        void EndUpload()
        {
            startUpload = false;
        }

        public void Dispose()
        {
            EndUpload();
            //保存文件
            SaveRecord();
        }
    }
}