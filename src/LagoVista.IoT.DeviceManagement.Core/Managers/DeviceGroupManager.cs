using LagoVista.Core;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Networking.AsyncMessaging;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.Logging.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public class DeviceGroupManager : ManagerBase, IDeviceGroupManager
    {
        private readonly IDeviceGroupRepo _defaultDeviceGroupRepo;
        private readonly IDeviceManagementRepo _defaultDeviceManagementRepo;

        private readonly IAsyncProxyFactory _asyncProxyFactory;

        public IDeviceGroupRepo GetDeviceGroupRepo(DeviceRepository deviceRepo)
        {
            return deviceRepo.RepositoryType.Value == RepositoryTypes.Local ?
                _asyncProxyFactory.Create<IDeviceGroupRepo>(
                    deviceRepo.OwnerOrganization.Id,
                    deviceRepo.Instance.Id,
                    TimeSpan.FromSeconds(120)) :
                _defaultDeviceGroupRepo;
        }

        public IDeviceManagementRepo GetDeviceManagementRepo(DeviceRepository deviceRepo)
        {
            return deviceRepo.RepositoryType.Value == RepositoryTypes.Local ?
                _asyncProxyFactory.Create<IDeviceManagementRepo>(
                    deviceRepo.OwnerOrganization.Id,
                    deviceRepo.Instance.Id,
                    TimeSpan.FromSeconds(120)) :
                _defaultDeviceManagementRepo;
        }

        public DeviceGroupManager(IDeviceGroupRepo deviceGroupRepo, IDeviceManagementRepo deviceManagementRepo, IAdminLogger logger, IAppConfig appConfig,
            IDependencyManager depmanager, ISecurity security, IAsyncProxyFactory asyncProxyFactory) :
            base(logger, appConfig, depmanager, security)
        {
            _defaultDeviceGroupRepo = deviceGroupRepo;
            _defaultDeviceManagementRepo = deviceManagementRepo;
            _asyncProxyFactory = asyncProxyFactory;
        }

        public async Task<InvokeResult> AddDeviceGroupAsync(DeviceRepository deviceRepo, DeviceGroup deviceGroup, EntityHeader org, EntityHeader user)
        {
            await AuthorizeAsync(deviceGroup, AuthorizeResult.AuthorizeActions.Create, user, org);
            ValidationCheck(deviceGroup, Actions.Create);
            await GetDeviceGroupRepo(deviceRepo).AddDeviceGroupAsync(deviceRepo, deviceGroup);
            return InvokeResult.Success;
        }

        public async Task<InvokeResult> UpdateDeviceGroupAsync(DeviceRepository deviceRepo, DeviceGroup deviceGroup, EntityHeader org, EntityHeader user)
        {
            await AuthorizeAsync(deviceGroup, AuthorizeResult.AuthorizeActions.Update, user, org);
            ValidationCheck(deviceGroup, Actions.Update);
            await GetDeviceGroupRepo(deviceRepo).UpdateDeviceGroupAsync(deviceRepo, deviceGroup);
            return InvokeResult.Success;
        }


        public async Task<DeviceGroup> GetDeviceGroupAsync(DeviceRepository deviceRepo, string groupId, EntityHeader org, EntityHeader user)
        {
            var deviceGroup = await GetDeviceGroupRepo(deviceRepo).GetDeviceGroupAsync(deviceRepo, groupId);
            await AuthorizeAsync(deviceGroup, AuthorizeResult.AuthorizeActions.Read, user, org);
            return deviceGroup;
        }

        public async Task<InvokeResult<DeviceGroupEntry>> AddDeviceToGroupAsync(DeviceRepository deviceRepo, String deviceGroupId, String deviceUniqueId, EntityHeader org, EntityHeader user)
        {
            var group = await GetDeviceGroupAsync(deviceRepo, deviceGroupId, org, user);
            await AuthorizeAsync(group, AuthorizeResult.AuthorizeActions.Update, user, org, "Add Device to Device Group");

            var device = await GetDeviceManagementRepo(deviceRepo).GetDeviceByIdAsync(deviceRepo, deviceUniqueId);
            await AuthorizeAsync(device, AuthorizeResult.AuthorizeActions.Update, user, org, "Add Devvice to Device Group");

            //TODO: Add localization
            if (group.Devices.Where(grp => grp.DeviceUniqueId == deviceUniqueId).Any())
            {
                return InvokeResult<DeviceGroupEntry>.FromError($"The device [{device.DeviceId}] already belongs to this device group and can not be added again.");
            }

            var entry = DeviceGroupEntry.FromDevice(device, user);
            group.Devices.Add(entry);

            device.DeviceGroups.Add(new EntityHeader() { Id = group.Id, Text = group.Name });

            await GetDeviceManagementRepo(deviceRepo).UpdateDeviceAsync(deviceRepo, device);
            await GetDeviceGroupRepo(deviceRepo).UpdateDeviceGroupAsync(deviceRepo, group);

            return InvokeResult<DeviceGroupEntry>.Create(entry);
        }

        public async Task<InvokeResult> DeleteDeviceGroupAsync(DeviceRepository deviceRepo, string deviceGroupId, EntityHeader org, EntityHeader user)
        {
            var deviceGroup = await GetDeviceGroupRepo(deviceRepo).GetDeviceGroupAsync(deviceRepo, deviceGroupId);

            await AuthorizeAsync(deviceGroup, AuthorizeResult.AuthorizeActions.Delete, user, org);

            await GetDeviceGroupRepo(deviceRepo).DeleteDeviceGroupAsync(deviceRepo, deviceGroupId);

            //TOOD: This should be much more efficient
            foreach (var device in deviceGroup.Devices)
            {
                try
                {
                    var deviceFromRepo = await GetDeviceManagementRepo(deviceRepo).GetDeviceByIdAsync(deviceRepo, device.DeviceUniqueId);
                    if (deviceFromRepo != null)
                    {
                        deviceFromRepo.DeviceGroups = deviceFromRepo.DeviceGroups.Where(grp => grp.Id != deviceGroupId).ToList();
                        await GetDeviceManagementRepo(deviceRepo).UpdateDeviceAsync(deviceRepo, deviceFromRepo);
                    }
                }
                catch (Exception) { /* Not the end of the world */}

            }

            return InvokeResult.Success;
        }

        public async Task<IEnumerable<DeviceGroupSummary>> GetDeviceGroupsForOrgAsync(DeviceRepository deviceRepo, string orgId, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, orgId, typeof(DeviceGroupSummary));
            return await GetDeviceGroupRepo(deviceRepo).GetDeviceGroupsForOrgAsync(deviceRepo, orgId);
        }

        public async Task<IEnumerable<EntityHeader>> GetDevicesInGroupAsync(DeviceRepository deviceRepo, string deviceGroupId, EntityHeader org, EntityHeader user)
        {
            var deviceGroup = await GetDeviceGroupRepo(deviceRepo).GetDeviceGroupAsync(deviceRepo, deviceGroupId);
            await AuthorizeAsync(deviceGroup, AuthorizeResult.AuthorizeActions.Read, user, org);
            throw new NotImplementedException();
        }

        public async Task<InvokeResult> RemoveDeviceFromGroupAsync(DeviceRepository deviceRepo, string deviceGroupId, string deviceUniqueId, EntityHeader org, EntityHeader user)
        {
            var group = await GetDeviceGroupRepo(deviceRepo).GetDeviceGroupAsync(deviceRepo, deviceGroupId);
            await AuthorizeAsync(group, AuthorizeResult.AuthorizeActions.Update, user, org, "Remove Device From Device Group");

            var device = await GetDeviceManagementRepo(deviceRepo).GetDeviceByIdAsync(deviceRepo, deviceUniqueId);
            await AuthorizeAsync(group, AuthorizeResult.AuthorizeActions.Update, user, org, "Remove Device From Device Group");

            //TODO: Add localization
            if (!group.Devices.Where(grp => grp.DeviceUniqueId == deviceUniqueId).Any())
            {
                return InvokeResult.FromError($"The device [{device.DeviceId}] does not belong to this device group and can not be removed again.");
            }

            var deviceInGroup = device.DeviceGroups.Where(devc => devc.Id == deviceGroupId).FirstOrDefault();
            if (deviceInGroup == null)
            {
                Logger.AddCustomEvent(LogLevel.Error, "DeviceGroupManager_RemoveDeviceFromGroup", "Device Group does not exist in list of groups for device.", org.Id.ToKVP("orgId"), deviceUniqueId.ToKVP("deviceId"), deviceGroupId.ToKVP("deviceGroupId"));
            }
            else
            {
                device.DeviceGroups.Remove(deviceInGroup);
                await GetDeviceManagementRepo(deviceRepo).UpdateDeviceAsync(deviceRepo, device);
            }

            var deviceGroupEntry = group.Devices.Where(dev => dev.DeviceUniqueId == deviceUniqueId).First();
            group.Devices.Remove(deviceGroupEntry);
            await GetDeviceGroupRepo(deviceRepo).UpdateDeviceGroupAsync(deviceRepo, group);

            return InvokeResult.Success;
        }

        public Task<bool> QueryKeyInUseAsync(DeviceRepository deviceRepo, string key, EntityHeader org)
        {
            return GetDeviceGroupRepo(deviceRepo).QueryKeyInUseAsync(deviceRepo, key, org.Id);
        }

        public async Task<DependentObjectCheckResult> CheckDeviceGroupInUseAsync(DeviceRepository deviceRepo, string groupId, EntityHeader org, EntityHeader user)
        {
            var group = await GetDeviceGroupRepo(deviceRepo).GetDeviceGroupAsync(deviceRepo, groupId);
            await AuthorizeAsync(group, AuthorizeResult.AuthorizeActions.Read, user, org);
            return await CheckForDepenenciesAsync(group);
        }

        public async Task<ListResponse<DeviceSummaryData>> GetDeviceGroupSummaryDataAsync(DeviceRepository deviceRepo, string groupId, ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            var group = await GetDeviceGroupRepo(deviceRepo).GetDeviceGroupAsync(deviceRepo, groupId);
            await AuthorizeAsync(group, AuthorizeResult.AuthorizeActions.Read, user, org);
            return await GetDeviceManagementRepo(deviceRepo).GetDeviceGroupSummaryDataAsync(deviceRepo, groupId, listRequest);
        }

    }
}
