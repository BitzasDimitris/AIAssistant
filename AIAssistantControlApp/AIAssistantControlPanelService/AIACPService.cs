using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using AIAssistantControlApp.AIAssistantControlPanelService;

namespace AIAssistantControlApp
{
    public class AIACPService
    {
        private AppServiceConnection _appServiceConnection;

        private AppServiceConnectionStatus _status;

        public delegate void StatusUpdateHandler(object sender, AIACPConnectionEventArgs e);
        public event StatusUpdateHandler OnServiceConnectionSuccess;
        public event StatusUpdateHandler OnServiceConnectionError;

        public AIACPService()
        {
            _appServiceConnection = new AppServiceConnection();
            _appServiceConnection.AppServiceName = "com.digicell.communicationService";
            _appServiceConnection.PackageFamilyName = "67b2eb07-74c3-47df-81e1-295a9608d586_nwavxkhm73mjy";
        }


        private async void SendServiceRequest(Action action)
        {
            _status = await _appServiceConnection.OpenAsync();
            if (_status != AppServiceConnectionStatus.Success)
            {

                OnServiceConnectionError?.Invoke(this,
                    new AIACPConnectionEventArgs(Enum.GetName(typeof(AppServiceConnectionStatus), _status)));
                return;
            }
            OnServiceConnectionSuccess?.Invoke(this, 
                new AIACPConnectionEventArgs(Enum.GetName(typeof(AppServiceConnectionStatus), _status)));
        }

        public void Launch()
        {
            //_appServiceConnection?.SendMessageAsync();
        }
    }
}
