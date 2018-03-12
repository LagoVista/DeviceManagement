using LagoVista.Core;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Models.Geo;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Interfaces;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.Logging.Loggers;
using System;
using System.Linq;
using System.Threading.Tasks;
using static LagoVista.Core.Models.AuthorizeResult;

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

            /* We need to populate the meta data so we can use it to validate the custom properties */
            await _deviceConfigHelper.PopulateDeviceConfigToDeviceAsync(device, deviceRepo.Instance, org, user);

            ValidationCheck(device, Actions.Update);

            if (deviceRepo.RepositoryType.Value == RepositoryTypes.AzureIoTHub)
            {
                var setRepoResult = await SetDeviceRepoAccessKeyAsync(deviceRepo, org, user);
                if (!setRepoResult.Successful) return setRepoResult;
            }


            device.DeviceURI = null;
            device.PropertiesMetaData = null;
            device.InputCommandEndPoints = null;

            device.LastUpdatedBy = user;
            device.LastUpdatedDate = DateTime.UtcNow.ToJSONString();

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

        public async Task<ListResponse<DeviceSummary>> GetDevicesForDeviceRepoAsync(DeviceRepository deviceRepo, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(Device));

            if (deviceRepo.RepositoryType.Value == RepositoryTypes.Local)
            {
                return await _deviceConnectorService.GetDevicesForOrgIdAsync(deviceRepo.Instance.Id, listRequest, org, user);
            }
            else
            {
                return await _deviceRepo.GetDevicesForRepositoryAsync(deviceRepo, org.Id, listRequest);
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

            if (String.IsNullOrEmpty(device.Name)) device.Name = device.DeviceId;

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

            if (device == null) return null;

            if (String.IsNullOrEmpty(device.Name)) device.Name = device.DeviceId;

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

        public async Task<ListResponse<DeviceSummary>> GetDevicesInStatusAsync(DeviceRepository deviceRepo, string status, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {            
            if (!Enum.TryParse<DeviceStates>(status, true, out DeviceStates newState))
            {
                /* We aren't using newState as parsed by the enum, we are just validating it */
                return ListResponse<DeviceSummary>.FromErrors(Resources.ErrorCodes.SetStatus_InvalidOption.ToErrorMessage());
            }
            //We will be comparing against the id, the id will always be lower case
            status = status.ToLower();

            //TODO: Need to extend manager for security on this getting device w/ status
            if (deviceRepo.RepositoryType.Value == RepositoryTypes.Local)
            {
                return await _deviceConnectorService.GetDevicesInStatusAsync(deviceRepo.Instance.Id, status, listRequest, org, user);
            }
            else
            {
                return await _deviceRepo.GetDevicesInStatusAsync(deviceRepo, status, listRequest);
            }
        }


        public async Task<ListResponse<DeviceSummary>> GetDevicesInCustomStatusAsync(DeviceRepository deviceRepo, string customStatus, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            //TODO: Need to extend manager for security on this getting device w/ status
            if (deviceRepo.RepositoryType.Value == RepositoryTypes.Local)
            {
                return await _deviceConnectorService.GetDevicesInCustomStatusAsync(deviceRepo.Instance.Id, customStatus, listRequest, org, user);
            }
            else
            {
                return await _deviceRepo.GetDevicesInCustomStatusAsync(deviceRepo, customStatus, listRequest);
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

        public Task<ListResponse<DeviceSummary>> GetDevicesWithDeviceTypeAsync(DeviceRepository deviceRepo, string deviceTypeId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            if (deviceRepo.RepositoryType.Value == RepositoryTypes.Local)
            {
                return _deviceConnectorService.GetDevicesWithDeviceTypeAsync(deviceRepo.Instance.Id, deviceTypeId, listRequest, org, user);
            }
            else
            {
                return _deviceRepo.GetDevicesWithDeviceTypeAsync(deviceRepo, deviceTypeId, listRequest);
            }
        }

        public async Task<DependentObjectCheckResult> CheckIfDeviceIdInUse(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user)
        {
            var device = await _deviceRepo.GetDeviceByIdAsync(deviceRepo, id);
            await AuthorizeAsync(device, AuthorizeActions.Update, user, org);
            return await CheckForDepenenciesAsync(device);
        }

        public Task<ListResponse<Device>> GetFullDevicesWithConfigurationAsync(DeviceRepository deviceRepo, string configurationId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            if (deviceRepo.RepositoryType.Value == RepositoryTypes.Local)
            {
                return _deviceConnectorService.GetFullDevicesWithConfigurationAsync(deviceRepo.Instance.Id, configurationId, listRequest, org, user);
            }
            else
            {
                return _deviceRepo.GetFullDevicesWithConfigurationAsync(deviceRepo, configurationId, listRequest);
            }
        }

        public async Task<InvokeResult> UpdateDeviceStatusAsync(DeviceRepository deviceRepo, string id, string status, EntityHeader org, EntityHeader user)
        {
            if (Enum.TryParse<DeviceStates>(status, true, out DeviceStates newState))
            {
                var device = await GetDeviceByIdAsync(deviceRepo, id, org, user);
                if (device == null)
                {
                    return InvokeResult.FromErrors(Resources.ErrorCodes.CouldNotFindDeviceWithId.ToErrorMessage());
                }

                var oldState = device.Status.Text;

                await AuthorizeAsync(device, AuthorizeActions.Update, user, org);
                
                if (deviceRepo.RepositoryType.Value == RepositoryTypes.AzureIoTHub)
                {
                    var setRepoResult = await SetDeviceRepoAccessKeyAsync(deviceRepo, org, user);
                    if (!setRepoResult.Successful) return setRepoResult;
                }

                device.Status = EntityHeader<DeviceStates>.Create(newState);
                device.LastUpdatedBy = user;
                device.LastUpdatedDate = DateTime.UtcNow.ToJSONString();
                device.Notes.Add(new DeviceNote()
                {
                    Title = "State Changed",
                    Notes = $"State changed from {oldState} to {device.Status.Text} by {user.Text}. ",
                    CreatedBy = user,
                    LastUpdatedBy = user,
                    CreationDate = device.LastUpdatedDate,
                    LastUpdatedDate = device.LastUpdatedDate,
                });

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
            else
            {
                return InvokeResult.FromErrors(Resources.ErrorCodes.SetStatus_InvalidOption.ToErrorMessage());
            }
        }

        public async Task<InvokeResult> AddNoteAsync(DeviceRepository deviceRepo, string id, DeviceNote note, EntityHeader org, EntityHeader user)
        {
            var device = await GetDeviceByIdAsync(deviceRepo, id, org, user);
            if (device == null)
            {
                return InvokeResult.FromErrors(Resources.ErrorCodes.CouldNotFindDeviceWithId.ToErrorMessage());
            }
            await AuthorizeAsync(device, AuthorizeActions.Update, user, org);

            if (deviceRepo.RepositoryType.Value == RepositoryTypes.AzureIoTHub)
            {
                var setRepoResult = await SetDeviceRepoAccessKeyAsync(deviceRepo, org, user);
                if (!setRepoResult.Successful) return setRepoResult;
            }

            device.LastUpdatedBy = user;
            device.LastUpdatedDate = DateTime.UtcNow.ToJSONString();

            if (String.IsNullOrEmpty(note.CreationDate)) note.CreationDate = device.CreationDate;
            if (String.IsNullOrEmpty(note.LastUpdatedDate)) note.LastUpdatedDate = device.LastUpdatedDate;
            if (EntityHeader.IsNullOrEmpty(note.LastUpdatedBy)) note.LastUpdatedBy = user;
            if (EntityHeader.IsNullOrEmpty(note.CreatedBy)) note.CreatedBy = user;

            device.Notes.Add(note);

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

        public async Task<InvokeResult> UpdateDeviceCustomStatusAsync(DeviceRepository deviceRepo, string id, string customstatus, EntityHeader org, EntityHeader user)
        {
            var device = await GetDeviceByIdAsync(deviceRepo, id, org, user);
            if (device == null)
            {
                return InvokeResult.FromErrors(Resources.ErrorCodes.CouldNotFindDeviceWithId.ToErrorMessage());
            }
            await AuthorizeAsync(device, AuthorizeActions.Update, user, org);

            if (deviceRepo.RepositoryType.Value == RepositoryTypes.AzureIoTHub)
            {
                var setRepoResult = await SetDeviceRepoAccessKeyAsync(deviceRepo, org, user);
                if (!setRepoResult.Successful) return setRepoResult;
            }

            var deviceStates = await _deviceConfigHelper.GetCustomDeviceStatesAsync(device.DeviceConfiguration.Id, org, user);
            if (deviceStates == null)
            {
                return InvokeResult.FromError("Could not load device states for device configuration.");
            }

            var newDeviceState = deviceStates.Value.States.Where(st => st.Key.ToLower() == customstatus.ToLower()).FirstOrDefault();
            if(newDeviceState == null)
            {
                return InvokeResult.FromError("Invalid status.");
            }

            device.CustomStatus = EntityHeader.Create(newDeviceState.Key, newDeviceState.Name);
            device.LastUpdatedBy = user;
            device.LastUpdatedDate = DateTime.UtcNow.ToJSONString();

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

        public async Task<InvokeResult> UpdateGeoLocationAsync(DeviceRepository deviceRepo, string id, GeoLocation geoLocation, EntityHeader org, EntityHeader user)
        {
            if(geoLocation == null)
            {
                return InvokeResult.FromError("Geolocation must not be null.");
            }
            

            var device = await GetDeviceByIdAsync(deviceRepo, id, org, user);
            if (device == null)
            {
                return InvokeResult.FromErrors(Resources.ErrorCodes.CouldNotFindDeviceWithId.ToErrorMessage());
            }

            await AuthorizeAsync(device, AuthorizeActions.Update, user, org);

            if (deviceRepo.RepositoryType.Value == RepositoryTypes.AzureIoTHub)
            {
                var setRepoResult = await SetDeviceRepoAccessKeyAsync(deviceRepo, org, user);
                if (!setRepoResult.Successful) return setRepoResult;
            }

            device.GeoLocation = geoLocation;
            device.LastUpdatedBy = user;
            device.LastUpdatedDate = DateTime.UtcNow.ToJSONString();

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

        public Task<ListResponse<DeviceSummary>> SearchByDeviceIdAsync(DeviceRepository deviceRepo, string searchString, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            if (deviceRepo.RepositoryType.Value == RepositoryTypes.Local)
            {
                return _deviceConnectorService.GetDevicesWithConfigurationAsync(deviceRepo.Instance.Id, searchString, listRequest, org, user);
            }
            else
            {
                return _deviceRepo.SearchByDeviceIdAsync(deviceRepo, searchString, listRequest);
            }
        }
    }
}

