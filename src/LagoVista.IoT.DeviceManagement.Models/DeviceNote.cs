using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Attributes;
using Newtonsoft.Json;
using LagoVista.IoT.DeviceManagement.Models.Resources;
using LagoVista.IoT.DeviceAdmin.Models.Resources;
using LagoVista.Core.Validation;

namespace LagoVista.IoT.DeviceManagement.Core.Models
{
    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.DeviceNotes_Title, DeviceManagementResources.Names.DeviceNotes_Help, DeviceManagementResources.Names.DeviceNotes_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeviceManagementResources))]
    public class DeviceNote : IAuditableEntity, IValidateable
    {
        [JsonProperty("id")]
        [FormField(LabelResource: DeviceLibraryResources.Names.Common_UniqueId, IsUserEditable: false, ResourceType: typeof(DeviceLibraryResources), IsRequired: true)]
        public string Id { get; set; }
        public string CreationDate { get; set; }
        public string LastUpdatedDate { get; set; }
        public EntityHeader CreatedBy { get; set; }
        public EntityHeader LastUpdatedBy { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DeviceNotes_TitleField, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsRequired:true)]
        public string Title { get; set; }
        [FormField(LabelResource: DeviceManagementResources.Names.DeviceNotes_Notes, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public string Notes { get; set; }
    }
}
