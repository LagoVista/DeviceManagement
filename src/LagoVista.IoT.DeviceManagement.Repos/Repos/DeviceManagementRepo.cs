using LagoVista.IoT.DeviceManagement.Core.Repos;
using System.Linq;
using System.Collections.Generic;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Threading.Tasks;
using LagoVista.CloudStorage.DocumentDB;
using LagoVista.Core.PlatformSupport;
using LagoVista.IoT.Logging.Loggers;
using System;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class DeviceManagementRepo : DocumentDBRepoBase<Device>, IDeviceManagementRepo
    {
        private bool _shouldConsolidateCollections;
        public DeviceManagementRepo(IDeviceManagementSettings repoSettings, IAdminLogger logger) : base(logger)
        {
            _shouldConsolidateCollections = repoSettings.ShouldConsolidateCollections;
        }

        protected override String GetCollectionName()
        {
            return "Devices";
        }

        public Task AddDeviceAsync(DeviceRepository deviceRepo, Device device)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceArchiveStorageSettings.AccessKey, deviceRepo.DeviceArchiveStorageSettings.ResourceName);

            return CreateDocumentAsync(device);
        }

        public Task DeleteDeviceAsync(DeviceRepository deviceRepo, string id)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceArchiveStorageSettings.AccessKey, deviceRepo.DeviceArchiveStorageSettings.ResourceName);

            return DeleteDocumentAsync(id);
        }

        public async Task DeleteDeviceByIdAsync(DeviceRepository deviceRepo, string deviceId)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceArchiveStorageSettings.AccessKey, deviceRepo.DeviceArchiveStorageSettings.ResourceName);

            var device = await this.GetDeviceByDeviceIdAsync(deviceRepo, deviceId);
            await DeleteDeviceAsync(deviceRepo, device.Id);
        }

        public async Task<Device> GetDeviceByDeviceIdAsync(DeviceRepository deviceRepo, string id)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceArchiveStorageSettings.AccessKey, deviceRepo.DeviceArchiveStorageSettings.ResourceName);

            return (await base.QueryAsync(device => device.DeviceId == id)).FirstOrDefault();
        }

        public async Task<bool> CheckIfDeviceIdInUse(DeviceRepository deviceRepo, string id, string orgid)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceArchiveStorageSettings.AccessKey, deviceRepo.DeviceArchiveStorageSettings.ResourceName);

            return (await base.QueryAsync(device => device.OwnerOrganization.Id == id && device.DeviceId == id)).Any();
        }

        public Task<Device> GetDeviceByIdAsync(DeviceRepository deviceRepo, string id)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceArchiveStorageSettings.AccessKey, deviceRepo.DeviceArchiveStorageSettings.ResourceName);

            return GetDocumentAsync(id);
        }

        public Task UpdateDeviceAsync(DeviceRepository deviceRepo, Device device)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceArchiveStorageSettings.AccessKey, deviceRepo.DeviceArchiveStorageSettings.ResourceName);

            return UpsertDocumentAsync(device);
        }

        public async Task<IEnumerable<DeviceSummary>> GetDevicesForLocationIdAsync(DeviceRepository deviceRepo, string locationId, int top, int take)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceArchiveStorageSettings.AccessKey, deviceRepo.DeviceArchiveStorageSettings.ResourceName);

            var items = await base.QueryAsync(qry => qry.Location.Id == locationId);

            return from item in items
                   select item.CreateSummary();
        }

        public async Task<IEnumerable<DeviceSummary>> GetDevicesForOrgIdAsync(DeviceRepository deviceRepo, string orgId, int top, int take)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceArchiveStorageSettings.AccessKey, deviceRepo.DeviceArchiveStorageSettings.ResourceName);

            var items = await base.QueryAsync(qry => qry.OwnerOrganization.Id == orgId);

            return from item in items
                   select item.CreateSummary();
        }

        public async Task<IEnumerable<DeviceSummary>> GetDevicesInStatusAsync(DeviceRepository deviceRepo, string status, int top, int take)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceArchiveStorageSettings.AccessKey, deviceRepo.DeviceArchiveStorageSettings.ResourceName);

            var items = await base.QueryAsync(qry => qry.Status.Id == status);

            return from item in items
                   select item.CreateSummary();
        }

        public async Task<IEnumerable<DeviceSummary>> GetDevicesWithConfigurationAsync(DeviceRepository deviceRepo, string configurationId, int top, int take)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceArchiveStorageSettings.AccessKey, deviceRepo.DeviceArchiveStorageSettings.ResourceName);

            var items = await base.QueryAsync(qry => qry.DeviceConfiguration.Id == configurationId);

            return from item in items
                   select item.CreateSummary();
        }

        public async Task<IEnumerable<DeviceSummary>> GetDevicesWithDeviceTypeAsync(DeviceRepository deviceRepo, string deviceTypeId, int top, int take)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceArchiveStorageSettings.AccessKey, deviceRepo.DeviceArchiveStorageSettings.ResourceName);

            var items = await base.QueryAsync(qry => qry.DeviceType.Id == deviceTypeId);

            return from item in items
                   select item.CreateSummary();
        }
    }
}
