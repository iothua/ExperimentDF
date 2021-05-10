using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Execution;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RecordSystem : IDisposable
{

    private static RecordSystem recordSystem;

    public static RecordSystem Self
    {
        get
        {
            if (recordSystem == null)
            {
                recordSystem = new RecordSystem();
            }
            return recordSystem;
        }
    }

    private RecordSystem()
    {
    }

    public void Dispose()
    {
        Terminate();
        recordDatas = null;
        savePath = null;
        recordSystem = null;
    }

    private struct RecordData
    {
        public int time;
        public string guid;
        public string command;
    }
    string savePath = Application.streamingAssetsPath + "/";
    bool recording = false;
    List<RecordData> recordDatas;
    int startTime = 0;

    int RecordTime => (int)(Time.time * 1000) - startTime;
    public void StartRecord()
    {
        recordDatas = new List<RecordData>();
        startTime = (int)(Time.time * 1000);
        recording = true;
    }
    public void RecordCommand(string guid ,string cmd)
    {
        if(recordDatas!= null && recording)
        recordDatas.Add(new RecordData
        {
            time = RecordTime,
            guid = guid,
            command = cmd
        }
        );
    }
    public void EndRecord(string name = null)
    {
        if (recording == false) return;
        if (name != null)
        {
            recordDatas.Add(new RecordData
            {
                time = RecordTime,
                guid = "0",
                command = "End"
            }
            );
            recordDatas = Compress(recordDatas);
            JsonHelper.SaveJson(JsonHelper.ObjectToJsonString(recordDatas), savePath + name + ".json");
        }
        recordDatas = null;
        recording = false;
    }

    //压缩间隔
    public int CompressInterval = 20;
    List<RecordData> Compress(List<RecordData> recordDatas)
    {
        List<RecordData> com = new List<RecordData>();
        int k = 1;
        for (int i = 0;i< recordDatas.Count;i++)
        {
            if (
                i != 0 &&
                i != recordDatas.Count && 
                recordDatas[i].guid == recordDatas[i - k].guid && //与前一个命令GUID相同
                recordDatas[i].guid == recordDatas[i + 1].guid && // 与后一个命令GUID相同
                recordDatas[i].command.Split('|')[0] == recordDatas[i - k].command.Split('|')[0] && //与前一个命令类型相同
                recordDatas[i].command.Split('|')[0] == recordDatas[i + 1].command.Split('|')[0] && //与后一个命令类型相同
                recordDatas[i].time - recordDatas[i - k].time < CompressInterval //与前一个命令时间间隔小于压缩间隔
                )
            {
                k++;
                continue;//丢弃
            }
            else
            {
                k = 1;
                com.Add(recordDatas[i]);//写入
            }
        }
        return com;
    }
    //Coroutine coroutine = null;
    Loxodon.Framework.Asynchronous.IAsyncResult result = null;
    public void StartReplay(string name)
    {
        if(File.Exists(savePath + name + ".json"))
            StartReplay(JsonHelper.JsonToObject<List<RecordData>>(JsonHelper.ReadJsonString(savePath + name + ".json")));
    }
    private void StartReplay(List<RecordData> recordDatas)
    {
        result = Executors.RunOnCoroutine(Replay(recordDatas));
    }
    public void ReplaySpeed(float s)
    {
        if (result == null) 
            Time.timeScale = 1;
        else
        Time.timeScale = s;
    }

    public void Terminate()
    {
        EndRecord();
        EndReplay();
    }
    

    IEnumerator Replay(List<RecordData> recordDatas)
    {
        if (recording) yield break;
        Expression.expressionBases.Clear();
        ReplaySpeed(1);
        startTime = (int)(Time.time * 1000);
        foreach (RecordData data in recordDatas)
        {
            while (RecordTime < data.time)
                yield return new WaitForFixedUpdate();
            string type = data.command.Split('|')[0];
            switch(type)
            {
                case "Create":
                    yield return Expression.RecordCreate(data.command.Split('|')[1], data.guid);
                    break;
                case "End":
                    EndReplay();
                    break;
                default:
                    foreach (Expression expressionBase in Expression.expressionBases)
                        if (expressionBase.GUID == data.guid)
                            expressionBase.SendCommand(data.command);
                    break;
            }
        }
        EndReplay();
    }
    void EndReplay()
    {
        if (result == null) return;
        foreach (Expression eb in Expression.expressionBases)
        {
            if (eb.gameObject != null)
                UnityEngine.Object.Destroy(eb.gameObject);
        }
        result.Cancel();
        result = null;
        ReplaySpeed(1);
    }
    
}
