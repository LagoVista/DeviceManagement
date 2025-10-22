// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 66eb7d4d9532403c548bf4d721944abe10570bd3335656c54a839e2fa0452165
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Interfaces;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.DeviceManagement.Core.Reporting;
using System.Resources;

[assembly: NeutralResourcesLanguage("en")]

namespace LagoVista.IoT.DeviceManagement.Core
{
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IDeviceGroupManager, DeviceGroupManager>();
            services.AddTransient<IDeviceManager, DeviceManager>();
            services.AddTransient<IDevicePEMManager, DevicePEMManager>();
            services.AddTransient<IDeviceLogManager, DeviceLogManager>();
            services.AddTransient<IDeviceRepositoryManager, DeviceRepositoryManager>();
            services.AddTransient<IDeviceArchiveReportUtils, DeviceArchiveReportUtils>();
            services.AddTransient<IDeviceMediaManager, DeviceMediaManager>();
            services.AddTransient<IDeviceArchiveManager, DeviceArchiveManager>();
            services.AddTransient<IDeviceArchiveManagerRemote, DeviceArchiveManager>();
            services.AddTransient<IDeviceRepositoryManagerRemote, DeviceRepositoryManager>();
            services.AddTransient<IFirmwareManager, FirmwareManager>();
            services.AddTransient<IDeviceExceptionManager, DeviceExceptionManager>();
            services.AddTransient<IDeviceStatusManager, DeviceStatusManager>();
        }

        public static void ConfigureFirmwareDownloadServices(IServiceCollection services)
        {
            services.AddTransient<IFirmwareDownloadManager, FirmwareDownloadManager>();
        }
    }
}
