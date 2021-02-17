using System;
using System.Collections.Generic;
using DigitalRubyShared;
using UnityEngine;

namespace ExperimentFramework.Inputs.Actions
{
    /// <summary>
    /// Tracks and calculates velocity for gestures
    /// </summary>
    public class GestureVelocityTracker
    {
        private struct VelocityHistory
        {
            public float VelocityX;
            public float VelocityY;
            public float Seconds;
        }

        private const int maxHistory = 8;

        private readonly System.Collections.Generic.Queue<VelocityHistory> history = new System.Collections.Generic.Queue<VelocityHistory>();
        private readonly System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
        private float previousX;
        private float previousY;

        private void AddItem(float velocityX, float velocityY, float elapsed)
        {
            VelocityHistory item = new VelocityHistory
            {
                VelocityX = velocityX,
                VelocityY = velocityY,
                Seconds = elapsed
            };
            history.Enqueue(item);
            if (history.Count > maxHistory)
            {
                history.Dequeue();
            }
            float totalSeconds = 0.0f;
            VelocityX = VelocityY = 0.0f;
            foreach (VelocityHistory h in history)
            {
                totalSeconds += h.Seconds;
            }
            foreach (VelocityHistory h in history)
            {
                float weight = h.Seconds / totalSeconds;
                VelocityX += (h.VelocityX * weight);
                VelocityY += (h.VelocityY * weight);
            }
            timer.Reset();
            timer.Start();
        }

        /// <summary>
        /// Reset velocity state
        /// </summary>
        public void Reset()
        {
            timer.Reset();
            VelocityX = VelocityY = 0.0f;
            history.Clear();
        }

        /// <summary>
        /// Reset and set previouos pos to nil
        /// </summary>
        public void Restart()
        {
            Restart(float.MinValue, float.MinValue);
        }

        /// <summary>
        /// Reset and set previous position
        /// </summary>
        /// <param name="previousX">Previous x pos</param>
        /// <param name="previousY">Previous y pos</param>
        public void Restart(float previousX, float previousY)
        {
            this.previousX = previousX;
            this.previousY = previousY;
            Reset();
            timer.Start();
        }

        /// <summary>
        /// Update velocity state
        /// </summary>
        /// <param name="x">Next x pos</param>
        /// <param name="y">Next y pos</param>
        public void Update(float x, float y)
        {
            float elapsed = ElapsedSeconds;
            if (previousX != float.MinValue)
            {
                float px = previousX;
                float py = previousY;
                float velocityX = (x - px) / elapsed;
                float velocityY = (y - py) / elapsed;
                AddItem(velocityX, velocityY, elapsed);
            }
            previousX = x;
            previousY = y;
        }

        /// <summary>
        /// Elapsed seconds
        /// </summary>
        public float ElapsedSeconds { get { return (float)timer.Elapsed.TotalSeconds; } }

        /// <summary>
        /// Current x velocity
        /// </summary>
        public float VelocityX { get; private set; }

        /// <summary>
        /// Current y velocity
        /// </summary>
        public float VelocityY { get; private set; }

        /// <summary>
        /// Current speed
        /// </summary>
        public float Speed { get { return (float)Math.Sqrt(VelocityX * VelocityX + VelocityY * VelocityY); } }
    }
    
    public delegate void GestureActionStateUpdatedDelegate(GestureAction gesture);
    
    public class GestureAction
    {
        private int minimumNumberOfTouchesToTrack = 1;
        private int maximumNumberOfTouchesToTrack = 1;
        private readonly GestureVelocityTracker velocityTracker = new GestureVelocityTracker();
        
        private GestureRecognizerState state = GestureRecognizerState.Possible;
        
        /// <summary>
        /// Fires when state is updated, use this instead of Updated, which has been deprecated.
        /// The gesture object has a CurrentTrackedTouches property where you can access the current touches.
        /// </summary>
        public event GestureActionStateUpdatedDelegate StateUpdated;
        
        /// <summary>
        /// Previous focus x
        /// </summary>
        protected float PrevFocusX { get; private set; }

        /// <summary>
        /// Previous focus y
        /// </summary>
        protected float PrevFocusY { get; private set; }
        
         /// <summary>
        /// Focus x value in pixels (average of all touches)
        /// </summary>
        /// <value>The focus x.</value>
        public float FocusX { get; private set; }

        /// <summary>
        /// Focus y value in pixels (average of all touches)
        /// </summary>
        /// <value>The focus y.</value>
        public float FocusY { get; private set; }

        /// <summary>
        /// Start focus x value in pixels (average of all touches)
        /// </summary>
        /// <value>The focus x.</value>
        public float StartFocusX { get; private set; }

        /// <summary>
        /// Start focus y value in pixels (average of all touches)
        /// </summary>
        /// <value>The focus y.</value>
        public float StartFocusY { get; private set; }

        /// <summary>
        /// Change in focus x in pixels
        /// </summary>
        /// <value>The change in x</value>
        public float DeltaX { get; private set; }

        /// <summary>
        /// Change in focus y in pixels
        /// </summary>
        /// <value>The change in y</value>
        public float DeltaY { get; private set; }

        /// <summary>
        /// The distance (in pixels) the gesture focus has moved from where it started along the x axis
        /// </summary>
        public float DistanceX { get; private set; }

        /// <summary>
        /// The distance (in pixels) the gesture focus has moved from where it started along the y axis
        /// </summary>
        public float DistanceY { get; private set; }

        /// <summary>
        /// Velocity x in pixels using focus
        /// </summary>
        /// <value>The velocity x value in pixels</value>
        public float VelocityX { get { return velocityTracker.VelocityX; } }

        /// <summary>
        /// Velocity y in pixels using focus
        /// </summary>
        /// <value>The velocity y value in pixels</value>
        public float VelocityY { get { return velocityTracker.VelocityY; } }

        /// <summary>
        /// The speed of the gesture in pixels using focus
        /// </summary>
        /// <value>The speed of the gesture</value>
        public float Speed { get { return velocityTracker.Speed; } }
        
        /// <summary>
        /// Average pressure of all tracked touches
        /// </summary>
        public float Pressure { get; private set; }

        /// <summary>
        /// A platform specific view object that this gesture can execute in, null if none
        /// </summary>
        /// <value>The platform specific view this gesture can execute in</value>
        public object PlatformSpecificView { get; set; }

        /// <summary>
        /// The platform specific view scale (default is 1.0). Change this if the view this gesture is attached to is being scaled.
        /// </summary>
        /// <value>The platform specific view scale</value>
        public float PlatformSpecificViewScale { get; set; }
        
        /// <summary>
        /// Determines whether two points are within a specified distance
        /// </summary>
        /// <returns>True if within distance false otherwise</returns>
        /// <param name="x1">The first x value in pixels.</param>
        /// <param name="y1">The first y value in pixels.</param>
        /// <param name="x2">The second x value in pixels.</param>
        /// <param name="y2">The second y value in pixels.</param>
        /// <param name="d">Distance in units</param>
        public bool PointsAreWithinDistance(float x1, float y1, float x2, float y2, float d)
        {
            return (DistanceBetweenPoints(x1, y1, x2, y2) <= d);
        }

        /// <summary>
        /// Gets the distance between two points, in units
        /// </summary>
        /// <returns>The distance between the two points in units.</returns>
        /// <param name="x1">The first x value in pixels.</param>
        /// <param name="y1">The first y value in pixels.</param>
        /// <param name="x2">The second x value in pixels.</param>
        /// <param name="y2">The second y value in pixels.</param>
        public float DistanceBetweenPoints(float x1, float y1, float x2, float y2)
        {
            float a = (float)(x2 - x1);
            float b = (float)(y2 - y1);
            float d = (float)Math.Sqrt(a * a + b * b) * PlatformSpecificViewScale;
            return DeviceInfo.PixelsToUnits(d);
        }

        /// <summary>
        /// Gets the distance of a vector, in units
        /// </summary>
        /// <param name="xVector">X vector</param>
        /// <param name="yVector">Y vector</param>
        /// <returns>The distance of the vector in units.</returns>
        public float Distance(float xVector, float yVector)
        {
            float d = (float)Math.Sqrt(xVector * xVector + yVector * yVector) * PlatformSpecificViewScale;
            return DeviceInfo.PixelsToUnits(d);
        }

        /// <summary>
        /// Get the distance of a length, in units
        /// </summary>
        /// <param name="length">Length</param>
        /// <returns>Distance in units</returns>
        public float Distance(float length)
        {
            float d = Math.Abs(length) * PlatformSpecificViewScale;
            return DeviceInfo.PixelsToUnits(d);
        }
        
        /// <summary>
        /// The minimum number of touches to track. This gesture will not start unless this many touches are tracked. Default is usually 1 or 2.
        /// Not all gestures will honor values higher than 1.
        /// </summary>
        public int MinimumNumberOfTouchesToTrack
        {
            get { return minimumNumberOfTouchesToTrack; }
            set
            {
                minimumNumberOfTouchesToTrack = (value < 1 ? 1 : value);
                if (minimumNumberOfTouchesToTrack > maximumNumberOfTouchesToTrack)
                {
                    maximumNumberOfTouchesToTrack = minimumNumberOfTouchesToTrack;
                }
            }
        }

        /// <summary>
        /// The maximum number of touches to track. This gesture will never track more touches than this. Default is usually 1 or 2.
        /// Not all gestures will honor values higher than 1.
        /// </summary>
        public int MaximumNumberOfTouchesToTrack
        {
            get { return maximumNumberOfTouchesToTrack; }
            set
            {
                maximumNumberOfTouchesToTrack = (value < 1 ? 1 : value);
                if (maximumNumberOfTouchesToTrack < minimumNumberOfTouchesToTrack)
                {
                    minimumNumberOfTouchesToTrack = maximumNumberOfTouchesToTrack;
                }
            }
        }
        
        /// <summary>
        /// Whether the current number of tracked touches is within the min and max number of touches to track
        /// </summary>
        public bool TrackedTouchCountIsWithinRange
        {
            get { return Input.touchCount >= minimumNumberOfTouchesToTrack && Input.touchCount <= maximumNumberOfTouchesToTrack; }
        }
        
        /// <summary>
        /// Get the current gesture recognizer state
        /// </summary>
        /// <value>Gesture recognizer state</value>
        public GestureRecognizerState State { get { return state; } }
        
        /// <summary>
        /// Calculate the focus of the gesture
        /// </summary>
        /// <param name="touches">Touches</param>
        /// <returns>True if this was the first focus calculation, false otherwise</returns>
        protected bool CalculateFocus(Touch[] touches)
        {
            return CalculateFocus(touches, false);
        }

        /// <summary>
        /// Calculate the focus of the gesture
        /// </summary>
        /// <param name="touches">Touches</param>
        /// <param name="resetFocus">True to force reset of the start focus, false otherwise</param>
        /// <returns>True if this was the first focus calculation, false otherwise</returns>
        protected bool CalculateFocus(Touch[] touches, bool resetFocus)
        {
            bool first = resetFocus || (StartFocusX == float.MinValue || StartFocusY == float.MinValue);

            FocusX = 0.0f;
            FocusY = 0.0f;
            Pressure = 0.0f;

            foreach (Touch t in touches)
            {
                FocusX += t.position.x;
                FocusY += t.position.y;
                Pressure += t.pressure;
            }

            float invTouchCount = 1.0f / (float)touches.Length;
            FocusX *= invTouchCount;
            FocusY *= invTouchCount;
            Pressure *= invTouchCount;

            if (first)
            {
                StartFocusX = FocusX;
                StartFocusY = FocusY;
                DeltaX = 0.0f;
                DeltaY = 0.0f;
                velocityTracker.Restart();
            }
            else
            {
                DeltaX = FocusX - PrevFocusX;
                DeltaY = FocusY - PrevFocusY;
            }

            velocityTracker.Update(FocusX, FocusY);

            DistanceX = FocusX - StartFocusX;
            DistanceY = FocusY - StartFocusY;

            PrevFocusX = FocusX;
            PrevFocusY = FocusY;

            return first;
        }
        
        /// <summary>
        /// Sets the state of the gesture. Continous gestures should set the executing state every time they change.
        /// </summary>
        /// <param name="value">True if state set successfully, false if the gesture was forced to fail or the state is pending a require gesture recognizer to fail state change</param>
        protected bool SetState(GestureRecognizerState value)
        {
            state = value;
            StateChanged();
            
            return true;
        }
        
        /// <summary>
        /// Called when state changes
        /// </summary>
        protected virtual void StateChanged()
        {
            // TODO: Remove Updated property
#pragma warning disable 0618

#pragma warning restore 0618

            if (StateUpdated != null)
            {
                StateUpdated(this);
            }
        }
        
        
        
        public virtual void TouchBegan()
        {
            
        }
        
        public virtual void TouchMoved()
        {
            
        }

        public virtual void TouchEnd()
        {
            if (State == GestureRecognizerState.Executing)
            {
                SetState(GestureRecognizerState.Ended);
            }
        }
    }
}