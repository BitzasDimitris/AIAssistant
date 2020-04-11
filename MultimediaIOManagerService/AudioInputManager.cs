using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace MultimediaIOManagerService
{
    class AudioInputManager
    {
        private IWaveIn captureDevice;
        public List<MMDevice> AudioInDevices = new List<MMDevice>();

        private MMDevice _selectedDevice;

        private int _bytesRecorded;

        private Action<int> _callBack;

        private WaveFileWriter writer;

        public AudioInputManager()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                LoadWasapiDevicesCombo();
            }

            _selectedDevice = AudioInDevices.Find(dev => dev.FriendlyName.Contains("HP"));
            captureDevice = new WasapiCapture(_selectedDevice);
            captureDevice.DataAvailable += OnDataAvailable;
            captureDevice.RecordingStopped += OnRecordingStopped;

            if (!Directory.Exists(Constants.RecordingsPath))
            {
                Directory.CreateDirectory(Constants.RecordingsPath);
            }
        }



        private void LoadWasapiDevicesCombo()
        {
            var deviceEnum = new MMDeviceEnumerator();
            AudioInDevices = deviceEnum.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToList();
        }



        public void RecordForDuration(float duration, Action<int> callBack = null)
        {
            var path = Path.Combine(Constants.RecordingsPath,
                $"recording_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}.wav");
            writer = new WaveFileWriter(path, captureDevice.WaveFormat);
            _callBack = callBack;
            _bytesRecorded = 0;
            captureDevice?.StartRecording();
            Task.Delay((int) (duration * 1000)).ContinueWith(t => { captureDevice?.StopRecording(); });
        }

        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            writer.Write(e.Buffer, 0, e.BytesRecorded);
            _bytesRecorded++;
        }

        private void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            _callBack?.Invoke(_bytesRecorded);
            _callBack = null;
            writer?.Flush();
            writer?.Dispose();
            writer = null;
        }

    }
}
