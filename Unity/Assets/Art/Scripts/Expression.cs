using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using DG.Tweening;

public class Expression : MonoBehaviour
{
    //引用
    public List<UnityEngine.Object> references = new List<UnityEngine.Object>();

    string guid;
    public string GUID
    {
        get
        {
            if (string.IsNullOrEmpty(guid))
                guid = Guid.NewGuid().ToString();
            return guid;
        }
        private set
        {
            guid = value;
        }
    }
    public static List<Expression> expressionBases = new List<Expression>();
    public static IEnumerator ExperimentCreate(string key, Action<Expression> onFinished = null, params string[] initialCommand)
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(key);
        yield return handle;
        Expression eb = Instantiate(handle.Result).GetComponent<Expression>();
        Addressables.Release(handle);
        expressionBases.Add(eb);
        RecordSystem.Self.RecordCommand(eb.GUID, "Create|" + key);
        foreach (string cmd in initialCommand)
        {
            eb.SendCommand(cmd);
        }
        onFinished?.Invoke(eb);
        
    }
    public static IEnumerator RecordCreate(string key, string guid, Action<Expression> onFinished = null)
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(key);
        yield return handle;
        Expression eb = Instantiate(handle.Result).GetComponent<Expression>();
        Addressables.Release(handle);
        expressionBases.Add(eb);
        eb.GUID = guid;
        onFinished?.Invoke(eb);
        
    }
    public void SendCommand(string command)
    {
        RecordSystem.Self.RecordCommand(GUID, command);
        if (Analysis(command)) return;
        BaseAnalysis(command);
    }
    public virtual bool Analysis(string command)
    {
        return false;
    }
    void BaseAnalysis(string command)
    {
        try
        {
            string type = command.Split('|')[0];
            string cmd = command.Split('|')[1];
            string[] cmds = cmd.Split(',');
            int o = int.Parse(cmds[0]);
            switch (type)
            {
                case "WorldMove"://WorldMove|0,1,1,1
                    WorldMove(cmds);
                    return;
                case "LocalMove"://LocalMove|0,1,1,1
                    LocalMove(cmds);
                    return;
                case "Rotate"://WorldRotate|0,1,1,1
                    Rotate(cmds);
                    return;
                case "Scale"://Scale|0,1,1,1
                    Scale(cmds);
                    return;
                case "Shader"://Shader|0,Float,_value,1.5
                    Shader(cmds);
                    return;
                case "Parent"://Parent|0,guid,0
                    Parent(cmds);
                    return;
                case "Active"://Active|0,0
                    Active(cmds);
                    return;
                case "Enable"://Active|0,0
                    Enable(cmds);
                    return;
                case "Destory"://Destory|-1
                    Destroy(cmds);
                    return;
                case "Animator"://Animator|-1,Int,value,0
                    Animator(cmds);
                    return;
            }
        }
        catch
        {
            Debug.LogError("命令解析失败");
        }
    }

    private void Animator(string[] cmds)
    {
        int o = int.Parse(cmds[0]);
        if (o < 0) return;
        switch (cmds[1])
        {
            case "Int":
                ((Animator)references[o]).SetInteger(cmds[2], int.Parse(cmds[3]));
                break;
            case "Float":
                ((Animator)references[o]).SetFloat(cmds[2], float.Parse(cmds[3]));
                break;
            case "Bool":
                ((Animator)references[o]).SetBool(cmds[2], cmds[3] != "0");
                break;
            case "State":
                ((Animator)references[o]).Play(cmds[2]);
                break;
        }
    }

    private void Destroy(string[] cmds)
    {
        int o = int.Parse(cmds[0]);
        if (o < 0)
            Destroy(gameObject);
        else
            Destroy(references[o]);
    }

    private void Enable(string[] cmds)
    {
        int o = int.Parse(cmds[0]);
        if (o < 0) return;
        ((Renderer)references[o]).enabled = cmds[1] != "0";
    }

    private void Active(string[] cmds)
    {
        int o = int.Parse(cmds[0]);
        if (o < 0)
            gameObject.SetActive(cmds[1] != "0");
        else
            ((GameObject)references[o]).SetActive(cmds[1] != "0");
    }

    private void Parent(string[] cmds)
    {
        int o = int.Parse(cmds[0]);
        Transform localTransform;
        if (o < 0)
        {
            localTransform = transform;
        }
        else
        {
            localTransform = (Transform)references[o];
        }
        if (cmds[1] == "0")
        {
            localTransform.parent = null;
        }
        else
        {
            foreach (Expression eb in expressionBases)
            {
                if (eb.GUID == cmds[1])
                {
                    int i = int.Parse(cmds[2]);
                    if (i < 0)
                    {
                        localTransform.parent = eb.transform;
                    }
                    else
                    {
                        localTransform.parent = (Transform)eb.references[i];
                    }
                }
            }
        }
    }

    private void Shader(string[] cmds)
    {
        int o = int.Parse(cmds[0]);
        if (o < 0) return;
        switch (cmds[2])
        {
            case "Float":
                if (cmds.Length < 6 || cmds[5] == "0")
                    ((Renderer)references[o]).materials[int.Parse(cmds[1])].SetFloat(cmds[3], float.Parse(cmds[4]));
                else
                    ((Renderer)references[o]).materials[int.Parse(cmds[1])].DOFloat(float.Parse(cmds[4]), cmds[3], float.Parse(cmds[5]));
                break;
            case "Int":
                ((Renderer)references[o]).materials[int.Parse(cmds[1])].SetInt(cmds[3], int.Parse(cmds[4]));
                break;
            case "Color":
                if (cmds.Length < 9 || cmds[8] == "0")
                    ((Renderer)references[o]).materials[int.Parse(cmds[1])].SetColor(cmds[3], new Color(float.Parse(cmds[4]), float.Parse(cmds[5]), float.Parse(cmds[6]), float.Parse(cmds[7])));
                else
                    ((Renderer)references[o]).materials[int.Parse(cmds[1])].DOColor(new Color(float.Parse(cmds[4]), float.Parse(cmds[5]), float.Parse(cmds[6]), float.Parse(cmds[7])), cmds[3], float.Parse(cmds[8]));
                break;
            case "Vector":
                ((Renderer)references[o]).materials[int.Parse(cmds[1])].SetColor(cmds[3], new Color(float.Parse(cmds[4]), float.Parse(cmds[5]), float.Parse(cmds[6]), float.Parse(cmds[7])));
                break;
            case "Texture":
                ((Renderer)references[o]).materials[int.Parse(cmds[1])].SetTexture(cmds[3], (Texture2D)references[int.Parse(cmds[4])]);
                break;
        }
    }

    private void Scale(string[] cmds)
    {
        int o = int.Parse(cmds[0]);
        Transform localTransform;
        if (o < 0)
        {
            localTransform = transform;
        }
        else
        {
            localTransform = (Transform)references[o];
        }
        if (cmds.Length < 5 || cmds[4] == "0")
            localTransform.localScale = new Vector3(float.Parse(cmds[1]), float.Parse(cmds[2]), float.Parse(cmds[3]));
        else
            transform.DOScale(new Vector3(float.Parse(cmds[1]), float.Parse(cmds[2]), float.Parse(cmds[3])), float.Parse(cmds[4]));
    }

    private void Rotate(string[] cmds)
    {
        int o = int.Parse(cmds[0]);
        Transform localTransform;
        if (o < 0)
        {
            localTransform = transform;
        }
        else
        {
            localTransform = (Transform)references[o];
        }
        if (cmds.Length < 5 || cmds[4] == "0")
            localTransform.localEulerAngles = new Vector3(float.Parse(cmds[1]), float.Parse(cmds[2]), float.Parse(cmds[3]));
        else
            transform.DOLocalRotate(new Vector3(float.Parse(cmds[1]), float.Parse(cmds[2]), float.Parse(cmds[3])), float.Parse(cmds[4]));
    }

    private void LocalMove(string[] cmds)
    {
        int o = int.Parse(cmds[0]);
        Transform localTransform;
        if (o < 0)
        {
            localTransform = transform;
        }
        else
        {
            localTransform = (Transform)references[o];
        }
        if (cmds.Length < 5 || cmds[4] == "0")
            localTransform.localPosition = new Vector3(float.Parse(cmds[1]), float.Parse(cmds[2]), float.Parse(cmds[3]));
        else
            transform.DOLocalMove(new Vector3(float.Parse(cmds[1]), float.Parse(cmds[2]), float.Parse(cmds[3])), float.Parse(cmds[4]));
    }

    private void WorldMove(string[] cmds)
    {
        int o = int.Parse(cmds[0]);
        Transform localTransform;
        if (o < 0)
        {
            localTransform = transform;
        }
        else
        {
            localTransform = (Transform)references[o];
        }
        if (cmds.Length < 5 || cmds[4] == "0")
            localTransform.position = new Vector3(float.Parse(cmds[1]), float.Parse(cmds[2]), float.Parse(cmds[3]));
        else
            transform.DOMove(new Vector3(float.Parse(cmds[1]), float.Parse(cmds[2]), float.Parse(cmds[3])), float.Parse(cmds[4]));
    }
}
