using LagoVista.Core;
using LagoVista.Core.Exceptions;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Models.Geo;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Rpc.Client;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceAdmin.Interfaces.Repos;
using LagoVista.IoT.DeviceManagement.Core.Interfaces;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.MediaServices.Interfaces;
using LagoVista.MediaServices.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static LagoVista.Core.Models.AuthorizeResult;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public class DeviceManager : ManagerBase, IDeviceManager
    {
        //private readonly string _deviceRepoKey;
        private readonly IDeviceManagementRepo _defaultRepo;
        private readonly IDeviceConfigHelper _deviceConfigHelper;
        private readonly IProxyFactory _proxyFactory;
        private readonly IMediaServicesManager _mediaServicesManager;
        private readonly IDeviceExceptionRepo _deviceExceptionRepo;
        private readonly IDeviceArchiveRepo _deviceArchiveRepo;
        private readonly IDeviceConnectionEventRepo _deviceConnectionEventRepo;
        private readonly IDeviceRepositoryRepo _deviceRepoRepo;
        private readonly IDeviceTypeRepo _deviceTypeRepo;

        public IDeviceManagementRepo GetRepo(DeviceRepository deviceRepo)
        {
            return (deviceRepo.RepositoryType.Value == RepositoryTypes.Local ||
                deviceRepo.RepositoryType.Value == RepositoryTypes.ClusteredMongoDB) ?
                _proxyFactory.Create<IDeviceManagementRepo>(
                    new ProxySettings
                    {
                        OrganizationId = deviceRepo.OwnerOrganization.Id,
                        InstanceId = deviceRepo.Instance?.Id
                    }) :
                _defaultRepo;
        }

        public DeviceManager(
            IDeviceManagementRepo deviceRepo,
            IDeviceConfigHelper deviceConfigHelper,
            IAdminLogger logger,
             IAppConfig appConfig,
            IDependencyManager depmanager,
            ISecurity security,
            IDeviceExceptionRepo deviceExceptionRepo,
            IDeviceArchiveRepo deviceArchiveRepo,
            IDeviceConnectionEventRepo deviceConnectionEventRepo,
            IMediaServicesManager mediaServicesManager,
            IDeviceRepositoryRepo deviceRepoRepo,
            IDeviceTypeRepo deviceTypeRepo,
            IProxyFactory proxyFactory) :
            base(logger, appConfig, depmanager, security)
        {
            _defaultRepo = deviceRepo ?? throw new ArgumentNullException(nameof(deviceRepo));
            _deviceConfigHelper = deviceConfigHelper ?? throw new ArgumentNullException(nameof(deviceConfigHelper));
            _proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
            _deviceExceptionRepo = deviceExceptionRepo ?? throw new ArgumentNullException(nameof(deviceExceptionRepo));
            _deviceArchiveRepo = deviceArchiveRepo ?? throw new ArgumentNullException(nameof(deviceArchiveRepo));
            _deviceConnectionEventRepo = deviceConnectionEventRepo ?? throw new ArgumentNullException(nameof(deviceConnectionEventRepo));
            _mediaServicesManager = mediaServicesManager ?? throw new ArgumentNullException(nameof(mediaServicesManager));
            _deviceRepoRepo = deviceRepoRepo ?? throw new ArgumentNullException(nameof(deviceRepoRepo));
            _deviceTypeRepo = deviceTypeRepo ?? throw new ArgumentNullException(nameof(deviceTypeRepo));
        }

        /* 
         * In some cases we are using a 3rd party device repo, if so we'll keep the access key in secure storage and make sure it's only used in the manager, the repo will
         * be smart enough pull add or update from external repos.
         * 
         * If it's stored in an external repo, we also store a copy of it in our local repo so we can store the rest of the meta data. if we always go through the repo
         * to add devices, they will stay in sync.
         */
        //private async Task<InvokeResult> SetDeviceRepoAccessKeyAsync(DeviceRepository deviceRepo, EntityHeader org, EntityHeader user)
        //{
        //    if (String.IsNullOrEmpty(_deviceRepoKey))
        //    {
        //        var keyResult = await _secureStorage.GetSecretAsync(deviceRepo.SecureAccessKeyId, user, org);
        //        if (!keyResult.Successful) return keyResult.ToInvokeResult();

        //        _deviceRepoKey = keyResult.Result;
        //    }

        //    deviceRepo.AccessKey = _deviceRepoKey;

        //    return InvokeResult.Success;
        //}

        public async Task<InvokeResult> AddDeviceAsync(DeviceRepository deviceRepo, Device device, EntityHeader org, EntityHeader user)
        {
            await AuthorizeAsync(device, AuthorizeActions.Create, user, org);
            device.OwnerOrganization = org;
            if (String.IsNullOrEmpty(device.PrimaryAccessKey))
            {
                var guidBytes = new List<byte>(Guid.NewGuid().ToByteArray());
                var timeStamp = new List<byte>(BitConverter.GetBytes(DateTime.Now.Ticks));
                guidBytes.AddRange(timeStamp);
                device.PrimaryAccessKey = System.Convert.ToBase64String(guidBytes.ToArray());
            }

            if (String.IsNullOrEmpty(device.SecondaryAccessKey))
            {
                var guidBytes = new List<byte>(Guid.NewGuid().ToByteArray());
                var timeStamp = new List<byte>(BitConverter.GetBytes(DateTime.Now.Ticks));
                guidBytes.AddRange(timeStamp);
                device.SecondaryAccessKey = System.Convert.ToBase64String(guidBytes.ToArray());
            }

            device.DeviceRepository = new EntityHeader
            {
                Id = deviceRepo.Id,
                Text = deviceRepo.Name
            };

            ValidationCheck(device, Actions.Create);

            var existingDevice = await GetDeviceByDeviceIdAsync(deviceRepo, device.DeviceId, org, user);
            if (existingDevice != null)
            {
                return InvokeResult.FromErrors(Resources.ErrorCodes.DeviceExists.ToErrorMessage($"DeviceId={device.DeviceId}"));
            }

            var repo = GetRepo(deviceRepo);
            var result = await repo.AddDeviceAsync(deviceRepo, device);

            return result;
        }

        public async Task<InvokeResult> UpdateDeviceAsync(DeviceRepository deviceRepo, Device device, EntityHeader org, EntityHeader user)
        {
            await AuthorizeAsync(device, AuthorizeActions.Update, user, org);

            /* We need to populate the meta data so we can use it to validate the custom properties */
            await _deviceConfigHelper.PopulateDeviceConfigToDeviceAsync(device, deviceRepo.Instance, org, user);

            ValidationCheck(device, Actions.Update);

            device.DeviceType.Value = null;
            device.DeviceURI = null;
            device.PropertiesMetaData = null;
            device.InputCommandEndPoints = null;
            device.DeviceLabel = null;
            device.DeviceIdLabel = null;
            device.DeviceNameLabel = null;
            device.DeviceTypeLabel = null;

            device.LastUpdatedBy = user;
            device.LastUpdatedDate = DateTime.UtcNow.ToJSONString();

            var repo = GetRepo(deviceRepo);
            await repo.UpdateDeviceAsync(deviceRepo, device);

            return InvokeResult.Success;
        }

        private IDeviceConnectionEventRepo GetConnectionEventRepo(DeviceRepository deviceRepo)
        {
            return deviceRepo.RepositoryType.Value == RepositoryTypes.Local
                ? _proxyFactory.Create<IDeviceConnectionEventRepo>(
                    new ProxySettings
                    {
                        OrganizationId = deviceRepo.OwnerOrganization.Id,
                        InstanceId = deviceRepo.Instance.Id
                    })
                : _deviceConnectionEventRepo;
        }

        public async Task<InvokeResult<Device>> GetDeviceByMacAddressAsync(DeviceRepository deviceRepo, string macAddress, EntityHeader org, EntityHeader user)
        {
            if (String.IsNullOrEmpty(macAddress))
            {
                throw new ArgumentNullException(nameof(macAddress));
            }

            if (!Regex.IsMatch(macAddress, @"^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$"))
            {
                throw new ValidationException("Invalid mac address", false, $"{macAddress} is not a valid mac address.");
            }

            var repo = GetRepo(deviceRepo);
            if (repo == null)
            {
                throw new NullReferenceException(nameof(repo));
            }

            var device = await repo.GetDeviceByMacAddressAsync(deviceRepo, macAddress);
            if (device == null)
            {
                return InvokeResult<Device>.FromError($"Could not find device with mac address {macAddress}");
            }

            await AuthorizeAsync(device, AuthorizeActions.Read, user, org);
            return InvokeResult<Device>.Create(device);
        }

        public async Task<ListResponse<DeviceConnectionEvent>> GetConnectionEventsForDeviceAsync(DeviceRepository deviceRepo, string deviceId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            await AuthorizeAsync(user, org, "deviceconnectionlog", $"DeviceRepoId={deviceRepo.Id},DeviceId={deviceId}");

            var connectionEventRepo = GetConnectionEventRepo(deviceRepo);

            return await connectionEventRepo.GetConnectionEventsForDeviceAsync(deviceRepo, deviceId, listRequest);
        }

        public async Task<ListResponse<DeviceSummary>> GetDevicesForDeviceRepoAsync(DeviceRepository deviceRepo, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(Device));
            var repo = GetRepo(deviceRepo);
            var result = await repo.GetDevicesForRepositoryAsync(deviceRepo, org.Id, listRequest);
            return result;
        }

        public async Task<ListResponse<DeviceSummary>> GetDevicesForDeviceRepoForUserAsync(DeviceRepository deviceRepo, string userId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(Device));
            var repo = GetRepo(deviceRepo);
            var result = await repo.GetDevicesForRepositoryForUserAsync(deviceRepo, userId, org.Id, listRequest);
            return result;
        }

        public Task<ListResponse<DeviceSummary>> GetDevicesForLocationIdAsync(DeviceRepository deviceRepo, string locationId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            //TODO: Need to extender manager class for location access.

            var repo = GetRepo(deviceRepo);
            return repo.GetDevicesForLocationIdAsync(deviceRepo, locationId, listRequest);
        }

        public async Task<Device> GetDeviceByDeviceIdAsync(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user, bool populateMetaData = false)
        {
            var repo = GetRepo(deviceRepo);
            if (repo == null)
            {
                throw new NullReferenceException(nameof(repo));
            }

            var device = await repo.GetDeviceByDeviceIdAsync(deviceRepo, id);
            if (device == null)
            {
                return null;
            }
            if (string.IsNullOrEmpty(device.Name))
            {
                device.Name = device.DeviceId;
            }

            await AuthorizeAsync(device, AuthorizeActions.Read, user, org);

            if (populateMetaData)
            {
                await _deviceConfigHelper.PopulateDeviceConfigToDeviceAsync(device, deviceRepo.Instance, org, user);
            }

            return device;
        }

        public async Task<Device> GetDeviceByIdAsync(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user, bool populateMetaData = false)
        {
            var repo = GetRepo(deviceRepo);
            var device = await repo.GetDeviceByIdAsync(deviceRepo, id);

            if (device == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(device.Name))
            {
                device.Name = device.DeviceId;
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
            var repo = GetRepo(deviceRepo);
            var device = await repo.GetDeviceByIdAsync(deviceRepo, id);

            await AuthorizeAsync(device, AuthorizeActions.Delete, user, org);

            await repo.DeleteDeviceAsync(deviceRepo, id);

            return InvokeResult.Success;
        }

        public async Task<ListResponse<DeviceSummary>> GetDevicesInStatusAsync(DeviceRepository deviceRepo, string status, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(Device), Actions.Read, "DeviceInState");


            if (!Enum.TryParse<DeviceStates>(status, true, out var newState))
            {
                /* We aren't using newState as parsed by the enum, we are just validating it */
                return ListResponse<DeviceSummary>.FromErrors(Resources.ErrorCodes.SetStatus_InvalidOption.ToErrorMessage());
            }
            //We will be comparing against the id, the id will always be lower case
            status = status.ToLower();

            //TODO: Need to extend manager for security on this getting device w/ status

            var repo = GetRepo(deviceRepo);
            return await repo.GetDevicesInStatusAsync(deviceRepo, status, listRequest);
        }

        public async Task<ListResponse<DeviceSummary>> GetChildDevicesAsync(DeviceRepository deviceRepo, String parentDeviceId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {

            await AuthorizeOrgAccessAsync(user, org, typeof(Device), Actions.Read, "ChildDevices");
            var repo = GetRepo(deviceRepo);
            return await repo.GetChildDevicesAsync(deviceRepo, parentDeviceId, listRequest);
        }


        public async Task<ListResponse<DeviceSummary>> GetDevicesInCustomStatusAsync(DeviceRepository deviceRepo, string customStatus, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            //TODO: Need to extend manager for security on this getting device w/ status

            var repo = GetRepo(deviceRepo);
            return await repo.GetDevicesInCustomStatusAsync(deviceRepo, customStatus, listRequest);
        }

        public Task<ListResponse<DeviceSummary>> GetDevicesWithConfigurationAsync(DeviceRepository deviceRepo, string configurationId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            //TODO: Need to extend manager for security on this getting device w/ configuration

            var repo = GetRepo(deviceRepo);
            return repo.GetDevicesWithConfigurationAsync(deviceRepo, configurationId, listRequest);
        }

        public Task<ListResponse<DeviceSummary>> GetDevicesWithDeviceTypeAsync(DeviceRepository deviceRepo, string deviceTypeId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            var repo = GetRepo(deviceRepo);
            return repo.GetDevicesWithDeviceTypeAsync(deviceRepo, deviceTypeId, listRequest);
        }

        public async Task<DependentObjectCheckResult> CheckIfDeviceIdInUse(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user)
        {
            var repo = GetRepo(deviceRepo);
            var device = await repo.GetDeviceByIdAsync(deviceRepo, id);
            await AuthorizeAsync(device, AuthorizeActions.Update, user, org);
            return await CheckForDepenenciesAsync(device);
        }

        public Task<ListResponse<Device>> GetFullDevicesWithConfigurationAsync(DeviceRepository deviceRepo, string configurationId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            var repo = GetRepo(deviceRepo);
            return repo.GetFullDevicesWithConfigurationAsync(deviceRepo, configurationId, listRequest);
        }

        public async Task<InvokeResult> UpdateDeviceStatusAsync(DeviceRepository deviceRepo, string id, string status, EntityHeader org, EntityHeader user)
        {
            if (Enum.TryParse<DeviceStates>(status, true, out var newState))
            {
                var device = await GetDeviceByIdAsync(deviceRepo, id, org, user);
                if (device == null)
                {
                    return InvokeResult.FromErrors(Resources.ErrorCodes.CouldNotFindDeviceWithId.ToErrorMessage());
                }

                var oldState = device.Status.Text;

                await AuthorizeAsync(device, AuthorizeActions.Update, user, org);

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

                var repo = GetRepo(deviceRepo);
                await repo.UpdateDeviceAsync(deviceRepo, device);

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

            if (string.IsNullOrEmpty(note.CreationDate))
            {
                note.CreationDate = device.CreationDate;
            }

            if (string.IsNullOrEmpty(note.LastUpdatedDate))
            {
                note.LastUpdatedDate = device.LastUpdatedDate;
            }

            if (EntityHeader.IsNullOrEmpty(note.LastUpdatedBy))
            {
                note.LastUpdatedBy = user;
            }

            if (EntityHeader.IsNullOrEmpty(note.CreatedBy))
            {
                note.CreatedBy = user;
            }

            if (String.IsNullOrEmpty(note.Id))
            {
                note.Id = Guid.NewGuid().ToId();
            }

            ValidationCheck(note, Actions.Create);

            device.Notes.Add(note);

            var repo = GetRepo(deviceRepo);
            await repo.UpdateDeviceAsync(deviceRepo, device);

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

            var deviceStates = await _deviceConfigHelper.GetCustomDeviceStatesAsync(device.DeviceConfiguration.Id, org, user);
            if (deviceStates == null)
            {
                return InvokeResult.FromError("Could not load device states for device configuration.");
            }

            var newDeviceState = deviceStates.Value.States.Where(st => st.Key.ToLower() == customstatus.ToLower()).FirstOrDefault();
            if (newDeviceState == null)
            {
                return InvokeResult.FromError("Invalid status.");
            }

            device.CustomStatus = EntityHeader.Create(newDeviceState.Key, newDeviceState.Name);
            device.LastUpdatedBy = user;
            device.LastUpdatedDate = DateTime.UtcNow.ToJSONString();

            var repo = GetRepo(deviceRepo);
            await repo.UpdateDeviceAsync(deviceRepo, device);

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> UpdateGeoLocationAsync(DeviceRepository deviceRepo, string id, GeoLocation geoLocation, EntityHeader org, EntityHeader user)
        {
            if (geoLocation == null)
            {
                return InvokeResult.FromError("Geolocation must not be null.");
            }

            // todo: would really like to add history of device llocations, likely only if it moved.


            var device = await GetDeviceByIdAsync(deviceRepo, id, org, user);
            if (device == null)
            {
                return InvokeResult.FromErrors(Resources.ErrorCodes.CouldNotFindDeviceWithId.ToErrorMessage());
            }

            await AuthorizeAsync(device, AuthorizeActions.Update, user, org);

            device.GeoLocation = geoLocation;
            device.LastUpdatedBy = user;
            device.LastUpdatedDate = DateTime.UtcNow.ToJSONString();
            device.LocationLastUpdatedDate = device.LastUpdatedDate;

            var repo = GetRepo(deviceRepo);
            await repo.UpdateDeviceAsync(deviceRepo, device);

            return InvokeResult.Success;
        }

        public Task<ListResponse<DeviceSummary>> SearchByDeviceIdAsync(DeviceRepository deviceRepo, string searchString, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            var repo = GetRepo(deviceRepo);
            return repo.SearchByDeviceIdAsync(deviceRepo, searchString, listRequest);
        }

        public async Task<InvokeResult> AttachChildDeviceAsync(DeviceRepository deviceRepo, string parentDeviceId, string childDeviceId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            if (parentDeviceId == childDeviceId)
            {
                return InvokeResult.FromError("Parent is the same as the child device.");
            }

            var repo = GetRepo(deviceRepo);

            var childDevice = await repo.GetDeviceByIdAsync(deviceRepo, childDeviceId);
            await AuthorizeAsync(childDevice, AuthorizeActions.Update, user, org, "Attach this child to a parent");

            var parentDevice = await repo.GetDeviceByIdAsync(deviceRepo, parentDeviceId);
            await AuthorizeAsync(childDevice, AuthorizeActions.Update, user, org, "Attach a child to this parent");

            childDevice.ParentDevice = new EntityHeader<string>()
            {
                Id = parentDeviceId,
                Text = parentDevice.Name,
                Value = parentDevice.DeviceId,
            };

            await repo.UpdateDeviceAsync(deviceRepo, childDevice);

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> RemoveChildDeviceAsync(DeviceRepository deviceRepo, string parentDeviceId, string childDeviceId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            var repo = GetRepo(deviceRepo);

            var childDevice = await repo.GetDeviceByIdAsync(deviceRepo, childDeviceId);
            var parentDevice = await repo.GetDeviceByIdAsync(deviceRepo, parentDeviceId);

            await AuthorizeAsync(childDevice, AuthorizeActions.Update, user, org, "Remove parent from this child device");
            await AuthorizeAsync(parentDevice, AuthorizeActions.Update, user, org, "Remove child from this device");

            childDevice.ParentDevice = null;

            await repo.UpdateDeviceAsync(deviceRepo, childDevice);

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> AddDeviceExceptionAsync(DeviceRepository deviceRepo, DeviceException deviceException)
        {
            await _deviceExceptionRepo.AddDeviceExceptionAsync(deviceRepo, deviceException);
            return InvokeResult.Success;
        }

        public async Task<InvokeResult> ClearDeviceErrorAsync(DeviceRepository deviceRepo, string deviceId, string errorCode, EntityHeader org, EntityHeader user)
        {
            var repo = GetRepo(deviceRepo);
            var device = await repo.GetDeviceByIdAsync(deviceRepo, deviceId);

            var errorToRemove = device.Errors.Where(err => err.DeviceErrorCode == errorCode).FirstOrDefault();
            if (errorToRemove != null)
            {
                device.Errors.Remove(errorToRemove);
                await repo.UpdateDeviceAsync(deviceRepo, device);

                await _deviceExceptionRepo.AddDeviceExceptionAsync(deviceRepo, new DeviceException()
                {
                    DeviceId = device.DeviceId,
                    DeviceUniqueId = deviceId,
                    DeviceRepositoryId = deviceRepo.Id,
                    Details = $"Error cleared by {user.Text}",
                    ErrorCode = errorCode,
                    Timestamp = DateTime.UtcNow.ToJSONString()
                });
                return InvokeResult.Success;
            }
            else
            {
                return InvokeResult.FromError($"Could not find error {errorCode} on {device.DeviceId}.");
            }
        }

        public async Task<InvokeResult> ClearDeviceDataAsync(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user)
        {
            var device = await GetDeviceByIdAsync(deviceRepo, id, org, user);
            device.Attributes = new System.Collections.Generic.List<AttributeValue>();
            device.Properties = new System.Collections.Generic.List<AttributeValue>();
            device.PropertyBag = new System.Collections.Generic.Dictionary<string, object>();
            device.Notes = new System.Collections.Generic.List<DeviceNote>();
            device.States = new System.Collections.Generic.List<AttributeValue>();
            device.DeviceTwinDetails = new System.Collections.Generic.List<DeviceTwinDetails>();
            device.Errors = new System.Collections.Generic.List<DeviceError>();
            device.GeoLocation = null;

            await UpdateDeviceAsync(deviceRepo, device, org, user);

            await _deviceArchiveRepo.ClearDeviceArchivesAsync(deviceRepo, id);
            await _deviceExceptionRepo.ClearDeviceExceptionsAsync(deviceRepo, id);

            return InvokeResult.Success;
        }

        public async Task<InvokeResult<MediaResource>> AddDeviceImageAsync(DeviceRepository deviceRepo, string deviceId, Stream stream, string fileName, string contentType, EntityHeader org, EntityHeader user)
        {
            var imageId = Guid.NewGuid().ToId();
            await AuthorizeAsync(user.Id, org.Id, "Upload Device Image", $"Firmware Id: {deviceId}");
            var mediaSummary = await _mediaServicesManager.AddResourceMediaAsync(imageId, stream, fileName, contentType, org, user);

            var device = await _defaultRepo.GetDeviceByIdAsync(deviceRepo, deviceId);
            device.DeviceImages.Add(mediaSummary.Result);

            return mediaSummary;
        }

        public async Task<MediaServices.Models.MediaItemResponse> GetDeviceImageAsync(DeviceRepository deviceRepo, string deviceId, string mediaId, EntityHeader org, EntityHeader user)
        {
            await AuthorizeAsync(user.Id, org.Id, "Get Device Image", $"Device Id: {deviceId}, Image Id: {mediaId}");
            return await _mediaServicesManager.GetResourceMediaAsync(mediaId, org, user);
        }

        public async Task<InvokeResult<Device>> CreateDeviceAsync(DeviceRepository deviceRepo, string deviceTypeId, EntityHeader org, EntityHeader user)
        {
            if (!deviceRepo.AutoGenerateDeviceIds)
            {
                return InvokeResult<Device>.FromError("Can only create a device without a device id and name with a repository that is configure to auto generate device ids.");
            }

            var timeStamp = DateTime.UtcNow.ToJSONString();

            var device = new Device();
            /* Note we just create it here for now then the record gets inserted we go ahead assign the name */
            device.DeviceId = $"{deviceRepo.Key}{deviceRepo.IncrementingDeviceNumber:0000000}";
            deviceRepo.IncrementingDeviceNumber++;
            deviceRepo.LastUpdatedBy = user;
            deviceRepo.LastUpdatedDate = timeStamp;
            await _deviceRepoRepo.UpdateDeviceRepositoryAsync(deviceRepo);
            device.Name = device.DeviceId;
            device.OwnerOrganization = org;
            device.CreatedBy = user;
            device.LastUpdatedBy = user;
            device.CreationDate = timeStamp;
            device.LastUpdatedDate = timeStamp;

            if (String.IsNullOrEmpty(device.PrimaryAccessKey))
            {
                var guidBytes = new List<byte>(Guid.NewGuid().ToByteArray());
                var timeStampBytes = new List<byte>(BitConverter.GetBytes(DateTime.Now.Ticks));
                guidBytes.AddRange(timeStampBytes);
                device.PrimaryAccessKey = System.Convert.ToBase64String(guidBytes.ToArray());
            }

            if (String.IsNullOrEmpty(device.SecondaryAccessKey))
            {
                var guidBytes = new List<byte>(Guid.NewGuid().ToByteArray());
                var timeStampBytes = new List<byte>(BitConverter.GetBytes(DateTime.Now.Ticks));
                guidBytes.AddRange(timeStampBytes);
                device.SecondaryAccessKey = System.Convert.ToBase64String(guidBytes.ToArray());
            }

            var deviceType = await _deviceTypeRepo.GetDeviceTypeAsync(deviceTypeId);

            if (!EntityHeader.IsNullOrEmpty(deviceType.Firmware))
            {
                device.DesiredFirmware = deviceType.Firmware;
            }

            if (!EntityHeader.IsNullOrEmpty(deviceType.FirmwareRevision))
            {
                device.DesiredFirmwareRevision = deviceType.FirmwareRevision;
            }

            device.DeviceType = new EntityHeader<DeviceAdmin.Models.DeviceType>() { Id = deviceType.Id, Text = deviceType.Name, Key = deviceType.Key };
            device.DeviceConfiguration = deviceType.DefaultDeviceConfiguration;

            if (deviceRepo.UserOwnedDevicesOnly)
            {
                device.AssignedUser = user;
                device.OwnerUser = user;
            }

            var result = await AddDeviceAsync(deviceRepo, device, org, user);
            if (!result.Successful)
            {
                return InvokeResult<Device>.FromInvokeResult(result);
            }

            return InvokeResult<Device>.Create(device);
        }

        public async Task<InvokeResult> UpdateDeviceMacAddressAsync(DeviceRepository deviceRepo, string id, string macAddress, EntityHeader org, EntityHeader user)
        {
            var device = await GetDeviceByIdAsync(deviceRepo, id, org, user);
            device.MacAddress = macAddress;
            device.LastUpdatedDate = DateTime.UtcNow.ToJSONString();
            device.LastUpdatedBy = user;
            return await UpdateDeviceAsync(deviceRepo, device, org, user);
        }
    }
}
