using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Timers;
using MultimediaIOManagerService.Enums;
using MultimediaIOManagerService.Structs;

namespace MultimediaIOManagerService
{

    /// <Handy_commands>
    /// console net start/stop MIOMS
    /// powerShell  Clear-Eventlog -LogName MultimediaIOManagerServiceLog
    /// </Handy_commands>


    public partial class MultimediaIOManagerService : ServiceBase
    {
        private const string EventLogSourceID = "MultimediaIOManagerServiceSource";
        private const string EventLogID = "MultimediaIOManagerServiceLog";

        private ServiceStatus _status;

        private Timer _myTimer;

        private int _eventID = 1;

        private AudioInputManager _audioInputManager;

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

        protected override void OnStart(string[] args)
        {
            // Update the service state to Start Pending.
            _status.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            _status.dwWaitHint = 100000;
            SetServiceStatus(ServiceHandle, ref _status);
            try
            {
                mEventLog?.WriteEntry("In OnStart", EventLogEntryType.Information);

                InitializeAudioInManager();

                _myTimer = new Timer {Interval = 5000};
                _myTimer.Elapsed += OnTimer;
                _myTimer.Start();

            }
            catch (Exception e)
            {
                mEventLog?.WriteEntry(e.Message, EventLogEntryType.Error, -1);
            }
            // Update the service state to Running.
            _status.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref _status);
        }

        protected void OnTimer(object sender, ElapsedEventArgs args)
        {
            //mEventLog?.WriteEntry("Test timer", EventLogEntryType.Information, _eventID++);
            _audioInputManager?.RecordForDuration(2, bytes =>
            {
                mEventLog?.WriteEntry($"{bytes} were recorded.", EventLogEntryType.Information, 1);
            });
        }


        protected override void OnStop()
        {
            // Update the service state to Stop Pending.
            _status.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            _status.dwWaitHint = 100000;
            SetServiceStatus(ServiceHandle, ref _status);

            mEventLog?.WriteEntry("In OnStop", EventLogEntryType.Information);        

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


        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);
    }
}
