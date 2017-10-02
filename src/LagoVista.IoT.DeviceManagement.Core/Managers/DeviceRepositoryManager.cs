using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.DeviceManagement.Repos;
using LagoVista.IoT.Logging.Loggers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public class DeviceRepositoryManager : ManagerBase, IDeviceRepositoryManager
    {

        IDeviceManagementSettings _deviceMgmtSettings;
        ISecureStorage _secureStorage;
        IDeviceRepositoryRepo _deviceRepositoryRepo;
        public DeviceRepositoryManager(IDeviceManagementSettings deviceMgmtSettings, IDeviceRepositoryRepo deviceRepositoryRepo, IAdminLogger logger, ISecureStorage secureStorage, IAppConfig appConfig, IDependencyManager dependencyManager, ISecurity security) : base(logger, appConfig, dependencyManager, security)
        {
            _deviceRepositoryRepo = deviceRepositoryRepo;
            _deviceMgmtSettings = deviceMgmtSettings;
            _secureStorage = secureStorage;
        }

        public async Task<InvokeResult> AddDeviceRepositoryAsync(DeviceRepository repo, EntityHeader org, EntityHeader user)
        {
            repo.DeviceArchiveStorageSettings = new ConnectionSettings()
            {
                AccountId = _deviceMgmtSettings.DefaultDeviceTableStorage.AccountId,
                AccessKey = _deviceMgmtSettings.DefaultDeviceTableStorage.AccessKey
            };

            repo.PEMStorageSettings = new ConnectionSettings()
            {
                AccountId = _deviceMgmtSettings.DefaultDeviceTableStorage.AccountId,
                AccessKey = _deviceMgmtSettings.DefaultDeviceTableStorage.AccessKey
            };

            repo.DeviceStorageSettings = new ConnectionSettings()
            {
                Uri = _deviceMgmtSettings.DefaultDeviceStorage.Uri,
                AccessKey = _deviceMgmtSettings.DefaultDeviceStorage.AccessKey,
                ResourceName = _deviceMgmtSettings.DefaultDeviceStorage.ResourceName
            };

            ValidationCheck(repo, Actions.Create);
            await AuthorizeAsync(repo, AuthorizeResult.AuthorizeActions.Create, user, org);

            if(!String.IsNullOrEmpty(repo.AccessKey))
            {
                var addSecretResult = await _secureStorage.AddSecretAsync(repo.AccessKey);
                if (!addSecretResult.Successful) return addSecretResult.ToInvokeResult();
                repo.SecureAccessKeyId = addSecretResult.Result;
                repo.AccessKey = null;
            }

            await _deviceRepositoryRepo.AddDeviceRepositoryAsync(repo);
            return InvokeResult.Success;
        }

        public async Task<DependentObjectCheckResult> CheckInUseAsync(string id, EntityHeader org, EntityHeader user)
        {
            var host = await _deviceRepositoryRepo.GetDeviceRepositoryAsync(id);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Read, user, org);
            return await CheckForDepenenciesAsync(host);
        }

        public async Task<InvokeResult> DeleteDeviceRepositoryAsync(String instanceId, EntityHeader org, EntityHeader user)
        {
            var host = await _deviceRepositoryRepo.GetDeviceRepositoryAsync(instanceId);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Read, user, org);
            await ConfirmNoDepenenciesAsync(host);
            await _deviceRepositoryRepo.DeleteAsync(instanceId);
            return InvokeResult.Success;
        }

        public async Task<DeviceRepository> GetDeviceRepositoryAsync(string instanceId, EntityHeader org, EntityHeader user)
        {
            var host = await _deviceRepositoryRepo.GetDeviceRepositoryAsync(instanceId);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Read, user, org);
            return host;
        }

        public async Task<IEnumerable<DeviceRepositorySummary>> GetDeploymentHostsForOrgAsync(string orgId, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, orgId, typeof(DeviceRepository));
            return await _deviceRepositoryRepo.GetDeviceRepositoriesForOrgAsync(orgId);
        }

        public Task<bool> QueryKeyInUserAsync(string key, EntityHeader org)
        {
            return _deviceRepositoryRepo.QueryRepoKeyInUseAsync(key, org.Id);
        }

        public async Task<InvokeResult> UpdateDeviceRepositoryAsync(DeviceRepository repo, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(repo, Actions.Update);
            await AuthorizeAsync(repo, AuthorizeResult.AuthorizeActions.Update, user, org);

            if (!String.IsNullOrEmpty(repo.AccessKey))
            {
                var addSecretResult = await _secureStorage.AddSecretAsync(repo.AccessKey);
                if (!addSecretResult.Successful) return addSecretResult.ToInvokeResult();
                repo.SecureAccessKeyId = addSecretResult.Result;
                repo.AccessKey = null;
            }

            await _deviceRepositoryRepo.UpdateDeviceRepositoryAsync(repo);
            return InvokeResult.Success;
        }
    }
}
