using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.AppService;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

        private ServiceHandler _serviceHandler;

        public MainPage()
        {
            InitializeComponent();
            _serviceHandler = new ServiceHandler();
            _serviceHandler.OnServiceConnectionSuccess += ServiceConnectedSuccessfully;
        }

        private void ServiceConnectedSuccessfully(object sender, EventArgs e)
        {
            Debug.WriteLine("Service connected.");
            ServiceToggleSwitch.IsOn = true;
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
