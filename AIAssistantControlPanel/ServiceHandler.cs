using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;

namespace AIAssistantControlPanel
{
    public class ServiceHandler
    {

        private AppServiceConnection _appServiceConnection;

        public delegate void StatusUpdateHandler(object sender, EventArgs e);
        public event StatusUpdateHandler OnServiceConnectionSuccess;
        public event StatusUpdateHandler OnServiceConnectionError;

        public ServiceHandler()
        {
            _appServiceConnection = new AppServiceConnection();
            _appServiceConnection.AppServiceName = "com.digicell.CommunicationService";
            _appServiceConnection.PackageFamilyName = Windows.ApplicationModel.Package.Current.Id.FamilyName;
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
