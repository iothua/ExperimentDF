using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExperimentFramework.Inputs
{
    public enum CursorStatus
    {
        /// <summary>
        /// 默认（左键交互状态）
        /// </summary>
        None,
        /// <summary>
        /// 按下
        /// </summary>
        Pressed,
        /// <summary>
        /// 在UI范围按下
        /// </summary>
        PressedUI,
        /// <summary>
        /// 按下物体预备
        /// </summary>
        PressedGrab,
        /// <summary>
        /// 抓取
        /// </summary>
        Grab,
        /// <summary>
        /// 释放
        /// </summary>
        Release,
        /// <summary>
        /// 抓取中...
        /// </summary>
        Crawling,

        /// <summary>
        /// 旋转（右键旋转状态）
        /// </summary>
        Rotate,
        /// <summary>
        /// 滚轮（滑动缩放状态）
        /// </summary>
        Zoom,
        /// <summary>
        /// 平移（中键平移状态）
        /// </summary>
        Pan,
    }

    public enum MouseIcon
    {
        None,
        Enter,
        Grab,
        Rotate,
        UpDown,
        LeftRight,
        Zoom,
        Pan
    }

    public enum MoveModel
    {
        Screen,//屏幕空间移动
        World,//世界空间移动
        Mixed//混合移动
    }
    public enum TouchOffsetType
    {
        OffsetRange = 1,
        NotOffset = 2
    }
}
