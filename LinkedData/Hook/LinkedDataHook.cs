using LinkedData.DataManagers;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Events.Hooks;
using Sitecore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedData.Hook
{
    public class LinkedDataHook : IHook
    {
        private static AlarmClock _alarmClock;
        private static SitecoreManagerFactory _factory;
        private TimeSpan Interval { get; set; }

        public LinkedDataHook(string interval)
            : base()
        {
            _factory = DependencyResolver.Instance.Resolve<SitecoreManagerFactory>();
            SetInterval(interval);
        }

        /// <summary>
        /// Initializes this instance.
        /// 
        /// </summary>
        public void Initialize()
        {
            if (_alarmClock != null)
                return;

            _alarmClock = new AlarmClock(Interval);
            _alarmClock.Ring += new EventHandler<EventArgs>(AlarmClock_Ring);
        }

        /// <summary>
        /// Handles the Ring event of the AlarmClock control.
        /// 
        /// </summary>
        /// <param name="sender">The source of the event.</param><param name="args">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private static void AlarmClock_Ring(object sender, EventArgs args)
        {
            foreach (var context in _factory.GetAllContexts())
            {
                context.Flush();
            }
        }

        private void SetInterval(string interval)
        {
            Assert.ArgumentNotNullOrEmpty(interval, "interval");
            Interval = TimeSpan.Parse(interval);
        }
    }
}