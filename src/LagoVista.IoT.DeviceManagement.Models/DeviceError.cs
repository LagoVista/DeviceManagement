using LagoVista.Core;
using LagoVista.Core.Attributes;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Models.Resources;
using System;

namespace LagoVista.IoT.DeviceManagement.Models
{
    [EntityDescription(DeviceManagementDomain.DeviceManagement, DeviceManagementResources.Names.DeviceError_Title, DeviceManagementResources.Names.DeviceError_Description,
        DeviceManagementResources.Names.DeviceError_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeviceManagementResources), Icon: "icon-ae-error-1")]
    public class DeviceError
    {        
        public DeviceError()
        {
            Id = Guid.NewGuid().ToId();
        }

        public string Id { get; set; }
        public int Count { get; set; }
        public string Timestamp { get; set; }
        public string FirstSeen { get; set; }
        public string LastSeen { get; set; }
        public string Expires { get; set; }
        public bool Active { 
            get
            {
                if (String.IsNullOrEmpty(Expires))
                    return true;

                return Expires.ToDateTime() > DateTime.UtcNow.Date;
            }
        }
        public string NextNotification { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDescription { get; set; }
        public string DeviceErrorCode { get; set; }
        public string LastDetails { get; set; }

        public bool Silenced { get; set; }
        public string SilencedTimeStamp { get; set; }
        public string SilencedBy { get; set; }
        public string SilencedById { get; set; }
    }
}
