using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Security;
using System.Security.Permissions;

namespace PlanSwitcher
{
    /// <summary>
    /// Logging utilities.
    /// </summary>
    class Logger
    {
        private const string source = "Power Plan Switcher";        

        private static Logger instance = null;
        private bool eventLogInitialized = false;

        private Logger()
        {
            string log = "Application";

            try
            {
                if (!EventLog.SourceExists(source))
                {
                    EventLog.CreateEventSource(source, log);                    
                }
                eventLogInitialized = true;
            }
            catch (SecurityException)
            {
                // No access to the system log.
            }
        }

        public static Logger Instance()
        {
            if (instance == null)
            {
                instance = new Logger();
            }
            return instance;
        }

        public void Info(string message)
        {
            if (eventLogInitialized)
            {
                EventLog.WriteEntry(source, message, EventLogEntryType.Information);
            }
        }

        public void Warn(string message)
        {
            if (eventLogInitialized)
            {
                EventLog.WriteEntry(source, message, EventLogEntryType.Warning);
            }
        }

        public void Err(string message)
        {
            if (eventLogInitialized)
            {
                EventLog.WriteEntry(source, message, EventLogEntryType.Error);
            }
        }
    }
}
