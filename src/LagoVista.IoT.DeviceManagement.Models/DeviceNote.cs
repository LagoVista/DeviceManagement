using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Attributes;
using Newtonsoft.Json;
using LagoVista.IoT.DeviceManagement.Models.Resources;
using LagoVista.IoT.DeviceAdmin.Models.Resources;
using LagoVista.Core.Validation;
using System.Collections.Generic;

namespace LagoVista.IoT.DeviceManagement.Core.Models
{
    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.DeviceNotes_Title, DeviceManagementResources.Names.DeviceNotes_Help,
        DeviceManagementResources.Names.DeviceNotes_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeviceManagementResources),FactoryUrl: "/api/device/note/factory")]
    public class DeviceNote : IValidateable, IAuditableEntity
    {
        [JsonProperty("id")]
        [FormField(LabelResource: DeviceLibraryResources.Names.Common_UniqueId, IsUserEditable: false, ResourceType: typeof(DeviceLibraryResources), IsRequired: true)]
        public string Id { get; set; }
        public string CreationDate { get; set; }
        public string LastUpdatedDate { get; set; }
        public EntityHeader CreatedBy { get; set; }
        public EntityHeader LastUpdatedBy { get; set; }
        public List<EntityChangeSet> AuditHistory { get; set; } = new List<EntityChangeSet>();

        public bool? IsDeleted { get; set; }
        public EntityHeader DeletedBy { get; set; }
        public string DeletionDate { get; set; }
        public bool IsDeprecated { get; set; }
        public EntityHeader DeprecatedBy { get; set; }
        public string DeprecationDate { get; set; }
        public string DeprecationNotes { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DeviceNotes_TitleField, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsRequired:true)]
        public string Title { get; set; }
        [FormField(LabelResource: DeviceManagementResources.Names.DeviceNotes_Notes, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public string Notes { get; set; }
    }
}
