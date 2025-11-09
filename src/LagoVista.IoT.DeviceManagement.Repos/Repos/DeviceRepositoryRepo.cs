// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 7c44e366bccdd2be2a0f94d3da9c4a86d4e51da16e55d2dfd869a395bbae3167
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.IoT.DeviceManagement.Core.Repos;
using System;
using System.Linq;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Threading.Tasks;
using LagoVista.CloudStorage.DocumentDB;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.CloudStorage.Storage;
using System.Security.Cryptography;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class DeviceRepositoryRepo : DocumentDBRepoBase<DeviceRepository>, IDeviceRepositoryRepo
    {
        private bool _shouldConsolidateCollections;
        private ICacheProvider _cacheProvider;
        public DeviceRepositoryRepo(IDeviceManagementSettings repoSettings, IAdminLogger logger, ICacheProvider cacheProvider) 
            : base(repoSettings.DeviceRepoStorage.Uri, repoSettings.DeviceRepoStorage.AccessKey, repoSettings.DeviceRepoStorage.ResourceName, logger, cacheProvider)
        {
            _shouldConsolidateCollections = repoSettings.ShouldConsolidateCollections;
            _cacheProvider = cacheProvider;
        }

        protected override bool ShouldConsolidateCollections => _shouldConsolidateCollections;


        public Task AddDeviceRepositoryAsync(DeviceRepository deviceRepo)
        {
            if (deviceRepo.DeviceArchiveStorageSettings != null) throw new Exception("Should never store archive settings in plain text.");
            if (deviceRepo.PEMStorageSettings != null) throw new Exception("Should never store pem storage settings in plain text.");
            if (deviceRepo.DeviceStorageSettings != null) throw new Exception("Should never store device storage settings in plain text.");

            return base.CreateDocumentAsync(deviceRepo);
        }

        public Task DeleteAsync(string repoId)
        {
            return DeleteDocumentAsync(repoId);
        }

        public async Task<ListResponse<DeviceRepositorySummary>> GetAvailableDeviceRepositoriesForOrgAsync(string orgId, ListRequest listRequest)
        {
            return await base.QuerySummaryAsync<DeviceRepositorySummary, DeviceRepository>(qry => qry.OwnerOrganization.Id == orgId && qry.Instance == null, rep => rep.Name, listRequest);
        }

        public async Task<ListResponse<DeviceRepositorySummary>> GetDeviceRepositoriesForOrgAsync(string orgId, ListRequest listRequest)
        {
            return await base.QuerySummaryAsync<DeviceRepositorySummary, DeviceRepository>(qry => qry.OwnerOrganization.Id == orgId, rep=>rep.Name, listRequest);
        }

        public Task<DeviceRepository> GetDeviceRepositoryAsync(string repoId)
        {
            return GetDocumentAsync(repoId);
        }

        public async Task<DeviceRepository> GetDeviceRepositoryForInstanceAsync(string instanceId)
        {
            var items = await base.QueryAsync(qry => qry.Instance.Id == instanceId);
            return items.FirstOrDefault();
        }

        public async Task<bool> QueryRepoKeyInUseAsync(string key, string orgId)
        {
            var items = await base.QueryAsync(attr => (attr.OwnerOrganization.Id == orgId || attr.IsPublic == true) && attr.Key == key);
            return items.Any();
        }

        public async Task UpdateDeviceRepositoryAsync(DeviceRepository deviceRepo)
        {
            await _cacheProvider.GetAsync($"basic_theme_repo_{deviceRepo.Id}");
            await base.UpsertDocumentAsync(deviceRepo);
        }
    }
}
