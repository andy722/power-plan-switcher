using System;
using System.Threading;
using System.Reflection;
using System.Runtime.InteropServices;

namespace InstantiationChecker
{
    /// <summary>
    /// Utilities for restricting application to a single instance.
    /// </summary>
    public static class SingleInstance
    {
        private static Mutex mutex;

        /// <summary>
        /// Returns true iff this instance is the only one running.
        /// </summary>
        public static bool Start()
        {
            bool onlyInstance = false;

            string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;

            // Note: using local mutex, so multiple instantiations are still 
            // possible across different sessions.
            string mutexName = String.Format("Local\\{0}", assemblyName);

            mutex = new Mutex(true, mutexName, out onlyInstance);
            return onlyInstance;
        }

        /// <summary>
        /// Marks the current instance as not running.
        /// </summary>
        static public void Stop()
        {
            mutex.ReleaseMutex();
        }

    }
}
