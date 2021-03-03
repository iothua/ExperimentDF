using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace Laboratory
{
    public class Timer : MonoBehaviour
    {
        public Transform h;
        public Transform m;
        public Transform s;
        public int hour;
        public int min;
        public int sec;
        void Start()
        {
            hour = DateTime.Now.Hour;
            min = DateTime.Now.Minute;
            sec = DateTime.Now.Second;
            InvokeRepeating("SetTimer", 0, 1);
        }

        // Update is called once per frame
        void SetTimer()
        {
            sec += 1;
            min += sec / 60;
            hour += min / 60;
            sec %= 60;
            min %= 60;
            hour %= 24;

            h.localEulerAngles = new Vector3(0, -30 * hour - 0.5f * min, 0);
            m.localEulerAngles = new Vector3(0, -6 * min, 0);
            s.localEulerAngles = new Vector3(0, -6 * sec, 0);

        }

        private void OnDestroy()
        {
            CancelInvoke();
        }
    }
}
