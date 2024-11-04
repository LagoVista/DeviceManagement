using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using System;
using LagoVista.Core;
using System.Collections.Generic;
using LagoVista.IoT.DeviceManagement.Models.Resources;
using LagoVista.UserAdmin.Models.Users;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.UserAdmin.Models.Orgs;
using LagoVista.UserAdmin.Models.Resources;

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
        Dedicated,

        /// <summary>
        /// dedicated - cloud
        /// </summary>
        [EnumLabel(DeviceRepository.DeviceRepository_Type_InClusterMongo, DeviceManagementResources.Names.Device_Repo_RepoType_InClusterMongoDB, typeof(DeviceManagementResources))]
        ClusteredMongoDB
    }

    public enum BillingModel
    {
        [EnumLabel(DeviceRepository.DeviceRepository_Type_InClusterMongo, DeviceManagementResources.Names.Device_Repo_RepoType_InClusterMongoDB, typeof(DeviceManagementResources))]
        PerDevice,
        [EnumLabel(DeviceRepository.DeviceRepository_Type_InClusterMongo, DeviceManagementResources.Names.Device_Repo_RepoType_InClusterMongoDB, typeof(DeviceManagementResources))]
        Consumption
    }

    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.Device_RepoTitle, DeviceManagementResources.Names.Device_Repo_Help,
        DeviceManagementResources.Names.Device_Repo_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeviceManagementResources), Icon: "icon-ae-device-repository",
        CreateUIUrl: "/devicemgmt/repository/new", ListUIUrl: "devicemgmt/repositories", EditUIUrl: "/devicemgmt/repository/{id}",
        GetListUrl: "/api/devicerepos", GetUrl: "/api/devicerepo/{id}", FactoryUrl: "/api/devicerepo/standard/factory", SaveUrl: "/api/devicerepo", DeleteUrl: "/api/devicerepo/{id}")]
    public class DeviceRepository : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase, IValidateable, IEntityHeaderEntity, IFormDescriptor, IFormDescriptorAdvanced, IFormConditionalFields, IIconEntity, IFormDescriptorAdvancedCol2, ISummaryFactory
    {
        public const string DeviceRepository_Type_NuvIoT = "nuviot";
        public const string DeviceRepository_Type_Local = "local";
        public const string DeviceRepository_Type_Dedicated = "dedicated";
        public const string DeviceRepository_Type_InClusterMongo = "inclusteredmongo";
        public const string DeviceRepository_Type_AzureITHub = "azureiothub";

        public const string DeviceRepository_BillingModel_PerDevice = "perdevice";
        public const string DeviceRepository_BillingModel_Batch = "perdevice";

        public DeviceRepository()
        {
            AuthKey1 = Guid.NewGuid().ToId() + Guid.NewGuid().ToId() + Guid.NewGuid().ToId();
            AuthKey2 = Guid.NewGuid().ToId() + Guid.NewGuid().ToId() + Guid.NewGuid().ToId();
            IncrementingDeviceNumber = 1;
            Icon = "icon-ae-device-repository";
            Id = Guid.NewGuid().ToId();
            DeviceTimeoutSeconds = 5 * 60;
        }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Repo_DevicesInUse, FieldType: FieldTypes.Integer, ResourceType: typeof(DeviceManagementResources), IsRequired: true, IsUserEditable: false)]
        public int DevicesInUse { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Repo_MaxDevices, FieldType: FieldTypes.Integer, ResourceType: typeof(DeviceManagementResources), IsRequired: true, IsUserEditable: false)]
        public int DeviceMaxDevices { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Device_Repo_RepoType, HelpResource: DeviceManagementResources.Names.Device_Repo_RepoType_Help, EnumType: (typeof(RepositoryTypes)), FieldType: FieldTypes.Picker, ResourceType: typeof(DeviceManagementResources), WaterMark: DeviceManagementResources.Names.Device_Repo_RepoType_Select, IsRequired: true, IsUserEditable: true)]
        public EntityHeader<RepositoryTypes> RepositoryType { get; set; }

        public ConnectionSettings DeviceStorageSettings { get; set; }
        public String DeviceStorageSecureSettingsId { get; set; }

        public ConnectionSettings DeviceWatchdogStorageSettings { get; set; }
        public String DeviceWatchdogStorageSecureId { get; set; }


        public ConnectionSettings DeviceArchiveStorageSettings { get; set; }
        public String DeviceArchiveStorageSettingsSecureId { get; set; }


        public ConnectionSettings PEMStorageSettings { get; set; }
        public String PEMStorageSettingsSecureId { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Common_Icon, FieldType: FieldTypes.Icon, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public string Icon { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DeviceRepo_DistroList, HelpResource: DeviceManagementResources.Names.DeviceRepo_DistroList_Help,
            WaterMark: DeviceManagementResources.Names.DeviceRepo_DistroList_Select, FieldType: FieldTypes.EntityHeaderPicker,
            EntityHeaderPickerUrl: "/api/distros",
            ResourceType: typeof(DeviceManagementResources), IsUserEditable: true, IsRequired: false)]
        public EntityHeader DistroList { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DeviceRepo_OfflineDistributionList, HelpResource:DeviceManagementResources.Names.DeviceRepo_OfflineDistributionList_Help,
            FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeviceManagementResources),
            EntityHeaderPickerUrl: "/api/distros", WaterMark: DeviceManagementResources.Names.Device_DistributionList_Select)]
        public EntityHeader OfflineDistributionList { get; set; }



        [FKeyProperty(nameof(Subscription), typeof(SubscriptionDTO), nameof(Subscription) + ".Id = {0}", "")]
        [FormField(LabelResource: DeviceManagementResources.Names.Device_Repo_Subscription, WaterMark: DeviceManagementResources.Names.Device_Repo_SubscriptionSelect, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true, IsRequired: true)]
        public EntityHeader Subscription { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Device_Repo_ResourceName, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources))]
        public String ResourceName { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Repo_AccessKeyName, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources))]
        public String AccessKeyName { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Repo_AccessKey, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources))]
        public String AccessKey { get; set; }

        public String SecureAccessKeyId { get; set; }

        [FKeyProperty(nameof(AppUser), typeof(AppUser), nameof(WatchdogNotificationUser) + ".Id = {0}", "")]

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Repo_DevceWatchDog_NotificationContact, HelpResource: DeviceManagementResources.Names.Device_Repo_DevceWatchDog_NotificationContact_Help, FieldType: FieldTypes.UserPicker, WaterMark: DeviceManagementResources.Names.Device_Repo_DevceWatchDog_NotificationContact_Select, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true, IsRequired: false)]
        public EntityHeader WatchdogNotificationUser { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.DeviceRepo_ServiceBoard, HelpResource: DeviceManagementResources.Names.DeviceRepo_ServiceBoard_Help, EntityHeaderPickerUrl: "/api/fslite/serviceboards",
            FieldType: FieldTypes.EntityHeaderPicker, WaterMark: DeviceManagementResources.Names.DeviceRepo_ServiceBoard_Select, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true, IsRequired: false)]
        public EntityHeader ServiceBoard { get; set; }


        [FKeyProperty(nameof(AppUser), typeof(AppUser), nameof(AssignedUser) + ".Id = {0}", "")]
        [FormField(LabelResource: DeviceManagementResources.Names.Device_Repo_AssignedUser, HelpResource: DeviceManagementResources.Names.Device_Repo_AssignedUser_Help, FieldType: FieldTypes.UserPicker, 
            WaterMark: DeviceManagementResources.Names.Device_Repo_AssignedUser_Select, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true, IsRequired: false)]
        public EntityHeader AssignedUser { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Repo_AuthKey1, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: false)]
        public String AuthKey1 { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Repo_AuthKey2, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: false)]
        public String AuthKey2 { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Repo_StorageCapacity, FieldType: FieldTypes.ProductPicker, EntityHeaderPickerUrl: "/api/productofferings/storagecapacity", ResourceType: typeof(DeviceManagementResources), WaterMark: DeviceManagementResources.Names.Device_Repo_StorageCapacity_Select, IsRequired: true, IsUserEditable: true)]
        public EntityHeader StorageCapacity { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Repo_UnitCapacity, FieldType: FieldTypes.ProductPicker, EntityHeaderPickerUrl: "/api/productofferings/devicecapacity", ResourceType: typeof(DeviceManagementResources), WaterMark: DeviceManagementResources.Names.Device_Repo_UnitCapacity_Select, IsRequired: true, IsUserEditable: true)]
        public EntityHeader DeviceCapacity { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Repo_AccessKey, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources))]
        public string Uri { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Repo_Instance, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeviceManagementResources))]
        public EntityHeader Instance { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DeviceRepo_DeviceNumber, HelpResource: DeviceManagementResources.Names.DeviceRepo_DeviceNumberHelp, FieldType: FieldTypes.Integer, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true)]
        public int IncrementingDeviceNumber { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DeviceRepo_AutoGenerateDeviceIds, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true)]
        public bool AutoGenerateDeviceIds { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DeviceRepo_UserOwnedDevices, HelpResource: DeviceManagementResources.Names.DeviceRepo_UserOwnedDevices_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true)]
        public bool UserOwnedDevicesOnly { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DeviceRepo_SecureUserOwnedDevices, HelpResource: DeviceManagementResources.Names.DeviceRepo_SecureUserOwnedDevices_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true)]
        public bool SecureUserOwnedDevices { get; set; }

        [FormField(LabelResource: UserAdminResources.Names.Organization_Logo_Light, UserAdminResources.Names.Organization_Logo_LightColor_Help, FieldType: FieldTypes.FileUpload, DisplayImageSize: "800x224", GeneratedImageSize: "1024x1024", UploadUrl: "/api/media/resource/public/upload", ResourceType: typeof(UserAdminResources))]
        public EntityHeader LightLogo { get; set; }

        [FormField(LabelResource: UserAdminResources.Names.Organization_Logo_DarkColor, UserAdminResources.Names.Organization_Logo_DarkColor_Help, FieldType: FieldTypes.FileUpload, DisplayImageSize: "800x224", GeneratedImageSize: "1024x1024", UploadUrl: "/api/media/resource/public/upload", ResourceType: typeof(UserAdminResources))]
        public EntityHeader DarkLogo { get; set; }

        [FormField(LabelResource: UserAdminResources.Names.Organization_PrimaryBgColor, FieldType: FieldTypes.Color, ResourceType: typeof(UserAdminResources))]
        public string PrimaryBgColor { get; set; } = "#1976D2";

        [FormField(LabelResource: UserAdminResources.Names.Organization_AccentColor, FieldType: FieldTypes.Color, ResourceType: typeof(UserAdminResources))]
        public string AccentColor { get; set; } = "#D48D17";

        [FormField(LabelResource: UserAdminResources.Names.Organization_PrimaryTextColor, FieldType: FieldTypes.Color, ResourceType: typeof(UserAdminResources))]
        public string PrimaryTextColor { get; set; } = "#F4F4F4";


        [FormField(LabelResource: DeviceManagementResources.Names.DeviceRepo_DeviceTimeoutSeconds, HelpResource: DeviceManagementResources.Names.DeviceRepo_DeviceTimeoutSeconds_Help, FieldType: FieldTypes.Integer, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true)]
        public int DeviceTimeoutSeconds { get; set; }

        public ConnectionSettings DeviceAccountConnection { get; set; }

        public string DeviceAccountPasswordSecureId { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.DeviceRepository_OnlineNotification, HelpResource:DeviceManagementResources.Names.DeviceRepository_OnlineNotification_Help, FieldType: FieldTypes.EntityHeaderPicker, EntityHeaderPickerUrl: "/api/notifications", ResourceType: typeof(DeviceManagementResources), WaterMark: DeviceManagementResources.Names.DeviceRepository_OnlineNotification_Select, IsRequired: false, IsUserEditable: true)]
        public EntityHeader DeviceOnlinNotification { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DeviceRepository_OfflineNotification, HelpResource: DeviceManagementResources.Names.DeviceRepository_OfflineNotification_Help, FieldType: FieldTypes.EntityHeaderPicker, EntityHeaderPickerUrl: "/api/notifications", ResourceType: typeof(DeviceManagementResources), WaterMark: DeviceManagementResources.Names.DeviceRepository_OfflineNotification_Select, IsRequired: false, IsUserEditable: true)]
        public EntityHeader DeviceOfflinNotification { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.DeviceRepo_DeviceTimeoutNotificationIntervalMinutes, HelpResource: DeviceManagementResources.Names.DeviceRepo_DeviceTimeoutNotificationIntervalMinutes_Help, FieldType: FieldTypes.Integer, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true)]
        public int DeviceTimeoutNotificationIntervalMinutes { get; set; }

        private string GetTableName(String suffix)
        {
            var tableName = (Key.Length > 20) ? $"{Key.Substring(0, 20)}{Id}{suffix}" : $"{Key}{Id}{suffix}";
            return tableName.Length > 63 ? tableName.Substring(0, 63) : tableName;
        }

        public string GetPEMStorageName()
        {
            return GetTableName("pems");
        }

        public string GetDeviceLocationStorageName()
        {
            return GetTableName("devicelocations");
        }

        public string GetDeviceWatchdogStorageName()
        {
            return GetTableName("watchdog");
        }

        public string GetDeviceArchiveStorageName()
        {
            return GetTableName("devicearchives");
        }

        public string GetDeviceConnectionEventStorageName()
        {
            return GetTableName("connectionevents");
        }

        public string GetSilencedAlarmsStorageName()
        {
            return GetTableName("silencedalarms");
        }

        public string GetDeviceExceptionsStorageName()
        {
            return GetTableName("exceptions");
        }

        public string GetDeviceCurrentStatusStorageName()
        {
            return GetTableName("currentdevicestatus");
        }

        public string GetDeviceStatusHistoryStorageName()
        {
            return GetTableName("devicestatushistory");
        }

        public string GetDeviceMediaStorageName()
        {
            return GetTableName("devicemedia").ToLower();
        }

        public string GetDeviceAccountDatabseeName()
        {
            return GetTableName("deviceaccount").ToLower();
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
                RepositoryType = RepositoryType.Text,
                Icon = Icon,
                Instance = Instance?.Text,
                InstanceId = Instance?.Id,
            };
        }

        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                nameof(Name),
                nameof(Key),
                nameof(Icon),
                nameof(RepositoryType),
                nameof(Subscription),
                nameof(DeviceCapacity),
                nameof(StorageCapacity),
                nameof(Description),
            };
        }

        public List<string> GetAdvancedFields()
        {
            return new List<string>()
            {
                nameof(Name),
                nameof(Key),
                nameof(Icon),
                nameof(Subscription),
                nameof(RepositoryType),
                nameof(ResourceName),
                nameof(AccessKeyName),
                nameof(AccessKey),
                nameof(DarkLogo),
                nameof(LightLogo),
                nameof(PrimaryTextColor),
                nameof(PrimaryBgColor),
                nameof(AccentColor),
                nameof(Description),
            };
        }

        public List<string> GetAdvancedFieldsCol2()
        {
            return new List<string>()
            {
                nameof(AutoGenerateDeviceIds),
                nameof(DeviceCapacity),
                nameof(StorageCapacity),
                nameof(DistroList),
                nameof(AssignedUser),
                nameof(WatchdogNotificationUser),
                nameof(DeviceTimeoutSeconds),
                nameof(DeviceTimeoutNotificationIntervalMinutes),
                nameof(OfflineDistributionList),
                nameof(DeviceOnlinNotification),
                nameof(DeviceOfflinNotification),
                nameof(UserOwnedDevicesOnly),
                nameof(ServiceBoard),
                nameof(SecureUserOwnedDevices),
            };
        }

        public FormConditionals GetConditionalFields()
        {
            return new FormConditionals()
            {
                ConditionalFields = new List<string>() { nameof(AccessKey), nameof(ResourceName), nameof(AccessKeyName) },
                Conditionals = new List<FormConditional>()
                {
                    new FormConditional()
                    {
                         Field = nameof(RepositoryType),
                         Value = DeviceRepository_Type_AzureITHub,
                         ForCreate = true,
                         ForUpdate = false,
                         VisibleFields = new List<string>() {nameof(ResourceName), nameof(AccessKeyName), nameof(AccessKey)},
                         RequiredFields = new List<string>() {nameof(ResourceName), nameof(AccessKeyName), nameof(AccessKey)}
                    },
                    new FormConditional()
                    {
                         Field = nameof(RepositoryType),
                         Value = DeviceRepository_Type_AzureITHub,
                         ForCreate = false,
                         ForUpdate = true,
                         VisibleFields = new List<string>() {nameof(ResourceName), nameof(AccessKeyName), nameof(AccessKey)},
                         RequiredFields = new List<string>() {nameof(ResourceName), nameof(AccessKeyName)}
                    },

                }
            };
        }

        [CustomValidator]
        public void Validate(ValidationResult result, Actions action)
        {
            //TODO: Needs localizations on error messages
            if (EntityHeader.IsNullOrEmpty(RepositoryType))
            {
                result.AddUserError("Respository Type is a Required Field.");
                return;
            }

            if (RepositoryType?.Value == RepositoryTypes.NuvIoT)
            {
                if (action == Actions.Create)
                {
                    if (DeviceArchiveStorageSettings == null) result.AddUserError("Device Archive Storage Settings are Required on Insert.");
                    if (PEMStorageSettings == null) result.AddUserError("PEM Storage Settings Are Required on Insert.");
                    if (DeviceStorageSettings == null) result.AddUserError("Device Storage Settings are Required on Insert.");
                }
                else if (action == Actions.Update)
                {
                    if (DeviceArchiveStorageSettings == null && String.IsNullOrEmpty(DeviceArchiveStorageSettingsSecureId)) result.AddUserError("Device Archive Storage Settings Or SecureId are Required when updating.");
                    if (PEMStorageSettings == null && String.IsNullOrEmpty(DeviceArchiveStorageSettingsSecureId)) result.AddUserError("PEM Storage Settings or Secure Id Are Required when updating.");
                    if (DeviceStorageSettings == null && String.IsNullOrEmpty(DeviceArchiveStorageSettingsSecureId)) result.AddUserError("Device Storage Settings Or Secure Id are Required when updating.");
                }
            }

            if (RepositoryType.Value == RepositoryTypes.AzureIoTHub)
            {
                if (String.IsNullOrEmpty(ResourceName)) result.AddUserError("Resource name which is the name of our Azure IoT Hub is a required field.");
                if (String.IsNullOrEmpty(AccessKeyName)) result.AddUserError("Access Key name is a Required field.");

                if (action == Actions.Create && String.IsNullOrEmpty(AccessKey)) result.AddUserError("Access Key is a Required field when adding a repository of type Azure IoT Hub");
                if (action == Actions.Update && String.IsNullOrEmpty(AccessKey) && String.IsNullOrEmpty(SecureAccessKeyId)) result.AddUserError("Access Key or ScureAccessKeyId is a Required when updating a repo of Azure IoT Hub.");


                if (!String.IsNullOrEmpty(AccessKey))
                {
                    if (!AccessKey.IsBase64String()) result.AddUserError("Access Key does not appear to be a Base 64 String.");
                }
            }
        }

        ISummaryData ISummaryFactory.CreateSummary()
        {
            return CreateSummary();
        }
    }

    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.Device_ReposTitle, DeviceManagementResources.Names.Device_Repo_Help,
       DeviceManagementResources.Names.Device_Repo_Description, EntityDescriptionAttribute.EntityTypes.Summary, typeof(DeviceManagementResources), Icon: "icon-ae-device-repository",
       GetListUrl: "/api/devicerepos", GetUrl: "/api/devicerepo/{id}", FactoryUrl: "/api/devicerepo/standard/factory", SaveUrl: "/api/devicerepo", DeleteUrl: "/api/devicerepo/{id}")]
    public class DeviceRepositorySummary : SummaryData
    {
        public string InstanceId { get; set; }
        public string Instance { get; set; }
        public string RepositoryType { get; set; }
    }
}
