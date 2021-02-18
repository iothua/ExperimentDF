using ExperimentFramework.Equipments;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ExperimentFramework.Interactive
{
    public class InteractiveBehavior : MonoBehaviour, IFeaturesOperate
    {

        public Features features;
        
        public GameObject InteractionObject => gameObject;

        public OperateStatus Status { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public EquipmentLocation Location { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string Guid { get; set; }

        public Features Features => features;

        public UnityEvent OnInitializeEquipment;
        public UnityEvent OnDestroyEquipment;

        public EventInteractionEquipment OnEquipmentEnter;
        public EventInteractionEquipment OnEquipmentExit;
        public EventInteractionEquipment OnEquipmentStay;
        public EventInteractionEquipmentRelease OnEquipmentRelease;
    }
}

