using System.Threading.Tasks;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.Core.Managers;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Interfaces;
using static LagoVista.Core.Models.AuthorizeResult;
using LagoVista.Core.Models;
using System.Collections.Generic;
using System;
using LagoVista.IoT.Logging.Loggers;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public class DeviceManager : ManagerBase, IDeviceManager
    {
        IDeviceManagementRepo _deviceRepo;
        IDeviceArchiveManager _deviceArchiveManager;
        ISecureStorage _secureStorage;

        public DeviceManager(IDeviceManagementRepo deviceRepo, IDeviceArchiveManager deviceArchiveManager, IAdminLogger logger, ISecureStorage secureStorage,
            IAppConfig appConfig, IDependencyManager depmanager, ISecurity security) : base(logger, appConfig, depmanager, security)
        {
            _deviceRepo = deviceRepo;
            _secureStorage = secureStorage;
            _deviceArchiveManager = deviceArchiveManager;
        }

        public async Task<InvokeResult> AddDeviceAsync(DeviceRepository deviceRepo, Device device, EntityHeader org, EntityHeader user)
        {
            await AuthorizeAsync(device, AuthorizeActions.Create, user, org);
            ValidationCheck(device, Actions.Create);
            device.DeviceRepository.Text = deviceRepo.Name;

            await _deviceRepo.AddDeviceAsync(deviceRepo, device);
            return InvokeResult.Success;
        }

        public async Task<InvokeResult> UpdateDeviceAsync(DeviceRepository deviceRepo, Device device, EntityHeader org, EntityHeader user)
        {


            await AuthorizeAsync(device, AuthorizeActions.Update, user, org);
            ValidationCheck(device, Actions.Update);
            await _deviceRepo.UpdateDeviceAsync(deviceRepo, device);
            return InvokeResult.Success;
        }

        public async Task<IEnumerable<DeviceSummary>> GetDevicesForOrgIdAsync(DeviceRepository deviceRepo, string orgId, int top, int take, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, orgId, typeof(Device));
            return await _deviceRepo.GetDevicesForOrgIdAsync(deviceRepo, orgId, top, take);
        }

        public Task<IEnumerable<DeviceSummary>> GetDevicesForLocationIdAsync(DeviceRepository deviceRepo, string locationId, int top, int take, EntityHeader org, EntityHeader user)
        {
            //TODO: Need to extender manager class for location access.

            return _deviceRepo.GetDevicesForLocationIdAsync(deviceRepo, locationId, top, take);
        }

        public async Task<Device> GetDeviceByDeviceIdAsync(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user)
        {
            var device = await _deviceRepo.GetDeviceByDeviceIdAsync(deviceRepo, id);
            if(device == null)
            {
                return null;
            }

            await AuthorizeAsync(device, AuthorizeActions.Read, user, org);
            return device;
        }

        public Task<bool> CheckIfDeviceIdInUse(DeviceRepository deviceRepo, string deviceId, string orgid)
        {
            return _deviceRepo.CheckIfDeviceIdInUse(deviceRepo, deviceId, orgid);
        }

        public async Task<Device> GetDeviceByIdAsync(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user)
        {
            var device = await _deviceRepo.GetDeviceByIdAsync(deviceRepo, id);
            await AuthorizeAsync(device, AuthorizeActions.Read, user, org);
            return device;
        }


        public async Task<InvokeResult> DeleteDeviceAsync(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user)
        {
            var device = await _deviceRepo.GetDeviceByIdAsync(deviceRepo, id);
            await AuthorizeAsync(device, AuthorizeActions.Delete, user, org);
            await _deviceRepo.DeleteDeviceAsync(deviceRepo, id);
            return InvokeResult.Success;
        }

        public Task<IEnumerable<DeviceSummary>> GetDevicesInStatusAsync(DeviceRepository deviceRepo, string status, int top, int take, EntityHeader org, EntityHeader user)
        {
            //TODO: Need to extend manager for security on this getting device w/ status
            return _deviceRepo.GetDevicesInStatusAsync(deviceRepo, status, top, take);
        }

        public Task<IEnumerable<DeviceSummary>> GetDevicesWithConfigurationAsync(DeviceRepository deviceRepo, string configurationId, int top, int take, EntityHeader org, EntityHeader user)
        {
            //TODO: Need to extend manager for security on this getting device w/ configuration
            return _deviceRepo.GetDevicesWithConfigurationAsync(deviceRepo, configurationId, top, take);
        }

        public async Task<DependentObjectCheckResult> CheckIfDeviceIdInUse(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user)
        {
            var device = await _deviceRepo.GetDeviceByIdAsync(deviceRepo, id);
            await AuthorizeAsync(device, AuthorizeActions.Update, user, org);
            return await CheckForDepenenciesAsync(device);
        }

        public IDeviceArchiveManager ArchiveManager { get { return _deviceArchiveManager; } }
    }
}
