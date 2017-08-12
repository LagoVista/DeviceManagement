using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.DeviceManagement.Core.Reporting;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddTransient<IDeviceArchiveReportUtils, DeviceArchiveReportUtils>();
            services.AddTransient<IDeviceArchiveManager, DeviceArchiveManager>();
            services.AddTransient<IDeviceRepositoryManager, DeviceRepositoryManager>();
        }
    }
}
