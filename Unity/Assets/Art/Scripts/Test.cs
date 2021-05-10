using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    Expression expressionBase1, expressionBase2, expressionBase3;
    public void Create()
    {
        StartCoroutine(Expression.ExperimentCreate("Cube", Done1, "Shader|0,0,Color,_BaseColor,0,0,0,1","WorldMove|-1,5,5,5"));
        StartCoroutine(Expression.ExperimentCreate("Sphere", Done2, "Shader|0,0,Color,_BaseColor,1,0,0,1"));
        StartCoroutine(Expression.ExperimentCreate("Eff", Done3, "WorldMove|-1,1,1,1","Scale|-1,200,200,200"));
    }
    void Done1(Expression expressionBase)
    {
        expressionBase1 = expressionBase;
    }
    void Done2(Expression expressionBase)
    {
        expressionBase2 = expressionBase;
    }
    void Done3(Expression expressionBase)
    {
        expressionBase3 = expressionBase;
    }
    public void Change()
    {
        switch (Random.Range(0, 10))
        {
            case 0:
                expressionBase1.SendCommand("Shader|0,0,Color,_BaseColor," + Random.Range(-3f, 3f) + "," + Random.Range(-3f, 3f) + "," + Random.Range(-3f, 3f) + "," + Random.Range(-3f, 3f));
                break;
            case 1:
                expressionBase2.SendCommand("Shader|0,0,Color,_BaseColor," + Random.Range(-3f, 3f) + "," + Random.Range(-3f, 3f) + "," + Random.Range(-3f, 3f) + "," + Random.Range(-3f, 3f));
                break;
            case 2:
                expressionBase1.SendCommand("WorldMove|-1," + Random.Range(-3f,3f) + "," + Random.Range(-3f, 3f) + "," + Random.Range(-3f, 3f)+ ",1");
                break;
            case 3:
                expressionBase2.SendCommand("WorldMove|-1," + Random.Range(-3f, 3f) + "," + Random.Range(-3f, 3f) + "," + Random.Range(-3f, 3f) + ",1");
                break;
            case 4:
                expressionBase3.SendCommand("Animator|0,State,New Animation2");
                break;
            case 5:
                expressionBase3.SendCommand("Animator|0,State,New Animation");
                break;
            case 6:
                expressionBase1.SendCommand("Parent|-1," + (Random.Range(0, 2)==0?expressionBase2.GUID:"0") + ",-1");
                break;
            case 7:
                expressionBase2.SendCommand("Parent|-1," + (Random.Range(0, 2) == 0 ? expressionBase1.GUID : "0") + ",-1");
                break;
            case 8:
                expressionBase1.SendCommand("Scale|-1," + Random.Range(-3f, 3f) + "," + Random.Range(-3f, 3f) + "," + Random.Range(-3f, 3f));
                break;
            case 9:
                expressionBase2.SendCommand("Scale|-1," + Random.Range(-3f, 3f) + "," + Random.Range(-3f, 3f) + "," + Random.Range(-3f, 3f));
                break;
        }
    }
    public void Time(float time)
    {
        UnityEngine.Time.timeScale = time;
    }
    Expression current;
    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out Expression expressionBase))
                current = expressionBase;
        }
        if (Input.GetMouseButtonUp(0))
        {
            current = null;
        }
         if(current)
        {
            Vector3 vector3  = new Vector3(Input.mousePosition.x, Input.mousePosition.y,5);
            current.SendCommand("WorldMove|-1," + Camera.main.ScreenToWorldPoint(vector3).x + "," + Camera.main.ScreenToWorldPoint(vector3).y + "," + Camera.main.ScreenToWorldPoint(vector3).z);
        }
    }

    public void RecordStart()
    {
        RecordSystem.Self.StartRecord();
    }
    public void RecordEnd(string s)
    {
        RecordSystem.Self.EndRecord(s);
    }
    public void RecordReplay(string s)
    {
        RecordSystem.Self.StartReplay(s);
    }
}
