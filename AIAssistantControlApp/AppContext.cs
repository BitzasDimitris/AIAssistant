using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AIAssistantControlApp
{
    class AppContext: ApplicationContext
    {
        private SystemTray _trayIcon;
        public AppContext()
        {

            _trayIcon = new SystemTray();
            
            _trayIcon.ContextMenuStrip = SetupContextMenuStrip();

            SetupIcons();
            _trayIcon.Animate = true;
            _trayIcon.Visible = true;
            _trayIcon.StartAnimation();
        }

        void ShowConfig(object sender, EventArgs e)
        {
//            // If we are already showing the window, merely focus it.
//            if (configWindow.Visible)
//            {
//                configWindow.Activate();
//            }
//            else
//            {
//                configWindow.ShowDialog();
//            }
        }

        void SetState(object sender, EventArgs args)
        {
            if (sender is ToolStripItem tsi)
            {
                if (tsi.Tag is ServiceState state)
                {
                    SetState(state);
                }
            }
        }

        private void SetState(ServiceState state)
        {
            _trayIcon.SetActiveSet(state);
        }

        void Exit(object sender, EventArgs e)
        {
            // We must manually tidy up and remove the icon before we exit.
            // Otherwise it will be left behind until the user mouses over.
            _trayIcon.Visible = false;
            Application.Exit();
        }

        private ContextMenuStrip SetupContextMenuStrip()
        {
            var menuStrip = new ContextMenuStrip();
            var stateGroupItem = new ToolStripMenuItem("Set State");
            var stateGroup = new ContextMenuStrip();
            stateGroup.Items.Add(new ToolStripButton("Normal", null, SetState) {Tag = ServiceState.Normal});
            stateGroup.Items.Add(new ToolStripButton("Interaction", null, SetState) {Tag = ServiceState.Interaction});
            stateGroup.Items.Add(new ToolStripButton("GoodSignal", null, SetState) {Tag = ServiceState.GoodSignal});
            stateGroup.Items.Add(new ToolStripButton("BadSignal", null, SetState) {Tag = ServiceState.BadSignal});
            stateGroupItem.DropDown = stateGroup;

            menuStrip.Items.Add("Configuration", null, ShowConfig);
            menuStrip.Items.Add(stateGroupItem);
            menuStrip.Items.Add("Exit", null, Exit);
            return menuStrip;
        }

        private void SetupIcons()
        {
            var iconSets = new Dictionary<ServiceState, string>();
            foreach (var val in Enum.GetValues(typeof(ServiceState)))
            {
                if(val is ServiceState state)
                    iconSets.Add(state, Enum.GetName(typeof(ServiceState), val));
            }
            _trayIcon.SetIconSets(iconSets);
        }
    }
}
