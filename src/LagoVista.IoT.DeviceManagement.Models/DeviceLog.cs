using LagoVista.Core.Models;

namespace LagoVista.IoT.DeviceManagement.Core.Models
{
    public class DeviceLog : TableStorageEntity
    {
        public const string Error = "ERROR";
        public const string Warning = "WARNING";
        public const string Info = "INFO";


        /* Partition Key will be the give id */

        public string DeviceId { get; set; }

        public string DateStamp { get; set; }

        public string EntryType { get; set; }

        public string Source { get; set; }

        public string Entry { get; set; }

        public string DeleteAfter { get; set; }
    }
}
