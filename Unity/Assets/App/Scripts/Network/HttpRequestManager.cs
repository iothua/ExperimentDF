using System.Collections;
using System.Text;
using Loxodon.Framework.Asynchronous;
using UnityEngine;
using UnityEngine.Networking;
using System;
using ExperimentFramework;

namespace App.Network
{
    public class HttpRequestManager
    {
        public HttpConfig Config;

        /// <summary>
        /// 身份码
        /// </summary>
        public string token;
        /// <summary>
        /// 公钥
        /// </summary>
        public string publicKey;

        public HttpRequestManager()
        {
            LoadHttpConfig();
        }

        async void LoadHttpConfig()
        {
            await AddressableManager.LoadSourceCoroutine<TextAsset>("DynamicAssets/Configs/HttpConfig.json", result =>
            {
                if (result == null) return;
                Config = JsonHelper.JsonToStruct<HttpConfig>(result.text);

                AddressableManager.AddressableRelease(result);
            });
        }

        public async void PostLogin(LoginData loginData, Action<HttpMsgCallback> onComplate)
        {
            string jsonData = JsonHelper.ObjectToJsonString(loginData);
            string url = Config.domain + Config.client + Config.login;
            await DoPost(url, jsonData, false, onComplate);
        }

        public async void GetPublicKey(Action onComplate)
        {
            string url = Config.domain + Config.client + Config.publicKey;

            await DoGet(url, result =>
            {
                if (result.success)
                {
                    publicKey = (string)result.data;

                    Debug.Log("publicKey:" + publicKey);
                    onComplate?.Invoke();
                }
                else
                {
                    LogManager.Debug(result.errorMessage);
                    //throw new Exception(result.msg);
                    onComplate?.Invoke();
                }
            });
        }

        public async void UploadExperimentAll(ExperimentDataInfos dataInfos)
        { 
            string url = Config.domain + Config.client + Config.UploadAllExperiments;
            var jsonData = JsonHelper.ObjectToJsonString(dataInfos);

            await DoPost(url, jsonData, true, null);
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        public async void GetUserInfo(string deviceId, Action<HttpMsgCallback> onCallback)
        {
            string url = Config.domain + Config.client + Config.userInfo;

            var device = new DeviceInfo()
            {
                deviceId = deviceId
            };

            var jsonData = JsonHelper.ObjectToJsonString(device);

            await DoPost(url, jsonData, true, onCallback);
        }

        /// <summary>
        /// 考试数据
        /// </summary>
        public async void PostExamStep(ExamData examData, Action<HttpMsgCallback> onCallback)
        {
            string url = Config.domain + Config.client + Config.UploadExamResults;
            var jsonData = JsonHelper.ObjectToJsonString(examData);

            await DoPost(url, jsonData, true, onCallback);
        }

        /// <summary>
        /// 练习数据
        /// </summary>
        public async void PostExerciseStep(ExerciseData exerciseData)
        {
            string jsonData = JsonHelper.ObjectToJsonString(exerciseData);
            string url = Config.domain + Config.client + Config.UploadExerciseResults;

            await DoPost(url, jsonData);
        }

        IEnumerator DoGet(string url, Action<HttpMsgCallback> onCallback = null)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                LogManager.Debug("网络异常" + webRequest.error);
                onCallback?.Invoke(new HttpMsgCallback()
                {
                    code = 500,
                    success = false,
                    msg = "网络异常，请检查网络...",
                    errorMessage = "网络异常...",
                });
                yield break;
            }

            if (webRequest.isDone)
            {
                var callback = JsonHelper.JsonToObject<HttpMsgCallback>(webRequest.downloadHandler.text);
                onCallback?.Invoke(callback);
            }
        }

        IEnumerator DoPost(string url, string postData, bool isToken = true, System.Action<HttpMsgCallback> onCallback = null)
        {
            UnityWebRequest webRequest = new UnityWebRequest(url, "POST");
            byte[] body = Encoding.UTF8.GetBytes(postData);

            webRequest.uploadHandler = new UploadHandlerRaw(body);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            //webRequest = UnityWebRequest.Post(url, postData);

            webRequest.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
            if (isToken)
            {
                if (string.IsNullOrEmpty(token))
                {
                    token = string.Empty;
                }
                webRequest.SetRequestHeader("token", token);
            }

            webRequest.SetRequestHeader("client", Config.client);

            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                LogManager.Debug("网络异常" + webRequest.error);
                onCallback?.Invoke(new HttpMsgCallback()
                {
                    code = 500,
                    success = false,
                    msg = "网络异常，请检查网络...",
                    errorMessage = "网络异常...",
                });
                yield break;
            }

            if (webRequest.isDone)
            {
                var callback = JsonHelper.JsonToObject<HttpMsgCallback>(webRequest.downloadHandler.text);
                onCallback?.Invoke(callback);
            }
        }

        public async void ExperimentFileUpload( ExperimentFileData fileData,Action<HttpMsgCallback> onCallback = null)
        {
            string url = Config.domain + Config.client + Config.UploadExamFile;

            await DoUploadPost(url, fileData, onCallback);
        }

        public IEnumerator DoExperimentFileUpload(ExperimentFileData fileData, Action<HttpMsgCallback> onCallback = null)
        {
            string url = Config.domain + Config.client + Config.UploadExamFile;

            yield return DoUploadPost(url, fileData, onCallback);
        }

        public IEnumerator DoUploadPost(string url, ExperimentFileData fileData,Action<HttpMsgCallback> onCallback = null)
        {
            WWWForm form = new WWWForm();

            form.AddBinaryData("file", fileData.file);

            UnityWebRequest webRequest = UnityWebRequest.Post(url, form);

            webRequest.SetRequestHeader("token", token);
            webRequest.SetRequestHeader("client", Config.client);
            webRequest.SetRequestHeader("type", fileData.type.ToString());
            webRequest.SetRequestHeader("resume", fileData.resume.ToString());
            webRequest.SetRequestHeader("fileName", fileData.fileName);
            webRequest.SetRequestHeader("clientFileId", fileData.clientFileId);

            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                LogManager.Debug("网络异常" + webRequest.error);
                onCallback?.Invoke(new HttpMsgCallback());
                yield break;
            }

            if (webRequest.isDone)
            {
                var callback = JsonHelper.JsonToObject<HttpMsgCallback>(webRequest.downloadHandler.text);
                onCallback?.Invoke(callback);

                Debug.Log(webRequest.downloadHandler.text);
            }
        }
    }
}