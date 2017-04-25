using LagoVista.IoT.DeviceManagement.Core.Managers;
using Microsoft.Extensions.DependencyInjection;

namespace LagoVista.IoT.DeviceManagement.Core
{
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IDeviceGroupManager, DeviceGroupManager>();
            services.AddTransient<IDeviceManager, DeviceManager>();
            services.AddTransient<IDeviceLogManager, DeviceLogManager>();
            services.AddTransient<IDeviceArchiveManager, DeviceArchiveManager>();
        }
    }
}
