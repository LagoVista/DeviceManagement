// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: d1fda5e42524ae775882fa9c2d8008dc3473a784a5a4b18b0895644326ff32f1
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Controllers;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using LagoVista.AspNetCore.Identity.Managers;

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
