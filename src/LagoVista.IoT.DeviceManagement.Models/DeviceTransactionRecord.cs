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
