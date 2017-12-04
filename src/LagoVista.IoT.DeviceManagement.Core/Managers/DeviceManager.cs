using System.Threading.Tasks;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.Core.Managers;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.Core.Interfaces;
using static LagoVista.Core.Models.AuthorizeResult;
using LagoVista.Core.Models;
using System.Collections.Generic;
using System;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.DeviceManagement.Core.Interfaces;
using LagoVista.Core.Models.UIMetaData;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public class DeviceManager : ManagerBase, IDeviceManager
    {
        IDeviceManagementRepo _deviceRepo;
        ISecureStorage _secureStorage;
        IDeviceConfigHelper _deviceConfigHelper;
        IDeviceManagementConnector _deviceConnectorService;

        String _deviceRepoKey;

        public DeviceManager(IDeviceManagementRepo deviceRepo, IDeviceManagementConnector deviceConnectorService,
            IDeviceConfigHelper deviceConfigHelper, IAdminLogger logger, ISecureStorage secureStorage,
            IAppConfig appConfig, IDependencyManager depmanager, ISecurity security) : base(logger, appConfig, depmanager, security)
        {
            _deviceRepo = deviceRepo;
            _secureStorage = secureStorage;
            _deviceConfigHelper = deviceConfigHelper;
            _deviceConnectorService = deviceConnectorService;
        }

        /* 
         * In some cases we are using a 3rd party device repo, if so we'll keep the access key in secure storage and make sure it's only used in the manager, the repo will
         * be smart enough pull add or update from external repos.
         * 
         * If it's stored in an external repo, we also store a copy of it in our local repo so we can store the rest of the meta data. if we always go through the repo
         * to add devices, they will stay in sync.
         */
        private async Task<InvokeResult> SetDeviceRepoAccessKeyAsync(DeviceRepository deviceRepo, EntityHeader org, EntityHeader user)
        {
            if (String.IsNullOrEmpty(_deviceRepoKey))
            {
                var keyResult = await _secureStorage.GetSecretAsync(deviceRepo.SecureAccessKeyId, user, org);
                if (!keyResult.Successful) return keyResult.ToInvokeResult();

                _deviceRepoKey = keyResult.Result;
            }

            deviceRepo.AccessKey = _deviceRepoKey;

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> AddDeviceAsync(DeviceRepository deviceRepo, Device device, EntityHeader org, EntityHeader user)
        {
            await AuthorizeAsync(device, AuthorizeActions.Create, user, org);
            ValidationCheck(device, Actions.Create);
            device.DeviceRepository = new EntityHeader
            {
                Id = deviceRepo.Id,
                Text = deviceRepo.Name
            };

            var existingDevice = await GetDeviceByDeviceIdAsync(deviceRepo, device.DeviceId, org, user);
            if (existingDevice != null)
            {
                return InvokeResult.FromErrors(Resources.ErrorCodes.DeviceExists.ToErrorMessage($"DeviceId={device.DeviceId}"));
            }

            if (deviceRepo.RepositoryType.Value == RepositoryTypes.AzureIoTHub)
            {
                var setRepoResult = await SetDeviceRepoAccessKeyAsync(deviceRepo, org, user);
                if (!setRepoResult.Successful) return setRepoResult;
            }

            InvokeResult result;

            if (deviceRepo.RepositoryType.Value == RepositoryTypes.Local)
            {
                result = await _deviceConnectorService.AddDeviceAsync(deviceRepo.Instance.Id, device, org, user);
            }
            else
            {
                result = await _deviceRepo.AddDeviceAsync(deviceRepo, device);
            }

            deviceRepo.AccessKey = null;
            return result;
        }

        public async Task<InvokeResult> UpdateDeviceAsync(DeviceRepository deviceRepo, Device device, EntityHeader org, EntityHeader user)
        {
            await AuthorizeAsync(device, AuthorizeActions.Update, user, org);
            ValidationCheck(device, Actions.Update);

            if (deviceRepo.RepositoryType.Value == RepositoryTypes.AzureIoTHub)
            {
                var setRepoResult = await SetDeviceRepoAccessKeyAsync(deviceRepo, org, user);
                if (!setRepoResult.Successful) return setRepoResult;
            }

            if (deviceRepo.RepositoryType.Value == RepositoryTypes.Local)
            {
                await _deviceConnectorService.UpdateDeviceAsync(deviceRepo.Instance.Id, device, org, user);
            }
            else
            {
                await _deviceRepo.UpdateDeviceAsync(deviceRepo, device);
            }
            deviceRepo.AccessKey = null;

            return InvokeResult.Success;
        }

        public async Task<ListResponse<DeviceSummary>> GetDevicesForOrgIdAsync(DeviceRepository deviceRepo, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(Device));

            if (deviceRepo.RepositoryType.Value == RepositoryTypes.Local)
            {
                return await _deviceConnectorService.GetDevicesForOrgIdAsync(deviceRepo.Instance.Id, listRequest, org, user);
            }
            else
            {
                return await _deviceRepo.GetDevicesForOrgIdAsync(deviceRepo, org.Id, listRequest);
            }
        }

        public Task<ListResponse<DeviceSummary>> GetDevicesForLocationIdAsync(DeviceRepository deviceRepo, string locationId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            //TODO: Need to extender manager class for location access.

            return _deviceRepo.GetDevicesForLocationIdAsync(deviceRepo, locationId, listRequest);
        }


        public async Task<Device> GetDeviceByDeviceIdAsync(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user, bool populateMetaData = false)
        {
            if (deviceRepo.RepositoryType.Value == RepositoryTypes.AzureIoTHub)
            {
                var setRepoResult = await SetDeviceRepoAccessKeyAsync(deviceRepo, org, user);
                if (!setRepoResult.Successful) return null;
            }

            Device device;

            if (deviceRepo.RepositoryType.Value == RepositoryTypes.Local)
            {
                device = await _deviceConnectorService.GetDeviceByDeviceIdAsync(deviceRepo.Instance.Id, id, org, user);
            }
            else
            {
                device = await _deviceRepo.GetDeviceByDeviceIdAsync(deviceRepo, id);
            }

            if (device == null) return null;

            await AuthorizeAsync(device, AuthorizeActions.Read, user, org);

            if (populateMetaData)
            {
                await _deviceConfigHelper.PopulateDeviceConfigToDeviceAsync(device, deviceRepo.Instance, org, user);
            }

            return device;
        }

        public async Task<Device> GetDeviceByIdAsync(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user, bool populateMetaData = false)
        {
            if (deviceRepo.RepositoryType.Value == RepositoryTypes.AzureIoTHub)
            {
                var setRepoResult = await SetDeviceRepoAccessKeyAsync(deviceRepo, org, user);
                if (!setRepoResult.Successful) return null;
            }

            Device device;

            if (deviceRepo.RepositoryType.Value == RepositoryTypes.Local)
            {
                device = await _deviceConnectorService.GetDeviceByIdAsync(deviceRepo.Instance.Id, id, org, user);
            }
            else
            {
                device = await _deviceRepo.GetDeviceByIdAsync(deviceRepo, id);
            }

            await AuthorizeAsync(device, AuthorizeActions.Read, user, org);
            deviceRepo.AccessKey = null;

            if (populateMetaData)
            {
                await _deviceConfigHelper.PopulateDeviceConfigToDeviceAsync(device, deviceRepo.Instance, org, user);
            }
            return device;
        }

        public async Task<InvokeResult> DeleteDeviceAsync(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user)
        {
            if (deviceRepo.RepositoryType.Value == RepositoryTypes.AzureIoTHub)
            {
                var setRepoResult = await SetDeviceRepoAccessKeyAsync(deviceRepo, org, user);
                if (!setRepoResult.Successful) return setRepoResult.ToInvokeResult();
            }

            var device = await _deviceRepo.GetDeviceByIdAsync(deviceRepo, id);
            await AuthorizeAsync(device, AuthorizeActions.Delete, user, org);

            if (deviceRepo.RepositoryType.Value == RepositoryTypes.Local)
            {
                await _deviceConnectorService.DeleteDeviceAsync(deviceRepo.Instance.Id, id, org, user);
                return InvokeResult.Success;
            }
            else
            {
                await _deviceRepo.DeleteDeviceAsync(deviceRepo, id);
                deviceRepo.AccessKey = null;

                return InvokeResult.Success;
            }
        }

        public Task<ListResponse<DeviceSummary>> GetDevicesInStatusAsync(DeviceRepository deviceRepo, string status, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            //TODO: Need to extend manager for security on this getting device w/ status
            if (deviceRepo.RepositoryType.Value == RepositoryTypes.Local)
            {
                return _deviceConnectorService.GetDevicesInStatusAsync(deviceRepo.Instance.Id, status, listRequest, org, user);
            }
            else
            {
                return _deviceRepo.GetDevicesInStatusAsync(deviceRepo, status, listRequest);
            }
        }

        public Task<ListResponse<DeviceSummary>> GetDevicesWithConfigurationAsync(DeviceRepository deviceRepo, string configurationId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            //TODO: Need to extend manager for security on this getting device w/ configuration
            if (deviceRepo.RepositoryType.Value == RepositoryTypes.Local)
            {
                 return _deviceConnectorService.GetDevicesWithConfigurationAsync(deviceRepo.Instance.Id, configurationId, listRequest, org, user);
            }
            else
            {
                return _deviceRepo.GetDevicesWithConfigurationAsync(deviceRepo, configurationId, listRequest);
            }
        }

        public async Task<DependentObjectCheckResult> CheckIfDeviceIdInUse(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user)
        {
            var device = await _deviceRepo.GetDeviceByIdAsync(deviceRepo, id);
            await AuthorizeAsync(device, AuthorizeActions.Update, user, org);
            return await CheckForDepenenciesAsync(device);
        }       
    }
}
