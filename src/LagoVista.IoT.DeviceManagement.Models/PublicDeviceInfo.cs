// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 6bf595184a7b8b62049da654623bcf5567cec8ac92bca746b3067c6fb6a0b4ea
// IndexVersion: 0
// --- END CODE INDEX META ---
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
