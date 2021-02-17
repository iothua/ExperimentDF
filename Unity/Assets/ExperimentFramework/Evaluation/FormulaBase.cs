using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ExperimentFramework.Evaluation
{
    public class FormulaBase
    {
        public ExperimentalReport Report;
        
        public virtual Dictionary<int, string> GetErrorValue(ExperimentalReport report)
        {
            Dictionary<int, string> errors = new Dictionary<int, string>();

            if (report.Automatics.Count != report.Manuals.Count)
            {
                throw new Exception("数据大小不匹配");
            }

            for (int i = 0; i < report.Automatics.Count; i++)
            {
                if (string.IsNullOrEmpty(report.Automatics[i].Value))
                {
                    errors.Add(i, report.Manuals[i]);
                    continue;
                }

                if (!IsErrorValue(i, report.Manuals[i]))
                {
                    errors.Add(i, report.Manuals[i]);
                }
                //
                // if (Regex.IsMatch(report.Automatics[i].Value, @"^\d+(\.\d+)?$") && report.Manuals[i] != "")
                // {
                //     if (float.Parse(report.Manuals[i]) != float.Parse(report.Automatics[i].Value))
                //     {
                //         errors.Add(i, report.Manuals[i]);
                //     }
                // }
                // else
                // {
                //     if (!report.Automatics[i].Value.Equals(report.Manuals[i]))
                //     {
                //         errors.Add(i, report.Manuals[i]);
                //     }
                // }
            }

            return errors;
        }

        /// <summary>
        /// 判断是否是错误数据，Flase 表示错误数据，True不是错误数据
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool IsErrorValue(int index,string value)
        {
            if (index == -1) return false;

            if (index >= Report.Automatics.Count)
            {
                return false;
            }

            string autoValue = Report.Automatics[index].Value;

            if (string.IsNullOrEmpty(autoValue)) return false;

            if (Regex.IsMatch(autoValue, @"^\d+(\.\d+)?$")&&value!="")
            {
                if (Mathf.Abs(float.Parse(value) - Mathf.Abs(float.Parse(autoValue))) > Report.Automatics[index].Range)
                {
                    return false;
                }
            }
            else
            {
                if (!autoValue.Equals(value))
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}