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
        }

        public static void ConfigureFirmwareDownloadServices(IServiceCollection services)
        {
            services.AddTransient<IFirmwareDownloadManager, FirmwareDownloadManager>();
        }
    }
}
