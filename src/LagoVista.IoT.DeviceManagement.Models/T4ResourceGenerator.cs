using System.Globalization;
using System.Reflection;  

//Resources:DeviceManagementResources:AttributeValue_Description
namespace LagoVista.IoT.DeviceManagement.Models.Resources
{
	public class DeviceManagementResources
	{
        private static global::System.Resources.ResourceManager _resourceManager;
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        private static global::System.Resources.ResourceManager ResourceManager 
		{
            get 
			{
                if (object.ReferenceEquals(_resourceManager, null)) 
				{
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LagoVista.IoT.DeviceManagement.Models.Resources.DeviceManagementResources", typeof(DeviceManagementResources).GetTypeInfo().Assembly);
                    _resourceManager = temp;
                }
                return _resourceManager;
            }
        }
        
        /// <summary>
        ///   Returns the formatted resource string.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        private static string GetResourceString(string key, params string[] tokens)
		{
			var culture = CultureInfo.CurrentCulture;;
            var str = ResourceManager.GetString(key, culture);

			for(int i = 0; i < tokens.Length; i += 2)
				str = str.Replace(tokens[i], tokens[i+1]);
										
            return str;
        }
        
        /// <summary>
        ///   Returns the formatted resource string.
        /// </summary>
		/*
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        private static HtmlString GetResourceHtmlString(string key, params string[] tokens)
		{
			var str = GetResourceString(key, tokens);
							
			if(str.StartsWith("HTML:"))
				str = str.Substring(5);

			return new HtmlString(str);
        }*/
		
		public static string AttributeValue_Description { get { return GetResourceString("AttributeValue_Description"); } }
//Resources:DeviceManagementResources:AttributeValue_Help

		public static string AttributeValue_Help { get { return GetResourceString("AttributeValue_Help"); } }
//Resources:DeviceManagementResources:AttributeValue_Inference

		public static string AttributeValue_Inference { get { return GetResourceString("AttributeValue_Inference"); } }
//Resources:DeviceManagementResources:AttributeValue_Key

		public static string AttributeValue_Key { get { return GetResourceString("AttributeValue_Key"); } }
//Resources:DeviceManagementResources:AttributeValue_LastUpdated

		public static string AttributeValue_LastUpdated { get { return GetResourceString("AttributeValue_LastUpdated"); } }
//Resources:DeviceManagementResources:AttributeValue_LastUpdatedBy

		public static string AttributeValue_LastUpdatedBy { get { return GetResourceString("AttributeValue_LastUpdatedBy"); } }
//Resources:DeviceManagementResources:AttributeValue_Name

		public static string AttributeValue_Name { get { return GetResourceString("AttributeValue_Name"); } }
//Resources:DeviceManagementResources:AttributeValue_State

		public static string AttributeValue_State { get { return GetResourceString("AttributeValue_State"); } }
//Resources:DeviceManagementResources:AttributeValue_Title

		public static string AttributeValue_Title { get { return GetResourceString("AttributeValue_Title"); } }
//Resources:DeviceManagementResources:AttributeValue_Type

		public static string AttributeValue_Type { get { return GetResourceString("AttributeValue_Type"); } }
//Resources:DeviceManagementResources:AttributeValue_Unit

		public static string AttributeValue_Unit { get { return GetResourceString("AttributeValue_Unit"); } }
//Resources:DeviceManagementResources:AttributeValue_Value

		public static string AttributeValue_Value { get { return GetResourceString("AttributeValue_Value"); } }
//Resources:DeviceManagementResources:Common_Description

		public static string Common_Description { get { return GetResourceString("Common_Description"); } }
//Resources:DeviceManagementResources:Common_Key

		public static string Common_Key { get { return GetResourceString("Common_Key"); } }
//Resources:DeviceManagementResources:Common_Key_Help

		public static string Common_Key_Help { get { return GetResourceString("Common_Key_Help"); } }
//Resources:DeviceManagementResources:Common_Key_Validation

		public static string Common_Key_Validation { get { return GetResourceString("Common_Key_Validation"); } }
//Resources:DeviceManagementResources:Common_Name

		public static string Common_Name { get { return GetResourceString("Common_Name"); } }
//Resources:DeviceManagementResources:Common_Notes

		public static string Common_Notes { get { return GetResourceString("Common_Notes"); } }
//Resources:DeviceManagementResources:DeivceNotes_Description

		public static string DeivceNotes_Description { get { return GetResourceString("DeivceNotes_Description"); } }
//Resources:DeviceManagementResources:Device_ActualFirmware

		public static string Device_ActualFirmware { get { return GetResourceString("Device_ActualFirmware"); } }
//Resources:DeviceManagementResources:Device_ActualFirmware_Date

		public static string Device_ActualFirmware_Date { get { return GetResourceString("Device_ActualFirmware_Date"); } }
//Resources:DeviceManagementResources:Device_ActualFirmware_Date_Help

		public static string Device_ActualFirmware_Date_Help { get { return GetResourceString("Device_ActualFirmware_Date_Help"); } }
//Resources:DeviceManagementResources:Device_ActualFirmware_Revision

		public static string Device_ActualFirmware_Revision { get { return GetResourceString("Device_ActualFirmware_Revision"); } }
//Resources:DeviceManagementResources:Device_AssignedUser

		public static string Device_AssignedUser { get { return GetResourceString("Device_AssignedUser"); } }
//Resources:DeviceManagementResources:Device_AssignedUser_Select

		public static string Device_AssignedUser_Select { get { return GetResourceString("Device_AssignedUser_Select"); } }
//Resources:DeviceManagementResources:Device_AssignedUserHelp

		public static string Device_AssignedUserHelp { get { return GetResourceString("Device_AssignedUserHelp"); } }
//Resources:DeviceManagementResources:Device_AttributeMetaData

		public static string Device_AttributeMetaData { get { return GetResourceString("Device_AttributeMetaData"); } }
//Resources:DeviceManagementResources:Device_Attributes

		public static string Device_Attributes { get { return GetResourceString("Device_Attributes"); } }
//Resources:DeviceManagementResources:Device_Attributes_Help

		public static string Device_Attributes_Help { get { return GetResourceString("Device_Attributes_Help"); } }
//Resources:DeviceManagementResources:Device_Capacity_100_Units

		public static string Device_Capacity_100_Units { get { return GetResourceString("Device_Capacity_100_Units"); } }
//Resources:DeviceManagementResources:Device_Capacity_1000_Units

		public static string Device_Capacity_1000_Units { get { return GetResourceString("Device_Capacity_1000_Units"); } }
//Resources:DeviceManagementResources:Device_Capacity_10000_Units

		public static string Device_Capacity_10000_Units { get { return GetResourceString("Device_Capacity_10000_Units"); } }
//Resources:DeviceManagementResources:Device_Capacity_100000_Units

		public static string Device_Capacity_100000_Units { get { return GetResourceString("Device_Capacity_100000_Units"); } }
//Resources:DeviceManagementResources:Device_Capacity_1000000_Units

		public static string Device_Capacity_1000000_Units { get { return GetResourceString("Device_Capacity_1000000_Units"); } }
//Resources:DeviceManagementResources:Device_Capacity_5_Units

		public static string Device_Capacity_5_Units { get { return GetResourceString("Device_Capacity_5_Units"); } }
//Resources:DeviceManagementResources:Device_Capacity_500_Units

		public static string Device_Capacity_500_Units { get { return GetResourceString("Device_Capacity_500_Units"); } }
//Resources:DeviceManagementResources:Device_Capacity_5000_Units

		public static string Device_Capacity_5000_Units { get { return GetResourceString("Device_Capacity_5000_Units"); } }
//Resources:DeviceManagementResources:Device_Capacity_50000_Units

		public static string Device_Capacity_50000_Units { get { return GetResourceString("Device_Capacity_50000_Units"); } }
//Resources:DeviceManagementResources:Device_Capacity_500000_Units

		public static string Device_Capacity_500000_Units { get { return GetResourceString("Device_Capacity_500000_Units"); } }
//Resources:DeviceManagementResources:Device_Capacity_Custom

		public static string Device_Capacity_Custom { get { return GetResourceString("Device_Capacity_Custom"); } }
//Resources:DeviceManagementResources:Device_ConnectionEstablishedTimeStamp

		public static string Device_ConnectionEstablishedTimeStamp { get { return GetResourceString("Device_ConnectionEstablishedTimeStamp"); } }
//Resources:DeviceManagementResources:Device_CustomStatus

		public static string Device_CustomStatus { get { return GetResourceString("Device_CustomStatus"); } }
//Resources:DeviceManagementResources:Device_CustomStatus_Help

		public static string Device_CustomStatus_Help { get { return GetResourceString("Device_CustomStatus_Help"); } }
//Resources:DeviceManagementResources:Device_DateProvisioned

		public static string Device_DateProvisioned { get { return GetResourceString("Device_DateProvisioned"); } }
//Resources:DeviceManagementResources:Device_DebugMode

		public static string Device_DebugMode { get { return GetResourceString("Device_DebugMode"); } }
//Resources:DeviceManagementResources:Device_DebugMode_Help

		public static string Device_DebugMode_Help { get { return GetResourceString("Device_DebugMode_Help"); } }
//Resources:DeviceManagementResources:Device_DefaultImage

		public static string Device_DefaultImage { get { return GetResourceString("Device_DefaultImage"); } }
//Resources:DeviceManagementResources:Device_Description

		public static string Device_Description { get { return GetResourceString("Device_Description"); } }
//Resources:DeviceManagementResources:Device_DesiredFirmware

		public static string Device_DesiredFirmware { get { return GetResourceString("Device_DesiredFirmware"); } }
//Resources:DeviceManagementResources:Device_DesiredFirmwareRevision

		public static string Device_DesiredFirmwareRevision { get { return GetResourceString("Device_DesiredFirmwareRevision"); } }
//Resources:DeviceManagementResources:Device_DeviceConfiguration

		public static string Device_DeviceConfiguration { get { return GetResourceString("Device_DeviceConfiguration"); } }
//Resources:DeviceManagementResources:Device_DeviceConfiguration_Select

		public static string Device_DeviceConfiguration_Select { get { return GetResourceString("Device_DeviceConfiguration_Select"); } }
//Resources:DeviceManagementResources:Device_DeviceId

		public static string Device_DeviceId { get { return GetResourceString("Device_DeviceId"); } }
//Resources:DeviceManagementResources:Device_DeviceImages

		public static string Device_DeviceImages { get { return GetResourceString("Device_DeviceImages"); } }
//Resources:DeviceManagementResources:Device_DeviceType

		public static string Device_DeviceType { get { return GetResourceString("Device_DeviceType"); } }
//Resources:DeviceManagementResources:Device_DeviceType_Select

		public static string Device_DeviceType_Select { get { return GetResourceString("Device_DeviceType_Select"); } }
//Resources:DeviceManagementResources:Device_DeviceURI

		public static string Device_DeviceURI { get { return GetResourceString("Device_DeviceURI"); } }
//Resources:DeviceManagementResources:Device_DeviceURI_Help

		public static string Device_DeviceURI_Help { get { return GetResourceString("Device_DeviceURI_Help"); } }
//Resources:DeviceManagementResources:Device_GeoLocation

		public static string Device_GeoLocation { get { return GetResourceString("Device_GeoLocation"); } }
//Resources:DeviceManagementResources:Device_GeoLocation_Help

		public static string Device_GeoLocation_Help { get { return GetResourceString("Device_GeoLocation_Help"); } }
//Resources:DeviceManagementResources:Device_Heading

		public static string Device_Heading { get { return GetResourceString("Device_Heading"); } }
//Resources:DeviceManagementResources:Device_Heading_Help

		public static string Device_Heading_Help { get { return GetResourceString("Device_Heading_Help"); } }
//Resources:DeviceManagementResources:Device_Help

		public static string Device_Help { get { return GetResourceString("Device_Help"); } }
//Resources:DeviceManagementResources:Device_ImageURL

		public static string Device_ImageURL { get { return GetResourceString("Device_ImageURL"); } }
//Resources:DeviceManagementResources:Device_inputCommandEndPoints

		public static string Device_inputCommandEndPoints { get { return GetResourceString("Device_inputCommandEndPoints"); } }
//Resources:DeviceManagementResources:Device_inputCommandEndPoints_Help

		public static string Device_inputCommandEndPoints_Help { get { return GetResourceString("Device_inputCommandEndPoints_Help"); } }
//Resources:DeviceManagementResources:Device_IsBeta

		public static string Device_IsBeta { get { return GetResourceString("Device_IsBeta"); } }
//Resources:DeviceManagementResources:Device_IsBeta_Help

		public static string Device_IsBeta_Help { get { return GetResourceString("Device_IsBeta_Help"); } }
//Resources:DeviceManagementResources:Device_IsConnected

		public static string Device_IsConnected { get { return GetResourceString("Device_IsConnected"); } }
//Resources:DeviceManagementResources:Device_LastContact

		public static string Device_LastContact { get { return GetResourceString("Device_LastContact"); } }
//Resources:DeviceManagementResources:Device_Location

		public static string Device_Location { get { return GetResourceString("Device_Location"); } }
//Resources:DeviceManagementResources:Device_Location_Select

		public static string Device_Location_Select { get { return GetResourceString("Device_Location_Select"); } }
//Resources:DeviceManagementResources:Device_MessageValues

		public static string Device_MessageValues { get { return GetResourceString("Device_MessageValues"); } }
//Resources:DeviceManagementResources:Device_MessageValues_Help

		public static string Device_MessageValues_Help { get { return GetResourceString("Device_MessageValues_Help"); } }
//Resources:DeviceManagementResources:Device_Notes

		public static string Device_Notes { get { return GetResourceString("Device_Notes"); } }
//Resources:DeviceManagementResources:Device_Organization

		public static string Device_Organization { get { return GetResourceString("Device_Organization"); } }
//Resources:DeviceManagementResources:Device_Organization_Select

		public static string Device_Organization_Select { get { return GetResourceString("Device_Organization_Select"); } }
//Resources:DeviceManagementResources:Device_ParentDevice

		public static string Device_ParentDevice { get { return GetResourceString("Device_ParentDevice"); } }
//Resources:DeviceManagementResources:Device_PrimaryKey

		public static string Device_PrimaryKey { get { return GetResourceString("Device_PrimaryKey"); } }
//Resources:DeviceManagementResources:Device_Properties

		public static string Device_Properties { get { return GetResourceString("Device_Properties"); } }
//Resources:DeviceManagementResources:Device_Properties_Help

		public static string Device_Properties_Help { get { return GetResourceString("Device_Properties_Help"); } }
//Resources:DeviceManagementResources:Device_Repo_AccessKey

		public static string Device_Repo_AccessKey { get { return GetResourceString("Device_Repo_AccessKey"); } }
//Resources:DeviceManagementResources:Device_Repo_AccessKeyName

		public static string Device_Repo_AccessKeyName { get { return GetResourceString("Device_Repo_AccessKeyName"); } }
//Resources:DeviceManagementResources:Device_Repo_AssignedUser

		public static string Device_Repo_AssignedUser { get { return GetResourceString("Device_Repo_AssignedUser"); } }
//Resources:DeviceManagementResources:Device_Repo_AssignedUser_Help

		public static string Device_Repo_AssignedUser_Help { get { return GetResourceString("Device_Repo_AssignedUser_Help"); } }
//Resources:DeviceManagementResources:Device_Repo_AssignedUser_Select

		public static string Device_Repo_AssignedUser_Select { get { return GetResourceString("Device_Repo_AssignedUser_Select"); } }
//Resources:DeviceManagementResources:Device_Repo_AuthKey1

		public static string Device_Repo_AuthKey1 { get { return GetResourceString("Device_Repo_AuthKey1"); } }
//Resources:DeviceManagementResources:Device_Repo_AuthKey2

		public static string Device_Repo_AuthKey2 { get { return GetResourceString("Device_Repo_AuthKey2"); } }
//Resources:DeviceManagementResources:Device_Repo_Description

		public static string Device_Repo_Description { get { return GetResourceString("Device_Repo_Description"); } }
//Resources:DeviceManagementResources:Device_Repo_DevceWatchDog_NotificationContact

		public static string Device_Repo_DevceWatchDog_NotificationContact { get { return GetResourceString("Device_Repo_DevceWatchDog_NotificationContact"); } }
//Resources:DeviceManagementResources:Device_Repo_DevceWatchDog_NotificationContact_Help

		public static string Device_Repo_DevceWatchDog_NotificationContact_Help { get { return GetResourceString("Device_Repo_DevceWatchDog_NotificationContact_Help"); } }
//Resources:DeviceManagementResources:Device_Repo_DevceWatchDog_NotificationContact_Select

		public static string Device_Repo_DevceWatchDog_NotificationContact_Select { get { return GetResourceString("Device_Repo_DevceWatchDog_NotificationContact_Select"); } }
//Resources:DeviceManagementResources:Device_Repo_DevicesInUse

		public static string Device_Repo_DevicesInUse { get { return GetResourceString("Device_Repo_DevicesInUse"); } }
//Resources:DeviceManagementResources:Device_Repo_Help

		public static string Device_Repo_Help { get { return GetResourceString("Device_Repo_Help"); } }
//Resources:DeviceManagementResources:Device_Repo_Instance

		public static string Device_Repo_Instance { get { return GetResourceString("Device_Repo_Instance"); } }
//Resources:DeviceManagementResources:Device_Repo_MaxDevices

		public static string Device_Repo_MaxDevices { get { return GetResourceString("Device_Repo_MaxDevices"); } }
//Resources:DeviceManagementResources:Device_Repo_RepoType

		public static string Device_Repo_RepoType { get { return GetResourceString("Device_Repo_RepoType"); } }
//Resources:DeviceManagementResources:Device_Repo_RepoType_AzureIoTHub

		public static string Device_Repo_RepoType_AzureIoTHub { get { return GetResourceString("Device_Repo_RepoType_AzureIoTHub"); } }
//Resources:DeviceManagementResources:Device_Repo_RepoType_Dedicated

		public static string Device_Repo_RepoType_Dedicated { get { return GetResourceString("Device_Repo_RepoType_Dedicated"); } }
//Resources:DeviceManagementResources:Device_Repo_RepoType_Help

		public static string Device_Repo_RepoType_Help { get { return GetResourceString("Device_Repo_RepoType_Help"); } }
//Resources:DeviceManagementResources:Device_Repo_RepoType_InClusterMongoDB

		public static string Device_Repo_RepoType_InClusterMongoDB { get { return GetResourceString("Device_Repo_RepoType_InClusterMongoDB"); } }
//Resources:DeviceManagementResources:Device_Repo_RepoType_Local

		public static string Device_Repo_RepoType_Local { get { return GetResourceString("Device_Repo_RepoType_Local"); } }
//Resources:DeviceManagementResources:Device_Repo_RepoType_NuvIoT

		public static string Device_Repo_RepoType_NuvIoT { get { return GetResourceString("Device_Repo_RepoType_NuvIoT"); } }
//Resources:DeviceManagementResources:Device_Repo_RepoType_Select

		public static string Device_Repo_RepoType_Select { get { return GetResourceString("Device_Repo_RepoType_Select"); } }
//Resources:DeviceManagementResources:Device_Repo_ResourceName

		public static string Device_Repo_ResourceName { get { return GetResourceString("Device_Repo_ResourceName"); } }
//Resources:DeviceManagementResources:Device_Repo_StorageCapacity

		public static string Device_Repo_StorageCapacity { get { return GetResourceString("Device_Repo_StorageCapacity"); } }
//Resources:DeviceManagementResources:Device_Repo_StorageCapacity_Select

		public static string Device_Repo_StorageCapacity_Select { get { return GetResourceString("Device_Repo_StorageCapacity_Select"); } }
//Resources:DeviceManagementResources:Device_Repo_Subscription

		public static string Device_Repo_Subscription { get { return GetResourceString("Device_Repo_Subscription"); } }
//Resources:DeviceManagementResources:Device_Repo_SubscriptionSelect

		public static string Device_Repo_SubscriptionSelect { get { return GetResourceString("Device_Repo_SubscriptionSelect"); } }
//Resources:DeviceManagementResources:Device_Repo_UnitCapacity

		public static string Device_Repo_UnitCapacity { get { return GetResourceString("Device_Repo_UnitCapacity"); } }
//Resources:DeviceManagementResources:Device_Repo_UnitCapacity_Select

		public static string Device_Repo_UnitCapacity_Select { get { return GetResourceString("Device_Repo_UnitCapacity_Select"); } }
//Resources:DeviceManagementResources:Device_RepoTitle

		public static string Device_RepoTitle { get { return GetResourceString("Device_RepoTitle"); } }
//Resources:DeviceManagementResources:Device_SecondaryKey

		public static string Device_SecondaryKey { get { return GetResourceString("Device_SecondaryKey"); } }
//Resources:DeviceManagementResources:Device_SerialNumber

		public static string Device_SerialNumber { get { return GetResourceString("Device_SerialNumber"); } }
//Resources:DeviceManagementResources:Device_ShowDiagnostics

		public static string Device_ShowDiagnostics { get { return GetResourceString("Device_ShowDiagnostics"); } }
//Resources:DeviceManagementResources:Device_ShowDiagnostics_Help

		public static string Device_ShowDiagnostics_Help { get { return GetResourceString("Device_ShowDiagnostics_Help"); } }
//Resources:DeviceManagementResources:Device_SilenceAlarms

		public static string Device_SilenceAlarms { get { return GetResourceString("Device_SilenceAlarms"); } }
//Resources:DeviceManagementResources:Device_SilenceAlarms_Help

		public static string Device_SilenceAlarms_Help { get { return GetResourceString("Device_SilenceAlarms_Help"); } }
//Resources:DeviceManagementResources:Device_Speed

		public static string Device_Speed { get { return GetResourceString("Device_Speed"); } }
//Resources:DeviceManagementResources:Device_Speed_Help

		public static string Device_Speed_Help { get { return GetResourceString("Device_Speed_Help"); } }
//Resources:DeviceManagementResources:Device_StateMachineMetaData

		public static string Device_StateMachineMetaData { get { return GetResourceString("Device_StateMachineMetaData"); } }
//Resources:DeviceManagementResources:Device_States

		public static string Device_States { get { return GetResourceString("Device_States"); } }
//Resources:DeviceManagementResources:Device_States_Help

		public static string Device_States_Help { get { return GetResourceString("Device_States_Help"); } }
//Resources:DeviceManagementResources:Device_Status

		public static string Device_Status { get { return GetResourceString("Device_Status"); } }
//Resources:DeviceManagementResources:Device_Status_Commissioned

		public static string Device_Status_Commissioned { get { return GetResourceString("Device_Status_Commissioned"); } }
//Resources:DeviceManagementResources:Device_Status_Commissioned_Help

		public static string Device_Status_Commissioned_Help { get { return GetResourceString("Device_Status_Commissioned_Help"); } }
//Resources:DeviceManagementResources:Device_Status_Decommissioned

		public static string Device_Status_Decommissioned { get { return GetResourceString("Device_Status_Decommissioned"); } }
//Resources:DeviceManagementResources:Device_Status_Degraded

		public static string Device_Status_Degraded { get { return GetResourceString("Device_Status_Degraded"); } }
//Resources:DeviceManagementResources:Device_Status_Error

		public static string Device_Status_Error { get { return GetResourceString("Device_Status_Error"); } }
//Resources:DeviceManagementResources:Device_Status_New

		public static string Device_Status_New { get { return GetResourceString("Device_Status_New"); } }
//Resources:DeviceManagementResources:Device_Status_PastDue

		public static string Device_Status_PastDue { get { return GetResourceString("Device_Status_PastDue"); } }
//Resources:DeviceManagementResources:Device_Status_PastDue_Help

		public static string Device_Status_PastDue_Help { get { return GetResourceString("Device_Status_PastDue_Help"); } }
//Resources:DeviceManagementResources:Device_Status_Ready

		public static string Device_Status_Ready { get { return GetResourceString("Device_Status_Ready"); } }
//Resources:DeviceManagementResources:Device_Status_Select

		public static string Device_Status_Select { get { return GetResourceString("Device_Status_Select"); } }
//Resources:DeviceManagementResources:Device_Stauts_Degraded

		public static string Device_Stauts_Degraded { get { return GetResourceString("Device_Stauts_Degraded"); } }
//Resources:DeviceManagementResources:Device_Storage_100GB

		public static string Device_Storage_100GB { get { return GetResourceString("Device_Storage_100GB"); } }
//Resources:DeviceManagementResources:Device_Storage_1GB

		public static string Device_Storage_1GB { get { return GetResourceString("Device_Storage_1GB"); } }
//Resources:DeviceManagementResources:Device_Storage_1TB

		public static string Device_Storage_1TB { get { return GetResourceString("Device_Storage_1TB"); } }
//Resources:DeviceManagementResources:Device_Storage_20GB

		public static string Device_Storage_20GB { get { return GetResourceString("Device_Storage_20GB"); } }
//Resources:DeviceManagementResources:Device_Storage_20MB

		public static string Device_Storage_20MB { get { return GetResourceString("Device_Storage_20MB"); } }
//Resources:DeviceManagementResources:Device_Storage_500GB

		public static string Device_Storage_500GB { get { return GetResourceString("Device_Storage_500GB"); } }
//Resources:DeviceManagementResources:Device_Storage_5TB

		public static string Device_Storage_5TB { get { return GetResourceString("Device_Storage_5TB"); } }
//Resources:DeviceManagementResources:Device_Storage_Custom

		public static string Device_Storage_Custom { get { return GetResourceString("Device_Storage_Custom"); } }
//Resources:DeviceManagementResources:Device_Title

		public static string Device_Title { get { return GetResourceString("Device_Title"); } }
//Resources:DeviceManagementResources:Device_Watchdog_Disable_Override

		public static string Device_Watchdog_Disable_Override { get { return GetResourceString("Device_Watchdog_Disable_Override"); } }
//Resources:DeviceManagementResources:Device_Watchdog_Disable_Override_Help

		public static string Device_Watchdog_Disable_Override_Help { get { return GetResourceString("Device_Watchdog_Disable_Override_Help"); } }
//Resources:DeviceManagementResources:Device_Watchdog_Notification_User

		public static string Device_Watchdog_Notification_User { get { return GetResourceString("Device_Watchdog_Notification_User"); } }
//Resources:DeviceManagementResources:Device_Watchdog_Notification_User_Help

		public static string Device_Watchdog_Notification_User_Help { get { return GetResourceString("Device_Watchdog_Notification_User_Help"); } }
//Resources:DeviceManagementResources:Device_Watchdog_Notification_User_Select

		public static string Device_Watchdog_Notification_User_Select { get { return GetResourceString("Device_Watchdog_Notification_User_Select"); } }
//Resources:DeviceManagementResources:Device_Watchdog_Seconds_Override

		public static string Device_Watchdog_Seconds_Override { get { return GetResourceString("Device_Watchdog_Seconds_Override"); } }
//Resources:DeviceManagementResources:Device_Watchdog_Seconds_Override_Help

		public static string Device_Watchdog_Seconds_Override_Help { get { return GetResourceString("Device_Watchdog_Seconds_Override_Help"); } }
//Resources:DeviceManagementResources:DeviceAddress_Address

		public static string DeviceAddress_Address { get { return GetResourceString("DeviceAddress_Address"); } }
//Resources:DeviceManagementResources:DeviceGroup_AssignedUser

		public static string DeviceGroup_AssignedUser { get { return GetResourceString("DeviceGroup_AssignedUser"); } }
//Resources:DeviceManagementResources:DeviceGroup_AssignedUserHelp

		public static string DeviceGroup_AssignedUserHelp { get { return GetResourceString("DeviceGroup_AssignedUserHelp"); } }
//Resources:DeviceManagementResources:DeviceGroup_Description

		public static string DeviceGroup_Description { get { return GetResourceString("DeviceGroup_Description"); } }
//Resources:DeviceManagementResources:DeviceGroup_Devices

		public static string DeviceGroup_Devices { get { return GetResourceString("DeviceGroup_Devices"); } }
//Resources:DeviceManagementResources:DeviceGroup_Devices_Help

		public static string DeviceGroup_Devices_Help { get { return GetResourceString("DeviceGroup_Devices_Help"); } }
//Resources:DeviceManagementResources:DeviceGroup_Help

		public static string DeviceGroup_Help { get { return GetResourceString("DeviceGroup_Help"); } }
//Resources:DeviceManagementResources:DeviceGroup_Repository

		public static string DeviceGroup_Repository { get { return GetResourceString("DeviceGroup_Repository"); } }
//Resources:DeviceManagementResources:DeviceGroup_Repository_Help

		public static string DeviceGroup_Repository_Help { get { return GetResourceString("DeviceGroup_Repository_Help"); } }
//Resources:DeviceManagementResources:DeviceGroup_Title

		public static string DeviceGroup_Title { get { return GetResourceString("DeviceGroup_Title"); } }
//Resources:DeviceManagementResources:DeviceNotes_Description

		public static string DeviceNotes_Description { get { return GetResourceString("DeviceNotes_Description"); } }
//Resources:DeviceManagementResources:DeviceNotes_Help

		public static string DeviceNotes_Help { get { return GetResourceString("DeviceNotes_Help"); } }
//Resources:DeviceManagementResources:DeviceNotes_Notes

		public static string DeviceNotes_Notes { get { return GetResourceString("DeviceNotes_Notes"); } }
//Resources:DeviceManagementResources:DeviceNotes_Title

		public static string DeviceNotes_Title { get { return GetResourceString("DeviceNotes_Title"); } }
//Resources:DeviceManagementResources:DeviceNotes_TitleField

		public static string DeviceNotes_TitleField { get { return GetResourceString("DeviceNotes_TitleField"); } }
//Resources:DeviceManagementResources:DeviceRepo_ServiceBoard

		public static string DeviceRepo_ServiceBoard { get { return GetResourceString("DeviceRepo_ServiceBoard"); } }
//Resources:DeviceManagementResources:DeviceRepo_ServiceBoard_Help

		public static string DeviceRepo_ServiceBoard_Help { get { return GetResourceString("DeviceRepo_ServiceBoard_Help"); } }
//Resources:DeviceManagementResources:DeviceRepo_ServiceBoard_Select

		public static string DeviceRepo_ServiceBoard_Select { get { return GetResourceString("DeviceRepo_ServiceBoard_Select"); } }
//Resources:DeviceManagementResources:Firmware_Default

		public static string Firmware_Default { get { return GetResourceString("Firmware_Default"); } }
//Resources:DeviceManagementResources:Firmware_Default_Select

		public static string Firmware_Default_Select { get { return GetResourceString("Firmware_Default_Select"); } }
//Resources:DeviceManagementResources:Firmware_Description

		public static string Firmware_Description { get { return GetResourceString("Firmware_Description"); } }
//Resources:DeviceManagementResources:Firmware_DeviceType

		public static string Firmware_DeviceType { get { return GetResourceString("Firmware_DeviceType"); } }
//Resources:DeviceManagementResources:Firmware_FirmwareSKU

		public static string Firmware_FirmwareSKU { get { return GetResourceString("Firmware_FirmwareSKU"); } }
//Resources:DeviceManagementResources:Firmware_Help

		public static string Firmware_Help { get { return GetResourceString("Firmware_Help"); } }
//Resources:DeviceManagementResources:Firmware_Title

		public static string Firmware_Title { get { return GetResourceString("Firmware_Title"); } }
//Resources:DeviceManagementResources:FirmwareReivsion_TimeStamp

		public static string FirmwareReivsion_TimeStamp { get { return GetResourceString("FirmwareReivsion_TimeStamp"); } }
//Resources:DeviceManagementResources:FirmwareRevision_Description

		public static string FirmwareRevision_Description { get { return GetResourceString("FirmwareRevision_Description"); } }
//Resources:DeviceManagementResources:FirmwareRevision_Help

		public static string FirmwareRevision_Help { get { return GetResourceString("FirmwareRevision_Help"); } }
//Resources:DeviceManagementResources:FirmwareRevision_Notes

		public static string FirmwareRevision_Notes { get { return GetResourceString("FirmwareRevision_Notes"); } }
//Resources:DeviceManagementResources:FirmwareRevision_Status

		public static string FirmwareRevision_Status { get { return GetResourceString("FirmwareRevision_Status"); } }
//Resources:DeviceManagementResources:FirmwareRevision_Status_Alpha

		public static string FirmwareRevision_Status_Alpha { get { return GetResourceString("FirmwareRevision_Status_Alpha"); } }
//Resources:DeviceManagementResources:FirmwareRevision_Status_Beta

		public static string FirmwareRevision_Status_Beta { get { return GetResourceString("FirmwareRevision_Status_Beta"); } }
//Resources:DeviceManagementResources:FirmwareRevision_Status_Obsolete

		public static string FirmwareRevision_Status_Obsolete { get { return GetResourceString("FirmwareRevision_Status_Obsolete"); } }
//Resources:DeviceManagementResources:FirmwareRevision_Status_Production

		public static string FirmwareRevision_Status_Production { get { return GetResourceString("FirmwareRevision_Status_Production"); } }
//Resources:DeviceManagementResources:FirmwareRevision_Status_Prototype

		public static string FirmwareRevision_Status_Prototype { get { return GetResourceString("FirmwareRevision_Status_Prototype"); } }
//Resources:DeviceManagementResources:FirmwareRevision_Status_Select

		public static string FirmwareRevision_Status_Select { get { return GetResourceString("FirmwareRevision_Status_Select"); } }
//Resources:DeviceManagementResources:FirmwareRevision_Title

		public static string FirmwareRevision_Title { get { return GetResourceString("FirmwareRevision_Title"); } }
//Resources:DeviceManagementResources:FirmwareRevision_Version

		public static string FirmwareRevision_Version { get { return GetResourceString("FirmwareRevision_Version"); } }
//Resources:DeviceManagementResources:FirmwareRevision_VersionCodeRegEx

		public static string FirmwareRevision_VersionCodeRegEx { get { return GetResourceString("FirmwareRevision_VersionCodeRegEx"); } }

		public static class Names
		{
			public const string AttributeValue_Description = "AttributeValue_Description";
			public const string AttributeValue_Help = "AttributeValue_Help";
			public const string AttributeValue_Inference = "AttributeValue_Inference";
			public const string AttributeValue_Key = "AttributeValue_Key";
			public const string AttributeValue_LastUpdated = "AttributeValue_LastUpdated";
			public const string AttributeValue_LastUpdatedBy = "AttributeValue_LastUpdatedBy";
			public const string AttributeValue_Name = "AttributeValue_Name";
			public const string AttributeValue_State = "AttributeValue_State";
			public const string AttributeValue_Title = "AttributeValue_Title";
			public const string AttributeValue_Type = "AttributeValue_Type";
			public const string AttributeValue_Unit = "AttributeValue_Unit";
			public const string AttributeValue_Value = "AttributeValue_Value";
			public const string Common_Description = "Common_Description";
			public const string Common_Key = "Common_Key";
			public const string Common_Key_Help = "Common_Key_Help";
			public const string Common_Key_Validation = "Common_Key_Validation";
			public const string Common_Name = "Common_Name";
			public const string Common_Notes = "Common_Notes";
			public const string DeivceNotes_Description = "DeivceNotes_Description";
			public const string Device_ActualFirmware = "Device_ActualFirmware";
			public const string Device_ActualFirmware_Date = "Device_ActualFirmware_Date";
			public const string Device_ActualFirmware_Date_Help = "Device_ActualFirmware_Date_Help";
			public const string Device_ActualFirmware_Revision = "Device_ActualFirmware_Revision";
			public const string Device_AssignedUser = "Device_AssignedUser";
			public const string Device_AssignedUser_Select = "Device_AssignedUser_Select";
			public const string Device_AssignedUserHelp = "Device_AssignedUserHelp";
			public const string Device_AttributeMetaData = "Device_AttributeMetaData";
			public const string Device_Attributes = "Device_Attributes";
			public const string Device_Attributes_Help = "Device_Attributes_Help";
			public const string Device_Capacity_100_Units = "Device_Capacity_100_Units";
			public const string Device_Capacity_1000_Units = "Device_Capacity_1000_Units";
			public const string Device_Capacity_10000_Units = "Device_Capacity_10000_Units";
			public const string Device_Capacity_100000_Units = "Device_Capacity_100000_Units";
			public const string Device_Capacity_1000000_Units = "Device_Capacity_1000000_Units";
			public const string Device_Capacity_5_Units = "Device_Capacity_5_Units";
			public const string Device_Capacity_500_Units = "Device_Capacity_500_Units";
			public const string Device_Capacity_5000_Units = "Device_Capacity_5000_Units";
			public const string Device_Capacity_50000_Units = "Device_Capacity_50000_Units";
			public const string Device_Capacity_500000_Units = "Device_Capacity_500000_Units";
			public const string Device_Capacity_Custom = "Device_Capacity_Custom";
			public const string Device_ConnectionEstablishedTimeStamp = "Device_ConnectionEstablishedTimeStamp";
			public const string Device_CustomStatus = "Device_CustomStatus";
			public const string Device_CustomStatus_Help = "Device_CustomStatus_Help";
			public const string Device_DateProvisioned = "Device_DateProvisioned";
			public const string Device_DebugMode = "Device_DebugMode";
			public const string Device_DebugMode_Help = "Device_DebugMode_Help";
			public const string Device_DefaultImage = "Device_DefaultImage";
			public const string Device_Description = "Device_Description";
			public const string Device_DesiredFirmware = "Device_DesiredFirmware";
			public const string Device_DesiredFirmwareRevision = "Device_DesiredFirmwareRevision";
			public const string Device_DeviceConfiguration = "Device_DeviceConfiguration";
			public const string Device_DeviceConfiguration_Select = "Device_DeviceConfiguration_Select";
			public const string Device_DeviceId = "Device_DeviceId";
			public const string Device_DeviceImages = "Device_DeviceImages";
			public const string Device_DeviceType = "Device_DeviceType";
			public const string Device_DeviceType_Select = "Device_DeviceType_Select";
			public const string Device_DeviceURI = "Device_DeviceURI";
			public const string Device_DeviceURI_Help = "Device_DeviceURI_Help";
			public const string Device_GeoLocation = "Device_GeoLocation";
			public const string Device_GeoLocation_Help = "Device_GeoLocation_Help";
			public const string Device_Heading = "Device_Heading";
			public const string Device_Heading_Help = "Device_Heading_Help";
			public const string Device_Help = "Device_Help";
			public const string Device_ImageURL = "Device_ImageURL";
			public const string Device_inputCommandEndPoints = "Device_inputCommandEndPoints";
			public const string Device_inputCommandEndPoints_Help = "Device_inputCommandEndPoints_Help";
			public const string Device_IsBeta = "Device_IsBeta";
			public const string Device_IsBeta_Help = "Device_IsBeta_Help";
			public const string Device_IsConnected = "Device_IsConnected";
			public const string Device_LastContact = "Device_LastContact";
			public const string Device_Location = "Device_Location";
			public const string Device_Location_Select = "Device_Location_Select";
			public const string Device_MessageValues = "Device_MessageValues";
			public const string Device_MessageValues_Help = "Device_MessageValues_Help";
			public const string Device_Notes = "Device_Notes";
			public const string Device_Organization = "Device_Organization";
			public const string Device_Organization_Select = "Device_Organization_Select";
			public const string Device_ParentDevice = "Device_ParentDevice";
			public const string Device_PrimaryKey = "Device_PrimaryKey";
			public const string Device_Properties = "Device_Properties";
			public const string Device_Properties_Help = "Device_Properties_Help";
			public const string Device_Repo_AccessKey = "Device_Repo_AccessKey";
			public const string Device_Repo_AccessKeyName = "Device_Repo_AccessKeyName";
			public const string Device_Repo_AssignedUser = "Device_Repo_AssignedUser";
			public const string Device_Repo_AssignedUser_Help = "Device_Repo_AssignedUser_Help";
			public const string Device_Repo_AssignedUser_Select = "Device_Repo_AssignedUser_Select";
			public const string Device_Repo_AuthKey1 = "Device_Repo_AuthKey1";
			public const string Device_Repo_AuthKey2 = "Device_Repo_AuthKey2";
			public const string Device_Repo_Description = "Device_Repo_Description";
			public const string Device_Repo_DevceWatchDog_NotificationContact = "Device_Repo_DevceWatchDog_NotificationContact";
			public const string Device_Repo_DevceWatchDog_NotificationContact_Help = "Device_Repo_DevceWatchDog_NotificationContact_Help";
			public const string Device_Repo_DevceWatchDog_NotificationContact_Select = "Device_Repo_DevceWatchDog_NotificationContact_Select";
			public const string Device_Repo_DevicesInUse = "Device_Repo_DevicesInUse";
			public const string Device_Repo_Help = "Device_Repo_Help";
			public const string Device_Repo_Instance = "Device_Repo_Instance";
			public const string Device_Repo_MaxDevices = "Device_Repo_MaxDevices";
			public const string Device_Repo_RepoType = "Device_Repo_RepoType";
			public const string Device_Repo_RepoType_AzureIoTHub = "Device_Repo_RepoType_AzureIoTHub";
			public const string Device_Repo_RepoType_Dedicated = "Device_Repo_RepoType_Dedicated";
			public const string Device_Repo_RepoType_Help = "Device_Repo_RepoType_Help";
			public const string Device_Repo_RepoType_InClusterMongoDB = "Device_Repo_RepoType_InClusterMongoDB";
			public const string Device_Repo_RepoType_Local = "Device_Repo_RepoType_Local";
			public const string Device_Repo_RepoType_NuvIoT = "Device_Repo_RepoType_NuvIoT";
			public const string Device_Repo_RepoType_Select = "Device_Repo_RepoType_Select";
			public const string Device_Repo_ResourceName = "Device_Repo_ResourceName";
			public const string Device_Repo_StorageCapacity = "Device_Repo_StorageCapacity";
			public const string Device_Repo_StorageCapacity_Select = "Device_Repo_StorageCapacity_Select";
			public const string Device_Repo_Subscription = "Device_Repo_Subscription";
			public const string Device_Repo_SubscriptionSelect = "Device_Repo_SubscriptionSelect";
			public const string Device_Repo_UnitCapacity = "Device_Repo_UnitCapacity";
			public const string Device_Repo_UnitCapacity_Select = "Device_Repo_UnitCapacity_Select";
			public const string Device_RepoTitle = "Device_RepoTitle";
			public const string Device_SecondaryKey = "Device_SecondaryKey";
			public const string Device_SerialNumber = "Device_SerialNumber";
			public const string Device_ShowDiagnostics = "Device_ShowDiagnostics";
			public const string Device_ShowDiagnostics_Help = "Device_ShowDiagnostics_Help";
			public const string Device_SilenceAlarms = "Device_SilenceAlarms";
			public const string Device_SilenceAlarms_Help = "Device_SilenceAlarms_Help";
			public const string Device_Speed = "Device_Speed";
			public const string Device_Speed_Help = "Device_Speed_Help";
			public const string Device_StateMachineMetaData = "Device_StateMachineMetaData";
			public const string Device_States = "Device_States";
			public const string Device_States_Help = "Device_States_Help";
			public const string Device_Status = "Device_Status";
			public const string Device_Status_Commissioned = "Device_Status_Commissioned";
			public const string Device_Status_Commissioned_Help = "Device_Status_Commissioned_Help";
			public const string Device_Status_Decommissioned = "Device_Status_Decommissioned";
			public const string Device_Status_Degraded = "Device_Status_Degraded";
			public const string Device_Status_Error = "Device_Status_Error";
			public const string Device_Status_New = "Device_Status_New";
			public const string Device_Status_PastDue = "Device_Status_PastDue";
			public const string Device_Status_PastDue_Help = "Device_Status_PastDue_Help";
			public const string Device_Status_Ready = "Device_Status_Ready";
			public const string Device_Status_Select = "Device_Status_Select";
			public const string Device_Stauts_Degraded = "Device_Stauts_Degraded";
			public const string Device_Storage_100GB = "Device_Storage_100GB";
			public const string Device_Storage_1GB = "Device_Storage_1GB";
			public const string Device_Storage_1TB = "Device_Storage_1TB";
			public const string Device_Storage_20GB = "Device_Storage_20GB";
			public const string Device_Storage_20MB = "Device_Storage_20MB";
			public const string Device_Storage_500GB = "Device_Storage_500GB";
			public const string Device_Storage_5TB = "Device_Storage_5TB";
			public const string Device_Storage_Custom = "Device_Storage_Custom";
			public const string Device_Title = "Device_Title";
			public const string Device_Watchdog_Disable_Override = "Device_Watchdog_Disable_Override";
			public const string Device_Watchdog_Disable_Override_Help = "Device_Watchdog_Disable_Override_Help";
			public const string Device_Watchdog_Notification_User = "Device_Watchdog_Notification_User";
			public const string Device_Watchdog_Notification_User_Help = "Device_Watchdog_Notification_User_Help";
			public const string Device_Watchdog_Notification_User_Select = "Device_Watchdog_Notification_User_Select";
			public const string Device_Watchdog_Seconds_Override = "Device_Watchdog_Seconds_Override";
			public const string Device_Watchdog_Seconds_Override_Help = "Device_Watchdog_Seconds_Override_Help";
			public const string DeviceAddress_Address = "DeviceAddress_Address";
			public const string DeviceGroup_AssignedUser = "DeviceGroup_AssignedUser";
			public const string DeviceGroup_AssignedUserHelp = "DeviceGroup_AssignedUserHelp";
			public const string DeviceGroup_Description = "DeviceGroup_Description";
			public const string DeviceGroup_Devices = "DeviceGroup_Devices";
			public const string DeviceGroup_Devices_Help = "DeviceGroup_Devices_Help";
			public const string DeviceGroup_Help = "DeviceGroup_Help";
			public const string DeviceGroup_Repository = "DeviceGroup_Repository";
			public const string DeviceGroup_Repository_Help = "DeviceGroup_Repository_Help";
			public const string DeviceGroup_Title = "DeviceGroup_Title";
			public const string DeviceNotes_Description = "DeviceNotes_Description";
			public const string DeviceNotes_Help = "DeviceNotes_Help";
			public const string DeviceNotes_Notes = "DeviceNotes_Notes";
			public const string DeviceNotes_Title = "DeviceNotes_Title";
			public const string DeviceNotes_TitleField = "DeviceNotes_TitleField";
			public const string DeviceRepo_ServiceBoard = "DeviceRepo_ServiceBoard";
			public const string DeviceRepo_ServiceBoard_Help = "DeviceRepo_ServiceBoard_Help";
			public const string DeviceRepo_ServiceBoard_Select = "DeviceRepo_ServiceBoard_Select";
			public const string Firmware_Default = "Firmware_Default";
			public const string Firmware_Default_Select = "Firmware_Default_Select";
			public const string Firmware_Description = "Firmware_Description";
			public const string Firmware_DeviceType = "Firmware_DeviceType";
			public const string Firmware_FirmwareSKU = "Firmware_FirmwareSKU";
			public const string Firmware_Help = "Firmware_Help";
			public const string Firmware_Title = "Firmware_Title";
			public const string FirmwareReivsion_TimeStamp = "FirmwareReivsion_TimeStamp";
			public const string FirmwareRevision_Description = "FirmwareRevision_Description";
			public const string FirmwareRevision_Help = "FirmwareRevision_Help";
			public const string FirmwareRevision_Notes = "FirmwareRevision_Notes";
			public const string FirmwareRevision_Status = "FirmwareRevision_Status";
			public const string FirmwareRevision_Status_Alpha = "FirmwareRevision_Status_Alpha";
			public const string FirmwareRevision_Status_Beta = "FirmwareRevision_Status_Beta";
			public const string FirmwareRevision_Status_Obsolete = "FirmwareRevision_Status_Obsolete";
			public const string FirmwareRevision_Status_Production = "FirmwareRevision_Status_Production";
			public const string FirmwareRevision_Status_Prototype = "FirmwareRevision_Status_Prototype";
			public const string FirmwareRevision_Status_Select = "FirmwareRevision_Status_Select";
			public const string FirmwareRevision_Title = "FirmwareRevision_Title";
			public const string FirmwareRevision_Version = "FirmwareRevision_Version";
			public const string FirmwareRevision_VersionCodeRegEx = "FirmwareRevision_VersionCodeRegEx";
		}
	}
}
