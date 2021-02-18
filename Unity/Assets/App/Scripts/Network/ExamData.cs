using System.Collections.Generic;
using System;

namespace App.Network
{
    [Serializable]
    public class ExamData
    {
        public string number;
        public int costTime;
        public string reportScore;
        
        public List<ExamStepData> stepList;

        public List<ExamReportData> reportList;
    }

    [Serializable]
    public class ExamReportData {
        public string answer;
        public int status;
        public string score;
    }

    [Serializable]
    public class ExamStepData
    {
        public int sortNo;
        public string score;
        public int status;
        public int isVideo;
        public string clientFileId;

        public int screenX;
        public int screenY;
        public string errorReason;
        public string comment;

        public List<ExamErrorData> actionList;
    }

    [Serializable]
    public class ExamErrorData
    {
        public int type;
        public string description;
        public int timeNode;
        public int positionX;
        public int positionY;
    }
}