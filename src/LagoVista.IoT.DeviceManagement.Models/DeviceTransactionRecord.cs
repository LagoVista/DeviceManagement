// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 8c8ed54db9bc2085c2e1c2b780171c8c2b11d6e425b90a4ccbcf3c624a52816d
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Models.Resources;

namespace LagoVista.IoT.DeviceManagement.Models
{
    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.DeviceAccountTransactionRecord_Title, DeviceManagementResources.Names.DeviceAccountTransactionRecord_Help,
    DeviceManagementResources.Names.DeviceAccountTransactionRecord_Help, EntityDescriptionAttribute.EntityTypes.Dto, typeof(DeviceManagementResources))]
    public class DeviceAccountTransactionRecord
    {
        public string TimeStamp { get; set; }

        public string TransactionType { get; set; }

        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string LocationId { get; set; }
        public string LocationName { get; set; }

        public double? DebitAmount { get; set; }
        public double? CreditAmount { get; set; }

        public string Description { get; set; }

        public double Balance { get; set; }
    }
}
