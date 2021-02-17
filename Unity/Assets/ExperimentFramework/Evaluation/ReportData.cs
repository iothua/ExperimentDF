using System;
using System.Collections.Generic;
using UnityEngine;

namespace ExperimentFramework.Evaluation
{

    [Serializable]
    public class ReportData
    {
        
        public List<Answer> Answers;

        /// <summary>
        /// 分数
        /// </summary>
        public float Fraction => GetFraction();
        /// <summary>
        /// 注解
        /// </summary>
        public string Note;

        public float SumFraction()
        {
            float sum = 0;
            
            foreach (var item in Answers)
            {
                sum += item.Fraction;
            }

            return sum;
        }

        public float GetFraction()
        {
            float fraction = 0;

            foreach (var item in Answers)
            {
                if(item.IsError) continue;
                fraction += item.Fraction;
            }

            return fraction;
        }
    }
}