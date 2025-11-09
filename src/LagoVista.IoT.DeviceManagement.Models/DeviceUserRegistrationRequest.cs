// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: ec69a24fd2efd061c80b04287305faa853f5f56921811cb562ebb14d94182466
// IndexVersion: 2
// --- END CODE INDEX META ---
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
