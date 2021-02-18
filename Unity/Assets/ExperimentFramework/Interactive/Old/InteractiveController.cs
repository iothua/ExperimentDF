using System;
using UnityEngine;

namespace ExperimentFramework.Interactive
{
    /// <summary>
    /// 交互控制端
    /// 1、不能读文件了
    /// 2、所有的距离检测点，在Awake初始化的时候，添加到集合点中
    /// </summary>
    [ExecuteInEditMode]
    public class InteractiveController : MonoBehaviour,IDisposable
    {
        private bool isEnable = false;

        //private Coroutine coroutineUpdate; //每帧遍历

        //距离检测时，D2D与屏幕坐标的系数
        public static int screenInteractiveCoefficient => Screen.height;
        
        public InteractiveSearch Search { get; private set; }

        //private WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);

        private float currentTime = 0;

        private Vector3 lampRotate;
        
        private void Awake()
        {
            Search = new InteractiveSearch();

            if (Application.isPlaying)
            {
                var context = Loxodon.Framework.Contexts.Context.GetApplicationContext();
                if (context != null)
                {
                    context.GetContainer().Register(this);
                }
            }

        }

        private void OnDestroy()
        {
            if (Application.isPlaying)
            {
                var context = Loxodon.Framework.Contexts.Context.GetApplicationContext();
                if (context != null)
                {
                    context.GetContainer().Unregister<InteractiveController>();
                }
                OnInteractiveDisable();

            }
        }

        private void Start()
        {
            IsEnable = true;
        }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsEnable
        {
            get { return isEnable; }
            set
            {

                if (isEnable == value) return;

                isEnable = value;

                if (isEnable)
                    OnInteractiveEnable();
                else
                    OnInteractiveDisable();
            }
        }

        /// <summary>
        /// 激活
        /// </summary>
        public void OnInteractiveEnable()
        {
            //TODO
            //MessageDispatcher.AddListener(EventMessage._GRAB_OBJECT, OnGrabObject);
            //MessageDispatcher.AddListener(EventMessage._RELEASE_OBJECT, OnReleaseObject);

        }


        /// <summary>
        /// 隐藏
        /// </summary>
        public void OnInteractiveDisable()
        {
            //TODO

            //MessageDispatcher.RemoveListener(EventMessage._GRAB_OBJECT, OnGrabObject);
            //MessageDispatcher.RemoveListener(EventMessage._RELEASE_OBJECT, OnReleaseObject);
            Search.dataManagers.Clear();
           
        }

        //void OnGrabObject(IMessage message)
        //{

        //    GameObject targetObject = message.Data as GameObject;

        //    if (targetObject != null)
        //    {
        //        Search.StartInteraction(targetObject, true, 0);
        //    }
        //}

        //void OnReleaseObject(IMessage message)
        //{
        //    GameObject targetObject = message.Data as GameObject;

        //    if (targetObject != null)
        //    {
        //        Search.StopInteraction(targetObject);
        //    }
        //}

        private void Update()
        {
            if (!Application.isPlaying) return;
            currentTime += Time.deltaTime;

            if (currentTime >= 0.1f)
            {
                currentTime = 0;
                Search.OnUpdate();
            }

            //var rotate = Camera.main.transform.eulerAngles;
            //if (!rotate.Equals(lampRotate))
            //{
            //    lampRotate = rotate;
            //    InteractionUtility.InteractionCamera.transform.eulerAngles = rotate;
            //}

        }

        public void Dispose()
        {
            
        }
    }
}

