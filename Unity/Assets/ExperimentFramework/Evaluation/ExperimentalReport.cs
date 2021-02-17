using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ExperimentFramework.Evaluation
{
    /// <summary>
    /// 答案分数
    /// </summary>
    [Serializable]
    public struct Answer
    {
        public string Value;
        public float Range;
        public float Fraction;
        
        public bool IsError;
    }

    /// <summary>
    /// 实验报告（后期要考虑模糊搜索）
    /// </summary>
    [Serializable]
    public class ExperimentalReport
    {
        /// <summary>
        /// 系统数据
        /// </summary>
        public List<Answer> Automatics = new List<Answer>();

        /// <summary>
        /// 手动数据
        /// </summary>
        public List<string> Manuals = new List<string>();

        /// <summary>
        /// 加载自动数据
        /// </summary>
        /// <param name="datas"></param>
        public void LoadAutomatics(List<Answer> datas)
        {
            Automatics = datas;
            string[] manuals = new string[datas.Count];
            for (int i = 0; i < manuals.Length; i++)
            {
                Manuals.Add("");
            }
        }

        /// <summary>
        /// 添加【自动数据】
        /// </summary>
        /// <param name="values"></param>
        public void AddAutomatics(Dictionary<int, string> values)
        {
            foreach (var item in values)
            {
                Answer answer = Automatics[item.Key];
                answer.Value = item.Value;

                Automatics[item.Key] = answer;
            }
        }

        /// <summary>
        /// 添加【手动数据】
        /// </summary>
        /// <param name="values"></param>
        public void AddManuals(Dictionary<int, string> values)
        {
            foreach (var item in values)
            {
                Manuals[item.Key] = item.Value;
            }
        }

        /// <summary>
        /// 返回错误的值
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, string> GetErrorValue()
        {
            Dictionary<int, string> errors = new Dictionary<int, string>();

            if(Automatics.Count!=Manuals.Count)
                throw new Exception("数据大小不匹配");

            for (int i = 0; i < Automatics.Count; i++)
            {
                if (string.IsNullOrEmpty(Automatics[i].Value))
                {
                    errors.Add(i, Manuals[i]);
                    continue;
                }

                if (!Automatics[i].Value.Equals(Manuals[i]))
                {
                    errors.Add(i, Manuals[i]);
                }
            }
            
            return errors;
        }
        
    }

}