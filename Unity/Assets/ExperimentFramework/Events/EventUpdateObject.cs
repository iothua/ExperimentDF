using System;
using System.Collections.Generic;
using UnityEngine;

namespace ExperimentFramework.Events
{
    /// <summary>
    /// 抓取物体自定义移动事件
    /// </summary>
    public struct UpdateObjectEventHandler
    {

        public Action<Vector3> action { get; private set; }

        public UpdateObjectEventHandler(Action<Vector3> action)
        {
            this.action = action;
        }
    }

    /// <summary>
    /// 抓取物体自定义移动事件
    /// </summary>
    public static class EventUpdateObject
    {
        private static Dictionary<GameObject, List<UpdateObjectEventHandler>> Values;

        public static void AddListener(GameObject key, Action< Vector3> action)
        {
            if (Values == null)
                Values = new Dictionary<GameObject, List<UpdateObjectEventHandler>>();

            if (IsHandler(key, action)) return;

            if (IsHandlerKey(key))
            {
                List<UpdateObjectEventHandler> values = Values[key];
                values.Add(new UpdateObjectEventHandler( action));

                Values[key] = values;
            }
            else
            {
                List<UpdateObjectEventHandler> values = new List<UpdateObjectEventHandler>();
                values.Add(new UpdateObjectEventHandler( action));

                Values.Add(key, values);
            }
        }

        public static bool IsHandlerKey(GameObject key)
        {
            if (Values == null) return false;
            
            return Values.ContainsKey(key);
        }

        public static bool IsHandler(GameObject key, Action< Vector3> action)
        {
            if (!IsHandlerKey(key)) return false;

            foreach (var item in Values[key])
            {
                if (item.action.Equals(action)) return true;
            }
            return false;
        }

        public static void RemoveListener(GameObject key)
        {
            if (Values == null) return;
            if (!IsHandlerKey(key)) return;

            Values.Remove(key);
        }

        public static void RemoveListenerAll()
        {
            if (Values == null) return;
            Values.Clear();
        }

        public static void RemoveListener(GameObject key, Action< Vector3> action)
        {
            if (Values == null) return;
            if (!IsHandler(key, action)) return;

            List<UpdateObjectEventHandler> values = Values[key];
            Values.Remove(key);
            foreach (var item in values)
            {
                if (item.action.Equals(action))
                {
                    values.Remove(item);
                    
                    return;
                }
            }
            
        }

        public static void SendListener(GameObject key, Vector3 position)
        {
            if (Values == null || !IsHandlerKey(key) || Values[key].Count == 0)
            {
                key.transform.position = position;

                return;
            }

            foreach (var item in Values[key])
            {
                item.action(position);
            }

            for (int i = 0; i < Values[key].Count; i++)
            {
                Values[key][i].action( position);
            }
        }

        public static void AddUpdateObject(this GameObject key, Action< Vector3> action)
        {
            AddListener(key, action);
        }

        public static void RemoveUpdateObject(this GameObject key, Action< Vector3> action)
        {
            RemoveListener(key, action);
        }
    }
}
