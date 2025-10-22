// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 3fc2bb04e53162fc80e1e3f5543a31e933d2edc282108fe2f7b6b6f2c20225af
// IndexVersion: 0
// --- END CODE INDEX META ---
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
