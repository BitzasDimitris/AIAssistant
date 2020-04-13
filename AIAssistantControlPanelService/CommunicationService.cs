using System;
using System.Diagnostics;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;

namespace AIAssistantControlPanelService
{
    public sealed class CommunicationService: IBackgroundTask
    {
        private BackgroundTaskDeferral _backgroundTaskDeferral;
        private AppServiceConnection _appServiceConnection;

        private AppServiceConnection _server;
        private AppServiceConnection _client;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral so that the service isn't terminated.
            this._backgroundTaskDeferral = taskInstance.GetDeferral();

            // Associate a cancellation handler with the background task.
            taskInstance.Canceled += OnTaskCanceled;

            // Retrieve the app service connection and set up a listener for incoming app service requests.
            var details = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            _appServiceConnection = details.AppServiceConnection;
            _appServiceConnection.RequestReceived += OnRequestReceived;
        }

        private async void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            if (args.Request.Message.ContainsKey("Client"))
            {
                if (_client == null)
                {
                    _client = sender;
                }
                OnRequestReceivedFromClient(args.Request);
            }
            else if (args.Request.Message.ContainsKey("Server"))
            {
                if (_server == null)
                {
                    _server = sender;
                }
                OnRequestReceivedFromServer(args.Request);
            }
            else
            {
                Debug.WriteLine("Not Known sender.");
            }
        }

        private async void OnRequestReceivedFromClient(AppServiceRequest request)
        {
            var commands =  new ValueSet();
            foreach (var commandKVP in request.Message)
            {
                if (commandKVP.Key == "Command")
                {
                    commands.Add(commandKVP);
                }
            }

            if (commands.Count > 0)
            {
                var response = await _server?.SendMessageAsync(commands);
                if (response != null)
                {
                    await request.SendResponseAsync(response.Message);
                }
                else
                {
                    await request.SendResponseAsync(new ValueSet(){ {"ServerNotFound", "ServerNotFound"}});
                }
            }
            else
            {
                await request.SendResponseAsync(null);
            }
            
        }

        private async void OnRequestReceivedFromServer(AppServiceRequest request)
        {
            var commands =  new ValueSet();
            foreach (var commandKVP in request.Message)
            {
                if (commandKVP.Key == "Command")
                {
                    commands.Add(commandKVP);
                }
            }

            if (commands.Count > 0)
            {
                var response = await _client?.SendMessageAsync(commands);
                if (response != null)
                {
                    await request.SendResponseAsync(response.Message);
                }
            }
            else
            {
                await request.SendResponseAsync(null);
            }
        }

        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            // Complete the service deferral.
            _backgroundTaskDeferral?.Complete();
        }
    }
}
