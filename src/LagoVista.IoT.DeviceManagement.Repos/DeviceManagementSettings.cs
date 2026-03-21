using LagoVista;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.DeviceManagement.Repos
{
    public class DeviceManagementSettings : IDeviceManagementSettings
    {
        public IConnectionSettings DeviceRepoStorage { get; }
        public IConnectionSettings DefaultDeviceStorage { get; }
        public IConnectionSettings DefaultDeviceTableStorage { get; }
        public ConnectionSettings DefaultDeviceAccountStorage { get; }


        public DeviceManagementSettings(IConfiguration configuration)
        {
            DeviceRepoStorage = configuration.CreateDefaultDBStorageSettings();
            DefaultDeviceStorage = configuration.CreateDefaultDBStorageSettings();
            DefaultDeviceTableStorage = configuration.CreateDefaultTableStorageSettings();
            var section = configuration.GetRequiredSection("DeviceAccountStorage");

            DefaultDeviceAccountStorage = new ConnectionSettings()
            {
                Uri = section.Require("ServerURL"),
                UserName = section.Require("UserName"),
                Password = section.Require("Password")
            };
        }
       
    }
}
