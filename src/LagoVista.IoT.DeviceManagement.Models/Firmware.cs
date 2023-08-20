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
        DeviceManagementResources.Names.Firmware_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeviceManagementResources))]
    public class Firmware : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase, IOwnedEntity, IKeyedEntity, INoSQLEntity, IValidateable, IFormDescriptor
    {
        public Firmware()
        {
            Revisions = new List<FirmwareRevision>();           
        }

        public string DatabaseName { get; set; }
        public string EntityType { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Common_Key, HelpResource: DeviceManagementResources.Names.Common_Key_Help, FieldType: FieldTypes.Key, RegExValidationMessageResource: DeviceManagementResources.Names.Common_Key_Validation, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public string Key { get; set; }

        public bool IsPublic { get; set; }
        public EntityHeader OwnerOrganization { get; set; }
        public EntityHeader OwnerUser { get; set; }

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
                DeviceType = DeviceType
            };
        }

        [FormField(LabelResource: DeviceManagementResources.Names.Firmware_Revisions, FieldType: FieldTypes.ChildListInline, ResourceType: typeof(DeviceManagementResources))]
        public List<FirmwareRevision> Revisions { get; set; }

        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                nameof(Name),
                nameof(Key),
                nameof(DeviceType),
                nameof(FirmwareSku),
                nameof(DefaultRevision),
                nameof(Description),
                nameof(Revisions),
            };
        }
    }

    public class FirmwareSummary : SummaryData
    {
        public string DeviceType { get; set; }
    }
}
