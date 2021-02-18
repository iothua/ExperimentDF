using System;
using System.Collections.Generic;


namespace App.Network
{
    [Serializable]
    public class ExperimentDataInfos
    {
        public List<ExperimentDataInfo> infos;
    }

    [Serializable]
    public class ExperimentDataInfo
    {
        public int subject;
        public string name;
        public string number;
        public int suggestCostTime;
        public string totalScore;
        public List<ExperimentStepInfo> stepList;
    }

    [Serializable]
    public class ExperimentStepInfo
    {
        public int sortNo;
        public string name;
        public int suggestCostTimePercent;
        public int scorePercent;
        public int isVideo;
    }

}
