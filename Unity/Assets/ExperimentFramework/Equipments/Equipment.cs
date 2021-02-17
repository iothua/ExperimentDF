using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public string fileName;

    public InteractiveBehavior Behavior;

    public IEnumerator Build()
    {
        yield return Build(fileName);
    }

    public IEnumerator Build(string fileName)
    {
        //创建碰撞体、模型、特效等
        //创建距离检测点

        yield break;
    }


}
