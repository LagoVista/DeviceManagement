using LagoVista.IoT.DeviceManagement.Core.Repos;
using System.Linq;
using System.Collections.Generic;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Threading.Tasks;
using LagoVista.CloudStorage.DocumentDB;
using LagoVista.Core.PlatformSupport;
using LagoVista.IoT.Logging.Loggers;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class DeviceManagementRepo : DocumentDBRepoBase<Device>, IDeviceManagementRepo
    {
        private bool _shouldConsolidateCollections;
        public DeviceManagementRepo(IDeviceManagementSettings repoSettings, IAdminLogger logger) : base(repoSettings.DeviceManagementDocDbStorage.Uri, repoSettings.DeviceManagementDocDbStorage.AccessKey, repoSettings.DeviceManagementDocDbStorage.ResourceName, logger)
        {
            _shouldConsolidateCollections = repoSettings.ShouldConsolidateCollections;
        }

        protected override bool ShouldConsolidateCollections => _shouldConsolidateCollections;

        public Task AddDeviceAsync(Device device)
        {
            return CreateDocumentAsync(device);
        }

        public Task DeleteDeviceAsync(string id)
        {
            return DeleteDocumentAsync(id);
        }

        public async Task DeleteDeviceByIdAsync(string deviceId)
        {
            var device = await this.GetDeviceByDeviceIdAsync(deviceId);
            await DeleteDeviceAsync(device.Id);
        }

        public async Task<Device> GetDeviceByDeviceIdAsync(string id)
        {
            return (await base.QueryAsync(device => device.DeviceId == id)).FirstOrDefault();
        }

        public async Task<bool> CheckIfDeviceIdInUse(string id, string orgid)
        {
            return (await base.QueryAsync(device => device.OwnerOrganization.Id == id && device.DeviceId == id)).Any();
        }

        public Task<Device> GetDeviceByIdAsync(string id)
        {
            return GetDocumentAsync(id);
        }

        public Task UpdateDeviceAsync(Device device)
        {
            return UpsertDocumentAsync(device);
        }

        public async Task<IEnumerable<DeviceSummary>> GetDevicesForLocationIdAsync(string locationId, int top, int take)
        {
            var items = await base.QueryAsync(qry => qry.Location.Id == locationId);

            return from item in items
                   select item.CreateSummary();
        }

        public async Task<IEnumerable<DeviceSummary>> GetDevicesForOrgIdAsync(string orgId, int top, int take)
        {
            var items = await base.QueryAsync(qry => qry.OwnerOrganization.Id == orgId);

            return from item in items
                   select item.CreateSummary();
        }

        public async Task<IEnumerable<DeviceSummary>> GetDevicesInStatusAsync(string status, int top, int take)
        {
            var items = await base.QueryAsync(qry => qry.Status.Id == status);

            return from item in items
                   select item.CreateSummary();
        }

        public async Task<IEnumerable<DeviceSummary>> GetDevicesWithConfigurationAsync(string configurationId, int top, int take)
        {
            var items = await base.QueryAsync(qry => qry.DeviceConfiguration.Id == configurationId);

            return from item in items
                   select item.CreateSummary();
        }
    }
}
