using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Core.Models
{
    public class DeviceGroupEntry : TableStorageEntity
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
    }
}
