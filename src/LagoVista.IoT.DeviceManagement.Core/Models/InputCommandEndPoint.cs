using LagoVista.IoT.DeviceAdmin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Core.Models
{
    public class InputCommandEndPoint
    {
        public string EndPoint { get; set; }

        public InputCommand InputCommand { get; set; }
    }
}
