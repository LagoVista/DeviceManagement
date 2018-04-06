using LagoVista.IoT.DeviceManagement.Core.Repos;
using System;
using System.Linq;
using System.Collections.Generic;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Threading.Tasks;
using LagoVista.Core.PlatformSupport;
using LagoVista.CloudStorage.DocumentDB;
using LagoVista.IoT.Logging.Loggers;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class DeviceGroupRepo : DocumentDBRepoBase<DeviceGroup>, IDeviceGroupRepo
    {
        public DeviceGroupRepo(IDeviceManagementSettings repoSettings, IAdminLogger logger) : base(logger)
        {
            
        }

        protected override String GetCollectionName()
        {
            return "Devices";
        }

        public Task AddDeviceGroupAsync(DeviceRepository deviceRepo, DeviceGroup deviceGroup)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceStorageSettings.AccessKey, deviceRepo.DeviceStorageSettings.ResourceName);
            return CreateDocumentAsync(deviceGroup);
        }

        public Task DeleteDeviceGroupAsync(DeviceRepository deviceRepo, string deviceGroupId)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceStorageSettings.AccessKey, deviceRepo.DeviceStorageSettings.ResourceName);
            return DeleteDocumentAsync(deviceGroupId);
        }

        public Task<DeviceGroup> GetDeviceGroupAsync(DeviceRepository deviceRepo, string id)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceStorageSettings.AccessKey, deviceRepo.DeviceStorageSettings.ResourceName);
            return GetDocumentAsync(id);
        }

        public async Task<IEnumerable<DeviceGroupSummary>> GetDeviceGroupsForOrgAsync(DeviceRepository deviceRepo, string orgId)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceStorageSettings.AccessKey, deviceRepo.DeviceStorageSettings.ResourceName);

            var items = await base.QueryAsync(qry => qry.OwnerOrganization.Id == orgId && qry.DeviceRepository.Id == deviceRepo.Id);

            return from item in items
                   select item.CreateSummary();
        }

        public async Task<bool> QueryKeyInUseAsync(DeviceRepository deviceRepo, string key, string orgId)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceStorageSettings.AccessKey, deviceRepo.DeviceStorageSettings.ResourceName);

            var items = await base.QueryAsync(attr => (attr.OwnerOrganization.Id == orgId || attr.IsPublic == true) && attr.Key == key);
            return items.Any();
        }

        public Task UpdateDeviceGroupAsync(DeviceRepository deviceRepo, DeviceGroup deviceGroup)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceStorageSettings.AccessKey, deviceRepo.DeviceStorageSettings.ResourceName);

            return UpsertDocumentAsync(deviceGroup);
        }
    }
}
