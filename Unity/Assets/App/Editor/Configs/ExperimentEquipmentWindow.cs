using System;
using System.Collections.Generic;
using App.Config;
using Sirenix.OdinInspector;
using UnityEngine;

namespace App
{
    [System.Serializable]
    public class ExperimentEquipmentWindow
    {
        
        [Button(ButtonSizes.Large,Name = "保存更改")]
        public void SaveEquipmentData()
        {
            var window = new ExperimentEquipmentConfig()
            {
                ShelfEquipments = ShelfEquipments,
                DesktopEquipments = DesktopEquipments,
                OtherEquipments = OtherEquipments
            };
            string jsonData = JsonHelper.ObjectToJsonString(window);

            if (string.IsNullOrEmpty(filePath))
            {
                string fileName = "临时" + DateTime.Now.ToString("yy-MM-dd-hh-mm-ss");
                filePath = ExperimentConfigWindow.EquipmentResourcePath(fileName) + fileName;
            }

            JsonHelper.SaveJson(jsonData, filePath);
            GetEquipmentData(window, filePath);
        }
        
        [InfoBox("各仪器放规格：\r\n" +
                 "1、仪器架仪器：试管架、卫生纸盒、细口瓶、广口瓶、大烧杯等\r\n" +
                 "2、桌面仪器：废液缸、操作仪器等\r\n" +
                 "3、其他仪器：由器材自己创建生成仪器，如试管、镊子、药匙、胶头滴管、玻璃棒等\r\n")]
        [InfoBox("注：系统会根据这些所在仪器的集合，在加载实验时打入标签，从而来达到不同的功能效果", InfoMessageType.Warning)]
        [LabelText("仪器架位置仪器")]
        public List<ExperimentEquipmentData> ShelfEquipments;
        
        /// <summary>
        /// 桌子上的仪器
        /// </summary>
        [LabelText("桌面位置仪器")]
        public List<ExperimentEquipmentData> DesktopEquipments;
        
        /// <summary>
        /// 其他上的仪器
        /// </summary>
        [LabelText("其他位置仪器"),Tooltip("如瓶盖、火柴棍、试管、镊子等是创建在其他仪器上的")]
        public List<ExperimentEquipmentData> OtherEquipments;
        private string filePath;
        
        public void GetEquipmentData(ExperimentEquipmentConfig window,string filePath)
        {
            ShelfEquipments = window.ShelfEquipments;
            OtherEquipments = window.OtherEquipments;
            DesktopEquipments = window.DesktopEquipments;
            this.filePath = filePath;
        }

    }
}