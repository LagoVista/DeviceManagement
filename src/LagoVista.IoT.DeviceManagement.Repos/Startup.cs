using LagoVista.CloudStorage.Storage;
using LagoVista.CloudStorage.Storage.StorageProviders.Cassandra;
using LagoVista.Core.Interfaces;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.DeviceManagement.Repos.DTOs;
using LagoVista.IoT.Logging.Loggers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LagoVista.IoT.DeviceManagement.Repos
{
    public class Startup
    {
        private static readonly TimeSpan RuntimeHistoryRetention = TimeSpan.FromDays(90);
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
            services.AddTransient<IDeviceManagementSettings, DeviceManagementSettings>();
            services.AddTransient<IFirmwareRepoSettings, FirmwareRepoSettings>();

            services.AddActivityRecordStore<DeviceArchiveActivityRecord, CassandraActivityRecordStore<DeviceArchiveActivityRecord>>(d => d.PartitionBy(r => r.OrganizationId).PartitionBy(r => r.DeviceUniqueId).RetainFor(RuntimeHistoryRetention));
            services.AddActivityRecordStore<SensorDataArchiveActivityRecord, CassandraActivityRecordStore<SensorDataArchiveActivityRecord>>(d => d.PartitionBy(r => r.OrganizationId).PartitionBy(r => r.DeviceUniqueId).RetainFor(RuntimeHistoryRetention));
            services.AddActivityRecordStore<DeviceExceptionActivityRecord, CassandraActivityRecordStore<DeviceExceptionActivityRecord>>(d => d.PartitionBy(r => r.OrganizationId).PartitionBy(r => r.DeviceUniqueId).RetainFor(RuntimeHistoryRetention));
            services.AddActivityRecordStore<DeviceStatusHistoryActivityRecord, CassandraActivityRecordStore<DeviceStatusHistoryActivityRecord>>(d => d.PartitionBy(r => r.OrganizationId).PartitionBy(r => r.DeviceUniqueId).RetainFor(RuntimeHistoryRetention));
            services.AddOperationalDataStore<DeviceCurrentStatusRecord, CassandraOperationalDataStore<DeviceCurrentStatusRecord>>();
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