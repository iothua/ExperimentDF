using System;
using System.Collections.Generic;

namespace App.Network
{
    /// <summary>
    /// 练习数据
    /// </summary>
    [Serializable]
    public class ExerciseData
    {
        public string number;
        public int level;

        public List<ExerciseStepData> stepList;
    }

    [Serializable]
    public class ExerciseStepData
    {
        public int sortNo;
        public int costTime;
        public int level;
    }
}