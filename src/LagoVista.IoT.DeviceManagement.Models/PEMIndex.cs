using System;

namespace LagoVista.IoT.DeviceManagement.Core.Models
{

    public class PEMIndex
    {
        public string RowKey { get; set; }
        public string PartitionKey { get; set; }
        public string DeviceId { get; set; }
        public String MessageId { get; set; }
        public String Status { get; set; }
        public String MessageType { get; set; }
        public String CreatedTimeStamp { get; set; }
        public double TotalProcessingMS { get; set; }
        public string JSON { get; set; }
    }
}
