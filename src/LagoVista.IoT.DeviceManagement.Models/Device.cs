using System.Linq;
using LagoVista.Core;
using System.Collections.Generic;
using LagoVista.Core.Models;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.Core.Attributes;
using Newtonsoft.Json;
using LagoVista.IoT.DeviceManagement.Models.Resources;
using LagoVista.IoT.DeviceAdmin.Models.Resources;
using LagoVista.Core.Models.Geo;
using LagoVista.IoT.DeviceManagement.Models;
using System;
using LagoVista.MediaServices.Models;
using LagoVista.UserAdmin.Models.Users;
using LagoVista.UserAdmin.Models.Orgs;
using LagoVista.UserAdmin.Models.Resources;

namespace LagoVista.IoT.DeviceManagement.Core.Models
{
    public enum DeviceStates
    {
        [EnumLabel(Device.New, DeviceManagementResources.Names.Device_Status_New, typeof(DeviceManagementResources))]
        New,
        [EnumLabel(Device.Commissioned, DeviceManagementResources.Names.Device_Status_Commissioned, typeof(DeviceManagementResources))]
        Commissioned,
        [EnumLabel(Device.Ready, DeviceManagementResources.Names.Device_Status_Ready, typeof(DeviceManagementResources))]
        Ready,
        [EnumLabel(Device.Degraded, DeviceManagementResources.Names.Device_Status_Degraded, typeof(DeviceManagementResources))]
        Degraded,
        [EnumLabel(Device.PastDue, DeviceManagementResources.Names.Device_Status_PastDue, typeof(DeviceManagementResources))]
        PastDue,
        [EnumLabel(Device.Error, DeviceManagementResources.Names.Device_Status_Error, typeof(DeviceManagementResources))]
        Error,
        [EnumLabel(Device.Decommissioned, DeviceManagementResources.Names.Device_Status_Decommissioned, typeof(DeviceManagementResources))]
        Decommissioned
    }

    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.Device_Title, DeviceManagementResources.Names.Device_Help, DeviceManagementResources.Names.Device_Description,
        EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeviceManagementResources), Icon: "icon-ae-core-2",
        GetListUrl: "/api/devices/{devicerepoid}", GetUrl: "/api/device/{devicerepoid}/{id}",
        FactoryUrl: "/api/device/{devicerepoid}/factory",
        SaveUrl: "/api/device/{devicerepoid}", DeleteUrl: "/api/device/{devicerepoid}/{id}")]
    public class Device : EntityBase, IValidateable, IFormDescriptorAdvanced, IFormDescriptor, IFormDescriptorAdvancedCol2, ISummaryFactory
    {
        public const string New = "new";
        public const string Commissioned = "commissioned";
        public const string Ready = "ready";
        public const string Degraded = "degraded";
        public const string PastDue = "pastdue";
        public const string Error = "error";
        public const string Decommissioned = "decommissioned";

        public Device()
        {
            Attributes = new List<AttributeValue>();
            PropertyBag = new Dictionary<string, object>();
            Properties = new List<AttributeValue>();
            States = new List<AttributeValue>();
            Status = new EntityHeader<DeviceStates>() { Value = DeviceStates.New, Id = Device.New, Text = DeviceManagementResources.Device_Status_New };
            Notes = new List<DeviceNote>();
            DeviceGroups = new List<EntityHeader>();
            DeviceTwinDetails = new List<DeviceTwinDetails>();
            Errors = new List<DeviceError>();
            SensorCollection = new List<Sensor>();
            DeviceImages = new List<MediaResource>();
            Relays = new List<Relay>();
            GeoFences = new List<GeoFence>();
            Id = Guid.NewGuid().ToId();
            Icon = "icon-ae-core-2";
            Key = Id.ToLower().Substring(0, 20);
            TimeZone = new EntityHeader()
            {
                Id = "UTC",
                Text = "(UTC) Coordinated Universal Time",
            };
        }
        public string LocationLastUpdatedDate { get; set; }


        [FormField(LabelResource: UserAdminResources.Names.Common_TimeZome, IsRequired: true, FieldType: FieldTypes.Picker, ResourceType: typeof(UserAdminResources), IsUserEditable: true)]
        public EntityHeader TimeZone { get; set; }


        [FKeyProperty(nameof(DeviceRepository), typeof(DeviceRepository), nameof(DeviceRepository) + ".Id = {0}")]
        public EntityHeader DeviceRepository { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Status, EnumType: (typeof(DeviceStates)), FieldType: FieldTypes.Picker, ResourceType: typeof(DeviceManagementResources), WaterMark: DeviceManagementResources.Names.Device_Status_Select, IsRequired: true, IsUserEditable: true)]
        public EntityHeader<DeviceStates> Status { get; set; }


        public string StatusTimestamp { get; set; }

        public Dictionary<string, decimal> Balances { get; set; } = new Dictionary<string, decimal>();


        /* Device ID is the ID associated with the device by the user, it generally will be unique, but can't assume it to be, it's primarily read only, it must however be unique for a device configuration. */
        [FormField(LabelResource: DeviceManagementResources.Names.Device_DeviceId, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public string DeviceId { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_DeviceImages, FieldType: FieldTypes.ChildList, ResourceType: typeof(DeviceManagementResources))]
        public List<MediaResource> DeviceImages { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_DefaultImage, UploadUrl: "/api/media/resource/public/upload", FieldType: FieldTypes.FileUpload, ResourceType: typeof(DeviceManagementResources))]
        public EntityHeader DefaultDeviceImage { get; set; }

        [FKeyProperty(nameof(DeviceConfiguration), WhereClause: "DeviceConfiguration.Id = {0}")]
        [FormField(LabelResource: DeviceManagementResources.Names.Device_DeviceConfiguration, EntityHeaderPickerUrl: "/api/deviceconfigs", FieldType: FieldTypes.EntityHeaderPicker, EditorPath: "/iotstudio/device/deviceconfiguration/{id}",
            WaterMark: DeviceManagementResources.Names.Device_DeviceConfiguration_Select, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public EntityHeader DeviceConfiguration { get; set; }


        [FKeyProperty(nameof(DeviceType), typeof(DeviceType), nameof(DeviceType) + ".Id = {0}", "")]
        [FormField(LabelResource: DeviceManagementResources.Names.Device_DeviceType, FieldType: FieldTypes.EntityHeaderPicker, EditorPath: "/iotstudio/device/devicemodel/{id}",
            EntityHeaderPickerUrl: "/api/devicetypes", WaterMark: DeviceManagementResources.Names.Device_DeviceType_Select, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public EntityHeader<DeviceType> DeviceType { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Common_Icon, FieldType: FieldTypes.Icon, ResourceType: typeof(DeviceManagementResources), IsRequired: true)]
        public string Icon { get; set; }


        public EntityHeader Customer { get; set; }
        public EntityHeader CustomerLocation { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Device_Location, FieldType: FieldTypes.EntityHeaderPicker, EntityHeaderPickerUrl: "/api/org/locations", ResourceType: typeof(DeviceManagementResources), WaterMark: DeviceManagementResources.Names.Device_Location_Select)]
        public EntityHeader<OrgLocation> Location { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Device_DistributionList, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeviceManagementResources), EditorPath: "/organization/distrolist/{id}",
           HelpResource: DeviceManagementResources.Names.Device_DistroList_Help, EntityHeaderPickerUrl: "/api/distros", WaterMark: DeviceManagementResources.Names.Device_DistributionList_Select)]
        public EntityHeader DistributionList { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_NotificationContacts, IsRequired: false, ChildListDisplayMember: "firstName", FieldType: FieldTypes.ChildListInline, EntityHeaderPickerUrl: "/api/distro/externalcontact/factory", ResourceType: typeof(DeviceManagementResources))]
        public List<ExternalContact> NotificationContacts { get; set; } = new List<ExternalContact>();

        [FormField(LabelResource: DeviceManagementResources.Names.Device_OfflineDistributionList, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeviceManagementResources), EditorPath: "/organization/distrolist/{id}",
          HelpResource: DeviceManagementResources.Names.Device_OfflineDistributionList_Help, EntityHeaderPickerUrl: "/api/distros", WaterMark: DeviceManagementResources.Names.Device_DistributionList_Select)]
        public EntityHeader OfflineDistributionList { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Device_DeviceURI, HelpResource: DeviceManagementResources.Names.Device_DeviceURI_Help, FieldType: FieldTypes.Text, IsUserEditable: false, ResourceType: typeof(DeviceManagementResources))]
        public string DeviceURI { get; set; }

        [FKeyProperty(nameof(Device), typeof(Device), nameof(ParentDevice) + ".Id = {0}", "")]
        [FormField(LabelResource: DeviceManagementResources.Names.Device_ParentDevice, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true, IsRequired: false)]
        public EntityHeader<string> ParentDevice { get; set; }

        [FKeyProperty(nameof(Firmware), typeof(Firmware), nameof(DesiredFirmware) + ".Id = {0}", "")]
        [FormField(LabelResource: DeviceManagementResources.Names.Device_DesiredFirmware, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true, IsRequired: false)]
        public EntityHeader DesiredFirmware { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_DesiredFirmwareRevision, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true, IsRequired: false)]
        public EntityHeader DesiredFirmwareRevision { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_ActualFirmware, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: false)]
        public string ActualFirmware { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_ActualFirmware_Revision, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: false)]
        public string ActualFirmwareRevision { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_ActualFirmware_Date, HelpResource: DeviceManagementResources.Names.Device_ActualFirmware_Date_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: false)]
        public string ActualFirmwareDate { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_ShowDiagnostics, HelpResource: DeviceManagementResources.Names.Device_ShowDiagnostics_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeviceManagementResources))]
        public string ShowDiagnostics { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_SerialNumber, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources))]
        public string SerialNumber { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Device_IsConnected, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: false)]
        public string IsConnected { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_ConnectionEstablishedTimeStamp, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: false)]
        public string ConnectionTimeStamp { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_LastContact, FieldType: FieldTypes.DateTime, ResourceType: typeof(DeviceManagementResources), IsUserEditable: false)]
        public string LastContact { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_PrimaryKey, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true, IsRequired: true)]
        public string PrimaryAccessKey { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_SecondaryKey, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true, IsRequired: true)]
        public string SecondaryAccessKey { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_GeoLocation, HelpResource: DeviceManagementResources.Names.Device_GeoLocation_Help, FieldType: FieldTypes.GeoLocation, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true, IsRequired: false)]
        public GeoLocation GeoLocation { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Device_GeoBoundingBox, HelpResource: DeviceManagementResources.Names.Device_GeoBoundingBox_Help, FieldType: FieldTypes.Custom, CustomFieldType: "boudningpolygon", ResourceType: typeof(DeviceManagementResources))]
        public List<GeoLocation> GeoPointsBoundingBox { get; set; } = new List<GeoLocation>();

        public bool HasGeoFix { get; set; }
        public bool GeoFixTimeStamp { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Heading, HelpResource: DeviceManagementResources.Names.Device_Heading_Help, FieldType: FieldTypes.Decimal, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true, IsRequired: false)]
        public double Heading { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Speed, HelpResource: DeviceManagementResources.Names.Device_Speed_Help, FieldType: FieldTypes.Decimal, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true, IsRequired: false)]
        public double Speed { get; set; }

        [JsonProperty("sim")]
        [FormField(LabelResource: DeviceManagementResources.Names.Device_SIM, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true, IsRequired: false)]
        public string SIM { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Device_CustomStatus, HelpResource: DeviceManagementResources.Names.Device_CustomStatus_Help, FieldType: FieldTypes.Picker, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true, IsRequired: false)]
        public EntityHeader CustomStatus { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.DeviceAddress_Address, FieldType: FieldTypes.ChildItem, ResourceType: typeof(DeviceManagementResources), FactoryUrl:"/api/address/factory", IsUserEditable: true, IsRequired: false)]
        public Address Address { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Device_MACAddress, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true, IsRequired: false)]
        public string MacAddress { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_iOS_BLE_Address, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true, IsRequired: false)]
        public string iosBLEAddress { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Watchdog_Disable_Override, HelpResource: DeviceManagementResources.Names.Device_Watchdog_Disable_Override_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true, IsRequired: false)]
        public bool DisableWatchdog { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Device_Watchdog_Seconds_Override, HelpResource: DeviceManagementResources.Names.Device_Watchdog_Seconds_Override_Help, FieldType: FieldTypes.Integer, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true, IsRequired: false)]
        public int? WatchdogSecondsOverride { get; set; }

        [FKeyProperty(nameof(AppUser), typeof(AppUser), nameof(AssignedUser) + ".Id = {0}", "")]
        [FormField(LabelResource: DeviceManagementResources.Names.Device_AssignedUser, HelpResource: DeviceManagementResources.Names.Device_AssignedUserHelp,
            WaterMark: DeviceManagementResources.Names.Device_AssignedUser_Select, FieldType: FieldTypes.UserPicker, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true, IsRequired: false)]
        public EntityHeader AssignedUser { get; set; }

        [FKeyProperty(nameof(AppUser), typeof(AppUser), nameof(WatchdogNotificationUser) + ".Id = {0}", "")]
        [FormField(LabelResource: DeviceManagementResources.Names.Device_Watchdog_Notification_User, HelpResource: DeviceManagementResources.Names.Device_Watchdog_Notification_User_Help, FieldType: FieldTypes.UserPicker, WaterMark: DeviceManagementResources.Names.Device_Watchdog_Notification_User_Select, ResourceType: typeof(DeviceManagementResources), IsUserEditable: true, IsRequired: false)]
        public EntityHeader WatchdogNotificationUser { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_DebugMode, HelpResource: DeviceManagementResources.Names.Device_DebugMode_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeviceManagementResources))]
        public bool DebugMode { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Device_IsBeta, HelpResource: DeviceManagementResources.Names.Device_IsBeta_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeviceManagementResources))]
        public bool IsBeta { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_SilenceAlarms, HelpResource: DeviceManagementResources.Names.Device_SilenceAlarms_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeviceManagementResources))]
        public bool SilenceAlarms { get; set; }

        public EntityHeader SilencedBy { get; set; }

        public string SilencedTimeStamp { get; set; }

        public OrgLocationDiagramReference DiagramReference { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Device_DisableGeofenceDetection, HelpResource: DeviceManagementResources.Names.Device_DisableGeofenceDetection_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeviceManagementResources))]
        public bool DisableGeofenceDetection { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_GeofenceTrackingMode, HelpResource: DeviceManagementResources.Names.Device_GeofenceTrackingMode_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeviceManagementResources))]
        public bool EnableGeofenceTrackingMode { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Device_Summary, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(DeviceManagementResources))]
        public string Summary { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Device_WiFiConnectionProfile, FieldType: FieldTypes.EntityHeaderPicker, ResourceType: typeof(DeviceManagementResources))]
        public EntityHeader WiFiConnectionProfile { get; set; }

        /// <summary>
        /// Properties are design time/values added with device configuration
        /// </summary>
        [FormField(LabelResource: DeviceManagementResources.Names.Device_Properties, FieldType: FieldTypes.ChildList, HelpResource: DeviceManagementResources.Names.Device_Properties_Help, ResourceType: typeof(DeviceManagementResources))]
        public List<CustomField> PropertiesMetaData { get; set; }

        /// <summary>
        /// Properties are design time/values added with device configuration
        /// </summary>
        [FormField(LabelResource: DeviceManagementResources.Names.Device_AttributeMetaData, FieldType: FieldTypes.ChildList, ResourceType: typeof(DeviceManagementResources))]
        public List<DeviceAdmin.Models.Attribute> AttributeMetaData { get; set; }

        /// <summary>
        /// Properties are design time/values added with device configuration
        /// </summary>
        [FormField(LabelResource: DeviceManagementResources.Names.Device_StateMachineMetaData, FieldType: FieldTypes.ChildList, ResourceType: typeof(DeviceManagementResources))]
        public List<StateMachine> StateMachineMetaData { get; set; }

        public Dictionary<string, object> PropertyBag { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Device_States, FieldType: FieldTypes.ChildList, HelpResource: DeviceManagementResources.Names.Device_States_Help, ResourceType: typeof(DeviceManagementResources))]
        public List<AttributeValue> Properties { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Device_States, FieldType: FieldTypes.ChildList, HelpResource: DeviceManagementResources.Names.Device_States_Help, ResourceType: typeof(DeviceManagementResources))]
        public List<AttributeValue> States { get; set; }

        /// <summary>
        /// Attributes are values that have been set by message or workflow
        /// </summary>
        [FormField(LabelResource: DeviceManagementResources.Names.Device_Attributes, FieldType: FieldTypes.ChildList, HelpResource: DeviceManagementResources.Names.Device_Attributes_Help, ResourceType: typeof(DeviceManagementResources))]
        public List<AttributeValue> Attributes { get; set; }

        /// <summary>
        /// Values as pull from the most recent messages
        /// </summary>
        [FormField(LabelResource: DeviceManagementResources.Names.Device_MessageValues, FieldType: FieldTypes.ChildList, HelpResource: DeviceManagementResources.Names.Device_MessageValues_Help, ResourceType: typeof(DeviceManagementResources))]
        public List<AttributeValue> MessageValues { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Device_inputCommandEndPoints, FieldType: FieldTypes.ChildList, HelpResource: DeviceManagementResources.Names.Device_inputCommandEndPoints_Help, ResourceType: typeof(DeviceManagementResources))]
        public List<InputCommandEndPoint> InputCommandEndPoints { get; set; }

        public List<EntityHeader> DeviceGroups { get; set; }


        [FormField(LabelResource: DeviceManagementResources.Names.Device_Pin, FieldType: FieldTypes.Password, ResourceType: typeof(DeviceManagementResources))]
        public string DevicePin { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_MustChangePin, HelpResource: DeviceManagementResources.Names.Device_MustChangePin_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeviceManagementResources))]
        public bool MustChangePin { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_PinChangeDate, FieldType: FieldTypes.ReadonlyLabel, IsUserEditable: false, ResourceType: typeof(DeviceManagementResources))]
        public string PinChangeDate { get; set; }

        public string DevicePinSecureid { get; set; }

        public string InternalSummary { get; set; }

        public string DeviceLabel { get; set; }
        public string DeviceIdLabel { get; set; }
        public string DeviceNameLabel { get; set; }
        public string DeviceTypeLabel { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_DeviceOwner, HelpResource: DeviceManagementResources.Names.Device_DeviceOwner_Help, FieldType: FieldTypes.EntityHeaderPicker, IsUserEditable: false, ResourceType: typeof(DeviceManagementResources))]
        public EntityHeader DeviceOwner { get; set; }

        public List<GeoFence> GeoFences { get; set; }

        public List<Sensor> SensorCollection { get; set; }

        public List<DeviceTwinDetails> DeviceTwinDetails { get; set; }

        public List<DeviceError> Errors { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_Notes, FieldType: FieldTypes.ChildList, ResourceType: typeof(DeviceManagementResources))]
        public List<DeviceNote> Notes { get; set; }

        public List<Relay> Relays { get; set; }

        [FormField(LabelResource: DeviceManagementResources.Names.Device_IPAddress, FieldType: FieldTypes.Text, ResourceType: typeof(DeviceManagementResources))]
        public string IPAddress { get; set; }

        LagoVista.Core.Interfaces.ISummaryData ISummaryFactory.CreateSummary()
        {
            return this.CreateSummary();
        }

        public DeviceSummary CreateSummary()
        {
            var summary = new DeviceSummary()
            {
                Id = this.Id,
                Key = Key,
                IsPublic = false,
                Name = string.IsNullOrEmpty(this.Name) ? this.DeviceId : this.Name,
                DeviceName = string.IsNullOrEmpty(this.Name) ? this.DeviceId : this.Name,
                DeviceId = this.DeviceId,
                SerialNumber = SerialNumber,
                DeviceConfiguration = DeviceConfiguration.Text,
                DeviceConfigurationId = DeviceConfiguration.Id,
                Status = Status.Text,
                DeviceType = DeviceType.Text,
                DeviceTypeId = DeviceType.Id,
                CustomStatus = CustomStatus,
                DefaultDeviceImage = DefaultDeviceImage,
                InternalSummary = InternalSummary,
                GeoLocation = GeoLocation,
                iosBLEAddress = iosBLEAddress,
                Icon = Icon,
                MacAddress = MacAddress,
                DeviceRepoId = DeviceRepository.Id,
                DeviceRepo = DeviceRepository.Text,
                LastContact = LastContact,
                Location = Location,
                Balances = Balances,
                DiagramReference = DiagramReference,
            };

            if (!String.IsNullOrEmpty(ActualFirmware) && !String.IsNullOrEmpty(ActualFirmwareRevision))
                summary.Firmware = $"{ActualFirmware} {ActualFirmwareRevision}";

            return summary;
        }

        public List<string> GetAdvancedFields()
        {
            return new List<string>()
            {
                nameof(Device.Name),
                nameof(Device.Icon),
                nameof(Device.DeviceId),
                nameof(Device.Status),
                nameof(Device.DeviceType),
                nameof(Device.DeviceConfiguration),
                nameof(Device.DefaultDeviceImage),
                nameof(Device.PrimaryAccessKey),
                nameof(Device.SecondaryAccessKey),
                nameof(Device.GeoLocation),
                nameof(Device.GeoPointsBoundingBox),
            };
        }

        public List<string> GetAdvancedFieldsCol2()
        {
            return new List<string>()
            {
                nameof(Device.SerialNumber),
                nameof(Device.DebugMode),
                nameof(Device.IsBeta),
                nameof(Device.SIM),
                nameof(Device.AssignedUser),
                nameof(Device.DistributionList),
                nameof(Device.OfflineDistributionList),
                nameof(Device.NotificationContacts),
                nameof(Device.TimeZone),
                nameof(Device.Location),
                nameof(Device.WatchdogNotificationUser),
                nameof(Device.DisableWatchdog),
                nameof(Device.WatchdogSecondsOverride),
            };
        } 

        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                nameof(Device.Name),
                nameof(Device.DeviceId),
                nameof(Device.Status),
                nameof(Device.DeviceType),
            };
        }

        public new EntityHeader<Device> ToEntityHeader()
        {
            return new EntityHeader<Device>()
            {
                Id = Id,
                Key = Key,
                Text = Name,
                Value = this
            };
        }

        [CustomValidator]
        public void Validate(ValidationResult result, Actions action)
        {
            try
            {
                if (Properties == null)
                {
                    Properties = new List<AttributeValue>();
                }

                if (PropertiesMetaData != null)
                {
                    foreach (var propertyMetaData in PropertiesMetaData)
                    {
                        var property = Properties.Where(prop => prop.Key == propertyMetaData.Key).FirstOrDefault();
                        if (property == null)
                        {
                            var prop = new AttributeValue()
                            {
                                AttributeType = propertyMetaData.FieldType,
                                Key = propertyMetaData.Key,
                                Name = propertyMetaData.Name,
                                Value = String.IsNullOrEmpty(propertyMetaData.DefaultValue) ? null : propertyMetaData.DefaultValue,
                                LastUpdated = DateTime.UtcNow.ToJSONString(),
                                LastUpdatedBy = LastUpdatedBy.Text
                            };

                            Properties.Add(prop);
                        }
                        else
                        {
                            if(property.Value == null)
                            {
                                property.Value = propertyMetaData.DefaultValue;
                            }

                            propertyMetaData.Validate(property.Value, result, action);
                        }
                    }
                }

                if (ParentDevice != null && string.IsNullOrEmpty(ParentDevice.Value))
                {
                    result.AddUserError("If parent device is set the parent device value must contain the Device Id");
                }
            }
            catch (Exception ex)
            {                
                result.AddSystemError(ex.Message);
            }
        }
    }


    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.Devices_Title, DeviceManagementResources.Names.Device_Help, DeviceManagementResources.Names.Device_Description,
    EntityDescriptionAttribute.EntityTypes.Summary, typeof(DeviceManagementResources), Icon: "icon-ae-core-2",
    GetListUrl: "/api/devices/{devicerepoid}", GetUrl: "/api/device/{devicerepoid}/{id}",
    FactoryUrl: "/api/device/{devicerepoid}/factory",
    SaveUrl: "/api/device/{devicerepoid}", DeleteUrl: "/api/device/{devicerepoid}/{id}")]

    public class DeviceSummary : SummaryData
    {
        public string DeviceConfiguration { get; set; }
        public string DeviceConfigurationId { get; set; }
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public string DeviceTypeId { get; set; }
        public string Firmware { get; set; }
        public string DeviceId { get; set; }
        public string SerialNumber { get; set; }
        public string Status { get; set; }
        public string InternalSummary { get; set; }
        public string LastContact { get; set; }
        public string MacAddress { get; set; }
        public string iosBLEAddress { get; set; }
        public string DeviceRepoId { get; set; }
        public string DeviceRepo { get; set; }
        public GeoLocation GeoLocation { get; set; }
        public EntityHeader DefaultDeviceImage { get; set; }

        public EntityHeader Location { get; set; }
        public EntityHeader CustomStatus { get; set; }
        public Dictionary<string, decimal> Balances { get; set; }
        public OrgLocationDiagramReference DiagramReference { get; set; }
    }
}
