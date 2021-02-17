using System;

namespace ExperimentFramework.Equipments
{
    /// <summary>
    /// 仪器位置状态
    /// </summary>
    public enum EquipmentLocationStatus
    {
        /// <summary>
        /// 架子上
        /// </summary>
        Shelf,
        /// <summary>
        /// 桌面上
        /// </summary>
        Desktop,
        /// <summary>
        /// 其他
        /// </summary>
        Other
    }

    /// <summary>
    /// 仪器位置
    /// </summary>
    public class EquipmentLocation
    {
        public EquipmentLocationStatus InitialLocationStatus;
        public EquipmentLocationStatus CurrentLocationStatus;
        public TransformData InitialTransform;
    }
}
