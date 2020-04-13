using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIAssistantControlApp.AIAssistantControlPanelService
{
    public class AIACPConnectionEventArgs: EventArgs
    {
        private string _status;
        public AIACPConnectionEventArgs(string status)
        {
            _status = status;
        }

        public string GetStatus()
        {
            return _status;
        }
    }
}
