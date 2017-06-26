using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.Logging.Loggers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public class DeviceRepositoryManager : ManagerBase, IDeviceRepositoryManager
    {

        IDeviceRepositoryRepo _deviceRepositoryRepo;
        public DeviceRepositoryManager(IDeviceRepositoryRepo deviceRepositoryRepo, IAdminLogger logger, IAppConfig appConfig, IDependencyManager dependencyManager, ISecurity security) : base(logger, appConfig, dependencyManager, security)
        {
            _deviceRepositoryRepo = deviceRepositoryRepo;
        }
        
        public async Task<InvokeResult> AddDeviceRepositoryAsync(DeviceRepository host, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(host, Actions.Create);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Create, user, org);
            await _deviceRepositoryRepo.AddDeviceRepositoryAsync(host);
            return InvokeResult.Success;
        }

        public async Task<DependentObjectCheckResult> CheckInUseAsync(string id, EntityHeader org, EntityHeader user)
        {
            var host = await _deviceRepositoryRepo.GetDeviceRepositoryAsync(id);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Read, user, org);
            return await CheckForDepenenciesAsync(host);
        }

        public async Task<InvokeResult> DeleteDeviceRepositoryAsync(String instanceId, EntityHeader org, EntityHeader user)
        {
            var host = await _deviceRepositoryRepo.GetDeviceRepositoryAsync(instanceId);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Read, user, org);
            await ConfirmNoDepenenciesAsync(host);
            await _deviceRepositoryRepo.DeleteAsync(instanceId);
            return InvokeResult.Success;
        }

        public async Task<DeviceRepository> GetDeviceRepositoryAsync(string instanceId, EntityHeader org, EntityHeader user)
        {
            var host = await _deviceRepositoryRepo.GetDeviceRepositoryAsync(instanceId);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Read, user, org);
            return host;
        }

        public async Task<IEnumerable<DeviceRepositorySummary>> GetDeploymentHostsForOrgAsync(string orgId, EntityHeader user)
        {
            await AuthorizeOrgAccess(user, orgId, typeof(DeviceRepository));
            return await _deviceRepositoryRepo.GetDeviceRepositoriesForOrgAsync(orgId);
        }
        
        public Task<bool> QueryKeyInUserAsync(string key, EntityHeader org)
        {
            return _deviceRepositoryRepo.QueryRepoKeyInUseAsync(key, org.Id);
        }

        public async Task<InvokeResult> UpdateDeviceRepositoryAsync(DeviceRepository host, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(host, Actions.Update);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Update, user, org);
            await _deviceRepositoryRepo.UpdateDeviceRepositoryAsync(host);
            return InvokeResult.Success;
        }
    }
}
