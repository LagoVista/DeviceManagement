using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Models
{
    public  class PublicDeviceInfo
    {
        public EntityHeader OwnerOrganziation { get; set; }
        public string DeviceTypeLabel { get; set; }
        public EntityHeader DeviceType { get; set; }
        public EntityHeader Customer { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string DeviceNameLabel { get; set; }
        public string DeviceName { get; set; }
        public string DeviceId { get; set; }
        public string DevicId { get; set; }
        public EntityHeader DeviceFirmware { get; set; }
        public EntityHeader DeviceFirmwareRevision { get; set; }
        public EntityHeader DeviceRepository { get; set; }
    }
}
