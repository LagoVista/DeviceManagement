using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Resources;
using System;
using LagoVista.Core;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Core.Models
{
    public enum RepositoryTypes
    {
        [EnumLabel(DeviceRepository.DeviceRepository_Type_NuvIoT, DeviceManagementResources.Names.Device_Repo_RepoType_NuvIoT, typeof(DeviceManagementResources))]
        NuvIoT,
        [EnumLabel(DeviceRepository.DeviceRepository_Type_AzureITHub, DeviceManagementResources.Names.Device_Repo_RepoType_AzureIoTHub, typeof(DeviceManagementResources))]
        AzureIoTHub
    }


    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.Device_RepoTitle, Resources.DeviceManagementResources.Names.Device_Repo_Help, Resources.DeviceManagementResources.Names.Device_Repo_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeviceManagementResources))]
    public class DeviceRepository : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase, IOwnedEntity, IKeyedEntity, INoSQLEntity, IValidateable, IEntityHeaderEntity, IFormDescriptor
    {
        public const string DeviceRepository_Type_NuvIoT = "nuviot";
        public const string DeviceRepository_Type_AzureITHub = "AzureIoTHub";

        public DeviceRepository()
        {
            AuthKey1 = Guid.NewGuid().ToId() + Guid.NewGuid().ToId() + Guid.NewGuid().ToId();
            AuthKey2 = Guid.NewGuid().ToId() + Guid.NewGuid().ToId() + Guid.NewGuid().ToId();
        }

        public string DatabaseName { get; set; }
        public string EntityType { get; set; }

        public bool IsPublic { get; set; }
        public EntityHeader OwnerOrganization { get; set; }
        public EntityHeader OwnerUser { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Device_Repo_RepoType, EnumType: (typeof(RepositoryTypes)), FieldType: FieldTypes.Picker, ResourceType: typeof(DeviceManagementResources), WaterMark: DeviceManagementResources.Names.Device_Repo_RepoType_Select, IsRequired: true, IsUserEditable: true)]
        public EntityHeader<RepositoryTypes> RepositoryType { get; set; }

        public ConnectionSettings DeviceStorageSettings { get; set; }
        public ConnectionSettings DeviceArchiveStorageSettings { get; set; }
        public ConnectionSettings PEMStorageSettings { get; set; }

        [FormField(LabelResource: Resources.DeviceManagementResources.Names.Common_Key, HelpResource: Resources.DeviceManagementResources.Names.Common_Key_Help, FieldType: FieldTypes.Key, RegExValidationMessageResource: Resources.DeviceManagementResources.Names.Common_Key_Validation, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public string Key { get; set; }

        [FormField(LabelResource: Resources.DeviceManagementResources.Names.Device_Repo_Subscription, WaterMark: Resources.DeviceManagementResources.Names.Device_Repo_SubscriptionSelect, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true, IsRequired: true)]
        public EntityHeader Subscription { get; set; }


        [FormField(LabelResource: Resources.DeviceManagementResources.Names.Device_Repo_ResourceName, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources))]
        public String ResourceName { get; set; }

        [FormField(LabelResource: Resources.DeviceManagementResources.Names.Device_Repo_AccessKeyName, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources))]
        public String AccessKeyName { get; set; }

        [FormField(LabelResource: Resources.DeviceManagementResources.Names.Device_Repo_AccessKey, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources))]
        public String AccessKey { get; set; }


        [FormField(LabelResource: Resources.DeviceManagementResources.Names.Device_Repo_AuthKey1, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: false)]
        public String AuthKey1 { get; set; }

        [FormField(LabelResource: Resources.DeviceManagementResources.Names.Device_Repo_AuthKey1, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: false)]
        public String AuthKey2 { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Repo_StorageCapacity, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeviceManagementResources), WaterMark: DeviceManagementResources.Names.Device_Repo_StorageCapacity_Select, IsRequired: true, IsUserEditable: true)]
        public EntityHeader StorageCapacity { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Repo_UnitCapacity, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeviceManagementResources), WaterMark: DeviceManagementResources.Names.Device_Repo_UnitCapacity_Select, IsRequired: true, IsUserEditable: true)]
        public EntityHeader DeviceCapacity { get; set; }

        public IEntityHeader ToEntityHeader()
        {
            return new EntityHeader()
            {
                Id = Id,
                Text = Name,
            };
        }

        public DeviceRepositorySummary CreateSummary()
        {
            return new DeviceRepositorySummary()
            {
                Id = Id,
                IsPublic = IsPublic,
                Key = Key,
                Name = Name,
                Description = Description
            };
        }

        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                nameof(Name),
                nameof(Key),
                nameof(Subscription),
                nameof(DeviceCapacity),
                nameof(StorageCapacity),
                nameof(Description),
            };
        }
    }

    public class DeviceRepositorySummary : SummaryData
    {

    }
}
