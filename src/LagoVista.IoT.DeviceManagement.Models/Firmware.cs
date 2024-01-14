using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Models.Resources;
using System;
using System.Collections.Generic;

namespace LagoVista.IoT.DeviceManagement.Core.Models
{
    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.Firmware_Title, DeviceManagementResources.Names.Firmware_Help,
        DeviceManagementResources.Names.Firmware_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeviceManagementResources), Icon: "icon-fo-firmware",
        GetListUrl: "/api/firmwares", GetUrl: "/api/firmware/{id}", SaveUrl: "/api/firmware", DeleteUrl: "/api/firmware/{id}", FactoryUrl: "/api/firmware/factory")]
    public class Firmware : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase,  IValidateable, IFormDescriptor, IIconEntity
    {
        public Firmware()
        {
            Revisions = new List<FirmwareRevision>();
            Icon = "icon-fo-firmware";
        }

        [FormField(LabelResource: DeviceManagementResources.Names.Common_Icon, FieldType: FieldTypes.Icon, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public string Icon { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Firmware_DeviceType, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public String DeviceType { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Firmware_FirmwareSKU, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public String FirmwareSku { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Firmware_Default, WaterMark: DeviceManagementResources.Names.Firmware_Default_Select, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeviceManagementResources), IsRequired: false)]
        public EntityHeader DefaultRevision { get; set; }

        public FirmwareSummary CreateSummary()
        {
            return new FirmwareSummary()
            {
                Id = Id,
                Description = Description,
                IsPublic = IsPublic,
                Key = Key,
                Name = Name,
                Icon = Icon,
                DeviceType = DeviceType
            };
        }

        [FormField(LabelResource: DeviceManagementResources.Names.Firmware_Revisions, FieldType: FieldTypes.ChildListInline, FactoryUrl: "/api/firmware/revision/factory", ResourceType: typeof(DeviceManagementResources))]
        public List<FirmwareRevision> Revisions { get; set; }

        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                nameof(Name),
                nameof(Key),
                nameof(Icon),
                nameof(DeviceType),
                nameof(FirmwareSku),
                nameof(DefaultRevision),
                nameof(Description),
                nameof(Revisions),
            };
        }
    }


    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.Firmware_Title, DeviceManagementResources.Names.Firmware_Help,
        DeviceManagementResources.Names.Firmware_Description, EntityDescriptionAttribute.EntityTypes.Summary, typeof(DeviceManagementResources), Icon: "icon-fo-firmware",
        GetListUrl: "/api/firmwares", GetUrl: "/api/firmware/{id}", SaveUrl: "/api/firmware", DeleteUrl: "/api/firmware/{id}", FactoryUrl: "/api/firmware/factory")]
    public class FirmwareSummary : SummaryData
    {
        public string DeviceType { get; set; }
    }
}
