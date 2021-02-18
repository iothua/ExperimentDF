using System;
using Newtonsoft.Json;
using UnityEngine;

namespace ExperimentFramework
{
    [Serializable]
    public struct KeyValueData
    {
        public string Key;
        public string Value;
    }
    
    [Serializable]
    public struct EVector3
    {
        public float X;
        public float Y;
        public float Z;

        public EVector3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public EVector3(float x, float y)
        {
            this.X = x;
            this.Y = y;
            this.Z = 0;
        }

        public EVector3(Vector3 vector3)
        {
            this.X = vector3.x;
            this.Y = vector3.y;
            this.Z = vector3.z;
        }

        public Vector3 UnityVector()
        {
            return new Vector3(X, Y, Z);
        }
        
        
        public static EVector3 zero => new EVector3(0, 0, 0);
        public static EVector3 one => new EVector3(1, 1, 1);
    }
    
    [Serializable]
    public class TransformData
    {
        //是否是局部坐标
        public bool IsLocal;
        
        [JsonIgnore]
        public Vector3 Position;
        [JsonIgnore]
        public Vector3 Rotate;
        [JsonIgnore]
        public Vector3 Scale = Vector3.one;
        public string PositionStr
        {
            get => Position.ToString();
            set=>Position = value.ToVector3();
        }

        public string RotateStr
        {
            get => Rotate.ToString();
            set =>Rotate = value.ToVector3();
        }
        
        public string ScaleStr
        {
            get => Scale.ToString();
            set => Scale = value.ToVector3();
        }

        public TransformData()
        {
        }

        public TransformData(Transform transform,bool isLocal=false)
        {
            this.IsLocal = isLocal;

            if (isLocal)
            {
                Position=transform.localPosition;
                Rotate=transform.localRotation.eulerAngles;
            }
            else
            {
                Position=transform.position;
                Rotate = transform.rotation.eulerAngles;
            }
            Scale=transform.localScale;
        }
    }
    
    public enum Axis
    {
        X,
        Y,
        Z
    }

    public enum PutAxis
    {
        MouseX,
        MouseY,
        All
    }

    /// <summary>
    /// 提示状态
    /// </summary>
    public enum HighlightTipsStatus
    {
        /// <summary>
        /// 无
        /// </summary>
        None,
        /// <summary>
        /// 高亮提示(不会随着鼠标移入移出等操作，来熄灭高亮)
        /// </summary>
        HighlightTips
    }
}