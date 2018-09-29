using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.DeviceManagement.Repos;
using LagoVista.IoT.Logging.Loggers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public class DeviceRepositoryManager : ManagerBase, IDeviceRepositoryManager
    {
        private IDeviceManagementSettings _deviceMgmtSettings;
        private ISecureStorage _secureStorage;
        private IDeviceRepositoryRepo _deviceRepositoryRepo;

        public DeviceRepositoryManager(
            IDeviceManagementSettings deviceMgmtSettings,
            IDeviceRepositoryRepo deviceRepositoryRepo,
            IAdminLogger logger,
            ISecureStorage secureStorage,
            IAppConfig appConfig,
            IDependencyManager dependencyManager,
            ISecurity security) : base(logger, appConfig, dependencyManager, security)
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

            //TODO: When we create a stand-along repo for a user, we will allocate it here.
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
            else if (repo.RepositoryType.Value == RepositoryTypes.Local)
            {

                //TODO: when we update this for remote server access, we need to figure out what if anything needs to be secured.
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
            /* If repository type == dedicated, values must be provided when inserting the record, this is confirmed on the validation in the next step */

            ValidationCheck(repo, Actions.Create);
            await AuthorizeAsync(repo, AuthorizeResult.AuthorizeActions.Create, user, org);

            var addKeyResult = await _secureStorage.AddSecretAsync(JsonConvert.SerializeObject(repo.DeviceArchiveStorageSettings));
            if (!addKeyResult.Successful)
            {
                return addKeyResult.ToInvokeResult();
            }

            repo.DeviceArchiveStorageSettingsSecureId = addKeyResult.Result;
            repo.DeviceArchiveStorageSettings = null;

            addKeyResult = await _secureStorage.AddSecretAsync(JsonConvert.SerializeObject(repo.DeviceStorageSettings));
            if (!addKeyResult.Successful)
            {
                return addKeyResult.ToInvokeResult();
            }

            repo.DeviceStorageSecureSettingsId = addKeyResult.Result;
            repo.DeviceStorageSettings = null;


            addKeyResult = await _secureStorage.AddSecretAsync(JsonConvert.SerializeObject(repo.PEMStorageSettings));
            if (!addKeyResult.Successful)
            {
                return addKeyResult.ToInvokeResult();
            }

            repo.PEMStorageSettingsSecureId = addKeyResult.Result;
            repo.PEMStorageSettings = null;

            if (!string.IsNullOrEmpty(repo.AccessKey))
            {
                var addSecretResult = await _secureStorage.AddSecretAsync(repo.AccessKey);
                if (!addSecretResult.Successful)
                {
                    return addSecretResult.ToInvokeResult();
                }

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

        public async Task<InvokeResult> DeleteDeviceRepositoryAsync(string deviceRepoId, EntityHeader org, EntityHeader user)
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

        public async Task<DeviceRepository> GetDeviceRepositoryWithSecretsAsync(string repoId, EntityHeader org, EntityHeader user)
        {
            var deviceRepo = await _deviceRepositoryRepo.GetDeviceRepositoryAsync(repoId);
            await AuthorizeAsync(deviceRepo, AuthorizeResult.AuthorizeActions.Read, user, org);

            if (deviceRepo.RepositoryType.Value != RepositoryTypes.Local)
            {
                var getSettingsResult = await _secureStorage.GetSecretAsync(deviceRepo.PEMStorageSettingsSecureId, user, org);
                if (!getSettingsResult.Successful)
                {
                    throw new Exception($"Could not restore secret for PEM Storage Connection Settings {deviceRepo.PEMStorageSettingsSecureId} ");
                }

                deviceRepo.PEMStorageSettings = JsonConvert.DeserializeObject<ConnectionSettings>(getSettingsResult.Result);

                getSettingsResult = await _secureStorage.GetSecretAsync(deviceRepo.DeviceArchiveStorageSettingsSecureId, user, org);
                if (!getSettingsResult.Successful)
                {
                    throw new Exception($"Could not restore secret for Device Archive Connection Settings {deviceRepo.DeviceArchiveStorageSettingsSecureId} ");
                }

                deviceRepo.DeviceArchiveStorageSettings = JsonConvert.DeserializeObject<ConnectionSettings>(getSettingsResult.Result);

                getSettingsResult = await _secureStorage.GetSecretAsync(deviceRepo.DeviceStorageSecureSettingsId, user, org);
                if (!getSettingsResult.Successful)
                {
                    throw new Exception($"Could not restore secret for Device Storage Connection Settings {deviceRepo.DeviceStorageSecureSettingsId} ");
                }

                deviceRepo.DeviceStorageSettings = JsonConvert.DeserializeObject<ConnectionSettings>(getSettingsResult.Result);

                if (!string.IsNullOrEmpty(deviceRepo.SecureAccessKeyId))
                {
                    getSettingsResult = await _secureStorage.GetSecretAsync(deviceRepo.SecureAccessKeyId, user, org);
                    if (!getSettingsResult.Successful)
                    {
                        throw new Exception($"Could not restore secret for PEM Storage Connection Settings {deviceRepo.SecureAccessKeyId} ");
                    }

                    deviceRepo.AccessKey = getSettingsResult.Result;
                }
            }
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

            if (!string.IsNullOrEmpty(repo.AccessKey))
            {
                var addSecretResult = await _secureStorage.AddSecretAsync(repo.AccessKey);
                if (!addSecretResult.Successful)
                {
                    return addSecretResult.ToInvokeResult();
                }

                if (!string.IsNullOrEmpty(repo.SecureAccessKeyId))
                {
                    await _secureStorage.RemoveSecretAsync(repo.SecureAccessKeyId);
                }

                repo.SecureAccessKeyId = addSecretResult.Result;
                repo.AccessKey = null;
            }

            if (repo.DeviceArchiveStorageSettings != null)
            {
                var addKeyResult = await _secureStorage.AddSecretAsync(JsonConvert.SerializeObject(repo.DeviceArchiveStorageSettings));
                if (!addKeyResult.Successful)
                {
                    return addKeyResult.ToInvokeResult();
                }

                if (!string.IsNullOrEmpty(repo.DeviceArchiveStorageSettingsSecureId))
                {
                    await _secureStorage.RemoveSecretAsync(repo.DeviceArchiveStorageSettingsSecureId);
                }

                repo.DeviceArchiveStorageSettingsSecureId = addKeyResult.Result;
                repo.DeviceArchiveStorageSettings = null;
            }

            if (repo.DeviceStorageSettings != null)
            {
                var addKeyResult = await _secureStorage.AddSecretAsync(JsonConvert.SerializeObject(repo.DeviceStorageSettings));
                if (!addKeyResult.Successful)
                {
                    return addKeyResult.ToInvokeResult();
                }

                if (!string.IsNullOrEmpty(repo.DeviceStorageSecureSettingsId))
                {
                    await _secureStorage.RemoveSecretAsync(repo.DeviceStorageSecureSettingsId);
                }

                repo.DeviceStorageSecureSettingsId = addKeyResult.Result;
                repo.DeviceStorageSettings = null;
            }

            if (repo.PEMStorageSettings != null)
            {
                var addKeyResult = await _secureStorage.AddSecretAsync(JsonConvert.SerializeObject(repo.PEMStorageSettings));
                if (!addKeyResult.Successful)
                {
                    return addKeyResult.ToInvokeResult();
                }

                if (!string.IsNullOrEmpty(repo.PEMStorageSettingsSecureId))
                {
                    await _secureStorage.RemoveSecretAsync(repo.PEMStorageSettingsSecureId);
                }

                repo.PEMStorageSettingsSecureId = addKeyResult.Result;
                repo.PEMStorageSettings = null;
            }

            await _deviceRepositoryRepo.UpdateDeviceRepositoryAsync(repo);
            return InvokeResult.Success;
        }
    }
}
