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

            CheckAutostart();

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

        private static void CheckAutostart()
        {
            string codeBase = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
            string executablePath = new Uri(codeBase).LocalPath;

            string currentDirectory = System.IO.Path.GetDirectoryName(executablePath);

            // Copy self to Startup folder if not started from there.
            string startupDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            if (currentDirectory != startupDirectory)
            {                
                string executableName = System.IO.Path.GetFileName(executablePath);
                System.IO.File.Copy(executablePath, startupDirectory + "/" + executableName, true);
            }
        }
    }
}