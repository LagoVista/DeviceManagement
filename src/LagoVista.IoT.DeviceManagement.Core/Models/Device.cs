using System;
using System.Collections.Generic;
using System.Text;
using LagoVista.Core.Models;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.Core.Attributes;
using LagoVista.IoT.DeviceManagement.Core.Resources;
using Newtonsoft.Json;
using LagoVista.IoT.DeviceAdmin.Resources;

namespace LagoVista.IoT.DeviceManagement.Core.Models
{
    public enum DeviceStates
    {
        [EnumLabel(Device.New, DeviceManagementResources.Names.Device_Status_New, typeof(DeviceManagementResources))]
        New,
        [EnumLabel(Device.Commissioned, DeviceManagementResources.Names.Device_Status_Commissioned, typeof(DeviceManagementResources))]
        Commissioned,
        [EnumLabel(Device.Ready, DeviceManagementResources.Names.Device_Status_Ready, typeof(DeviceManagementResources))]
        Ready,
        [EnumLabel(Device.Degraded, DeviceManagementResources.Names.Device_Status_Degraded, typeof(DeviceManagementResources))]
        Degraded,
        [EnumLabel(Device.PastDue, DeviceManagementResources.Names.Device_Status_PastDue, typeof(DeviceManagementResources))]
        PastDue,
        [EnumLabel(Device.Error, DeviceManagementResources.Names.Device_Status_Error, typeof(DeviceManagementResources))]
        Error,
        [EnumLabel(Device.Decommissioned, DeviceManagementResources.Names.Device_Status_Decommissioned, typeof(DeviceManagementResources))]
        Decommissioned
        
    }

    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.Device_Title, Resources.DeviceManagementResources.Names.Device_Help, Resources.DeviceManagementResources.Names.Device_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeviceManagementResources))]
    public class Device : IOwnedEntity, IIDEntity, IValidateable, INoSQLEntity, IAuditableEntity
    {
        public const string New = "new";
        public const string Commissioned = "commissioned";
        public const string Ready = "ready";
        public const string Degraded = "degraded";
        public const string PastDue = "pastdue";
        public const string Error = "error";
        public const string Decommissioned = "decommissioned";

        public Device()
        {
            Properties = new List<CustomField>();
            Status = new EntityHeader<DeviceStates>() { Value = DeviceStates.New, Id = Device.New, Text = DeviceManagementResources.Device_Status_New };
            Notes = new List<DeviceNote>();
        }

        public string DatabaseName { get; set; }
        public string EntityType { get; set; }
        public string CreationDate { get; set; }
        public string LastUpdatedDate { get; set; }
        public EntityHeader CreatedBy { get; set; }
        public EntityHeader LastUpdatedBy { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Device_Status, EnumType: (typeof(DeviceStates)), FieldType: FieldTypes.Picker, ResourceType: typeof(DeviceManagementResources), WaterMark: DeviceManagementResources.Names.Device_Status_Select, IsRequired: true, IsUserEditable: true)]
        public EntityHeader<DeviceStates> Status { get; set; }

        [JsonProperty("id")]
        [FormField(LabelResource: DeviceLibraryResources.Names.Common_UniqueId, IsUserEditable: false, ResourceType: typeof(DeviceLibraryResources), IsRequired: true)]
        public string Id { get; set; }


        /* Device ID is the ID associated with the device by the user, it generally will be unique, but can't assume it to be, it's primarily read only, it must however be unique for a device configuration. */
        [FormField(LabelResource: DeviceManagementResources.Names.Device_DeviceId, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsRequired:true)]
        public string DeviceId { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Device_DeviceConfiguration, FieldType: FieldTypes.EntityHeaderPicker, WaterMark:DeviceManagementResources.Names.Device_DeviceConfiguration_Select, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public EntityHeader DeviceConfiguration { get; set; }
        public bool IsPublic { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Organization, EnumType: (typeof(DeviceStates)), FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeviceManagementResources), IsRequired: true, WaterMark: DeviceManagementResources.Names.Device_Organization_Select)]
        public EntityHeader OwnerOrganization { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Location, EnumType: (typeof(DeviceStates)), FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeviceManagementResources), WaterMark: DeviceManagementResources.Names.Device_Location_Select)]
        public EntityHeader Location { get; set; }

        public EntityHeader OwnerUser { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Properties, EnumType: (typeof(DeviceStates)), FieldType: FieldTypes.ChildList, ResourceType: typeof(DeviceManagementResources))]
        public List<CustomField> Properties { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_SerialNumber, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources))]
        public string SerialNumber { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_FirmwareVersion, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources))]
        public string FirmwareVersion { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_IsConnected, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable:false)]
        public string IsConnected { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_LastContact, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: false)]
        public string LastContact { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Notes, FieldType: FieldTypes.ChildList, ResourceType: typeof(DeviceManagementResources))]
        public List<DeviceNote> Notes { get; set; }

        public DeviceSummary CreateSummary()
        {
            return new DeviceSummary()
            {
                Id = this.Id,
                DeviceId = this.DeviceId,
                SerialNumber = SerialNumber,
                DeviceConfiguration = DeviceConfiguration.Text,
                Status = Status.Text
            };
        }

        public EntityHeader<Device> ToEntityHeader()
        {
            return new EntityHeader<Device>()
            {
                Id = Id,
                Text = DeviceId,
                Value = this
            };
        }
    }

    public class DeviceSummary 
    {
        public string Id { get; set; }
        public string DeviceConfiguration { get; set; }
        public string DeviceId { get; set; }
        public string SerialNumber { get; set; }
        public string Status { get; set; }


    }
}
