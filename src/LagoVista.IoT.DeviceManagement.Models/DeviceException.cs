using LagoVista.Core;
using LagoVista.Core.Attributes;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Models.Resources;
using System;
using System.Collections.Generic;

namespace LagoVista.IoT.DeviceManagement.Models
{
    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.DeviceException_Title, DeviceManagementResources.Names.DeviceException_Description,
        DeviceManagementResources.Names.DeviceException_Description, EntityDescriptionAttribute.EntityTypes.Summary, typeof(DeviceManagementResources), Icon: "icon-ae-error-1")]
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
