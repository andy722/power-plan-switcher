using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Reflection;
using System.Collections.Generic;

namespace PlanSwitcher
{

    public interface UICallback
    {
        void UpdateBatteryState();
    }

    public class TrayApplicationContext : ApplicationContext, UICallback
    {
        /// <summary>
        /// Power state update interval, milliseconds.
        /// </summary>
        private const int TimerInterval = 5 * 1000;

        /// <summary>
        /// The list of components to be disposed on context disposal.
        /// </summary>
        private System.ComponentModel.IContainer components = new System.ComponentModel.Container();

        private NotifyIcon notifyIcon;

        private IPowerManager powerManager;

        private List<PowerPlan> plans;

        private Timer refreshTimer;

        public TrayApplicationContext() 
		{
			InitializeContext();

            refreshTimer = new System.Windows.Forms.Timer(components);
            refreshTimer.Interval = TimerInterval;
            refreshTimer.Tick += new System.EventHandler(TimerHandler);
            refreshTimer.Enabled = true;

            powerManager = PowerManagerProvider.CreatePowerManager(this);
            plans = powerManager.GetPlans();

            AddMenuItems();
            UpdateBatteryState();
		}

        private void InitializeContext()
        {
            notifyIcon = new NotifyIcon(components)
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = null,
                Text = "Power Plan manager",
                Visible = true
            };
            notifyIcon.ContextMenuStrip.Opening += OnContextMenuStripOpening;
            notifyIcon.MouseUp += OnNotifyIconMouseUp;
        }

        private void TimerHandler(object sender, EventArgs e)
        {
            UpdateBatteryState();
        }

        public void UpdateBatteryState()
        {
            PowerPlan currentPlan = powerManager.GetCurrentPlan();
            bool isCharging = powerManager.IsCharging();
            int percentValue = powerManager.GetChargeValue();

            UpdateBatteryState(currentPlan.name, isCharging, percentValue);
        }

        private void UpdateBatteryState(string planName, bool isCharging, int percentValue)
        {
            Icon icon;
            if (percentValue >= 86)
            {
                icon = isCharging ? Properties.Resources.batt_ch_4 : Properties.Resources.batt_4;
            }
            else if (percentValue >= 62)
            {
                icon = isCharging ? Properties.Resources.batt_ch_3 : Properties.Resources.batt_3;
            }
            else if (percentValue >= 38)
            {
                icon = isCharging ? Properties.Resources.batt_ch_2 : Properties.Resources.batt_2;
            }
            else if (percentValue >= 14)
            {
                icon = isCharging ? Properties.Resources.batt_ch_1 : Properties.Resources.batt_1;
            }
            else
            {
                icon = isCharging ? Properties.Resources.batt_ch_0 : Properties.Resources.batt_0;
            }

            notifyIcon.Icon = icon;
            notifyIcon.Text = planName + " (" + percentValue + "%)";
        }

        private void AddMenuItems()
        {
            // Add an item for each power plan.
            PowerPlan currentPlan = powerManager.GetCurrentPlan();
            foreach (PowerPlan p in plans)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(p.name);
                PowerPlan pp = p;
                item.Click += delegate(object sender, EventArgs args)
                {
                    powerManager.SetActive(pp);
                };
                item.Checked = (currentPlan == p);
                notifyIcon.ContextMenuStrip.Items.Add(item);
            }

            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());

            // Utility items.
            {
                var item = new ToolStripMenuItem("Open control panel");
                item.Click += OpenControlPanelClick;
                notifyIcon.ContextMenuStrip.Items.Add(item);
            }

            {
                var item = new ToolStripMenuItem("Exit");
                item.Click += ExitClick;
                notifyIcon.ContextMenuStrip.Items.Add(item);
            }
        }

        #region Interaction handlers

        private void OnContextMenuStripOpening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            int idx = plans.IndexOf(powerManager.GetCurrentPlan());

            foreach (ToolStripItem item in notifyIcon.ContextMenuStrip.Items) 
            {
                if (item is ToolStripMenuItem)
                {
                    ((ToolStripMenuItem) item).Checked = false;
                }
            }

            ((ToolStripMenuItem) notifyIcon.ContextMenuStrip.Items[idx]).Checked = true;
        }

        private void OpenControlPanelClick(object sender, EventArgs e)
        {
            powerManager.OpenControlPanel();
        }

        private void OnNotifyIconMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(notifyIcon, null);
            }
        }

        private void ExitClick(object sender, EventArgs e)
        {
            ExitThread();
        }

        #endregion

        protected override void Dispose( bool disposing )
		{
			if (disposing && (components != null)) 
            {
                components.Dispose(); 
            }
		}

        protected override void ExitThreadCore()
        {
            notifyIcon.Visible = false;
            base.ExitThreadCore();
        }
    }
}
