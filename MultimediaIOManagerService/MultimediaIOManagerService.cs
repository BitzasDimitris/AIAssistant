using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using MultimediaIOManagerService.Enums;
using MultimediaIOManagerService.Structs;

namespace MultimediaIOManagerService
{

    /// <Handy_commands>
    /// console net start/stop MIOMS
    /// powerShell  Clear-Eventlog -LogName MultimediaIOManagerServiceLog
    /// </Handy_commands>

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public partial class MultimediaIOManagerService : ServiceBase, IMIOMService
    {
        private const string EventLogSourceID = "MultimediaIOManagerServiceSource";
        private const string EventLogID = "MultimediaIOManagerServiceLog";

        private ServiceStatus _status;

        private int _eventID = 1;

        private AudioInputManager _audioInputManager;

        private MIOMServiceState _currentState;

        private Uri _interactionAddress = new Uri("http://localhost:21100/mioms");

        private ServiceHost _serviceHost;

        public MultimediaIOManagerService()
        {
            InitializeComponent();
            mEventLog = new EventLog();
            if (!EventLog.SourceExists(EventLogSourceID))
            {
                EventLog.CreateEventSource(EventLogSourceID, EventLogID);
            }

            mEventLog.Source = EventLogSourceID;
            mEventLog.Log = EventLogID;
        }

        public MIOMServiceState GetState()
        {
            using (var service = _serviceHost?.SingletonInstance as MultimediaIOManagerService)
            {
                return service._currentState;
            }
        }


        protected override void OnStart(string[] args)
        {
            // Update the service state to Start Pending.
            _status.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            _status.dwWaitHint = 100000;
            SetServiceStatus(ServiceHandle, ref _status);

            _currentState = MIOMServiceState.Initializing;
            try
            {
                
                mEventLog?.WriteEntry("In OnStart", EventLogEntryType.Information);

                InitializeAudioInManager();

            }
            catch (Exception e)
            {
                mEventLog?.WriteEntry(e.Message, EventLogEntryType.Error, -1);
            }
            try
            {
                SetupServiceCommunication();
            }
            catch (Exception e)
            {
                mEventLog?.WriteEntry(e.Message, EventLogEntryType.Error, -1);
            }

            _currentState = MIOMServiceState.Idle;
            // Update the service state to Running.
            _status.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref _status);
        }


        protected override void OnStop()
        {
            // Update the service state to Stop Pending.
            _status.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            _status.dwWaitHint = 100000;
            SetServiceStatus(ServiceHandle, ref _status);

            mEventLog?.WriteEntry("In OnStop", EventLogEntryType.Information);
            StopServiceCommunication();

            // Update the service state to Stopped.
            _status.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(ServiceHandle, ref _status);
        }

        protected override void OnContinue()
        {
            mEventLog?.WriteEntry("In OnContinue.");
        }

        protected void InitializeAudioInManager()
        {
            _audioInputManager = new AudioInputManager();
        }


        protected void SetupServiceCommunication()
        {
            // Create the ServiceHost.
            _serviceHost = new ServiceHost(this, _interactionAddress);
            // Enable metadata publishing.
            var smb = new ServiceMetadataBehavior
            {
                HttpGetEnabled = true, MetadataExporter = {PolicyVersion = PolicyVersion.Policy15}
            };
            _serviceHost.Description.Behaviors.Add(smb);
            // Open the ServiceHost to start listening for messages. Since
            // no endpoints are explicitly configured, the runtime will create
            // one endpoint per base address for each service contract implemented
            // by the service.
            try
            {
                _serviceHost.Open(TimeSpan.FromSeconds(3));
            }
            catch (Exception e)
            {
                mEventLog?.WriteEntry(e.Message, EventLogEntryType.Error);
            }
        }

        protected void StopServiceCommunication()
        {
            _serviceHost?.Close();
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);
    }
}
