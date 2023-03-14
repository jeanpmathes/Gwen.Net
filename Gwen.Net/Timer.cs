using System;
using System.Collections.Generic;
using Gwen.Net.Platform;

namespace Gwen.Net
{
    public delegate void ElapsedEventHandler(object sender, EventArgs args);

    /// <summary>
    ///     Render based timer.
    /// </summary>
    /// <remarks>
    ///     This is not very accurate timer because it depends on render events.
    /// </remarks>
    public class Timer : IDisposable
    {
        private static readonly List<Timer> timers = new();
        private static float lastTime;
        private static bool started;
        private bool enabled;

        private int timerValue;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Timer" /> class.
        /// </summary>
        public Timer()
        {
            Interval = 0;
            IsOneTime = false;
            enabled = false;

            timers.Add(this);
        }

        /// <summary>
        ///     Timer interval in milliseconds.
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        ///     If true, timer is disabled when timeout occurs.
        /// </summary>
        public bool IsOneTime { get; set; }

        /// <summary>
        ///     Is timer enabled.
        /// </summary>
        public bool IsEnabled
        {
            get => enabled;
            set
            {
                if (value)
                {
                    Start();
                }
                else
                {
                    Stop();
                }
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Invoked when the timeout occurs.
        /// </summary>
        public event ElapsedEventHandler Elapsed;

        /// <summary>
        ///     Start the timer.
        /// </summary>
        public void Start()
        {
            enabled = true;
            timerValue = Interval;
        }

        /// <summary>
        ///     Stop the timer.
        /// </summary>
        public void Stop()
        {
            enabled = false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                timers.Remove(this);
            }
        }

        internal static void Tick()
        {
            if (timers.Count == 0)
            {
                return;
            }

            float currentTime = GwenPlatform.GetTimeInSeconds();

            if (!started)
            {
                lastTime = currentTime;
                started = true;

                return;
            }

            var diff = (int) ((currentTime - lastTime) * 1000.0f);

            foreach (Timer timer in timers)
            {
                if (timer.enabled)
                {
                    timer.timerValue -= diff;

                    if (timer.timerValue <= 0)
                    {
                        if (timer.Elapsed != null)
                        {
                            timer.Elapsed(timer, EventArgs.Empty);
                        }

                        if (!timer.IsOneTime)
                        {
                            timer.timerValue = timer.Interval + timer.timerValue;
                        }
                        else
                        {
                            timer.enabled = false;
                        }
                    }
                }
            }

            lastTime = currentTime;
        }
    }
}
