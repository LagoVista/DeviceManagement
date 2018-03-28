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
            if (EntityHeader.IsNullOrEmpty(repo.RepositoryType))
            {
                return InvokeResult.FromErrors(new ErrorMessage("Respository Type is a Required Field."));
            }

            if (repo.RepositoryType.Value == RepositoryTypes.NuvIoT ||
                repo.RepositoryType.Value == RepositoryTypes.AzureIoTHub)
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
            }
            else
            {
                repo.DeviceArchiveStorageSettings = new ConnectionSettings()
                {
                    Uri = "mysql",
                    ResourceName = "nuviot",
                    Port = "3306"
                };

                repo.PEMStorageSettings = new ConnectionSettings()
                {
                    Uri = "mongodb",
                    Port = "27017",
                    ResourceName = "nuviot"
                };

                repo.DeviceStorageSettings = new ConnectionSettings()
                {
                    Uri = "mongodb",
                    ResourceName = "nuviot",
                    Port = "27017"
                };
            }

            ValidationCheck(repo, Actions.Create);
            await AuthorizeAsync(repo, AuthorizeResult.AuthorizeActions.Create, user, org);

            if (!String.IsNullOrEmpty(repo.AccessKey))
            {
                var addSecretResult = await _secureStorage.AddSecretAsync(repo.AccessKey);
                if (!addSecretResult.Successful) return addSecretResult.ToInvokeResult();
                repo.SecureAccessKeyId = addSecretResult.Result;
                repo.AccessKey = null;
            }

            await _deviceRepositoryRepo.AddDeviceRepositoryAsync(repo);
            return InvokeResult.Success;
        }

        public async Task<DependentObjectCheckResult> CheckInUseAsync(string deviceRepoId, EntityHeader org, EntityHeader user)
        {
            var deviceRepo = await _deviceRepositoryRepo.GetDeviceRepositoryAsync(deviceRepoId);
            await AuthorizeAsync(deviceRepo, AuthorizeResult.AuthorizeActions.Read, user, org);
            return await CheckForDepenenciesAsync(deviceRepo);
        }

        public async Task<InvokeResult> DeleteDeviceRepositoryAsync(String deviceRepoId, EntityHeader org, EntityHeader user)
        {
            var deviceRepo = await _deviceRepositoryRepo.GetDeviceRepositoryAsync(deviceRepoId);
            await AuthorizeAsync(deviceRepo, AuthorizeResult.AuthorizeActions.Read, user, org);
            await ConfirmNoDepenenciesAsync(deviceRepo);
            await _deviceRepositoryRepo.DeleteAsync(deviceRepoId);
            return InvokeResult.Success;
        }

        public async Task<DeviceRepository> GetDeviceRepositoryAsync(string deviceRepoId, EntityHeader org, EntityHeader user)
        {
            var deviceRepo = await _deviceRepositoryRepo.GetDeviceRepositoryAsync(deviceRepoId);
            await AuthorizeAsync(deviceRepo, AuthorizeResult.AuthorizeActions.Read, user, org);
            return deviceRepo;
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

                if (!string.IsNullOrEmpty(repo.SecureAccessKeyId))
                {
                    await _secureStorage.RemoveSecretAsync(repo.SecureAccessKeyId);
                }

                repo.SecureAccessKeyId = addSecretResult.Result;
                repo.AccessKey = null;
            }

            await _deviceRepositoryRepo.UpdateDeviceRepositoryAsync(repo);
            return InvokeResult.Success;
        }
    }
}
