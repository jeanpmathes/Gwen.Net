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
        private static readonly List<Timer> m_Timers = new();
        private static float m_LastTime;
        private static bool m_Started;
        private bool m_Enabled;

        private int m_TimerValue;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Timer" /> class.
        /// </summary>
        public Timer()
        {
            Interval = 0;
            IsOneTime = false;
            m_Enabled = false;

            m_Timers.Add(this);
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
            get => m_Enabled;
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
            m_Enabled = true;
            m_TimerValue = Interval;
        }

        /// <summary>
        ///     Stop the timer.
        /// </summary>
        public void Stop()
        {
            m_Enabled = false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_Timers.Remove(this);
            }
        }

        internal static void Tick()
        {
            if (m_Timers.Count == 0)
            {
                return;
            }

            float currentTime = GwenPlatform.GetTimeInSeconds();

            if (!m_Started)
            {
                m_LastTime = currentTime;
                m_Started = true;

                return;
            }

            var diff = (int) ((currentTime - m_LastTime) * 1000.0f);

            foreach (Timer timer in m_Timers)
            {
                if (timer.m_Enabled)
                {
                    timer.m_TimerValue -= diff;

                    if (timer.m_TimerValue <= 0)
                    {
                        if (timer.Elapsed != null)
                        {
                            timer.Elapsed(timer, EventArgs.Empty);
                        }

                        if (!timer.IsOneTime)
                        {
                            timer.m_TimerValue = timer.Interval + timer.m_TimerValue;
                        }
                        else
                        {
                            timer.m_Enabled = false;
                        }
                    }
                }
            }

            m_LastTime = currentTime;
        }
    }
}
