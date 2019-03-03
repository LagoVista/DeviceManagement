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
using LagoVista.IoT.DeviceManagement.Core.Interfaces;

namespace LagoVista.IoT.DeviceManagement.Rest.Controllers
{
    public class ClientAppAPIController : LagoVistaBaseController
    {
        IDeviceRepositoryManager _repoManager;

        public ClientAppAPIController(IDeviceRepositoryManager repoManager, 
            UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
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
           
            return await _repoManager.GetDeviceRepositoryForInstanceAsync(instanceId, OrgEntityHeader, UserEntityHeader);
        }

    }
}
