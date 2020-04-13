using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AIAssistantControlPanel
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private NavigationViewItem _lastItem;


        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// When app is loaded, kick off the desktop process
        /// and listen to app service connection events
        /// </summary>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (!App.ComingFromBackground)
            {
                if (ApiInformation.IsApiContractPresent("Windows.ApplicationModel.FullTrustAppContract", 1, 0))
                {
                    App.AppServiceConnected += MainPage_AppServiceConnected;
                    App.AppServiceDisconnected += MainPage_AppServiceDisconnected;
                    await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
                }
            }
            else
            {
                App.ComingFromBackground = true;
            }
            
        }

        /// <summary>
        /// Ask user if they want to reconnect to the desktop process
        /// </summary>
        private async void Reconnect()
        {
            if (App.IsForeground)
            {
                await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            }
        }

        /// <summary>
        /// When the desktop process is connected, get ready to send/receive requests
        /// </summary>
        private async void MainPage_AppServiceConnected(object sender, AppServiceTriggerDetails e)
        {
            App.Connection.RequestReceived += AppServiceConnection_RequestReceived;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ServiceToggleSwitch.IsOn = false; });
        }



        /// <summary>
        /// When the desktop process is disconnected, reconnect if needed
        /// </summary>
        private async void MainPage_AppServiceDisconnected(object sender, EventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Reconnect);            
        }

        /// <summary>
        /// </summary>
        private async void AppServiceConnection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            if (args.Request.Message.ContainsKey("Launch"))
            {
                App.ComingFromBackground = true;
                InitializeComponent();
            }

            await args.Request.SendResponseAsync(new ValueSet(){{"Launched", "ready"}});
        }


        private void ServiceToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            //Toggle service
        }

        private void NavigationView_OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (!(args.InvokedItemContainer is NavigationViewItem item))
            {
                Navigation.Content = null;
                return;
            }
            if (item == _lastItem)
                return;
            if (item == null || item == _lastItem)
                return;
            var clickedView = item.Tag?.ToString() ?? "SettingsPage";
            if (!NavigateToView(clickedView)) return;
            _lastItem = item;
        }

        private bool NavigateToView(string clickedView)
        {
            var view = Assembly.GetExecutingAssembly()
                .GetType($"AIAssistantControlPanel.Views.{clickedView}");

            if (string.IsNullOrWhiteSpace(clickedView) || view == null)
            {
                return false;
            }

            ContentFrame.Navigate(view, null, new EntranceNavigationTransitionInfo());
            return true;
        }

        private void NavView_OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (ContentFrame.CanGoBack)
                ContentFrame.GoBack();
        }

        private void ContentFrame_OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
