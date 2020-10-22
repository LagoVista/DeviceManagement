using LagoVista.Core.Interfaces;
using LagoVista.IoT.DeviceManagement.Core.Repos;


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
        }
    }
}