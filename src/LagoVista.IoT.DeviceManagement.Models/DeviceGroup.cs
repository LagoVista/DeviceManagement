// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 0553bd3acd8d0b1955337b56cd143ce8854855cbc149ef7f6ee9d57215cfaf1d
// IndexVersion: 2
// --- END CODE INDEX META ---
using System.Collections.Generic;
using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Models.Resources;

namespace LagoVista.IoT.DeviceManagement.Core.Models
{
    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.DeviceGroup_Title, DeviceManagementResources.Names.DeviceGroup_Help, 
        DeviceManagementResources.Names.DeviceGroup_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeviceManagementResources), Icon: "icon-ae-device-groups",
        GetListUrl: "/api/repo/{devicerepoid}/groups", GetUrl: "/api/repo/{devicerepoid}/group/{id}",  SaveUrl: "/api/repo/{devicerepoid}/group",
        FactoryUrl: "/api/repo/{devicerepoid}/group/factory", DeleteUrl: "/api/repo/{devicerepoid}/group/{id}")]
    public class DeviceGroup : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase, IValidateable, IFormDescriptor, IIconEntity
    {
        public DeviceGroup()
        {
            Devices = new List<DeviceGroupEntry>();
            Icon = "icon-ae-device-groups";
        }

        [FormField(LabelResource: DeviceManagementResources.Names.Common_Icon, FieldType: FieldTypes.Icon, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public string Icon { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DeviceGroup_AssignedUser, HelpResource: DeviceManagementResources.Names.DeviceGroup_AssignedUserHelp, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true, IsRequired: false)]
        public EntityHeader AssignedUser { get; set; }

        [FKeyProperty(nameof(DeviceRepository), WhereClause:nameof(DeviceRepository) + ".Id = {0}")]
        [FormField(LabelResource: DeviceManagementResources.Names.DeviceGroup_Repository, HelpResource: DeviceManagementResources.Names.DeviceGroup_Repository_Help, FieldType: FieldTypes.EntityHeaderPicker, RegExValidationMessageResource: DeviceManagementResources.Names.Common_Key_Validation, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public EntityHeader DeviceRepository { get; set; }

        public DeviceGroupSummary CreateSummary()
        {
            return new DeviceGroupSummary()
            {
                 Id = Id,
                 Description = Description,
                 IsPublic = IsPublic, 
                 Key = Key,
                 Icon = Icon,
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
                nameof(DeviceGroup.Icon),
                nameof(DeviceGroup.DeviceRepository),
                nameof(DeviceGroup.Description),
            };
        }

        [FormField(LabelResource: DeviceManagementResources.Names.DeviceGroup_Devices, HelpResource: DeviceManagementResources.Names.DeviceGroup_Devices_Help, FieldType: FieldTypes.ChildList, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public List<DeviceGroupEntry> Devices { get; set; }

        public new EntityHeader<DeviceGroup> ToEntityHeader()
        {
            return new EntityHeader<DeviceGroup>()
            {
                Id = Id,
                Text = Name,
                Value = this
            };
        }
    }

    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.DeviceGroup_Summaries_Title, DeviceManagementResources.Names.DeviceGroup_Help,
        DeviceManagementResources.Names.DeviceGroup_Description, EntityDescriptionAttribute.EntityTypes.Summary, typeof(DeviceManagementResources), Icon: "icon-ae-device-groups",
        GetListUrl: "/api/repo/{devicerepoid}/groups", GetUrl: "/api/repo/{devicerepoid}/group/{id}", SaveUrl: "/api/repo/{devicerepoid}/group",
        FactoryUrl: "/api/repo/{devicerepoid}/group/factory", DeleteUrl: "/api/repo/{devicerepoid}/group/{id}")]
    public class DeviceGroupSummary : SummaryData
    {
        public string RepoId { get; set; }
        public string RepoName { get; set; }
    }
}
