using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using System;
using LagoVista.Core;
using System.Collections.Generic;
using LagoVista.IoT.DeviceManagement.Models.Resources;

namespace LagoVista.IoT.DeviceManagement.Core.Models
{
    public enum RepositoryTypes
    {
        /// <summary>
        /// shared - cloud
        /// </summary>
        [EnumLabel(DeviceRepository.DeviceRepository_Type_NuvIoT, DeviceManagementResources.Names.Device_Repo_RepoType_NuvIoT, typeof(DeviceManagementResources))]
        NuvIoT,
        /// <summary>
        /// iot hub
        /// </summary>
        [EnumLabel(DeviceRepository.DeviceRepository_Type_AzureITHub, DeviceManagementResources.Names.Device_Repo_RepoType_AzureIoTHub, typeof(DeviceManagementResources))]
        AzureIoTHub,
        /// <summary>
        /// on premise
        /// </summary>
        [EnumLabel(DeviceRepository.DeviceRepository_Type_Local, DeviceManagementResources.Names.Device_Repo_RepoType_Local, typeof(DeviceManagementResources))]
        Local,
        /// <summary>
        /// dedicated - cloud
        /// </summary>
        [EnumLabel(DeviceRepository.DeviceRepository_Type_Dedicated, DeviceManagementResources.Names.Device_Repo_RepoType_Dedicated, typeof(DeviceManagementResources))]
        Dedicated
    }

    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.Device_RepoTitle, DeviceManagementResources.Names.Device_Repo_Help, DeviceManagementResources.Names.Device_Repo_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeviceManagementResources))]
    public class DeviceRepository : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase, IOwnedEntity, IKeyedEntity, INoSQLEntity, IValidateable, IEntityHeaderEntity, IFormDescriptor
    {
        public const string DeviceRepository_Type_NuvIoT = "nuviot";
        public const string DeviceRepository_Type_Local = "local";
        public const string DeviceRepository_Type_Dedicated = "dedicated";
        public const string DeviceRepository_Type_AzureITHub = "azureiothub";

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


        [FormField(LabelResource: DeviceManagementResources.Names.Device_Repo_RepoType, HelpResource:DeviceManagementResources.Names.Device_Repo_RepoType_Help, EnumType: (typeof(RepositoryTypes)), FieldType: FieldTypes.Picker, ResourceType: typeof(DeviceManagementResources), WaterMark: DeviceManagementResources.Names.Device_Repo_RepoType_Select, IsRequired: true, IsUserEditable: true)]
        public EntityHeader<RepositoryTypes> RepositoryType { get; set; }

        public ConnectionSettings DeviceStorageSettings { get; set; }
        public String DeviceStorageSecureSettingsId { get; set; }


        public ConnectionSettings DeviceArchiveStorageSettings { get; set; }
        public String DeviceArchiveStorageSettingsSecureId { get; set; }


        public ConnectionSettings PEMStorageSettings { get; set; }
        public String PEMStorageSettingsSecureId { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Common_Key, HelpResource: DeviceManagementResources.Names.Common_Key_Help, FieldType: FieldTypes.Key, RegExValidationMessageResource: DeviceManagementResources.Names.Common_Key_Validation, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public string Key { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Repo_Subscription, WaterMark: DeviceManagementResources.Names.Device_Repo_SubscriptionSelect, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true, IsRequired: true)]
        public EntityHeader Subscription { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Device_Repo_ResourceName, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources))]
        public String ResourceName { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Repo_AccessKeyName, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources))]
        public String AccessKeyName { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Repo_AccessKey, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources))]
        public String AccessKey { get; set; }

        public String SecureAccessKeyId { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Device_Repo_AuthKey1, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: false)]
        public String AuthKey1 { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Repo_AuthKey1, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: false)]
        public String AuthKey2 { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Repo_StorageCapacity, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeviceManagementResources), WaterMark: DeviceManagementResources.Names.Device_Repo_StorageCapacity_Select, IsRequired: true, IsUserEditable: true)]
        public EntityHeader StorageCapacity { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Repo_UnitCapacity, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeviceManagementResources), WaterMark: DeviceManagementResources.Names.Device_Repo_UnitCapacity_Select, IsRequired: true, IsUserEditable: true)]
        public EntityHeader DeviceCapacity { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Repo_AccessKey, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources))]
        public string Uri { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Repo_Instance, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeviceManagementResources))]
        public EntityHeader Instance { get; set; }

        public string GetPEMStorageName()
        {
            if(Key.Length > 20)
                return $"{Key.Substring(0, 20)}{Id}pems";
            else
                return $"{Key}{Id}pems";
        }

        public string GetDeviceLocationStorageName()
        {
            if (Key.Length > 20)
                return $"{Key.Substring(0, 20)}{Id}devicelocations";
            else
                return $"{Key}{Id}devicelocations";
        }

        public string GetDeviceArchiveStorageName()
        {
            if (Key.Length > 20)
                return $"{Key.Substring(0, 20)}{Id}devicearchives";
            else
                return $"{Key}{Id}devicearchives";
        }

        public string GetDeviceMediaStorageName()
        {
            if (Key.Length > 20)
                return $"{Key.Substring(0, 20)}{Id}devicemedia";
            else
                return $"{Key}{Id}devicemedia".ToLower();
        }

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
                Description = Description,
                RepositoryType = RepositoryType.Text
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

        [CustomValidator]
        public void Validate(ValidationResult result, Actions action)
        {
            //TODO: Needs localizations on error messages
            if(EntityHeader.IsNullOrEmpty(RepositoryType))
            {
                result.AddUserError("Respository Type is a Required Field.");
                return;
            }

            if(action == Actions.Create)
            {
                if (DeviceArchiveStorageSettings == null) result.AddUserError("Device Archive Storage Settings are Required on Insert.");
                if (PEMStorageSettings == null) result.AddUserError("PEM Storage Settings Are Required on Insert.");
                if (DeviceStorageSettings == null) result.AddUserError("Device Storage Settings are Required on Insert.");
            }
            else if(action == Actions.Update)
            {
                if (DeviceArchiveStorageSettings == null && String.IsNullOrEmpty(DeviceArchiveStorageSettingsSecureId)) result.AddUserError("Device Archive Storage Settings Or SecureId are Required when updating.");
                if (PEMStorageSettings == null && String.IsNullOrEmpty(DeviceArchiveStorageSettingsSecureId)) result.AddUserError("PEM Storage Settings or Secure Id Are Required when updating.");
                if (DeviceStorageSettings == null && String.IsNullOrEmpty(DeviceArchiveStorageSettingsSecureId)) result.AddUserError("Device Storage Settings Or Secure Id are Required when updating.");
            }

            if(RepositoryType.Value == RepositoryTypes.AzureIoTHub)
            {
                if (String.IsNullOrEmpty(ResourceName)) result.AddUserError("Resource name which is the name of our Azure IoT Hub is a required field.");
                if (String.IsNullOrEmpty(AccessKeyName)) result.AddUserError("Access Key name is a Required field.");

                if (action == Actions.Create && String.IsNullOrEmpty(AccessKey)) result.AddUserError("Access Key is a Required field when adding a repository of type Azure IoT Hub");
                if (action == Actions.Update && String.IsNullOrEmpty(AccessKey) && String.IsNullOrEmpty(SecureAccessKeyId)) result.AddUserError("Access Key or ScureAccessKeyId is a Required when updating a repo of Azure IoT Hub.");


                if (!String.IsNullOrEmpty(AccessKey))
                {
                    if(!AccessKey.IsBase64String()) result.AddUserError("Access Key does not appear to be a Base 64 String.");
                }
            }
        }
    }

    public class DeviceRepositorySummary : SummaryData
    {
        public string RepositoryType { get; set; }
    }
}
