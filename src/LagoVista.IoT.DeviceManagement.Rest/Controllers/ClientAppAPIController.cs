using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Controllers;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LagoVista.AspNetCore.Identity.Managers;
using LagoVista.Core.Exceptions;
using LagoVista.Core.Models;

namespace LagoVista.IoT.DeviceManagement.Rest.Controllers
{
    public class ClientAppAPIController : LagoVistaBaseController
    {
        IDeploymentInstanceRepo _deploymentInstanceRepo;
        IDeviceRepositoryManager _repoManager;

        public ClientAppAPIController(IDeviceRepositoryManager repoManager, IDeploymentInstanceRepo deploymentInstanceRepo, 
            UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _deploymentInstanceRepo = deploymentInstanceRepo;
            _repoManager = repoManager;
        }

        private String GetClaimValue(String claimId)
        {
            var claim = User.Claims.Where(clm => clm.Type == claimId).FirstOrDefault();
            var value = claim == null ? String.Empty : claim.Value;
            return value;
        }

        protected async Task<DeviceRepository> GetDeviceRepositoryWithSecretsAsync()
        {
            var instanceId = GetClaimValue(ClaimsFactory.InstanceId);
            if(String.IsNullOrEmpty(instanceId))
            {
                throw new Exception("Missing claim for instance id.");
            }

            var instance = await _deploymentInstanceRepo.GetInstanceAsync(instanceId);
            if(instance == null)
            {
                throw new RecordNotFoundException("DeploymentInstance", instanceId);
            }

            if(EntityHeader.IsNullOrEmpty(instance.DeviceRepository))
            {
                throw new InvalidOperationException($"Device Repository not set on {instance.Name}.");
            }

            return await _repoManager.GetDeviceRepositoryWithSecretsAsync(instance.DeviceRepository.Id, OrgEntityHeader, UserEntityHeader);
        }

    }
}
