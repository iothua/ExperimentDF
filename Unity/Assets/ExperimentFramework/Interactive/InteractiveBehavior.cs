using System;
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

        public Func<InteractiveBehavior, bool> OnCanInteractive;
        
        public UnityEvent OnInitializeInteractive;
        public UnityEvent OnDestroyInteractive;
        
        public EventInteractive OnInteractiveEnter;
        public EventInteractive OnInteractiveExit;
        public EventInteractive OnInteractiveStay;
        public EventInteractiveStatus OnInteractiveRelease;
    }
}

