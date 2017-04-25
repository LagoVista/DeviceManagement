﻿using System.Globalization;
using System.Reflection;  

//Resources:DeviceManagementResources:Common_Key
namespace LagoVista.IoT.DeviceManagement.Core.Resources
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LagoVista.IoT.DeviceManagement.Core.Resources.DeviceManagementResources", typeof(DeviceManagementResources).GetTypeInfo().Assembly);
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
		
		public static string Common_Key { get { return GetResourceString("Common_Key"); } }
//Resources:DeviceManagementResources:Common_Key_Help

		public static string Common_Key_Help { get { return GetResourceString("Common_Key_Help"); } }
//Resources:DeviceManagementResources:Common_Key_Validation

		public static string Common_Key_Validation { get { return GetResourceString("Common_Key_Validation"); } }
//Resources:DeviceManagementResources:DeivceNotes_Description

		public static string DeivceNotes_Description { get { return GetResourceString("DeivceNotes_Description"); } }
//Resources:DeviceManagementResources:Device_DateProvisioned

		public static string Device_DateProvisioned { get { return GetResourceString("Device_DateProvisioned"); } }
//Resources:DeviceManagementResources:Device_Description

		public static string Device_Description { get { return GetResourceString("Device_Description"); } }
//Resources:DeviceManagementResources:Device_DeviceConfiguration

		public static string Device_DeviceConfiguration { get { return GetResourceString("Device_DeviceConfiguration"); } }
//Resources:DeviceManagementResources:Device_DeviceConfiguration_Select

		public static string Device_DeviceConfiguration_Select { get { return GetResourceString("Device_DeviceConfiguration_Select"); } }
//Resources:DeviceManagementResources:Device_DeviceId

		public static string Device_DeviceId { get { return GetResourceString("Device_DeviceId"); } }
//Resources:DeviceManagementResources:Device_FirmwareVersion

		public static string Device_FirmwareVersion { get { return GetResourceString("Device_FirmwareVersion"); } }
//Resources:DeviceManagementResources:Device_Help

		public static string Device_Help { get { return GetResourceString("Device_Help"); } }
//Resources:DeviceManagementResources:Device_IsConnected

		public static string Device_IsConnected { get { return GetResourceString("Device_IsConnected"); } }
//Resources:DeviceManagementResources:Device_LastContact

		public static string Device_LastContact { get { return GetResourceString("Device_LastContact"); } }
//Resources:DeviceManagementResources:Device_Location

		public static string Device_Location { get { return GetResourceString("Device_Location"); } }
//Resources:DeviceManagementResources:Device_Location_Select

		public static string Device_Location_Select { get { return GetResourceString("Device_Location_Select"); } }
//Resources:DeviceManagementResources:Device_Notes

		public static string Device_Notes { get { return GetResourceString("Device_Notes"); } }
//Resources:DeviceManagementResources:Device_Organization

		public static string Device_Organization { get { return GetResourceString("Device_Organization"); } }
//Resources:DeviceManagementResources:Device_Organization_Select

		public static string Device_Organization_Select { get { return GetResourceString("Device_Organization_Select"); } }
//Resources:DeviceManagementResources:Device_Properties

		public static string Device_Properties { get { return GetResourceString("Device_Properties"); } }
//Resources:DeviceManagementResources:Device_SerialNumber

		public static string Device_SerialNumber { get { return GetResourceString("Device_SerialNumber"); } }
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
//Resources:DeviceManagementResources:Device_Title

		public static string Device_Title { get { return GetResourceString("Device_Title"); } }
//Resources:DeviceManagementResources:DeviceGroup_Description

		public static string DeviceGroup_Description { get { return GetResourceString("DeviceGroup_Description"); } }
//Resources:DeviceManagementResources:DeviceGroup_Help

		public static string DeviceGroup_Help { get { return GetResourceString("DeviceGroup_Help"); } }
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

		public static class Names
		{
			public const string Common_Key = "Common_Key";
			public const string Common_Key_Help = "Common_Key_Help";
			public const string Common_Key_Validation = "Common_Key_Validation";
			public const string DeivceNotes_Description = "DeivceNotes_Description";
			public const string Device_DateProvisioned = "Device_DateProvisioned";
			public const string Device_Description = "Device_Description";
			public const string Device_DeviceConfiguration = "Device_DeviceConfiguration";
			public const string Device_DeviceConfiguration_Select = "Device_DeviceConfiguration_Select";
			public const string Device_DeviceId = "Device_DeviceId";
			public const string Device_FirmwareVersion = "Device_FirmwareVersion";
			public const string Device_Help = "Device_Help";
			public const string Device_IsConnected = "Device_IsConnected";
			public const string Device_LastContact = "Device_LastContact";
			public const string Device_Location = "Device_Location";
			public const string Device_Location_Select = "Device_Location_Select";
			public const string Device_Notes = "Device_Notes";
			public const string Device_Organization = "Device_Organization";
			public const string Device_Organization_Select = "Device_Organization_Select";
			public const string Device_Properties = "Device_Properties";
			public const string Device_SerialNumber = "Device_SerialNumber";
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
			public const string Device_Title = "Device_Title";
			public const string DeviceGroup_Description = "DeviceGroup_Description";
			public const string DeviceGroup_Help = "DeviceGroup_Help";
			public const string DeviceGroup_Title = "DeviceGroup_Title";
			public const string DeviceNotes_Description = "DeviceNotes_Description";
			public const string DeviceNotes_Help = "DeviceNotes_Help";
			public const string DeviceNotes_Notes = "DeviceNotes_Notes";
			public const string DeviceNotes_Title = "DeviceNotes_Title";
			public const string DeviceNotes_TitleField = "DeviceNotes_TitleField";
		}
	}
}
