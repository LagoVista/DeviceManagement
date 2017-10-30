using LagoVista.CloudStorage.DocumentDB;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using System;
using System.Collections.Generic;
using LagoVista.Core.Validation;
using System.Threading.Tasks;
using LagoVista.IoT.Logging.Loggers;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos.Azure
{
    public class DeviceManagementRepo : DocumentDBRepoBase<Device>, IDeviceManagementRepo
    {
        private bool _shouldConsolidateCollections;

        public DeviceManagementRepo(IDeviceManagementSettings repoSettings, IAdminLogger logger) : base(logger)
        {
            _shouldConsolidateCollections = repoSettings.ShouldConsolidateCollections;
        }

        protected override bool ShouldConsolidateCollections => _shouldConsolidateCollections; 

        protected override String GetCollectionName()
        {
            return "Devices";
        }

        public Task<InvokeResult> AddDeviceAsync(DeviceRepository repo, Device device)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CheckIfDeviceIdInUse(DeviceRepository repo, string id, string orgid)
        {
            throw new NotImplementedException();
        }

        public Task DeleteDeviceAsync(DeviceRepository repo, string id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteDeviceByIdAsync(DeviceRepository repo, string deviceId)
        {
            throw new NotImplementedException();
        }

        public Task<Device> GetDeviceByDeviceIdAsync(DeviceRepository repo, string id)
        {
            throw new NotImplementedException();
        }

        public Task<Device> GetDeviceByIdAsync(DeviceRepository repo, string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DeviceSummary>> GetDevicesForLocationIdAsync(DeviceRepository repo, string locationId, int top, int take)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DeviceSummary>> GetDevicesForOrgIdAsync(DeviceRepository repo, string orgId, int top, int take)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DeviceSummary>> GetDevicesInStatusAsync(DeviceRepository repo, string status, int top, int take)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DeviceSummary>> GetDevicesWithConfigurationAsync(DeviceRepository repo, string configurationId, int top, int take)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DeviceSummary>> GetDevicesWithDeviceTypeAsync(DeviceRepository repo, string deviceTypeId, int top, int take)
        {
            throw new NotImplementedException();
        }

        public Task UpdateDeviceAsync(DeviceRepository repo, Device device)
        {
            throw new NotImplementedException();
        }
    }
}
