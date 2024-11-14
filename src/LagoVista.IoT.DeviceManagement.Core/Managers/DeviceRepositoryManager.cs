using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.DeviceManagement.Repos;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.UserAdmin.Interfaces.Repos.Orgs;
using LagoVista.UserAdmin.Models.Orgs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public class DeviceRepositoryManager : ManagerBase, IDeviceRepositoryManager
    {
        private IDeviceManagementSettings _deviceMgmtSettings;
        private ISecureStorage _secureStorage;
        private IDeviceRepositoryRepo _deviceRepositoryRepo;
        private IAdminLogger _adminLogger;
        private readonly ICacheProvider _cacheProvider;
        private IOrganizationRepo _orgRepo;

        public DeviceRepositoryManager(
            IDeviceManagementSettings deviceMgmtSettings,
            IDeviceRepositoryRepo deviceRepositoryRepo,
            IAdminLogger logger,
            ICacheProvider cacheProvider,
            ISecureStorage secureStorage,
            IAppConfig appConfig,
            IOrganizationRepo orgRepo,
            IDependencyManager dependencyManager,
            ISecurity security) : base(logger, appConfig, dependencyManager, security)
        {
            _deviceRepositoryRepo = deviceRepositoryRepo;
            _deviceMgmtSettings = deviceMgmtSettings;
            _secureStorage = secureStorage;
            _adminLogger = logger;
            _orgRepo = orgRepo;
            _cacheProvider = cacheProvider;
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

                repo.DeviceWatchdogStorageSettings = new ConnectionSettings()
                {
                    Uri = _deviceMgmtSettings.DefaultDeviceTableStorage.Uri,
                    AccessKey = _deviceMgmtSettings.DefaultDeviceTableStorage.AccessKey,
                    ResourceName = $"Watchdlog{repo.Id}{repo.Key}"
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

                repo.DeviceWatchdogStorageSettings = new ConnectionSettings()
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

            var addKeyResult = await _secureStorage.AddSecretAsync(org, JsonConvert.SerializeObject(repo.DeviceArchiveStorageSettings));
            if (!addKeyResult.Successful)
            {
                return addKeyResult.ToInvokeResult();
            }

            repo.DeviceArchiveStorageSettingsSecureId = addKeyResult.Result;
            repo.DeviceArchiveStorageSettings = null;

            addKeyResult = await _secureStorage.AddSecretAsync(org, JsonConvert.SerializeObject(repo.DeviceStorageSettings));
            if (!addKeyResult.Successful)
            {
                return addKeyResult.ToInvokeResult();
            }

            repo.DeviceStorageSecureSettingsId = addKeyResult.Result;
            repo.DeviceStorageSettings = null;

            addKeyResult = await _secureStorage.AddSecretAsync(org, JsonConvert.SerializeObject(repo.DeviceWatchdogStorageSettings));
            if (!addKeyResult.Successful)
            {
                return addKeyResult.ToInvokeResult();
            }

            repo.DeviceWatchdogStorageSecureId = addKeyResult.Result;
            repo.DeviceWatchdogStorageSettings = null;


            addKeyResult = await _secureStorage.AddSecretAsync(org, JsonConvert.SerializeObject(repo.PEMStorageSettings));
            if (!addKeyResult.Successful)
            {
                return addKeyResult.ToInvokeResult();
            }

            repo.PEMStorageSettingsSecureId = addKeyResult.Result;
            repo.PEMStorageSettings = null;

            if (!string.IsNullOrEmpty(repo.AccessKey))
            {
                var addSecretResult = await _secureStorage.AddSecretAsync(org, repo.AccessKey);
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


        // Not super crazy about bypassing security when we pass in a PIN, not really worried about exploit from outside world, but we are bypassing 
        // the internal security tracking.  This should only be used when we are using the repo to get a device id, and we will perform
        // our security check there.  Likely revisit repo in the futurue.
        public async Task<DeviceRepository> GetDeviceRepositoryWithSecretsAsync(string repoId, EntityHeader org, EntityHeader user, String pin = null)
        {
            var sw = Stopwatch.StartNew();

            var deviceRepo = await _deviceRepositoryRepo.GetDeviceRepositoryAsync(repoId);

            // If we are passing in a PIN, we are not expecting to have info in the claims so we can use this authorize method.
            if (pin == null) // TODO: Need to secure && false)
                await AuthorizeAsync(deviceRepo, AuthorizeResult.AuthorizeActions.Read, user, org);
            else
                if (deviceRepo.OwnerOrganization.Id != org.Id)
                    throw new UnauthorizedAccessException("Org mismatch.");

            if (deviceRepo.RepositoryType.Value != RepositoryTypes.Local)
            {
                var getSettingsResult = await _secureStorage.GetSecretAsync(org, deviceRepo.PEMStorageSettingsSecureId, user);
                if (!getSettingsResult.Successful)
                {
                    throw new Exception($"Could not restore secret for PEM Storage Connection Settings {deviceRepo.PEMStorageSettingsSecureId}, {getSettingsResult.Errors.First().Message}");
                }

                deviceRepo.PEMStorageSettings = JsonConvert.DeserializeObject<ConnectionSettings>(getSettingsResult.Result);

                getSettingsResult = await _secureStorage.GetSecretAsync(org, deviceRepo.DeviceArchiveStorageSettingsSecureId, user);
                if (!getSettingsResult.Successful)
                {
                    throw new Exception($"Could not restore secret for Device Archive Connection Settings {deviceRepo.DeviceArchiveStorageSettingsSecureId}, {getSettingsResult.Errors.First().Message} ");
                }

                deviceRepo.DeviceArchiveStorageSettings = JsonConvert.DeserializeObject<ConnectionSettings>(getSettingsResult.Result);

                getSettingsResult = await _secureStorage.GetSecretAsync(org, deviceRepo.DeviceWatchdogStorageSecureId, user);
                if (!getSettingsResult.Successful)
                {
                    throw new Exception($"Could not restore secret for Device Archive Connection Settings {deviceRepo.DeviceWatchdogStorageSecureId}, {getSettingsResult.Errors.First().Message} ");
                }

                deviceRepo.DeviceWatchdogStorageSettings = JsonConvert.DeserializeObject<ConnectionSettings>(getSettingsResult.Result);


                getSettingsResult = await _secureStorage.GetSecretAsync(org, deviceRepo.DeviceStorageSecureSettingsId, user);
                if (!getSettingsResult.Successful)
                {
                    throw new Exception($"Could not restore secret for Device Storage Connection Settings {deviceRepo.DeviceStorageSecureSettingsId}, {getSettingsResult.Errors.First().Message} ");
                }

                deviceRepo.DeviceStorageSettings = JsonConvert.DeserializeObject<ConnectionSettings>(getSettingsResult.Result);

                if (!string.IsNullOrEmpty(deviceRepo.SecureAccessKeyId))
                {
                    getSettingsResult = await _secureStorage.GetSecretAsync(org, deviceRepo.SecureAccessKeyId, user);
                    if (!getSettingsResult.Successful)
                    {
                        throw new Exception($"Could not restore secret for PEM Storage Connection Settings {deviceRepo.SecureAccessKeyId}, {getSettingsResult.Errors.First().Message} ");
                    }

                    deviceRepo.AccessKey = getSettingsResult.Result;
                }
            }

            _adminLogger.Trace($"[DeviceRepositoryManager__GetDeviceRepositoryWithSecretsAsync] - Got Repo {deviceRepo.Name} in {sw.Elapsed.TotalMilliseconds} ms");

            return deviceRepo;
        }

        public async Task<ListResponse<DeviceRepositorySummary>> GetDeploymentHostsForOrgAsync(string orgId, ListRequest listRequest, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, orgId, typeof(DeviceRepository));
            return await _deviceRepositoryRepo.GetDeviceRepositoriesForOrgAsync(orgId, listRequest);
        }


        public async Task<ListResponse<DeviceRepositorySummary>> GetAvailableDeploymentHostsForOrgAsync(string orgId, ListRequest listRequest, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, orgId, typeof(DeviceRepository));
            return await _deviceRepositoryRepo.GetAvailableDeviceRepositoriesForOrgAsync(orgId, listRequest);
        }

        public Task<bool> QueryKeyInUserAsync(string key, EntityHeader org)
        {
            return _deviceRepositoryRepo.QueryRepoKeyInUseAsync(key, org.Id);
        }

        public async Task<InvokeResult> UpdateDeviceRepositoryAsync(DeviceRepository repo, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(repo, Actions.Update);
            await AuthorizeAsync(repo, AuthorizeResult.AuthorizeActions.Update, user, org);

            if (repo.RepositoryType.Value == RepositoryTypes.NuvIoT ||
               repo.RepositoryType.Value == RepositoryTypes.AzureIoTHub)
            {
                if (String.IsNullOrEmpty(repo.DeviceArchiveStorageSettingsSecureId))
                {
                    repo.DeviceArchiveStorageSettings = new ConnectionSettings()
                    {
                        AccountId = _deviceMgmtSettings.DefaultDeviceTableStorage.AccountId,
                        AccessKey = _deviceMgmtSettings.DefaultDeviceTableStorage.AccessKey
                    };
                }

                if (String.IsNullOrEmpty(repo.PEMStorageSettingsSecureId))
                {
                    repo.PEMStorageSettings = new ConnectionSettings()
                    {
                        AccountId = _deviceMgmtSettings.DefaultDeviceTableStorage.AccountId,
                        AccessKey = _deviceMgmtSettings.DefaultDeviceTableStorage.AccessKey
                    };
                }

                if (String.IsNullOrEmpty(repo.DeviceStorageSecureSettingsId))
                {
                    repo.DeviceStorageSettings = new ConnectionSettings()
                    {
                        Uri = _deviceMgmtSettings.DefaultDeviceStorage.Uri,
                        AccessKey = _deviceMgmtSettings.DefaultDeviceStorage.AccessKey,
                        ResourceName = _deviceMgmtSettings.DefaultDeviceStorage.ResourceName
                    };
                }

                if (String.IsNullOrEmpty(repo.DeviceWatchdogStorageSecureId))
                {
                    repo.DeviceWatchdogStorageSettings = new ConnectionSettings()
                    {
                        Uri = _deviceMgmtSettings.DefaultDeviceTableStorage.Uri,
                        AccessKey = _deviceMgmtSettings.DefaultDeviceTableStorage.AccessKey,
                        ResourceName = $"Watchdlog{repo.Id}{repo.Key}"
                    };
                }
            }
            else if (repo.RepositoryType.Value == RepositoryTypes.Local)
            {

                //TODO: when we update this for remote server access, we need to figure out what if anything needs to be secured.
                if (String.IsNullOrEmpty(repo.DeviceArchiveStorageSettingsSecureId))
                {
                    repo.DeviceArchiveStorageSettings = new ConnectionSettings()
                    {
                        Uri = "mysql",
                        ResourceName = "nuviot",
                        Port = "3306"
                    };
                }

                if (String.IsNullOrEmpty(repo.DeviceWatchdogStorageSecureId))
                {
                    repo.DeviceWatchdogStorageSettings = new ConnectionSettings()
                    {
                        Uri = "mysql",
                        ResourceName = "nuviot",
                        Port = "3306"
                    };
                }

                if (String.IsNullOrEmpty(repo.PEMStorageSettingsSecureId))
                {
                    repo.PEMStorageSettings = new ConnectionSettings()
                    {
                        Uri = "mongodb",
                        Port = "27017",
                        ResourceName = "nuviot"
                    };
                }

                if (String.IsNullOrEmpty(repo.DeviceStorageSecureSettingsId))
                {
                    repo.DeviceStorageSettings = new ConnectionSettings()
                    {
                        Uri = "mongodb",
                        ResourceName = "nuviot",
                        Port = "27017"
                    };
                }
            }

            if(repo.DeviceAccountConnection != null && !String.IsNullOrEmpty(repo.DeviceAccountConnection.Password))
            {
                var addSecretResult = await _secureStorage.AddSecretAsync(org, repo.DeviceAccountConnection.Password);

                if (!string.IsNullOrEmpty(repo.DeviceAccountPasswordSecureId))
                {
                    await _secureStorage.RemoveSecretAsync(org, repo.DeviceAccountPasswordSecureId);
                }

                repo.DeviceAccountPasswordSecureId = addSecretResult.Result;
                repo.DeviceAccountConnection.Password = null;
            }

            if (!string.IsNullOrEmpty(repo.AccessKey))
            {
                var addSecretResult = await _secureStorage.AddSecretAsync(org, repo.AccessKey);
                if (!addSecretResult.Successful)
                {
                    return addSecretResult.ToInvokeResult();
                }

                if (!string.IsNullOrEmpty(repo.SecureAccessKeyId))
                {
                    await _secureStorage.RemoveSecretAsync(org, repo.SecureAccessKeyId);
                }

                repo.SecureAccessKeyId = addSecretResult.Result;
                repo.AccessKey = null;
            }

            if (repo.DeviceArchiveStorageSettings != null)
            {
                var addKeyResult = await _secureStorage.AddSecretAsync(org, JsonConvert.SerializeObject(repo.DeviceArchiveStorageSettings));
                if (!addKeyResult.Successful)
                {
                    return addKeyResult.ToInvokeResult();
                }

                if (!string.IsNullOrEmpty(repo.DeviceArchiveStorageSettingsSecureId))
                {
                    await _secureStorage.RemoveSecretAsync(org, repo.DeviceArchiveStorageSettingsSecureId);
                }

                repo.DeviceArchiveStorageSettingsSecureId = addKeyResult.Result;
                repo.DeviceArchiveStorageSettings = null;
            }

            if (repo.DeviceStorageSettings != null)
            {
                var addKeyResult = await _secureStorage.AddSecretAsync(org, JsonConvert.SerializeObject(repo.DeviceStorageSettings));
                if (!addKeyResult.Successful)
                {
                    return addKeyResult.ToInvokeResult();
                }

                if (!string.IsNullOrEmpty(repo.DeviceStorageSecureSettingsId))
                {
                    await _secureStorage.RemoveSecretAsync(org, repo.DeviceStorageSecureSettingsId);
                }

                repo.DeviceStorageSecureSettingsId = addKeyResult.Result;
                repo.DeviceStorageSettings = null;
            }

            if (repo.PEMStorageSettings != null)
            {
                var addKeyResult = await _secureStorage.AddSecretAsync(org, JsonConvert.SerializeObject(repo.PEMStorageSettings));
                if (!addKeyResult.Successful)
                {
                    return addKeyResult.ToInvokeResult();
                }

                if (!string.IsNullOrEmpty(repo.PEMStorageSettingsSecureId))
                {
                    await _secureStorage.RemoveSecretAsync(org, repo.PEMStorageSettingsSecureId);
                }

                repo.PEMStorageSettingsSecureId = addKeyResult.Result;
                repo.PEMStorageSettings = null;
            }

            if (repo.DeviceWatchdogStorageSettings != null)
            {
                var addKeyResult = await _secureStorage.AddSecretAsync(org, JsonConvert.SerializeObject(repo.DeviceWatchdogStorageSettings));
                if (!addKeyResult.Successful)
                {
                    return addKeyResult.ToInvokeResult();
                }

                if (!string.IsNullOrEmpty(repo.DeviceWatchdogStorageSecureId))
                {
                    await _secureStorage.RemoveSecretAsync(org, repo.DeviceWatchdogStorageSecureId);
                }

                repo.DeviceWatchdogStorageSecureId = addKeyResult.Result;
                repo.DeviceWatchdogStorageSettings = null;
            }   

            await _deviceRepositoryRepo.UpdateDeviceRepositoryAsync(repo);
            return InvokeResult.Success;
        }

        public async Task<DeviceRepository> GetDeviceRepositoryForInstanceAsync(string instanceId, EntityHeader org, EntityHeader user)
        {
            var deviceRepo = await _deviceRepositoryRepo.GetDeviceRepositoryForInstanceAsync(instanceId);
            await AuthorizeAsync(deviceRepo, AuthorizeResult.AuthorizeActions.Read, user, org);

            if (deviceRepo.RepositoryType.Value != RepositoryTypes.Local)
            {
                var getSettingsResult = await _secureStorage.GetSecretAsync(org, deviceRepo.PEMStorageSettingsSecureId, user);
                if (!getSettingsResult.Successful)
                {
                    throw new Exception($"Could not restore secret for PEM Storage Connection Settings {deviceRepo.PEMStorageSettingsSecureId} ");
                }

                deviceRepo.PEMStorageSettings = JsonConvert.DeserializeObject<ConnectionSettings>(getSettingsResult.Result);

                getSettingsResult = await _secureStorage.GetSecretAsync(org, deviceRepo.DeviceArchiveStorageSettingsSecureId, user);
                if (!getSettingsResult.Successful)
                {
                    throw new Exception($"Could not restore secret for Device Archive Connection Settings {deviceRepo.DeviceArchiveStorageSettingsSecureId} ");
                }

                deviceRepo.DeviceArchiveStorageSettings = JsonConvert.DeserializeObject<ConnectionSettings>(getSettingsResult.Result);

                getSettingsResult = await _secureStorage.GetSecretAsync(org, deviceRepo.DeviceWatchdogStorageSecureId, user);
                if (!getSettingsResult.Successful)
                {
                    throw new Exception($"Could not restore secret for Device watchdog Connection Settings {deviceRepo.DeviceWatchdogStorageSecureId} ");
                }

                deviceRepo.DeviceWatchdogStorageSettings = JsonConvert.DeserializeObject<ConnectionSettings>(getSettingsResult.Result);

                getSettingsResult = await _secureStorage.GetSecretAsync(org, deviceRepo.DeviceStorageSecureSettingsId, user);
                if (!getSettingsResult.Successful)
                {
                    throw new Exception($"Could not restore secret for Device Storage Connection Settings {deviceRepo.DeviceStorageSecureSettingsId} ");
                }

                deviceRepo.DeviceStorageSettings = JsonConvert.DeserializeObject<ConnectionSettings>(getSettingsResult.Result);

                if (!string.IsNullOrEmpty(deviceRepo.SecureAccessKeyId))
                {
                    getSettingsResult = await _secureStorage.GetSecretAsync(org, deviceRepo.SecureAccessKeyId, user);
                    if (!getSettingsResult.Successful)
                    {
                        throw new Exception($"Could not restore secret for PEM Storage Connection Settings {deviceRepo.SecureAccessKeyId} ");
                    }

                    deviceRepo.AccessKey = getSettingsResult.Result;
                }
            }
            return deviceRepo;
        }

        public async Task<InvokeResult<BasicTheme>> GetBasicThemeForRepoAsync(string orgid, string repoid)
        {
            var json = await _cacheProvider.GetAsync($"basic_theme_repo_{repoid}");
            if (string.IsNullOrEmpty(json))
            {
                var org = await _deviceRepositoryRepo.GetDeviceRepositoryAsync(repoid);
                var basicTheme = new BasicTheme()
                {
                    PrimaryTextColor = org.PrimaryTextColor,
                    PrimryBGColor = org.PrimaryBgColor,
                    AccentColor = org.AccentColor
                };

                await _cacheProvider.AddAsync($"basic_theme_repo_{repoid}", JsonConvert.SerializeObject(basicTheme));
                return InvokeResult<BasicTheme>.Create(basicTheme);
            }
            else
            {
                var theme = JsonConvert.DeserializeObject<BasicTheme>(json);
                return InvokeResult<BasicTheme>.Create(theme);

            }
        }

        public Task<InvokeResult<string>> GetRepoLogoAsync(string logId)
        {
            throw new NotImplementedException();
        }
    }
}
