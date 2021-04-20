using LagoVista.Core;
using System;


namespace LagoVista.IoT.DeviceManagement.Core.Models
{
    public class FirmwareDownloadRequest
    {
        public FirmwareDownloadRequest()
        {
            DownloadId = Guid.NewGuid().ToId();
            Timestamp = DateTime.UtcNow.ToJSONString();
            Error = "noerror";
        }

        public string DownloadId { get; set; }
        public string OrgId { get; set; }
        public bool Expired { get; set; }
        public string Status { get; set; }
        public int PercentRequested { get; set; }
        public string DeviceRepoId { get; set; }
        public string DeviceId { get; set; }
        public string Timestamp { get; set; }
        public string ExpiresUTC { get; set; }
        public string FirmwareName { get; set; }
        public string FirmwareId { get; set; }
        public string FirmwareRevisionId { get; set; }
        public string Error { get; set; }
    }
}
