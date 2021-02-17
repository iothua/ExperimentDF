using System.Collections.Generic;
using UnityEngine;

namespace ExperimentFramework.Evaluation
{
    /// <summary>
    /// 报告数据处理
    /// </summary>
    public class ReportDataBase : MonoBehaviour
    {
        public void InitReportData()
        {
            //OperateVerifySystem.AddListener(GetReportData);
            //MessageDispatcher.AddListener("table",ReportTable);
        }

        public virtual Dictionary<int, string> GetReportData(List<int> nums)
        {
            return new Dictionary<int, string>();
        }

        public void DestroyReportData()
        {
            //OperateVerifySystem.RemoveListener(GetReportData);
            //MessageDispatcher.RemoveListener("table", ReportTable);
        }

        // public virtual void ReportTable(IMessage message)
        // {
        //     //Table处理
        //     var data = (int) message.Data;
        //
        //     if (data == 0||data==1||data ==2)
        //     {
        //         //ExperimentMain.Evaluation.Report.Automatics[0]= 添加数据
        //         //ExperimentMain.SendCommand("实验报告|0,1,2,3");
        //     }
        // }

    }
}