using ExperimentFramework.Equipments;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExperimentFramework.Interactive
{
    public class InteractiveBehavior : MonoBehaviour, IFeaturesOperate
    {
        public GameObject InteractionObject => gameObject;

        public OperateStatus Status { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public EquipmentLocation Location { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string Guid { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public Features Features => throw new System.NotImplementedException();
    }
}

