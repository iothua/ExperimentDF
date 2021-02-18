using System;
using System.Collections.Generic;
using ExperimentFramework.Evaluation;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace App
{
    /// <summary>
    /// 实验动作窗体
    /// </summary>
    [Serializable]
    public class ExperimentActionWindow
    {
        [LabelText("动作数据")]
        [TableList(ShowIndexLabels = true)]
        public List<ExperimentActionDataWindow> ActionWindows;

        private List<ActionData> _actionDatas;
        
        public List<ActionData> GetActionDatas()
        {
            if (_actionDatas == null)
            {
                _actionDatas = new List<ActionData>();
            }

            _actionDatas.Clear();

            foreach (var window in ActionWindows)
            {
                _actionDatas.Add(window.GetActionData());
            }

            return _actionDatas;
        }

        public void SetActionData(List<ActionData> actionDatas)
        {
            _actionDatas = actionDatas;

            if (ActionWindows == null)
            {
                ActionWindows = new List<ExperimentActionDataWindow>();
            }
            
            ActionWindows.Clear();

            if (actionDatas == null) return;
            
            foreach (var actionData in actionDatas)
            {
                var window = new ExperimentActionDataWindow();
                window.SetActionData(actionData);

                ActionWindows.Add(window);
            }
        }
    }

    public class LabelDataWindow
    {
        [HideInEditorMode]
        public bool IsDelete;

        [LabelText("添加动作数据")]
        [TableList(ShowIndexLabels = true)]
        public List<LabelData> Labels;

        [ShowIf("IsDelete")]
        [LabelText("添加删除动作数据")]
        [TableList(ShowIndexLabels = true)]
        public List<LabelData> DeleteLabels;
    }
    

    [Serializable]
    public class ExperimentActionDataWindow
    {
        /// <summary>
        /// 动作标识
        /// </summary>
        [VerticalGroup("动作设置")]
        public string TagID;
        
        /// <summary>
        /// 关键动作(用于触发步骤)
        /// </summary>
        [VerticalGroup("动作设置")]
        //[LabelText("是否属于【关键动作】")]
        public bool KeyAction;
        
        /// <summary>
        /// 分数
        /// </summary>
        [VerticalGroup("动作设置")]
        [Range(0,1),Tooltip("基于该步骤分数的百分比")]
        public float Fraction;
        
        /// <summary>
        /// 说明
        /// </summary>
        [TextArea,LabelWidth(200)]
        public string Description;
        
        /// <summary>
        /// 关联动作节点，当这些关联动作都完成时，该动作才会被启动
        /// </summary>
        [LabelText("动作索引"),Tooltip("例：0,1,2当动作0，1，2均完成时，该动作才会完成")]
        [VerticalGroup("关联与标签设置")]
        public string RelationActions;

        [VerticalGroup("关联与标签设置")]
        [Button("标签设置")]
        public void SetLabel()
        {
            CreateWindow(SetLabels, DeleteLabels, (result1, result2) =>
            {
                SetLabels = result1;
                DeleteLabels = result2;
            });
        }
        
        public void DeleteLabel()
        {
            CreateWindow(DeleteLabels, result => DeleteLabels = result);
        }
        [VerticalGroup("关联与标签设置")]
        [Button("关联标签")]
        public void RelationLabel()
        {
            CreateWindow(RelationLabels, result => RelationLabels = result);
        }

        /// <summary>
        /// 设置标签
        /// 当该动作启动时，需要给哪些仪器标签，如果没有设置，则默认指定第0个。
        /// </summary>
        private List<LabelData> SetLabels;

        private List<LabelData> DeleteLabels;
        
        /// <summary>
        /// 关联标签(同一个仪器，多个标签可用逗号分开)
        /// </summary>
        private List<LabelData> RelationLabels;
        
        /// <summary>
        /// 错误说明
        /// </summary>
        [TextArea,LabelWidth(200)]
        public string Error;

        /// <summary>
        /// 是否属于进度值(适用于记录数据操作)
        /// </summary>
        [VerticalGroup("设置")]
        [Tooltip("该动作是否属于动作进度，只有当该动作完成一定次数时，才会完成该动作")]
        public bool IsProgress;
        
        /// <summary>
        /// 达到的进度目标
        /// </summary>
        [Tooltip("需要完成动作次数")]
        [VerticalGroup("设置")]
        [ShowIf("IsProgress")]
        public float ProgressGoal;

        /// <summary>
        /// 操作命令
        /// </summary>
        [Tooltip("描述以'|'隔开，例：试管|离开|试管架")]
        [VerticalGroup("设置")]
        public string Command;

        private ActionData _actionData;

        public void SetActionData(ActionData actionData)
        {
            this._actionData = actionData;

            TagID = actionData.TagID;
            Description = actionData.Description;
            KeyAction = actionData.KeyAction;
            RelationActions = actionData.RelationActions;
            
            SetLabels = actionData.SetLabels;
            RelationLabels = actionData.RelationLabels;
            DeleteLabels = actionData.DeleteLabels;
            
            Fraction = actionData.Fraction;
            Error = actionData.Error;
            IsProgress = actionData.IsProgress;
            ProgressGoal = actionData.ProgressGoal;
            Command = actionData.Command;
        }

        public ActionData GetActionData()
        {
            _actionData.TagID = TagID;
            _actionData.Description = Description;
            _actionData.KeyAction = KeyAction;
            _actionData.RelationActions = RelationActions;
            
            _actionData.SetLabels = SetLabels;
            _actionData.RelationLabels = RelationLabels;
            _actionData.DeleteLabels = DeleteLabels;
            
            _actionData.Fraction = Fraction;
            _actionData.Error = Error;
            _actionData.IsProgress = IsProgress;
            _actionData.ProgressGoal = ProgressGoal;
            _actionData.Command = Command;
            
            return _actionData;
        }

        void CreateWindow(List<LabelData> labelDatas,Action<List<LabelData>> onClose)
        {
            var labelDataWindow = new LabelDataWindow {Labels = labelDatas};

            var window = OdinEditorWindow.InspectObject(labelDataWindow);
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(500, 200);
            window.titleContent = new GUIContent($"【{TagID}】动作标签设置");

            window.OnClose += () =>
            {
                onClose?.Invoke(labelDataWindow.Labels);
            };
        }

        void CreateWindow(List<LabelData> labelDatas1, List<LabelData> labelDatas2,
            Action<List<LabelData>, List<LabelData>> onClose)
        {
            var labelDataWindow = new LabelDataWindow {Labels = labelDatas1,DeleteLabels = labelDatas2,IsDelete = true};

            var window = OdinEditorWindow.InspectObject(labelDataWindow);
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(500, 200);
            window.titleContent = new GUIContent($"【{TagID}】动作标签设置");

            window.OnClose += () =>
            {
                onClose?.Invoke(labelDataWindow.Labels, labelDataWindow.DeleteLabels);
            };
        }
    }
}