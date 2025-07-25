﻿using LagoVista.Core;
using LagoVista.Core.Exceptions;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Models.Geo;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Rpc.Client;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceAdmin.Interfaces.Repos;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.DeviceManagement.Core.Interfaces;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.MediaServices.Interfaces;
using LagoVista.MediaServices.Models;
using LagoVista.UserAdmin.Interfaces.Repos.Account;
using LagoVista.UserAdmin.Interfaces.Repos.Orgs;
using LagoVista.UserAdmin.Models.Orgs;
using LagoVista.UserAdmin.Models.Users;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static LagoVista.Core.Models.AuthorizeResult;
using LagoVista.PDFServices;
using QRCoder;
using RingCentral;
using LagoVista.UserAdmin.Interfaces.Repos.Users;

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
        private readonly IOrgLocationRepo _orgLocationRepo;
        private readonly ISecureStorage _secureStorage;
        private readonly ILocationDiagramRepo _diagramRepo;
        private readonly ILinkShortener _linkShortener;
        private readonly IAppConfig _appConfig;
        private readonly IDeviceGroupRepo _deviceGroupRepo;
        private readonly ISilencedAlarmsRepo _silencedAlarmsRepo;
        private readonly IDeviceStatusManager _deviceStatusManager;
        private readonly IDeviceOwnerRepo _deviceOwnerRepo;
        private readonly INotificationPublisher _notificationPublisher;
        private readonly IFirmwareRepo _firmwareRepo;
        private readonly IAppUserRepo _appUserRepo;

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
            IOrgLocationRepo orgLocationRepo,
            IProxyFactory proxyFactory,
            ISecureStorage secureStorage,
            ILinkShortener linkShortener,
            IDeviceGroupRepo deviceGroupRepo,
            ISilencedAlarmsRepo silencedAlarmsRepo,
            ILocationDiagramRepo locationDiagramRepo,
            IDeviceStatusManager devicestatusManager,
            IDeviceOwnerRepo deviceOwnerRepo,
            INotificationPublisher notificationPublisher,
            IAppUserRepo appUserRepo,
            IFirmwareRepo firmwareRepo) :
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
            _orgLocationRepo = orgLocationRepo ?? throw new ArgumentNullException(nameof(orgLocationRepo));
            _secureStorage = secureStorage ?? throw new ArgumentNullException(nameof(secureStorage));
            _linkShortener = linkShortener ?? throw new ArgumentNullException(nameof(linkShortener));
            _appConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
            _deviceGroupRepo = deviceGroupRepo ?? throw new ArgumentNullException(nameof(deviceGroupRepo));
            _silencedAlarmsRepo = silencedAlarmsRepo ?? throw new ArgumentNullException(nameof(silencedAlarmsRepo));
            _deviceStatusManager = devicestatusManager ?? throw new ArgumentNullException(nameof(devicestatusManager));
            _deviceOwnerRepo = deviceOwnerRepo ?? throw new ArgumentException(nameof(deviceOwnerRepo));
            _diagramRepo = locationDiagramRepo ?? throw new ArgumentNullException(nameof(locationDiagramRepo));
            _notificationPublisher = notificationPublisher ?? throw new ArgumentNullException(nameof(notificationPublisher));
            _firmwareRepo = firmwareRepo ?? throw new ArgumentNullException(nameof(firmwareRepo));
            _appUserRepo = appUserRepo ?? throw new ArgumentNullException(nameof(appUserRepo));
        }

        /* 
         * In some cases we are using a 3rd party result repo, if so we'll keep the access key in secure storage and make sure it's only used in the manager, the repo will
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

        public async Task<InvokeResult<Device>> AddDeviceAsync(DeviceRepository deviceRepo, Device device, bool reassign, EntityHeader org, EntityHeader user)
        {
            var timeStamp = DateTime.UtcNow.ToJSONString();

            if (EntityHeader.IsNullOrEmpty(device.DeviceConfiguration) && !EntityHeader.IsNullOrEmpty(device.DeviceType))
            {
                var deviceType = await _deviceTypeRepo.GetDeviceTypeAsync(device.DeviceType.Id);
                if (EntityHeader.IsNullOrEmpty(device.DeviceType))
                {
                    return InvokeResult<Device>.FromError($"The Device Model {deviceType.Name} does not have an associated Device Configuration and can not be used to create a Device.");
                }

                device.DeviceConfiguration = deviceType.DefaultDeviceConfiguration;
            }

            await AuthorizeAsync(device, AuthorizeActions.Create, user, org);
            device.OwnerOrganization = org;
            device.CreatedBy = user;
            device.CreationDate = timeStamp;
            device.LastUpdatedDate = timeStamp;
            if (String.IsNullOrEmpty(device.PrimaryAccessKey))
            {
                var guidBytes = new List<byte>(Guid.NewGuid().ToByteArray());
                var timeStampTicks = new List<byte>(BitConverter.GetBytes(DateTime.Now.Ticks));
                guidBytes.AddRange(timeStampTicks);
                device.PrimaryAccessKey = System.Convert.ToBase64String(guidBytes.ToArray());
            }

            if (String.IsNullOrEmpty(device.SecondaryAccessKey))
            {
                var guidBytes = new List<byte>(Guid.NewGuid().ToByteArray());
                var timeStamTicks = new List<byte>(BitConverter.GetBytes(DateTime.Now.Ticks));
                guidBytes.AddRange(timeStamTicks);
                device.SecondaryAccessKey = System.Convert.ToBase64String(guidBytes.ToArray());
            }

            device.DeviceRepository = new EntityHeader
            {
                Id = deviceRepo.Id,
                Text = deviceRepo.Name
            };

            ValidationCheck(device, Actions.Create);

            if (!String.IsNullOrEmpty(device.DevicePin))
            {
                var pinAddResult = await _secureStorage.AddSecretAsync(org, device.DevicePin);
                device.DevicePinSecureid = pinAddResult.Result;
                device.DevicePin = null;
            }

            var repo = GetRepo(deviceRepo);

            var existingDeviceResult = await GetDeviceByDeviceIdAsync(deviceRepo, device.DeviceId, org, user);
            if (existingDeviceResult != null && existingDeviceResult.Successful)
            {
                var existingDevice = existingDeviceResult.Result;

                if (reassign)
                {
                    var note = $"Device replaced by {user.Text};";
                    if (existingDevice.DeviceConfiguration.Id != device.DeviceConfiguration.Id)
                    {
                        note += $" Device Configuration Changed from {existingDevice.DeviceConfiguration.Text} to {device.DeviceConfiguration.Text}; ";
                    }

                    if (existingDevice.DeviceType.Id != device.DeviceType.Id)
                    {
                        note += $" Device Configuration Changed from {existingDevice.DeviceType.Text} to {device.DeviceConfiguration.Text}; ";
                    }

                    existingDevice.DeviceConfiguration = device.DeviceConfiguration;
                    existingDevice.DeviceType = device.DeviceType;
                    existingDevice.Name = device.Name;
                    existingDevice.iosBLEAddress = device.iosBLEAddress;
                    existingDevice.MacAddress = device.MacAddress;
                    existingDevice.Notes.Insert(0, new DeviceNote()
                    {
                        Id = Guid.NewGuid().ToId(),
                        Title = "Device Replaced",
                        Notes = note,
                        CreatedBy = user,
                        LastUpdatedBy = user,
                        LastUpdatedDate = timeStamp,
                        CreationDate = timeStamp,
                    });

                    existingDevice.LastUpdatedBy = user;
                    existingDevice.LastUpdatedDate = timeStamp;

                    Console.WriteLine($"[DeviceManager__AddDeviceAsync__Updating Existing] - Reassign ${reassign} - {existingDevice.Id}");

                    await repo.UpdateDeviceAsync(deviceRepo, existingDevice);

                    return InvokeResult<Device>.Create(existingDevice);
                }
                else
                    return InvokeResult<Device>.FromErrors(new ErrorMessage()
                    {
                        Message = Resources.ErrorCodes.DeviceExists.Message,
                        ErrorCode = Resources.ErrorCodes.DeviceExists.Code,
                        SystemError = false,
                        Details = $"A device with the device id: {device.DeviceId} already exists."
                    });
            }

            var result = await repo.AddDeviceAsync(deviceRepo, device);
            if (!result.Successful) return InvokeResult<Device>.FromInvokeResult(result);

            if (!EntityHeader.IsNullOrEmpty(device.Location))
            {
                var location = await _orgLocationRepo.GetLocationAsync(device.Location.Id);
                location.Devices.Add(new LocationDevice()
                {
                    Device = device.ToEntityHeader(),
                    DeviceRepo = device.DeviceRepository
                });

                await _orgLocationRepo.UpdateLocationAsync(location);
            }

            return InvokeResult<Device>.Create(device);
        }

        public async Task<InvokeResult> GenerateDeviceLabelAsync(DeviceRepository deviceRepo, string id, Stream stream, EntityHeader org, EntityHeader user)
        {
            var device = await GetDeviceByIdAsync(deviceRepo, id, org, user);

            using (var generator = new PDFGenerator())
            {
                if (String.IsNullOrEmpty(device.Result.ShortLinkAddress))
                {
                    var result = await _linkShortener.ShortenLinkAsync($"https://www.nuviot.com/devicemgmt/device/{org.Id}/{device.Result.DeviceRepository.Id}/{device.Result.Id}/view");
                    if (result.Successful)
                    {
                        device.Result.ShortLinkAddress = result.Result;
                        await UpdateDeviceAsync(deviceRepo, device.Result, org, user);
                    }
                    else
                        return result.ToInvokeResult();
                }

                generator.Margin = new Margin(0.2, 0.2, 0.2, 0.2);
                generator.ShowPageNumbers = false;
                generator.StartDocument(false, false, 1.96, 1.18);
               // generator.AddText(0.1, 0.1, device.Result.DeviceType.Text, Style.Body);
                generator.AddText(0.05, 0.1, device.Result.Name, Style.Body);
                generator.AddText(0.05, 0.3, device.Result.DeviceId, Style.Body);
                if (!String.IsNullOrEmpty(device.Result.SerialNumber))
                    generator.AddText(0.1, 0.5, device.Result.SerialNumber, Style.Body);

                generator.AddLogo(80, 50, 50, 0);

                using (var qrGenerator = new QRCodeGenerator())
                using (var qrCodeData = qrGenerator.CreateQrCode(device.Result.ShortLinkAddress, QRCodeGenerator.ECCLevel.Q))
                using (var qrCode = new PngByteQRCode(qrCodeData))
                using (var qrMS = new MemoryStream(qrCode.GetGraphic(20)))
                    generator.AddImage(1.15, 0.25, 1.1, 1.1, qrMS);

                generator.Write(stream);
                stream.Seek(0, SeekOrigin.Begin);

                return InvokeResult.Success;
            }
        }

        public async Task AuthenticateDevicePinAsync(DeviceRepository deviceRepo, string deviceId, string pin)
        {

        }

        public async Task<InvokeResult<Device>> UpdateDeviceAsync(DeviceRepository deviceRepo, Device device, EntityHeader org, EntityHeader user)
        {
            if (String.IsNullOrEmpty(device.Key)) device.Key = device.DeviceId.ToLower();
            var repo = GetRepo(deviceRepo);

            await AuthorizeAsync(device, AuthorizeActions.Update, user, org);

            var previousDevice = await repo.GetDeviceByIdAsync(deviceRepo, device.Id);

            Console.WriteLine($"Updating Device Location {device.Id}.");

            if (EntityHeader.IsNullOrEmpty(device.Location) && !EntityHeader.IsNullOrEmpty(previousDevice.Location))
            {
                Console.WriteLine("Remove device location.");
                // It had a value previously but does not any more.
                var location = await _orgLocationRepo.GetLocationAsync(previousDevice.Location.Id);
                var existing = location.Devices.FirstOrDefault(dev => dev.Device.Id == device.Id);
                if (existing != null)
                {
                    location.Devices.Remove(existing);
                    await _orgLocationRepo.UpdateLocationAsync(location);
                }
            }
            else if (!EntityHeader.IsNullOrEmpty(device.Location) && EntityHeader.IsNullOrEmpty(previousDevice.Location))
            {
                // It had a value previously but does not any more.
                var location = await _orgLocationRepo.GetLocationAsync(device.Location.Id);
                if (!location.Devices.Any(dev => dev.Device.Id == device.Id))
                {
                    location.Devices.Add(new LocationDevice()
                    {
                        Device = device.ToEntityHeader(),
                        DeviceRepo = device.DeviceRepository
                    });

                    await _orgLocationRepo.UpdateLocationAsync(location);
                }
            }
            else if (!EntityHeader.IsNullOrEmpty(device.Location) && !EntityHeader.IsNullOrEmpty(previousDevice.Location) &&
                 device.Location.Id != previousDevice.Location.Id)
            {
                Console.WriteLine("Changed.");

                // Device Location Changed.
                var previousLocation = await _orgLocationRepo.GetLocationAsync(previousDevice.Location.Id);
                var existing = previousLocation.Devices.FirstOrDefault(dev => dev.Device.Id == device.Id);
                if (existing != null)
                {
                    previousLocation.Devices.Remove(existing);
                    await _orgLocationRepo.UpdateLocationAsync(previousLocation);
                }

                var newLocation = await _orgLocationRepo.GetLocationAsync(device.Location.Id);
                if (!newLocation.Devices.Any(dev => dev.Device.Id == device.Id))
                {
                    newLocation.Devices.Add(new LocationDevice()
                    {
                        Device = device.ToEntityHeader(),
                        DeviceRepo = device.DeviceRepository
                    });
                    await _orgLocationRepo.UpdateLocationAsync(previousLocation);
                }

                Console.WriteLine("Change, done.");
            }


            device.DeviceType.Value = null;

            /* We need to populate the meta data so we can use it to validate the custom properties */
            await _deviceConfigHelper.PopulateDeviceConfigToDeviceAsync(device, deviceRepo.Instance, org, user);

            ValidationCheck(device, Actions.Update);

            device.DeviceURI = null;
            device.PropertiesMetaData = null;
            device.InputCommandEndPoints = null;
            device.DeviceLabel = null;
            device.AttributeMetaData = null;
            device.StateMachineMetaData = null;
            device.DeviceIdLabel = null;
            device.DeviceNameLabel = null;
            device.DeviceTypeLabel = null;

            device.LastUpdatedBy = user;
            device.LastUpdatedDate = DateTime.UtcNow.ToJSONString();

            if (!String.IsNullOrEmpty(device.DevicePin))
            {
                var pinAddResult = await _secureStorage.AddSecretAsync(org, device.DevicePin);
                device.DevicePinSecureid = pinAddResult.Result;
                device.DevicePin = null;
            }

            await repo.UpdateDeviceAsync(deviceRepo, device);
            Console.WriteLine("Updated location.");


            return InvokeResult<Device>.Create(device);
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

            if (!EntityHeader.IsNullOrEmpty(device.Location))
                device.Location.Value = await _orgLocationRepo.GetLocationAsync(device.Location.Id);

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
            //await AuthorizeOrgAccessAsync(user, org, typeof(Device));
            var repo = GetRepo(deviceRepo);
            var result = await repo.GetDevicesForRepositoryAsync(deviceRepo, org.Id, listRequest);
            return result;
        }

        public async Task<ListResponse<DeviceSummary>> GetDevicesForCustomerAsync(DeviceRepository deviceRepo, string customerId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(Device));
            var repo = GetRepo(deviceRepo);
            var result = await repo.GetDevicesForCustomerAsync(deviceRepo, org.Id, customerId, listRequest);
            return result;
        }

        public async Task<ListResponse<DeviceSummary>> GetDevicesForCustomerLocationAsync(DeviceRepository deviceRepo, string customerId, string customerLocationId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(Device));
            var repo = GetRepo(deviceRepo);
            var result = await repo.GetDevicesForCustomerLocationAsync(deviceRepo, org.Id, customerId, customerLocationId, listRequest);
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

        public async Task<InvokeResult<Device>> GetDeviceByDeviceIdAsync(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user, bool populateMetaData = false)
        {
            var timings = new List<ResultTiming>();
            var sw = Stopwatch.StartNew();
            var repo = GetRepo(deviceRepo);
            timings.Add(new ResultTiming() { Key = "Loaded Repo", Ms = sw.Elapsed.TotalMilliseconds });
            if (repo == null)
            {
                throw new NullReferenceException(nameof(repo));
            }
            sw.Restart();
            timings.Add(new ResultTiming() { Key = "Loaded Device", Ms = sw.Elapsed.TotalMilliseconds });

            var device = await repo.GetDeviceByDeviceIdAsync(deviceRepo, id);
            if (device == null)
            {
                return null;
            }
            if (string.IsNullOrEmpty(device.Name))
            {
                device.Name = device.DeviceId;
            }

            if (String.IsNullOrEmpty(device.Key))
            {
                device.Key = device.Id.ToLower();
            }

            if (!EntityHeader.IsNullOrEmpty(device.Location))
                device.Location.Value = await _orgLocationRepo.GetLocationAsync(device.Location.Id);

            var result = InvokeResult<Device>.Create(device);
            result.Timings.AddRange(timings);

            await AuthorizeAsync(device, AuthorizeActions.Read, user, org);

            if (populateMetaData)
            {
                var populateConfig = await _deviceConfigHelper.PopulateDeviceConfigToDeviceAsync(device, deviceRepo.Instance, org, user);
                result.Timings.AddRange(populateConfig.Timings);

                var deviceType = await _deviceTypeRepo.GetDeviceTypeAsync(device.DeviceType.Id);
                device.DesiredFirmware = deviceType.Firmware;
                device.DesiredFirmwareRevision = deviceType.FirmwareRevision;
            }

            return result;
        }

        public async Task<InvokeResult<Device>> GetDeviceByIdAsync(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user, bool populateMetaData = false)
        {
            var timings = new List<ResultTiming>();
            var sw = Stopwatch.StartNew();
            var repo = GetRepo(deviceRepo);
            timings.Add(new ResultTiming() { Key = "Loaded Repo", Ms = sw.Elapsed.TotalMilliseconds });
            sw.Restart();
            var device = await repo.GetDeviceByIdAsync(deviceRepo, id);
            timings.Add(new ResultTiming() { Key = "Loaded Device", Ms = sw.Elapsed.TotalMilliseconds });

            if (device == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(device.Name))
            {
                device.Name = device.DeviceId;
            }

            if (String.IsNullOrEmpty(device.Key))
            {
                device.Key = device.Key;
            }

            await AuthorizeAsync(device, AuthorizeActions.Read, user, org);
            deviceRepo.AccessKey = null;

            var result = InvokeResult<Device>.Create(device);
            result.Timings.AddRange(timings);

            if (!EntityHeader.IsNullOrEmpty(device.Location))
                device.Location.Value = await _orgLocationRepo.GetLocationAsync(device.Location.Id);


            if (populateMetaData)
            {
                var populateConfig = await _deviceConfigHelper.PopulateDeviceConfigToDeviceAsync(device, deviceRepo.Instance, org, user);
                result.Timings.AddRange(populateConfig.Timings);

                var deviceType = await _deviceTypeRepo.GetDeviceTypeAsync(device.DeviceType.Id);
                device.DesiredFirmware = deviceType.Firmware;
                device.DesiredFirmwareRevision = deviceType.FirmwareRevision;
            }
            return result;
        }

        public async Task<InvokeResult<Device>> GetDeviceByIdWithPinAsync(DeviceRepository deviceRepo, string id, string enteredPin, EntityHeader org, EntityHeader user, bool populateMetaData = false)
        {
            var timings = new List<ResultTiming>();
            var sw = Stopwatch.StartNew();
            var repo = GetRepo(deviceRepo);
            timings.Add(new ResultTiming() { Key = "Got Repo", Ms = sw.Elapsed.TotalMilliseconds });
            sw.Restart();

            var device = await repo.GetDeviceByIdAsync(deviceRepo, id);
            timings.Add(new ResultTiming() { Key = "Loaded Device", Ms = sw.Elapsed.TotalMilliseconds });

            if (device == null)
            {
                Logger.Trace($"[DeviceManager__GetDeviceByIdWithPinAsync] Could not load device for id: {id}.");
                return InvokeResult<Device>.FromError($"Could ont find device with id: {id}");
            }

            if (string.IsNullOrEmpty(device.Name))
            {
                device.Name = device.DeviceId;
            }

            if (String.IsNullOrEmpty(device.Key))
            {
                device.Key = device.Key;
            }

            if (String.IsNullOrEmpty(device.DevicePinSecureid))
            {
                Logger.Trace($"[DeviceManager__GetDeviceByIdWithPinAsync] does not have pin: {id}.");
                return InvokeResult<Device>.FromError("Device does not have a PIN.");
            }

            sw.Restart();
            var devicePin = await _secureStorage.GetSecretAsync(org, device.DevicePinSecureid, user);
            timings.Add(new ResultTiming() { Key = "Get Secret", Ms = sw.Elapsed.TotalMilliseconds });

            if (devicePin.Result != enteredPin)
            {
                Logger.Trace($"[DeviceManager__GetDeviceByIdWithPinAsync] invalid pin: {id}.");
                return InvokeResult<Device>.FromError("Invalid PIN.");
            }

            if (!EntityHeader.IsNullOrEmpty(device.Location))
                device.Location.Value = await _orgLocationRepo.GetLocationAsync(device.Location.Id);

            deviceRepo.AccessKey = null;

            var result = InvokeResult<Device>.Create(device);
            result.Timings.AddRange(timings);

            if (populateMetaData)
            {
                var populateConfig = await _deviceConfigHelper.PopulateDeviceConfigToDeviceAsync(device, deviceRepo.Instance, org, user);
                result.Timings.AddRange(populateConfig.Timings);
            }

            return result;
        }

        public async Task<InvokeResult> DeleteDeviceAsync(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user)
        {
            var repo = GetRepo(deviceRepo);
            var device = await repo.GetDeviceByIdAsync(deviceRepo, id);

            foreach (var grp in device.DeviceGroups)
            {
                await _deviceGroupRepo.RemoveDeviceGromGroupAsync(deviceRepo, grp.Id, id);
            }

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

            //TODO: Need to extend manager for security on this getting result w/ status

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
            //TODO: Need to extend manager for security on this getting result w/ status

            var repo = GetRepo(deviceRepo);
            return await repo.GetDevicesInCustomStatusAsync(deviceRepo, customStatus, listRequest);
        }

        public Task<ListResponse<DeviceSummary>> GetDevicesWithConfigurationAsync(DeviceRepository deviceRepo, string configurationId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            //TODO: Need to extend manager for security on this getting result w/ configuration

            var repo = GetRepo(deviceRepo);
            return repo.GetDevicesWithConfigurationAsync(deviceRepo, configurationId, listRequest);
        }

        public Task<ListResponse<DeviceSummary>> GetDevicesWithDeviceTypeAsync(DeviceRepository deviceRepo, string deviceTypeId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            var repo = GetRepo(deviceRepo);
            return repo.GetDevicesWithDeviceTypeAsync(deviceRepo, deviceTypeId, listRequest);
        }

        public Task<ListResponse<DeviceSummary>> GetDevicesWithDeviceTypeKeyAsync(DeviceRepository deviceRepo, string deviceTypeKey, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            var repo = GetRepo(deviceRepo);
            return repo.GetDevicesWithDeviceTypeKeyAsync(deviceRepo, deviceTypeKey, listRequest);
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
                var result = await GetDeviceByIdAsync(deviceRepo, id, org, user);
                if (result == null)
                {

                    return InvokeResult.FromErrors(Resources.ErrorCodes.CouldNotFindDeviceWithId.ToErrorMessage());
                }

                if (!result.Successful)
                {
                    return result.ToInvokeResult();
                }

                var device = result.Result;

                var oldState = device.Status.Text;

                await AuthorizeAsync(device, AuthorizeActions.Update, user, org);

                if (String.IsNullOrEmpty(device.Key)) device.Key = device.DeviceId;

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
            var result = await GetDeviceByIdAsync(deviceRepo, id, org, user);
            if (result == null)
            {

                return InvokeResult.FromErrors(Resources.ErrorCodes.CouldNotFindDeviceWithId.ToErrorMessage());
            }

            if (!result.Successful)
            {
                return result.ToInvokeResult();
            }

            var device = result.Result;

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
            var sw = Stopwatch.StartNew();
            var result = await GetDeviceByIdAsync(deviceRepo, id, org, user);
            if (result == null)
            {
                return InvokeResult.FromErrors(Resources.ErrorCodes.CouldNotFindDeviceWithId.ToErrorMessage());
            }

            result.Timings.Add(new ResultTiming() { Key = "getdevice", Ms = sw.ElapsedMilliseconds });
            sw.Restart();
            if (!result.Successful)
            {
                return result.ToInvokeResult();
            }

            var device = result.Result;
            await AuthorizeAsync(device, AuthorizeActions.Update, user, org);

            var deviceStates = await _deviceConfigHelper.GetCustomDeviceStatesAsync(device.DeviceConfiguration.Id, org, user);
            if (deviceStates == null)
            {
                return InvokeResult.FromError("Could not load device states for device configuration.");
            }

            result.Timings.Add(new ResultTiming() { Key = "getcustomstatus", Ms = sw.ElapsedMilliseconds });
            sw.Restart();

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

            result.Timings.Add(new ResultTiming() { Key = "udpatedevice", Ms = sw.ElapsedMilliseconds });

            return result.ToInvokeResult();
        }

        public async Task<InvokeResult> UpdateGeoLocationAsync(DeviceRepository deviceRepo, string id, GeoLocation geoLocation, EntityHeader org, EntityHeader user)
        {
            if (geoLocation == null)
            {
                return InvokeResult.FromError("Geolocation must not be null.");
            }

            // todo: would really like to add history of result llocations, likely only if it moved.
            var result = await GetDeviceByIdAsync(deviceRepo, id, org, user);
            if (result == null)
            {

                return InvokeResult.FromErrors(Resources.ErrorCodes.CouldNotFindDeviceWithId.ToErrorMessage());
            }

            if (!result.Successful)
            {
                return result.ToInvokeResult();
            }

            var device = result.Result;

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

            var errorToRemove = device.Errors.Where(err => err.DeviceErrorCode == errorCode || err.Id == errorCode).FirstOrDefault();
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
            var result = await GetDeviceByIdAsync(deviceRepo, id, org, user);
            if (result == null)
            {

                return InvokeResult.FromErrors(Resources.ErrorCodes.CouldNotFindDeviceWithId.ToErrorMessage());
            }

            if (!result.Successful)
            {
                return result.ToInvokeResult();
            }

            var device = result.Result;

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

        public async Task<InvokeResult<Device>> CreateDeviceAsync(DeviceRepository deviceRepo, DeviceType deviceType, EntityHeader org, EntityHeader user)
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

            var result = await AddDeviceAsync(deviceRepo, device, false, org, user);
            if (!result.Successful)
            {
                return result;
            }

            return InvokeResult<Device>.Create(device);
        }


        public async Task<InvokeResult> AddDeviceToLocationAsync(DeviceRepository deviceRepo, string id, string locationId, EntityHeader org, EntityHeader user)
        {
            var timeStamp = DateTime.UtcNow.ToJSONString();
            var result = await GetDeviceByIdAsync(deviceRepo, id, org, user);
            if (result == null)
            {

                return InvokeResult.FromErrors(Resources.ErrorCodes.CouldNotFindDeviceWithId.ToErrorMessage());
            }

            if (!result.Successful)
            {
                return result.ToInvokeResult();
            }

            var device = result.Result; var location = await _orgLocationRepo.GetLocationAsync(id);
            if (location.OwnerOrganization.Id != org.Id)
                throw new NotAuthorizedException($"Org mismatch requested org {location.OwnerOrganization.Text}, user org {org.Text}.");

            if (!EntityHeader.IsNullOrEmpty(device.Location))
            {
                var previousLocation = await _orgLocationRepo.GetLocationAsync(device.Location.Id);
                var devices = previousLocation.Devices.Where(dev => dev.Device.Id == id);
                foreach (var existingevice in devices)
                {
                    previousLocation.Devices.Remove(existingevice);
                }

                previousLocation.LastUpdatedBy = user;
                previousLocation.LastUpdatedDate = timeStamp;
                await _orgLocationRepo.UpdateLocationAsync(previousLocation);
            }

            location.Devices.Add(new UserAdmin.Models.Orgs.LocationDevice() { Device = device.ToEntityHeader(), DeviceRepo = deviceRepo.ToEntityHeader() });
            location.LastUpdatedBy = user;
            location.LastUpdatedDate = timeStamp;
            await _orgLocationRepo.UpdateLocationAsync(location);

            device.LastUpdatedDate = timeStamp;
            device.LastUpdatedBy = user;

            return (await UpdateDeviceAsync(deviceRepo, device, org, user)).ToInvokeResult();
        }


        public async Task<InvokeResult> RemoveDeviceFromLocation(DeviceRepository deviceRepo, string id, string locationId, EntityHeader org, EntityHeader user)
        {
            var timeStamp = DateTime.UtcNow.ToJSONString();

            var result = await GetDeviceByIdAsync(deviceRepo, id, org, user);
            if (result == null)
            {

                return InvokeResult.FromErrors(Resources.ErrorCodes.CouldNotFindDeviceWithId.ToErrorMessage());
            }

            if (!result.Successful)
            {
                return result.ToInvokeResult();
            }

            var device = result.Result; var previousLocation = await _orgLocationRepo.GetLocationAsync(device.Location.Id);
            var devices = previousLocation.Devices.Where(dev => dev.Device.Id == id);
            foreach (var existingevice in devices)
            {
                previousLocation.Devices.Remove(existingevice);
            }

            previousLocation.LastUpdatedBy = user;
            previousLocation.LastUpdatedDate = timeStamp;
            await _orgLocationRepo.UpdateLocationAsync(previousLocation);

            device.Location = null;
            device.LastUpdatedBy = user;
            device.LastUpdatedDate = timeStamp;

            return (await UpdateDeviceAsync(deviceRepo, device, org, user)).ToInvokeResult();
        }

        public async Task<InvokeResult<Device>> CreateDeviceForDeviceKeyAsync(DeviceRepository deviceRepo, string deviceTypeKey, EntityHeader org, EntityHeader user)
        {
            var deviceType = await _deviceTypeRepo.GetDeviceTypeForKeyAsync(org.Id, deviceTypeKey);

            if (deviceType == null)
                throw new RecordNotFoundException(nameof(deviceType), deviceTypeKey);

            return await CreateDeviceAsync(deviceRepo, deviceType, org, user);
        }

        public async Task<InvokeResult<Device>> CreateDeviceAsync(DeviceRepository deviceRepo, string deviceTypeId, EntityHeader org, EntityHeader user)
        {
            var deviceType = await _deviceTypeRepo.GetDeviceTypeAsync(deviceTypeId);
            return await CreateDeviceAsync(deviceRepo, deviceType, org, user);
        }

        public async Task<InvokeResult> UpdateDeviceMacAddressAsync(DeviceRepository deviceRepo, string id, string macAddress, EntityHeader org, EntityHeader user)
        {
            var result = await GetDeviceByIdAsync(deviceRepo, id, org, user);
            if (result == null)
            {

                return InvokeResult.FromErrors(Resources.ErrorCodes.CouldNotFindDeviceWithId.ToErrorMessage());
            }

            if (!result.Successful)
            {
                return result.ToInvokeResult();
            }

            var device = result.Result; device.MacAddress = macAddress;
            device.LastUpdatedDate = DateTime.UtcNow.ToJSONString();
            device.LastUpdatedBy = user;
            return (await UpdateDeviceAsync(deviceRepo, device, org, user)).ToInvokeResult();
        }

        public async Task<InvokeResult> UpdateDeviceiOSBleAddressAsync(DeviceRepository deviceRepo, string id, string iosBLEAddress, EntityHeader org, EntityHeader user)
        {
            var result = await GetDeviceByIdAsync(deviceRepo, id, org, user);
            if (result == null)
            {

                return InvokeResult.FromErrors(Resources.ErrorCodes.CouldNotFindDeviceWithId.ToErrorMessage());
            }

            if (!result.Successful)
            {
                return result.ToInvokeResult();
            }

            var device = result.Result; device.iosBLEAddress = iosBLEAddress;
            device.LastUpdatedDate = DateTime.UtcNow.ToJSONString();
            device.LastUpdatedBy = user;
            return (await UpdateDeviceAsync(deviceRepo, device, org, user)).ToInvokeResult();
        }

        public async Task<InvokeResult<Device>> GetDeviceByiOSBLEAddressAsync(DeviceRepository deviceRepo, string iosBLEAddress, EntityHeader org, EntityHeader user)
        {
            if (String.IsNullOrEmpty(iosBLEAddress))
            {
                throw new ArgumentNullException(nameof(iosBLEAddress));
            }

            if (!Regex.IsMatch(iosBLEAddress, @"^[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}$"))
            {
                throw new ValidationException("Invalid mac address", false, $"{iosBLEAddress} is not a valid mac address.");
            }

            var repo = GetRepo(deviceRepo);
            if (repo == null)
            {
                throw new NullReferenceException(nameof(repo));
            }

            var device = await repo.GetDeviceByiOSBLEAddressAsync(deviceRepo, iosBLEAddress);
            if (device == null)
            {
                return InvokeResult<Device>.FromError($"Could not find device with mac address {iosBLEAddress}");
            }

            await AuthorizeAsync(device, AuthorizeActions.Read, user, org);
            return InvokeResult<Device>.Create(device);
        }

        public async Task<InvokeResult<string>> GetShortenedDeviceLinkAsync(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user)
        {
            var link = $"{_appConfig.WebAddress}/devicemgmt/device/{org.Id}/{deviceRepo.Id}/{id}/view";
            var shortened = await _linkShortener.ShortenLinkAsync(link);
            return shortened;
        }


        public Task<InvokeResult<Device>> HandleDeviceOnlineAsync(Device device, EntityHeader org, EntityHeader user)
        {
            return Task.FromResult(InvokeResult<Device>.Create(device));
        }

        public Task<InvokeResult<Device>> HandleDeviceOfflineAsync(Device device, EntityHeader org, EntityHeader user)
        {
            return Task.FromResult(InvokeResult<Device>.Create(device));
        }


        public async Task<InvokeResult<string>> GetDevicePinAsync(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user)
        {
            var repo = GetRepo(deviceRepo);
            if (repo == null)
            {
                throw new NullReferenceException(nameof(repo));
            }

            var device = await repo.GetDeviceByIdAsync(deviceRepo, id);
            if (device == null)
            {
                return InvokeResult<string>.FromError($"Could not find device with id {id}");
            }

            if (String.IsNullOrEmpty(device.DevicePinSecureid))
            {
                return InvokeResult<string>.FromError("Device does not have a PIN assigned.");
            }

            await AuthorizeAsync(device, AuthorizeActions.Read, user, org, "GetDevicePin");

            var getSecretResult = await _secureStorage.GetSecretAsync(org, device.DevicePinSecureid, user);
            if (!getSecretResult.Successful)
                return InvokeResult<string>.FromInvokeResult(getSecretResult.ToInvokeResult());

            return InvokeResult<string>.Create(getSecretResult.Result);
        }

        public async Task<InvokeResult<Device>> SetDevicePinAsync(DeviceRepository deviceRepo, string id, string pin, EntityHeader org, EntityHeader user)
        {
            if (String.IsNullOrEmpty(pin))
            {
                return InvokeResult<Device>.FromError($"Must provide a pin");
            }

            var regEx = new Regex(@"^[A-Za-z0-9]{4,8}$");
            if (!regEx.Match(pin).Success)
            {
                return InvokeResult<Device>.FromError($"Must provide a pin that is between 4 and 9 characters and must include only letters and numbers.");
            }

            var device = await GetDeviceByIdAsync(deviceRepo, id, org, user);
            device.Result.DevicePin = pin;
            await UpdateDeviceAsync(deviceRepo, device.Result, org, user);

            return InvokeResult<Device>.Create(device.Result);
        }

        public async Task<InvokeResult<Device>> ClearDevicePinAsync(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user)
        {
            var result = await GetDeviceByIdAsync(deviceRepo, id, org, user);

            var device = result.Result;
            if (String.IsNullOrEmpty(device.DevicePinSecureid))
                return result;

            await _secureStorage.RemoveSecretAsync(org, device.DevicePinSecureid);
            device.DevicePinSecureid = null;
            await UpdateDeviceAsync(deviceRepo, device, org, user);

            return InvokeResult<Device>.Create(result.Result);
        }

        public async Task<ListResponse<DeviceOwnerUser>> GetDeviceOwnersForDeviceAsync(DeviceRepository deviceRepo, string deviceId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            // security check, will throw if not authorized.
            var result = await GetDeviceByIdAsync(deviceRepo, deviceId, org, user);

            return await _deviceOwnerRepo.GetDeviceOwnersForDeviceAsync(deviceId, listRequest);
        }

        public async Task<InvokeResult> SetDeviceOwnerRegistrationAsync(DeviceRepository deviceRepo, string id, DeviceOwnerUser owner, EntityHeader org, EntityHeader user)
        {
            var result = await GetDeviceByIdAsync(deviceRepo, id, org, user);
            if (result.Successful)
            {
                var device = result.Result;
                return (await UpdateDeviceAsync(deviceRepo, result.Result, org, user)).ToInvokeResult();
            }
            else
            {
                return result.ToInvokeResult();
            }
        }

        public async Task<InvokeResult> SilenceAlarmsAsync(DeviceRepository deviceRepo, Device device, EntityHeader org, EntityHeader user)
        {
            device.SilenceAlarms = true;
            device.SilencedBy = org;
            device.SilencedTimeStamp = DateTime.UtcNow.ToJSONString();

            await UpdateDeviceAsync(deviceRepo, device, org, user);

            await _deviceStatusManager.SetSilenceAlarmAsync(deviceRepo, device.Id, true, org, user);

            await _silencedAlarmsRepo.AddSilencedAlarmAsync(deviceRepo, new SilencedAlarm()
            {
                Disabled = true,
                Device = device.ToEntityHeader(),
                DeviceRepo = deviceRepo.ToEntityHeader(),
                User = user,
                Timestamp = DateTime.UtcNow.ToJSONString(),
            });

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> SilenceErrorAsync(DeviceRepository deviceRepo, Device device, string errorId, EntityHeader org, EntityHeader user)
        {
            var error = device.Errors.SingleOrDefault(err => err.Id == errorId);
            if (error != null)
            {
                if (error.Silenced)
                    return InvokeResult.FromError("error has already been silenced.");

                error.Silenced = true;
                error.SilencedBy = user.Text;
                error.SilencedById = user.Id;
                error.SilencedTimeStamp = DateTime.UtcNow.ToJSONString();
                return (await UpdateDeviceAsync(deviceRepo, device, org, user)).ToInvokeResult();
            }

            return InvokeResult.FromError("Sorry, could not find the error on this device to silence, it might have been cleared.");
        }

        public async Task<InvokeResult> SilenceErrorAsync(DeviceRepository deviceRepo, string id, string errorId, EntityHeader org, EntityHeader user)
        {
            var device = await GetDeviceByIdAsync(deviceRepo, id, org, user);
            return await SilenceErrorAsync(deviceRepo, device.Result, errorId, org, user);
        }

        public async Task<InvokeResult> SilenceAlarmsAsync(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user)
        {
            var result = await GetDeviceByIdAsync(deviceRepo, id, org, user);
            var device = result.Result;
            device.SilenceAlarms = true;
            device.SilencedBy = org;
            device.SilencedTimeStamp = DateTime.UtcNow.ToJSONString();


            await UpdateDeviceAsync(deviceRepo, device, org, user);

            await _deviceStatusManager.SetSilenceAlarmAsync(deviceRepo, id, true, org, user);

            await _silencedAlarmsRepo.AddSilencedAlarmAsync(deviceRepo, new SilencedAlarm()
            {
                Disabled = true,
                Device = device.ToEntityHeader(),
                DeviceRepo = deviceRepo.ToEntityHeader(),
                User = user,
                Timestamp = DateTime.UtcNow.ToJSONString(),
            });

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> EnableAlarmsAsync(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user)
        {
            var result = await GetDeviceByIdAsync(deviceRepo, id, org, user);
            var device = result.Result;
            device.SilenceAlarms = false;
            device.SilencedBy = null;
            device.SilencedTimeStamp = null;

            await UpdateDeviceAsync(deviceRepo, device, org, user);

            await _silencedAlarmsRepo.AddSilencedAlarmAsync(deviceRepo, new SilencedAlarm()
            {
                Disabled = false,
                Device = device.ToEntityHeader(),
                DeviceRepo = deviceRepo.ToEntityHeader(),
                User = user,
                Timestamp = DateTime.UtcNow.ToJSONString(),
            }); ;

            return InvokeResult.Success;
        }

        public Task<ListResponse<SilencedAlarm>> GetSilenceAlarmsAsync(DeviceRepository deviceRepo, string id, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            return _silencedAlarmsRepo.GetSilencedAlarmAsync(deviceRepo, listRequest, id);
        }

        public async Task<InvokeResult> CommissionDeviceAsync(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user)
        {
            return InvokeResult.Success;
        }

        public async Task<InvokeResult> DecommissionDeviceAsync(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user)
        {
            return InvokeResult.Success;
        }

        public async Task<InvokeResult<List<GeoLocation>>> GetDeviceBoundingBoxAsync(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user)
        {
            var result = await GetDeviceByIdAsync(deviceRepo, id, org, user);
            var device = result.Result;
            if (device.GeoPointsBoundingBox.Any())
            {
                return InvokeResult<List<GeoLocation>>.Create(device.GeoPointsBoundingBox);
            }

            if (EntityHeader.IsNullOrEmpty(device.Location))
            {
                return InvokeResult<List<GeoLocation>>.FromError("Device does not have a location set.");
            }

            var location = await _orgLocationRepo.GetLocationAsync(device.Location.Id);
            if (location.GeoPointsBoundingBox.Any())
            {
                return InvokeResult<List<GeoLocation>>.Create(location.GeoPointsBoundingBox);
            }

            var firstReference = location.DiagramReferences.FirstOrDefault();
            if (firstReference == null)
            {
                return InvokeResult<List<GeoLocation>>.FromError("Location does not have any diagram references.");
            }

            var diagarm = await _diagramRepo.GetLocationDiagramAsync(firstReference.LocationDiagram.Id);
            var layer = diagarm.Layers.FirstOrDefault(lyr => lyr.Id == firstReference.LocationDiagramLayer.Id);
            var shape = layer.Shapes.FirstOrDefault(shp => shp.Id == firstReference.LocationDiagramShape.Id);

            if (shape.GeoPoints.Any())
                return InvokeResult<List<GeoLocation>>.Create(shape.GeoPoints);

            return InvokeResult<List<GeoLocation>>.FromError("Diagram shape does not have any geo points.");
        }

        public async Task<InvokeResult<Device>> AttachToDiagramAsync(DeviceRepository deviceRepo, string id, OrgLocationDiagramReference diagramReference, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(diagramReference, Actions.Create);
            var result = await GetDeviceByIdAsync(deviceRepo, id, org, user);

            if (null != result.Result.DiagramReference)
                return InvokeResult<Device>.FromError($"Device is already associated with: {result.Result.DiagramReference.LocationDiagram.Text}/{result.Result.DiagramReference.LocationDiagramLayer.Text}/{result.Result.DiagramReference.LocationDiagramShape.Text}");

            var diagram = await _diagramRepo.GetLocationDiagramAsync(diagramReference.LocationDiagram.Id);
            if (diagram.OwnerOrganization.Id != org.Id)
                throw new NotAuthorizedException("Org mismatch between diagram and current user.");

            var layer = diagram.Layers.FirstOrDefault(lyr => lyr.Id == diagramReference.LocationDiagramLayer.Id);
            var shape = layer.Shapes.FirstOrDefault(shp => shp.Id == diagramReference.LocationDiagramShape.Id);
            var deviceReference = new DeviceReference()
            {
                DeviceRepository = deviceRepo.ToEntityHeader(),
                Device = result.Result.ToEntityHeader()
            };

            shape.Devices.Add(deviceReference);

            result.Result.DiagramReference = diagramReference;
            await UpdateDeviceAsync(deviceRepo, result.Result, org, user);
            await _diagramRepo.UpdateLocationDiagramAsync(diagram);

            return InvokeResult<Device>.Create(result.Result);
        }

        public async Task<InvokeResult<Device>> RemoveFromDiagramAsync(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user)
        {

            var result = await GetDeviceByIdAsync(deviceRepo, id, org, user);
            var diagramReference = result.Result.DiagramReference;
            if (result.Result.DiagramReference == null)
                return InvokeResult<Device>.FromError("Device is not attached to a diagram");

            var diagram = await _diagramRepo.GetLocationDiagramAsync(diagramReference.LocationDiagram.Id);
            var layer = diagram.Layers.FirstOrDefault(lyr => lyr.Id == diagramReference.LocationDiagramLayer.Id);
            var shape = layer.Shapes.FirstOrDefault(shp => shp.Id == diagramReference.LocationDiagramShape.Id);
            var existing = shape.Devices.FirstOrDefault(dvc => dvc.Device.Id == id);
            shape.Devices.Remove(existing);
            result.Result.DiagramReference = null;

            await _diagramRepo.UpdateLocationDiagramAsync(diagram);
            await UpdateDeviceAsync(deviceRepo, result.Result, org, user);

            return InvokeResult<Device>.Create(result.Result);
        }

        public async Task<InvokeResult<Device>> CompleteQaCheckAsync(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user)
        {
            var device = await GetDeviceByIdAsync(deviceRepo, id, org, user);
            device.Result.QaCheckCompleted = true;
            device.Result.QaCheckCompletedBy = user;
            device.Result.QaCheckCompletedTimeeStamp = DateTime.UtcNow.ToJSONString();
            var result = await UpdateDeviceAsync(deviceRepo, device.Result, org, user);
            device.Result.AuditHistory.Add(new EntityChangeSet()
            {
                ChangeDate = DateTime.UtcNow.ToJSONString(),
                ChangedBy = user,
                Changes = { new EntityChange() { Field = nameof(Device.QaCheckCompleted), NewValue = "true", OldValue = "false", Notes = $"Changed by: {user.Text}" } }
            });

            if (result.Successful)
                return InvokeResult<Device>.Create(device.Result);

            // TODO:YUCK!
            return InvokeResult<Device>.FromInvokeResult(result.ToInvokeResult());
        }

        public async Task<InvokeResult<Device>> RemoveQaCheckAsync(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user)
        {
            var device = await GetDeviceByIdAsync(deviceRepo, id, org, user);
            if(device.Result.QaCheckCompleted == false)
            {
                return InvokeResult<Device>.Create(device.Result);
            }

            device.Result.QaCheckCompleted = false;
            device.Result.QaCheckCompletedBy = null;
            device.Result.QaCheckCompletedTimeeStamp = null;
            device.Result.AuditHistory.Add(new EntityChangeSet()
            {
                ChangeDate = DateTime.UtcNow.ToJSONString(),
                ChangedBy = user,
                Changes = { new EntityChange() { Field = nameof(Device.QaCheckCompleted), NewValue = "false", OldValue = "true", Notes = $"Changed by: {user.Text}" } }
            });
            var result = await UpdateDeviceAsync(deviceRepo, device.Result, org, user);
            if (result.Successful)
                return InvokeResult<Device>.Create(device.Result);

            // TODO:YUCK!
            return InvokeResult<Device>.FromInvokeResult(result.ToInvokeResult());
        }

        public async Task<InvokeResult<PublicDeviceInfo>> GetPublicDeviceInfo(DeviceRepository deviceRepo, string deviceId)
        {
            var repo = GetRepo(deviceRepo);
            var device = await repo.GetDeviceByIdAsync(deviceRepo, deviceId);
            var deviceType = await _deviceTypeRepo.GetDeviceTypeAsync(device.DeviceType.Id);

            var deviceInfo = new PublicDeviceInfo()
            {
                Customer = device.Customer,
             
                OwnerOrganziation = device.OwnerOrganization,
                DeviceType = device.DeviceType,
                DeviceName = device.Name,
                DeviceRepository = device.DeviceRepository,
                DeviceId = device.DeviceId,
                DeviceNameLabel = device.DeviceNameLabel,
                DeviceTypeLabel = device.DeviceTypeLabel,
            };

            if(!EntityHeader.IsNullOrEmpty(deviceType.Firmware))
            {
                deviceInfo.DeviceFirmware = deviceType.Firmware;
                if (EntityHeader.IsNullOrEmpty(deviceType.FirmwareRevision))
                {
                    var firmware = await _firmwareRepo.GetFirmwareAsync(deviceType.Firmware.Id);
                    deviceInfo.DeviceFirmwareRevision = firmware.DefaultRevision;
                }
                else
                {
                    deviceInfo.DeviceFirmwareRevision = deviceType.FirmwareRevision;
                }
            }

            if(!EntityHeader.IsNullOrEmpty(device.AssignedUser))
            {
                var user = await _appUserRepo.FindByIdAsync(device.AssignedUser.Id);
                deviceInfo.ContactName = user.Name;
                deviceInfo.ContactEmail = user.Email;
                deviceInfo.ContactPhone = user.PhoneNumber;
            }
            else if(!EntityHeader.IsNullOrEmpty(deviceRepo.AssignedUser))
            {
                var user = await _appUserRepo.FindByIdAsync(deviceRepo.AssignedUser.Id);
                deviceInfo.ContactName = user.Name;
                deviceInfo.ContactEmail = user.Email;
                deviceInfo.ContactPhone = user.PhoneNumber;
            }

            return InvokeResult<PublicDeviceInfo>.Create(deviceInfo);
        }
    }
}
