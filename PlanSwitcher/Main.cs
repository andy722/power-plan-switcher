using InstantiationChecker;
using System;
using System.Windows.Forms;

namespace PlanSwitcher
{
    static class PlanSwitcher
    {
        static void Main()
        {

            if (!SingleInstance.Start()) 
            {
                // Another instance is already started.
                return;
            }

            try
            {
                var applicationContext = new TrayApplicationContext();
                Application.Run(applicationContext);
            }
            catch (Exception ex)
            {
                Logger.Instance().Err(ex.Message);
            }

            SingleInstance.Stop();
        }
    }
}