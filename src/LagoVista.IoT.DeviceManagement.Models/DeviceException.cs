using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Models
{
    public class DeviceException
    {
        public string Timestamp{ get; set; }
        public string DeviceRepositoryId { get; set; }
        public string DeviceId { get; set; }
        public string ErrorCode { get; set; }
        public string Details { get; set; }
    }
}
