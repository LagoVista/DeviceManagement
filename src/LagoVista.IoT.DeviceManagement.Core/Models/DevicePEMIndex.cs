using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Core.Models
{

    public interface IDevicePEMIndex
    {
        String PEM_URI { get; set; }
        String Status { get; set; }
        String MessageId { get; set; }
        String CreatedTimeStamp { get; set; }
        int TotalProcessingMS { get; set; }
    }

    public class DevicePEMIndex : TableStorageEntity, IDevicePEMIndex
    {
        public String PEM_URI { get; set; }

        public String Status { get; set; }

        public string MessageId { get; set; }

        public String CreatedTimeStamp { get; set; }

        public int TotalProcessingMS { get; set; }

    }
}
