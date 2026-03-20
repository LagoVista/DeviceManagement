// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 97419ef16c99c06646cab7c81f7e5a2d25472d18cfffbea3e78d2d881940f0d1
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Interfaces;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.Logging.Loggers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace LagoVista.IoT.DeviceManagement.Repos
{
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IDeviceManagementRepo, Repos.DeviceManagementRepo>();
            services.AddTransient<IDeviceArchiveRepo, Repos.DeviceArchiveRepo>();
            services.AddTransient<IDeviceExceptionRepo, Repos.DeviceExceptionRepo>();
            services.AddTransient<IDeviceConnectionEventRepo, Repos.DeviceConnectionEventRepo>();
            services.AddTransient<IDeviceStatusChangeRepo, Repos.DeviceStatusChangeRepo>();
            services.AddTransient<IDeviceGroupRepo, Repos.DeviceGroupRepo>();
            services.AddTransient<IDeviceLogRepo, Repos.DeviceLogRepo>();
            services.AddTransient<IDevicePEMRepo, Repos.DevicePEMRepo>();
            services.AddTransient<IDeviceMediaItemRepo, Repos.DeviceMediaItemRepo>();
            services.AddTransient<IDeviceMediaRepo, Repos.DeviceMediaRepo>();
            services.AddTransient<IDeviceRepositoryRepo, Repos.DeviceRepositoryRepo>();
            services.AddTransient<IFirmwareRepo, Repos.FirmwareRepo>();
            services.AddTransient<ISensorDataArchiveRepo, Repos.SensorDataArchiveRepo>();
            services.AddTransient<ISilencedAlarmsRepo, Repos.SilencedAlarmsRepo>();
        }
    }
}

namespace LagoVista.DependencyInjection
{
    public static class DeviceManagementModule
    {
        public static void AddDeviceManagementModule(this IServiceCollection services, IConfigurationRoot configRoot, IAdminLogger logger)
        {
            LagoVista.IoT.DeviceManagement.Repos.Startup.ConfigureServices(services);
            LagoVista.IoT.DeviceManagement.Core.Startup.ConfigureServices(services);
            services.AddMetaDataHelper<DeviceGroup>();
        }
    }
}