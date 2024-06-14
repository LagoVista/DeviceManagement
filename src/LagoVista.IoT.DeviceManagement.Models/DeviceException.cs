using LagoVista.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Models
{
    public class DeviceException
    {
        public DeviceException()
        {
            AdditionalDetails = new List<string>();
            Id = Guid.NewGuid().ToId();
        }

        public string Id { get; set; }
        public string Timestamp{ get; set; }
        public string DeviceRepositoryId { get; set; }
        public string DeviceUniqueId { get; set; }
        public string DeviceId { get; set; }
        public string ErrorCode { get; set; }
        public string Event { get; set; }
        public bool Cleared { get; set; }
        public string Details { get; set; }
        public List<string> AdditionalDetails { get; set; }
    }
}
