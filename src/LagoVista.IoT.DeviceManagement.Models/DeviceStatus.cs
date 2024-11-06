using LagoVista.Core.Attributes;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Models.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Models
{
    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.DevicesStatus_Title, DeviceManagementResources.Names.DevicesStatus_Description,
        DeviceManagementResources.Names.DevicesStatus_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeviceManagementResources), Icon: "icon-pz-stamp-1")]
    public class DeviceStatus
    {
        public const string DeviceStatus_New = "New";
        public const string DeviceStatus_Online = "Online";
        public const string DeviceStatus_TimeedOut = "Timed Out";

        public string DeviceId { get; set; }
        public string DeviceUniqueId { get; set; }
        public string Timestamp { get; set; }
        public string LastContact { get; set; }
        public string LastNotified { get; set; }
        public string WatchdogCheckPoint { get; set; }
        public int WatchdogTimeoutSeconds { get; set; }
        public string PreviousStatus { get; set; }
        public string CurrentStatus { get; set; }
        public string Details { get; set; }

        public bool SilenceAlarm { get; set; }
    }
}
