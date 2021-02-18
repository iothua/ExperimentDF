using System;
using System.Collections.Generic;
using ExperimentFramework;

namespace App.Config
{
    /// <summary>
    /// 器材虚影配置
    /// </summary>
    [Serializable]
    public class EquipmentShadowConfig
    {
        public List<KeyValueData> ShadowDatas;

        public Dictionary<string, string> DicShadowDatas { get; set; }

        public void ToDictionary()
        {
            if (ShadowDatas == null || ShadowDatas.Count == 0)
            {
                DicShadowDatas = new Dictionary<string, string>();
            }

            foreach (var shadowData in ShadowDatas)
            {
                if (DicShadowDatas == null)
                {
                    DicShadowDatas = new Dictionary<string, string>();
                }

                DicShadowDatas.Add(shadowData.Key, shadowData.Value);
            }
        }
    }
}