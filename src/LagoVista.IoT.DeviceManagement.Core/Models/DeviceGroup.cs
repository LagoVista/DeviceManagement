using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Resources;

namespace LagoVista.IoT.DeviceManagement.Core.Models
{
    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.DeviceGroup_Title, Resources.DeviceManagementResources.Names.DeviceGroup_Help, Resources.DeviceManagementResources.Names.DeviceGroup_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeviceManagementResources))]
    public class DeviceGroup : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase, IOwnedEntity, IKeyedEntity, INoSQLEntity, IValidateable
    {
        public string DatabaseName { get; set; }
        public string EntityType { get; set; }
        
        public bool IsPublic { get; set; }
        public EntityHeader OwnerOrganization { get; set; }
        public EntityHeader OwnerUser { get; set; }

        [FormField(LabelResource: Resources.DeviceManagementResources.Names.Common_Key, HelpResource: Resources.DeviceManagementResources.Names.Common_Key_Help, FieldType: FieldTypes.Key, RegExValidationMessageResource: Resources.DeviceManagementResources.Names.Common_Key_Validation, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public string Key { get; set; }

        public DeviceGroupSummary CreateSummary()
        {
            return new DeviceGroupSummary()
            {
                 Id = Id,
                 Description = Description,
                 IsPublic = IsPublic, 
                 Key = Key,
                 Name = Name
            };
        }

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

    }
}
