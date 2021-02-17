using System.Collections.Generic;
using System.Linq;
using ExperimentFramework.Inputs;
using HighlightPlus;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ExperimentFramework.Interactive
{
    /// <summary>
    /// 仪器工具，用于生成/查找仪器的相关数据
    /// 创建模型界面、特效界面
    /// </summary>
    [System.Serializable]
    public class Features
    {

        public string Key;

        /// <summary>
        /// 仪器显示名称
        /// </summary>
        public string DisplayName;

        
        private string commonName;
        
        /// <summary>
        /// 仪器命令名称
        /// </summary>
        public string CommonName
        {
            get
            {
                if (string.IsNullOrEmpty(commonName))
                {
                    commonName = DisplayName;
                }

                return commonName;
            }
            set => commonName = value;
        }
        
        [Title("仪器功能")]
        [LabelText("激活【标签】")]
        public bool ActiveLabel = true;
        
        [LabelText("激活【自由下落】")]
        public bool ActiveDropOut = true;

        [LabelText("自由下落高度")]
        public float DropOutHeight = 0.2f;

        [LabelText("激活【抓取移动】")]
        public bool ActiveGrabMove = true;

        [LabelText("抓取移动模式")]
        public MoveModel moveMod = MoveModel.Mixed;

        [LabelText("激活【阴影】 ")]
        public bool ActiveShadow = true;

        [ShowIf("ActiveShadow")]
        [LabelText("阴影大小")]
        public Vector3 ShadowObjectScale = new Vector3(0.01f, 0.01f, 0.01f);
        
        [LabelText("激活【实时虚影】")]
        public bool ActiveRealtimePhantom;

        [LabelText("仪器虚影Key")]
        public string ShadowKey;
        [LabelText("仪器虚影偏移坐标")]
        public TransformData ShadowOffsetTransform;

        [LabelText("特写聚焦距离")]
        public Vector2 FocusDistance = new Vector2(0.3f, 0);

        [LabelText("激活【抓取仪器旋转】")]
        public bool IsRotate=false;

        [LabelText("旋转角度")]
        public Vector3 RotateValue=new Vector3(0,0,0);
        
        //[LabelText("高亮提示坐标")]
        //public Vector2 TipsOffset = new Vector2(0, 0.3f);

        [Title("鼠标图标")]
        public MouseIcon IdleIcon = MouseIcon.None;
        public MouseIcon GrabIcon = MouseIcon.Grab;
        public MouseIcon EnterIcon = MouseIcon.Enter;

        private bool initHighlight;
        
        /// <summary>
        /// 碰撞节点
        /// </summary>
        [Title("节点信息")]
        [LabelText("抓取碰撞体")]
        public BoxCollider Collider;
        [LabelText("发送碰撞体")]
        public BoxCollider SendCollider;

        
        [LabelText("发送碰撞等级")]
        [ValueDropdown("SendColliderLevelS")]
        public int SendColliderLevel = 3;

        [LabelText("接收碰撞体")]
        public GameObject ReceiveCollider;

        [LabelText("接收碰撞等级")]
        [ValueDropdown("ReceiveColliderLevelS")]
        public int ReceiveColliderLevel = 3;

        private static ValueDropdownList<int> SendColliderLevelS = new ValueDropdownList<int>()
        {
            { "低：只被高级接收碰撞体阻挡", 1 },
            { "中：被高级和中级接收碰撞体阻挡", 2 },
            { "高：被所有接收碰撞体阻挡", 3 },
        };

        private static ValueDropdownList<int> ReceiveColliderLevelS = new ValueDropdownList<int>()
        {
            { "低：只阻挡高级发送碰撞体", 1 },
            { "中：阻挡高级和中级发送碰撞体", 2 },
            { "高：阻挡所有发送碰撞体", 3 },
        };
        public LayerMask GetSendLayerMask()
        {
            switch (SendColliderLevel)
            {
                case 1:
                    return 1<< GetLayer(3);
                case 2:
                    return 1 << GetLayer(3) | 1<< GetLayer(2);
                case 3:
                default:
                    return 1 << GetLayer(3) | 1 << GetLayer(2) | 1<< GetLayer(1);
            }
        }
        public int GetReceiveLayer()
        {
            return GetLayer(ReceiveColliderLevel);
        }
        public static int GetLayer(int level)
        {
            switch (level)
            {
                case 1:
                    return LayerMask.NameToLayer("collision1");
                case 2:
                    return LayerMask.NameToLayer("collision2");
                case 3:
                default:
                    return LayerMask.NameToLayer("collision3");
            }
        }
        public Bounds SendBounds => SendCollider.bounds;
        
        /// <summary>
        /// 高亮特效
        /// </summary>
        public HighlightEffect Highlight;

        public HighlightEffect Highlighting
        {
            get
            {
                if (!initHighlight && Highlight != null)
                {
                    //if (ExperimentMain.ExperimentContext == null) return Highlight;
                    
                    ////初始化高亮配置文件
                    //var equipmentManager = ExperimentMain.ExperimentContext.GetService<EquipmentManagement>();

                    //if (equipmentManager == null) return null;

                    //Highlight.ProfileLoad(equipmentManager.EquipmentEffect);

                    initHighlight = true;
                }

                return Highlight;
            }

        }

        /// <summary>
        /// 仪器label,多个Label,用分号结束
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// 提示状态
        /// </summary>
        public HighlightTipsStatus TipsStatus { get; set; }

        public void DeleteLabel(string label)
        {
            List<string> equipmentLabels = Label.Split(',').ToList();
                    
            var deleteLabels = label.Split(',');
                    
            //取两个数组的非交集
            for (int i = 0; i < deleteLabels.Length; i++)
            {
                if (equipmentLabels.Contains(deleteLabels[i]))
                {
                    equipmentLabels.Remove(deleteLabels[i]);
                }
            }

            switch (equipmentLabels.Count)
            {
                case 0:
                    Label = string.Empty;
                    break;
                case 1:
                    Label = equipmentLabels[0];
                    break;
                default:
                    Label = string.Join(",", equipmentLabels);
                    break;
            }
        }

        public void AddLabel(string label)
        {
            if (string.IsNullOrEmpty(Label))
            {
                Label = label;
            }
            else
            {
                Label += "," + label;
            }
        }
    }
}