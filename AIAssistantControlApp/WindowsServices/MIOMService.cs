using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AIAssistantControlApp.MIOMService;
using MultimediaIOManagerService.Enums;

namespace AIAssistantControlApp.WindowsServices
{
    public class MIOMService: ClientBase<IMIOMService>, IMIOMService
    {
        private const string ServiceName = "MultimediaIOManagerService";

        private readonly ServiceController _serviceController;


        public MIOMService()
        {
            _serviceController = new ServiceController(ServiceName);
        }


        public void Start()
        {
            if (_serviceController.Status == ServiceControllerStatus.Stopped)
            {
                try
                {
                    _serviceController?.Start();
                    _serviceController?.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(2));
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, $"Error trying to start {ServiceName}", MessageBoxButtons.OK);
                }
            }
            else if(_serviceController.Status == ServiceControllerStatus.Paused)
            {
                try
                {
                    _serviceController?.Continue();
                    _serviceController?.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(2));
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, $"Error trying to continue {ServiceName}", MessageBoxButtons.OK);
                }
            }
            
        }

        public void Stop()
        {
            if (_serviceController.CanStop)
            {
                try
                {
                    _serviceController?.Stop();
                    _serviceController?.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(2));
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, $"Error trying to stop {ServiceName}", MessageBoxButtons.OK);
                }
            }
            else if (_serviceController.CanPauseAndContinue)
            {
                try
                {
                    _serviceController?.Pause();
                    _serviceController?.WaitForStatus(ServiceControllerStatus.Paused, TimeSpan.FromSeconds(2));
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, $"Error trying to pause {ServiceName}", MessageBoxButtons.OK);
                }
            }
        }

        public void Restart()
        {
            Stop();
            Start();
        }

        public MIOMServiceState GetState()
        {
            return Channel.GetState();
        }

        public Task<MIOMServiceState> GetStateAsync()
        {
            return Channel.GetStateAsync();
        }
    }
}
