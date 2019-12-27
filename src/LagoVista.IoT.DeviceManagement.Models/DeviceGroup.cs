using System.Collections.Generic;
using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Models.Resources;

namespace LagoVista.IoT.DeviceManagement.Core.Models
{
    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.DeviceGroup_Title, DeviceManagementResources.Names.DeviceGroup_Help, DeviceManagementResources.Names.DeviceGroup_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeviceManagementResources))]
    public class DeviceGroup : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase, IOwnedEntity, IKeyedEntity, INoSQLEntity, IValidateable, IFormDescriptor
    {
        public DeviceGroup()
        {
            Devices = new List<DeviceGroupEntry>();
        }

        public string DatabaseName { get; set; }
        public string EntityType { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DeviceGroup_AssignedUser, HelpResource: DeviceManagementResources.Names.DeviceGroup_AssignedUserHelp, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true, IsRequired: false)]
        public EntityHeader AssignedUser { get; set; }

        public bool IsPublic { get; set; }
        public EntityHeader OwnerOrganization { get; set; }
        public EntityHeader OwnerUser { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DeviceGroup_Repository, HelpResource: DeviceManagementResources.Names.DeviceGroup_Repository_Help, FieldType: FieldTypes.EntityHeaderPicker, RegExValidationMessageResource: DeviceManagementResources.Names.Common_Key_Validation, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public EntityHeader DeviceRepository { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Common_Key, HelpResource: DeviceManagementResources.Names.Common_Key_Help, FieldType: FieldTypes.Key, RegExValidationMessageResource: DeviceManagementResources.Names.Common_Key_Validation, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public string Key { get; set; }

        public DeviceGroupSummary CreateSummary()
        {
            return new DeviceGroupSummary()
            {
                 Id = Id,
                 Description = Description,
                 IsPublic = IsPublic, 
                 Key = Key,
                 Name = Name,
                 RepoId = DeviceRepository.Id,
                 RepoName = DeviceRepository.Text
            };
        }

        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                nameof(DeviceGroup.Name),
                nameof(DeviceGroup.Key),
                nameof(DeviceGroup.DeviceRepository),
                nameof(DeviceGroup.Description),
            };
        }

        [FormField(LabelResource: DeviceManagementResources.Names.DeviceGroup_Devices, HelpResource: DeviceManagementResources.Names.DeviceGroup_Devices_Help, FieldType: FieldTypes.ChildList, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public List<DeviceGroupEntry> Devices { get; set; }

        public EntityHeader<DeviceGroup> ToEntityHeader()
        {
            return new EntityHeader<DeviceGroup>()
            {
                Id = Id,
                Text = Name,
                Value = this
            };
        }
    }

    public class DeviceGroupSummary : SummaryData
    {
        public string RepoId { get; set; }
        public string RepoName { get; set; }
    }
}
