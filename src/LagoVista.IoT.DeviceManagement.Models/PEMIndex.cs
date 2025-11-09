// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: b5a105952182c0fe3688ebf84fa3dc9779e260c84e392752dba037dbae82570b
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;
using LagoVista.IoT.DeviceManagement.Models.Resources;
using System;

namespace LagoVista.IoT.DeviceManagement.Core.Models
{

    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.PEMIndex_Title, DeviceManagementResources.Names.PEMIndex_Description, DeviceManagementResources.Names.PEMIndex_Description,
       EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeviceManagementResources), Icon: "icon-ae-core-2",
       GetListUrl: "/api/device/{devicerepoid}/pems/errors/{errorreason}")]
    public class PEMIndex
    {
        public string RowKey { get; set; }
        public string PartitionKey { get; set; }
        public string DeviceId { get; set; }
        public string SolutionVersion { get; set; }
        public string RuntimeVersion { get; set; }
        public String MessageId { get; set; }
        public String Status { get; set; }
        public String MessageType { get; set; }
        public String CreatedTimeStamp { get; set; }
        public double TotalProcessingMS { get; set; }
        public string JSON { get; set; }
    }
}
