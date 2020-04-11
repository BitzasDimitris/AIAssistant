using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;

namespace AIAssistantControlApp
{
    public class AIAssistantControlPanelService
    {
        private AppServiceConnection _appServiceConnection;

        public delegate void StatusUpdateHandler(object sender, EventArgs e);
        public event StatusUpdateHandler OnServiceConnectionSuccess;
        public event StatusUpdateHandler OnServiceConnectionError;

        public AIAssistantControlPanelService()
        {
            _appServiceConnection = new AppServiceConnection();
            _appServiceConnection.AppServiceName = "com.digicell.communicationService";
            _appServiceConnection.PackageFamilyName = "67b2eb07-74c3-47df-81e1-295a9608d586_nwavxkhm73mjy";
        }

        public void TryToConnect()
        {
            InitializeServiceConnection();
        }

        private async void InitializeServiceConnection()
        {
            var status = await _appServiceConnection.OpenAsync();
            if (status != AppServiceConnectionStatus.Success)
            {
                Debug.WriteLine(Enum.GetName(typeof(AppServiceConnectionStatus), status));
                OnServiceConnectionError?.Invoke(this, null);
                return;
            }
            OnServiceConnectionSuccess?.Invoke(this, null);
        }
    }
}
