using System.Diagnostics;
using DigitalRubyShared;
using UnityEngine;

namespace ExperimentFramework.Inputs.Actions
{
    public class RotateGestureAction : GestureAction
    {
        private bool needsDistanceThreshold;
        private float startX;
        private float startY;
        private readonly Stopwatch timeBelowSpeedUnitsToRestartThresholdUnits = new Stopwatch();
        
        public RotateGestureAction()
        {
            ThresholdUnits = 0.2f;
        }
        
        public void ProcessTouches(bool resetFocus)
        {
            bool firstFocus = CalculateFocus(Input.touches, resetFocus);
        
            if (firstFocus)
            {
                timeBelowSpeedUnitsToRestartThresholdUnits.Reset();
                timeBelowSpeedUnitsToRestartThresholdUnits.Start();
                if (ThresholdUnits <= 0.0f)
                {
                    // we can start right away, no minimum move threshold
                    needsDistanceThreshold = false;
                    SetState(GestureRecognizerState.Began);
                }
                else
                {
                    needsDistanceThreshold = true;
                    SetState(GestureRecognizerState.Possible);
                }
                startX = FocusX;
                startY = FocusY;
            }
            else if (!needsDistanceThreshold && (State == GestureRecognizerState.Began || State == GestureRecognizerState.Executing))
            {
                if (SpeedUnitsToRestartThresholdUnits > 0.0f && Distance(VelocityX, VelocityY) < SpeedUnitsToRestartThresholdUnits &&
                    (float)timeBelowSpeedUnitsToRestartThresholdUnits.Elapsed.TotalSeconds >= TimeToRestartThresholdUnits)
                {
                    if (!needsDistanceThreshold)
                    {
                        needsDistanceThreshold = true;
                        startX = FocusX;
                        startY = FocusY;
                    }
                }
                else
                {
                    timeBelowSpeedUnitsToRestartThresholdUnits.Reset();
                    timeBelowSpeedUnitsToRestartThresholdUnits.Start();
                    SetState(GestureRecognizerState.Executing);
                }
            }
            else if (TrackedTouchCountIsWithinRange)
            {
                if (needsDistanceThreshold)
                {
                    float distance = Distance(FocusX - startX, FocusY - startY);
                    if (distance >= ThresholdUnits)
                    {
                        needsDistanceThreshold = false;
                        SetState(GestureRecognizerState.Began);
                    }
                    else if (State != GestureRecognizerState.Executing)
                    {
                        SetState(GestureRecognizerState.Possible);
                    }
                }
                else
                {
                    SetState(GestureRecognizerState.Possible);
                }
            }
        }
        
        /// <summary>
        /// How many units away the pan must move to execute - default is 0.2
        /// </summary>
        /// <value>The threshold in units</value>
        public float ThresholdUnits { get; set; }
        
        /// <summary>
        /// The speed in units per second which if the pan gesture drops under, ThresholdUnits will be re-enabled and
        /// the gesture will not send execution events until the threshold units is exceeded again.
        /// Both SpeedUnitsToRestartThresholdUnits and TimeToRestartThresholdUnits conditions must be met to re-enable ThresholdUnits.
        /// 0.1 is a good value.
        /// </summary>
        public float SpeedUnitsToRestartThresholdUnits { get; set; }
        
        /// <summary>
        /// The number of seconds that speed must be below SpeedUnitsToRestartThresholdUnits in order to re-enable ThresholdUnits.
        /// Set to 0 for immediately re-enabling ThresholdUnits if the gesture speed drops below SpeedUnitsToRestartThresholdUnits.
        /// Both SpeedUnitsToRestartThresholdUnits and TimeToRestartThresholdUnits conditions must be met to re-enable ThresholdUnits.
        /// 0.2 is a good value.
        /// </summary>
        public float TimeToRestartThresholdUnits { get; set; }
    }
}