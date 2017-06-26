using LagoVista.IoT.DeviceManagement.Core.Repos;
using Microsoft.Extensions.DependencyInjection;

namespace LagoVista.IoT.DeviceManagement.Repos
{
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IDeviceManagementRepo, Repos.DeviceManagementRepo>();
            services.AddTransient<IDeviceArchiveRepo, Repos.DeviceArchiveRepo>();
            services.AddTransient<IDeviceGroupEntryRepo, Repos.DeviceGroupEntryRepo>();
            services.AddTransient<IDeviceGroupRepo, Repos.DeviceGroupRepo>();
            services.AddTransient<IDeviceLogRepo, Repos.DeviceLogRepo>();
            services.AddTransient<IDevicePEMIndexesRepo, Repos.DevicePEMIndexesRepo>();
            services.AddTransient<IDevicePEMRepo, Repos.DevicePEMRepo>();
            services.AddTransient<IDeviceRepositoryRepo, Repos.DeviceRepositoryRepo>();
        }
    }
}