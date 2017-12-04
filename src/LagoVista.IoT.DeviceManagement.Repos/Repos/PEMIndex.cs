using LagoVista.IoT.DeviceManagement.Core.Models;
using Microsoft.WindowsAzure.Storage.Table;


namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class PEMIndex : TableEntity, IPEMIndex
    {
        public string MessageId { get; set; }
        public string DeviceId { get; set; }
        public string Status { get; set; }
        public string MessageType { get; set; }
        public string ErrorReason { get; set; }
        public string CreatedTimeStamp { get; set; }
        public double TotalProcessingMS { get; set; }
        public string JSON { get; set; }
    }
}
