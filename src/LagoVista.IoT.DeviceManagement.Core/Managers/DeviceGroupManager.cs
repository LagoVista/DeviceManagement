using LagoVista.Core.Managers;
using System;
using System.Collections.Generic;
using LagoVista.Core.Models;
using System.Threading.Tasks;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Interfaces;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Logging.Loggers;
using System.Linq;
using LagoVista.Core;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public class DeviceGroupManager : ManagerBase, IDeviceGroupManager
    {
        IDeviceGroupRepo _deviceGroupRepo;
        IDeviceManagementRepo _deviceManagementRepo;
        IDeviceGroupEntryRepo _deviceGroupEntryRepo;
        IAdminLogger _adminLogger;

        public DeviceGroupManager(IDeviceGroupRepo deviceGroupRepo, IDeviceManagementRepo deviceManagementRepo, IDeviceGroupEntryRepo deviceGroupEntryRepo, IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security) : 
            base(logger, appConfig, depmanager, security)
        {
            _deviceGroupRepo = deviceGroupRepo;
            _deviceGroupEntryRepo = deviceGroupEntryRepo;
            _deviceManagementRepo = deviceManagementRepo;
            _adminLogger = logger;
        }

        public async Task<InvokeResult> AddDeviceGroupAsync(DeviceRepository deviceRepo, DeviceGroup deviceGroup, EntityHeader org, EntityHeader user)
        {
            await AuthorizeAsync(deviceGroup, AuthorizeResult.AuthorizeActions.Create, user, org);
            ValidationCheck(deviceGroup, Actions.Create);
            await _deviceGroupRepo.AddDeviceGroupAsync(deviceRepo, deviceGroup);
            return InvokeResult.Success;
        }

        public async Task<InvokeResult> UpdateDeviceGroupAsync(DeviceRepository deviceRepo, DeviceGroup deviceGroup, EntityHeader org, EntityHeader user)
        {
            await AuthorizeAsync(deviceGroup, AuthorizeResult.AuthorizeActions.Update, user, org);
            ValidationCheck(deviceGroup, Actions.Update);
            await _deviceGroupRepo.UpdateDeviceGroupAsync(deviceRepo, deviceGroup);
            return InvokeResult.Success;
        }


        public async Task<DeviceGroup> GetDeviceGroupAsync(DeviceRepository deviceRepo, string groupId, EntityHeader org, EntityHeader user)
        {
            var deviceGroup = await _deviceGroupRepo.GetDeviceGroupAsync(deviceRepo, groupId);
            await AuthorizeAsync(deviceGroup, AuthorizeResult.AuthorizeActions.Read, user, org);
            return deviceGroup;
        }

        public async Task<InvokeResult> AddDeviceToGroupAsync(DeviceRepository deviceRepo, String deviceGroupId, String deviceId, EntityHeader org, EntityHeader user)
        {
            var group = await GetDeviceGroupAsync(deviceRepo, deviceGroupId, org, user);
            var device = await _deviceManagementRepo.GetDeviceByIdAsync(deviceRepo, deviceId);

            await AuthorizeAsync(group, AuthorizeResult.AuthorizeActions.Update, user, org);
            await AuthorizeAsync(device, AuthorizeResult.AuthorizeActions.Read, user, org);
            await _deviceGroupEntryRepo.AddDeviceToGroupAsync(deviceRepo, group.ToEntityHeader(), device.ToEntityHeader());

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> DeleteDeviceGroupAsync(DeviceRepository deviceRepo, string deviceGroupId, EntityHeader org, EntityHeader user)
        {
            var deviceGroup = await _deviceGroupRepo.GetDeviceGroupAsync(deviceRepo, deviceGroupId);

            //TODO: This one also needs to cleanup the table storage for the devices.
            await AuthorizeAsync(deviceGroup, AuthorizeResult.AuthorizeActions.Delete, user, org);

            return InvokeResult.Success;
        }

        public async Task<IEnumerable<DeviceGroupSummary>> GetDeviceGroupsForOrgAsync(DeviceRepository deviceRepo, string orgId, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, orgId, typeof(DeviceGroupSummary));
            return await _deviceGroupRepo.GetDeviceGroupsForOrgAsync(deviceRepo, orgId);
        }

        public async Task<IEnumerable<EntityHeader>> GetDevicesInGroupAsync(DeviceRepository deviceRepo, string deviceGroupId, EntityHeader org, EntityHeader user)
        {
            var deviceGroup = await _deviceGroupRepo.GetDeviceGroupAsync(deviceRepo, deviceGroupId);
            await AuthorizeAsync(deviceGroup, AuthorizeResult.AuthorizeActions.Read, user, org);
            return await _deviceGroupEntryRepo.GetDevicesInGroupAsync(deviceRepo, deviceGroupId);
        }

        public async Task<InvokeResult> RemoveDeviceFromGroupAsync(DeviceRepository deviceRepo, string deviceGroupId, string deviceId, EntityHeader org, EntityHeader user)
        {
            var group = await _deviceGroupRepo.GetDeviceGroupAsync(deviceRepo, deviceGroupId);
            await AuthorizeAsync(group, AuthorizeResult.AuthorizeActions.Update, user, org);
            await _deviceGroupEntryRepo.RemoveDeviceFromGroupAsync(deviceRepo, deviceGroupId, deviceId);

            var device = await _deviceManagementRepo.GetDeviceByIdAsync(deviceRepo, deviceId);
            var deviceInGroup = device.DeviceGroups.Where(devc=>devc.Id == deviceId).FirstOrDefault();
            if (deviceInGroup == null)
            {
                _adminLogger.AddCustomEvent(LogLevel.Error, "DeviceGroupManager", "Device does not exist in group", org.Id.ToKVP("orgId"), deviceId.ToKVP("deviceId"), deviceGroupId.ToKVP("deviceGroupId"));
            }
            else
            {
                device.DeviceGroups.Remove(deviceInGroup);
            }

            device.DeviceGroups.Add(group.ToEntityHeader());
            await _deviceManagementRepo.UpdateDeviceAsync(deviceRepo, device);

            return InvokeResult.Success;
        }

        public Task<bool> QueryKeyInUseAsync(DeviceRepository deviceRepo, string key, EntityHeader org)
        {
            return _deviceGroupRepo.QueryKeyInUseAsync(deviceRepo, key, org.Id);
        }

        public async Task<DependentObjectCheckResult> CheckDeviceGroupInUseAsync(DeviceRepository deviceRepo, string groupId, EntityHeader org, EntityHeader user)
        {
            var group = await _deviceGroupRepo.GetDeviceGroupAsync(deviceRepo, groupId);
            await AuthorizeAsync(group, AuthorizeResult.AuthorizeActions.Read, user, org);
            return await CheckForDepenenciesAsync(group);
        }
    }
}
