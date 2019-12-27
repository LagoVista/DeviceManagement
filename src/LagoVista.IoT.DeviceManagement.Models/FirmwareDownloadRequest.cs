using LagoVista.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Core.Models
{
    public class FirmwareDownloadRequest
    {
        public FirmwareDownloadRequest()
        {
            DownloadId = Guid.NewGuid().ToString();
            Timestamp = DateTime.UtcNow.ToJSONString();
        }

        public string DownloadId { get; set; }
        public string OrgId { get; set; }
        public bool Expired { get; set; }
        public string DeviceId { get; set; }
        public string Timestamp { get; set; }
        public string ExpiresUTC { get; set; }
        public string FirmwareId { get; set; }
        public string FirmwareRevisionId { get; set; }
    }
}
