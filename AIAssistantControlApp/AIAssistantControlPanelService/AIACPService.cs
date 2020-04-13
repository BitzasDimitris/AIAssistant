using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Collections;
using AIAssistantControlApp.AIAssistantControlPanelService;
using AIAssistantControlApp.MIOMService;

namespace AIAssistantControlApp
{
    public class AIACPService
    {
        private AppServiceConnection _connection = null;


        private AppContext _appContext;

        public delegate void StatusUpdateHandler(object sender, AIACPConnectionEventArgs e);
        public event StatusUpdateHandler OnServiceConnectionSuccess;
        public event StatusUpdateHandler OnServiceConnectionError;

        public AIACPService(AppContext appContext)
        {
            _appContext = appContext;

        }

        public async void Launch()
        {
            SendRequest(new ValueSet(){{"Launch", "par"}}, null);
//            IEnumerable<AppListEntry> appListEntries = await Package.Current.GetAppListEntriesAsync();
//            var appEntry = appListEntries.Where(entry => entry.DisplayInfo.DisplayName.Contains("Panel"))?.First()??
//                appListEntries.First();
//            await appEntry.LaunchAsync();
        }

        private async Task<bool> InitializeAppServiceConnection()
        {
            _connection = new AppServiceConnection();
            _connection.AppServiceName = "AIAssistantControlPanelService";
            _connection.PackageFamilyName = Package.Current.Id.FamilyName;
            _connection.RequestReceived += OnRequestReceived;
            _connection.ServiceClosed += OnServiceClosed;

            AppServiceConnectionStatus status = await _connection.OpenAsync();
            if (status != AppServiceConnectionStatus.Success)
            {
                _connection = null;
                return false;
            }

            return true;
        }

        private void OnServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            _connection = null;
        }


        public async void SendRequest(ValueSet valueSet, Action<AppServiceResponse> callBack)
        {
            if (_connection == null)
            {
                var connected = await InitializeAppServiceConnection();
                if (connected)
                {
                    var result = await _connection.SendMessageAsync(valueSet);
                    callBack?.Invoke(result);
                }
            }
            else
            {
                var result = await _connection.SendMessageAsync(valueSet);
                callBack?.Invoke(result);
            }
            
        }

        private void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var request = args.Request.Message;
            var response = new ValueSet();
            switch (request["Command"])
            {
                case "State":
                    response.Add("State", _appContext.ServiceState);
                    break;
            }
        }
    }
}
