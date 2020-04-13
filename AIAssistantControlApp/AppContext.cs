using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MultimediaIOManagerService.Enums;

namespace AIAssistantControlApp
{
    class AppContext: ApplicationContext
    {
        private SystemTray _trayIcon;
        private AIACPService _controlPanelService;

        private WindowsServices.MIOMService _miomService;

        private MIOMServiceState _serviceState;

        private ToolStripLabel _serviceStateLabel = new ToolStripLabel("State:");

        public AppContext()
        {

            _trayIcon = new SystemTray();
            
            _trayIcon.ContextMenuStrip = SetupContextMenuStrip();

            SetupIcons();
            _trayIcon.Animate = true;
            _trayIcon.Visible = true;
            _trayIcon.StartAnimation();

            _controlPanelService = new AIACPService();

            _controlPanelService.OnServiceConnectionSuccess += ControlPanelServiceConnectionSuccess;
            _controlPanelService.OnServiceConnectionError += ControlPanelServiceConnectionError;

            _miomService = new WindowsServices.MIOMService();
            Task.Delay(1000).ContinueWith(t => _miomService.Start());
            var timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += (sender, args) =>
            {
                _serviceState = _miomService.GetState();
                _serviceStateLabel.Text = $"State: {Enum.GetName(_serviceState.GetType(), _serviceState)}";
                _trayIcon.ContextMenuStrip.Update();
                Debug.WriteLine($"State: {_serviceState}");
            };
//            timer.Tick += async (sender, args) =>
//            {
//                _serviceState = await _miomService.GetStateAsync();
//                _serviceStateLabel.Text = $"State: {Enum.GetName(_serviceState.GetType(), _serviceState)}";
//                _trayIcon.ContextMenuStrip.Update();
//                Debug.WriteLine($"State: {_serviceState}");
//            };
            timer.Start();

            Application.ApplicationExit += HideTrayIcon;
            Application.ThreadException += HideTrayIcon;
            Application.ThreadExit += HideTrayIcon;
        }

        private void ControlPanelServiceConnectionSuccess(object sender, EventArgs e)
        {
//            SetState(ServiceState.GoodSignal);
//            Task.Delay(3000).ContinueWith((a) => SetState(ServiceState.Normal));
        }

        private void ControlPanelServiceConnectionError(object sender, EventArgs e)
        {
//            SetState(ServiceState.BadSignal);
        }

        private void HideTrayIcon(object sender, EventArgs e)
        {
            _trayIcon.Visible = false;
        }


        void ShowConfig(object sender, EventArgs e)
        {
            _controlPanelService.Launch();
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
            _miomService.Stop();
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
            //menuStrip.Items.Add(stateGroupItem);
            menuStrip.Items.Add(_serviceStateLabel);
            menuStrip.Items.Add("Exit", null, Exit);
            //Add status text
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
