using LagoVista.IoT.DeviceManagement.Core.Repos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Threading.Tasks;
using LagoVista.CloudStorage.DocumentDB;
using LagoVista.IoT.Logging.Loggers;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class DeviceRepositoryRepo : DocumentDBRepoBase<DeviceRepository>, IDeviceRepositoryRepo
    {
        private bool _shouldConsolidateCollections;
        public DeviceRepositoryRepo(IDeviceManagementSettings repoSettings, IAdminLogger logger) : base(repoSettings.DeviceRepoStorage.Uri, repoSettings.DeviceRepoStorage.AccessKey, repoSettings.DeviceRepoStorage.ResourceName, logger)
        {
            _shouldConsolidateCollections = repoSettings.ShouldConsolidateCollections;
        }

        protected override bool ShouldConsolidateCollections => _shouldConsolidateCollections;


        public Task AddDeviceRepositoryAsync(DeviceRepository deviceRepo)
        {
            return base.CreateDocumentAsync(deviceRepo);
        }

        public Task DeleteAsync(string repoId)
        {
            return DeleteDocumentAsync(repoId);
        }

        public async Task<IEnumerable<DeviceRepositorySummary>> GetDeviceRepositoriesForOrgAsync(string orgId)
        {
            var items = await base.QueryAsync(qry => qry.OwnerOrganization.Id == orgId);

            return from item in items
                   select item.CreateSummary();
        }

        public Task<DeviceRepository> GetDeviceRepositoryAsync(string repoId)
        {
            return GetDocumentAsync(repoId);
        }

        public async Task<bool> QueryRepoKeyInUseAsync(string key, string orgId)
        {
            var items = await base.QueryAsync(attr => (attr.OwnerOrganization.Id == orgId || attr.IsPublic == true) && attr.Key == key);
            return items.Any();
        }

        public Task UpdateDeviceRepositoryAsync(DeviceRepository deviceRepo)
        {
            return base.UpsertDocumentAsync(deviceRepo);
        }
    }
}
