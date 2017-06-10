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

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public class DeviceGroupManager : ManagerBase, IDeviceGroupManager
    {
        IDeviceGroupRepo _deviceGroupRepo;
        IDeviceManagementRepo _deviceManagementRepo;
        IDeviceGroupEntryRepo _deviceGroupEntryRepo;

        public DeviceGroupManager(IDeviceGroupRepo deviceGroupRepo, IDeviceManagementRepo deviceManagementRepo, IDeviceGroupEntryRepo deviceGroupEntryRepo, IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security) : 
            base(logger, appConfig, depmanager, security)
        {
            _deviceGroupRepo = deviceGroupRepo;
            _deviceGroupEntryRepo = deviceGroupEntryRepo;
            _deviceManagementRepo = deviceManagementRepo;
        }

        public async Task<InvokeResult> AddDeviceGroupAsync(DeviceGroup deviceGroup, EntityHeader org, EntityHeader user)
        {
            await AuthorizeAsync(deviceGroup, AuthorizeResult.AuthorizeActions.Create, user, org);
            await _deviceGroupRepo.AddDeviceGroupAsync(deviceGroup);
            ValidationCheck(deviceGroup, Actions.Create);
            return InvokeResult.Success;
        }

        public async Task<InvokeResult> UpdateDeviceGroupAsync(DeviceGroup deviceGroup, EntityHeader org, EntityHeader user)
        {
            await AuthorizeAsync(deviceGroup, AuthorizeResult.AuthorizeActions.Update, user, org);
            ValidationCheck(deviceGroup, Actions.Update);
            await _deviceGroupRepo.UpdateDeviceGroupAsync(deviceGroup);
            return InvokeResult.Success;
        }


        public async Task<DeviceGroup> GetDeviceGroupAsync(string groupId, EntityHeader org, EntityHeader user)
        {
            var deviceGroup = await _deviceGroupRepo.GetDeviceGroupAsync(groupId);
            await AuthorizeAsync(deviceGroup, AuthorizeResult.AuthorizeActions.Read, user, org);
            return deviceGroup;
        }

        public async Task<InvokeResult> AddDeviceToGroupAsync(String deviceGroupId, String deviceId, EntityHeader org, EntityHeader user)
        {
            var group = await GetDeviceGroupAsync(deviceGroupId, org, user);
            var device = await _deviceManagementRepo.GetDeviceByIdAsync(deviceId);

            await AuthorizeAsync(group, AuthorizeResult.AuthorizeActions.Update, user, org);
            await AuthorizeAsync(device, AuthorizeResult.AuthorizeActions.Read, user, org);
            await _deviceGroupEntryRepo.AddDeviceToGroupAsync(group.ToEntityHeader(), device.ToEntityHeader());

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> DeleteDeviceGroupAsync(string deviceGroupId, EntityHeader org, EntityHeader user)
        {
            var deviceGroup = await _deviceGroupRepo.GetDeviceGroupAsync(deviceGroupId);

            //TODO: This one also needs to cleanup the table storage for the devices.
            await AuthorizeAsync(deviceGroup, AuthorizeResult.AuthorizeActions.Delete, user, org);

            return InvokeResult.Success;
        }

        public async Task<IEnumerable<DeviceGroupSummary>> GetDeviceGroupsForOrgAsync(string orgId, EntityHeader user)
        {
            await AuthorizeOrgAccess(user, orgId, typeof(DeviceGroupSummary));
            return await _deviceGroupRepo.GetDeviceGroupsForOrgAsync(orgId);
        }

        public async Task<IEnumerable<EntityHeader>> GetDevicesInGroupAsync(string deviceGroupId, EntityHeader org, EntityHeader user)
        {
            var deviceGroup = await _deviceGroupRepo.GetDeviceGroupAsync(deviceGroupId);
            await AuthorizeAsync(deviceGroup, AuthorizeResult.AuthorizeActions.Read, user, org);
            return await _deviceGroupEntryRepo.GetDevicesInGroupAsync(deviceGroupId);
        }

        public async Task<InvokeResult> RemoveDeviceFromGroupAsync(string deviceGroupId, string deviceId, EntityHeader org, EntityHeader user)
        {
            var group = await _deviceGroupRepo.GetDeviceGroupAsync(deviceGroupId);
            await AuthorizeAsync(group, AuthorizeResult.AuthorizeActions.Update, user, org);
            await _deviceGroupEntryRepo.RemoveDeviceFromGroupAsync(deviceGroupId, deviceId);
            return InvokeResult.Success;
        }

        public Task<bool> QueryKeyInUseAsync(string key, EntityHeader org)
        {
            return _deviceGroupRepo.QueryKeyInUseAsync(key, org.Id);
        }

        public async Task<DependentObjectCheckResult> CheckDeviceGroupInUseAsync(string groupId, EntityHeader org, EntityHeader user)
        {
            var group = await _deviceGroupRepo.GetDeviceGroupAsync(groupId);
            await AuthorizeAsync(group, AuthorizeResult.AuthorizeActions.Read, user, org);
            return await CheckForDepenenciesAsync(group);
        }
    }
}
