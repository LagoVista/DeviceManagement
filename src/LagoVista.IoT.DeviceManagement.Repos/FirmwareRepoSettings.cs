using LagoVista.Core.Interfaces;
using LagoVista.IoT.DeviceManagement.Core;
using Microsoft.Extensions.Configuration;

namespace LagoVista.IoT.DeviceManagement.Repos
{
    public class FirmwareRepoSettings : IFirmwareRepoSettings
    {
        public IConnectionSettings FirmwareDocDBSettings { get; }

        public IConnectionSettings FirmwareRequestSettings { get; }

        public IConnectionSettings FirmwareBinSettings { get; }

        public FirmwareRepoSettings(IConfiguration configuration)
        {
            FirmwareDocDBSettings = configuration.CreateDefaultDBStorageSettings();
            FirmwareRequestSettings = configuration.CreateDefaultTableStorageSettings();
            FirmwareBinSettings = configuration.CreateDefaultTableStorageSettings();
        }
    }
}
