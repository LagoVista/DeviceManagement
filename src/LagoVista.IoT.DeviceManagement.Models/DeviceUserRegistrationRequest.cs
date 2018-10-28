using LagoVista.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System;

namespace LagoVista.IoT.DeviceManagement
{
    public class DeviceUserRegistrationRequest
    {
        public string Email { get; set; }
        public String PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public Device Device {get; set;}
    }
}
