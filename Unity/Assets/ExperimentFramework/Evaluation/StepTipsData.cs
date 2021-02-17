using System;

namespace ExperimentFramework.Evaluation
{
    /// <summary>
    /// 步骤提示数据
    /// </summary>
    [Serializable]
    public struct StepTipsData
    {
        public int index;
        public bool isLast;
        public string stepName;
        public string actionName;
        public float fraction;
    }
}