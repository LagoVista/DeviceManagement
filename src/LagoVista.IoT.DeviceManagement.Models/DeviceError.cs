using LagoVista.Core;
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Models
{
    public class DeviceError
    {        
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
    }
}
