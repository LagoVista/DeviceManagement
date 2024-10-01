using LagoVista.Core.Models;
using LagoVista.Core;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Controllers;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Collections.Generic;
using LagoVista.UserAdmin.Models.Orgs;
using LagoVista.UserAdmin.Interfaces.Repos.Users;
using System.Text.RegularExpressions;
using LagoVista.UserAdmin.Interfaces.Repos.Account;

namespace LagoVista.IoT.DeviceManagement.Rest.Controllers
{

    [AllowAnonymous]
    public class OwnedDeviceLoginController : Controller
    {
        private readonly IDeviceRepositoryManager _repoManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IDeviceManager _deviceManager;
        private readonly IAdminLogger _adminLogger;
        private readonly IAppUserRepo _appUserRepo;
        private readonly IDeviceOwnerRepo _deviceOwnerRepo;


        public OwnedDeviceLoginController(IDeviceRepositoryManager repoManager, IDeviceOwnerRepo deviceOwnerRepo, IAdminLogger adminLogger, IDeviceManager deviceManager, SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
            _adminLogger = adminLogger ?? throw new ArgumentNullException(nameof(adminLogger));
            _deviceManager = deviceManager ?? throw new ArgumentNullException(nameof(deviceManager));
            _deviceOwnerRepo = deviceOwnerRepo ?? throw new ArgumentNullException(nameof(deviceOwnerRepo));
        }

        [HttpGet("/api/device/{orgid}/{devicerepoid}/{id}/{pin}/signin")]
        public async Task<InvokeResult<DeviceOwnerUser>> GetDeviceAsync(string orgId, string devicerepoid, string id, string pin)
        {
            var fullSw = Stopwatch.StartNew();
            var sw = Stopwatch.StartNew();
            var org = EntityHeader.Create(orgId, "PIN Device Access");
            var user = EntityHeader.Create(Guid.Empty.ToId(), "PIN Device Access");
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, org, user, pin);

            _adminLogger.Trace($"[OwnedDeviceLoginController__GetDeviceAsync] Got device repo: {repo.Name}.");
            sw.Stop();
            var result = await _deviceManager.GetDeviceByIdWithPinAsync(repo, id, pin, org, user);

            if (!result.Successful)
                return InvokeResult<DeviceOwnerUser>.FromInvokeResult(result.ToInvokeResult());

            result.Timings.Add(new ResultTiming() { Key = $"Full device load {result.Result.Name}", Ms = fullSw.Elapsed.TotalMilliseconds });

            if (!EntityHeader.IsNullOrEmpty(result.Result.DeviceOwner))
            {
                await _signInManager.SignOutAsync();

                var owner = await _deviceOwnerRepo.FindByIdAsync(result.Result.DeviceOwner.Id);
                var appUser = new AppUser()
                {
                    Email = owner.EmailAddress,
                    PhoneNumber = owner.PhoneNumber,
                    SecurityStamp = owner.Id,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CurrentRepo = owner.CurrentRepo,
                    CurrentDevice = owner.CurrentDevice,
                    CurrentDeviceId = owner.CurrentDeviceId,
                    OwnerOrganization = org,
                };

                await _signInManager.SignInAsync(appUser, false);
                return InvokeResult<DeviceOwnerUser>.Create(owner);
            }
            else
            {
                var owner = new AppUser()
                {
                    Email = "ANONYMOUS@ANONYMOUS.NET",
                    UserName = "ANONYMOUS",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    LoginType = LoginTypes.DeviceOwner,
                    CurrentDevice = EntityHeader.Create(result.Result.Id, result.Result.Key, result.Result.Name),
                    CurrentDeviceId = result.Result.DeviceId,
                    OwnerOrganization = org,
                    CurrentRepo = result.Result.DeviceRepository
                };

                var deviceUser = new DeviceOwnerUser()
                {
                    FirstName = "ANONYMOUS",
                    LastName = "ANONYMOUS",
                    EmailAddress = "ANONYMOUS@ANONYMOUS.NET",
                    IsAnonymous = true,
                    CurrentDevice = owner.CurrentDevice,
                    CurrentDeviceId = owner.CurrentDeviceId,
                    CurrentRepo = owner.CurrentRepo
                };

                await _signInManager.SignInAsync(owner, false);

                return InvokeResult<DeviceOwnerUser>.Create(deviceUser);
            }
        }
    }

    // Device Owner Base has attribute for authenticated.
    public class OwnedDeviceController : DeviceOwnerBaseController
    {
        private readonly IDeviceRepositoryManager _repoManager;
        private readonly IDeviceManager _deviceManager;
        private readonly IRemoteConfigurationManager _remoteConfigurationManager;
        public OwnedDeviceController(IDeviceRepositoryManager repoManager, IDeviceManager deviceManager, IRemoteConfigurationManager remoteConfigurationManager, IAdminLogger adminLogger) : base(adminLogger)
        {
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
            _deviceManager = deviceManager ?? throw new ArgumentNullException(nameof(deviceManager));
            _remoteConfigurationManager = remoteConfigurationManager ?? throw new ArgumentNullException(nameof(remoteConfigurationManager));
        }

        [HttpGet("/device/api/ownerreg/set")]
        public async Task<InvokeResult> SetOwnerRegistrationAsync([FromBody] DeviceOwnerUser ownerRegistration)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(CurrentDeviceRepo.Id, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.SetDeviceOwnerRegistrationAsync(repo, CurrentDevice.Id, ownerRegistration, OrgEntityHeader, UserEntityHeader);
        }

        [HttpPut("/device/api/device/sensor")]
        public async Task<InvokeResult> SetDeviceSensorThreshold([FromBody] SensorSettings settings)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(CurrentDeviceRepo.Id, OrgEntityHeader, UserEntityHeader);
            var result = await _deviceManager.GetDeviceByIdAsync(repo, CurrentDevice.Id, OrgEntityHeader, UserEntityHeader);
            if (result.Successful)
            {
                var device = result.Result;
                var sensor = device.SensorCollection.FirstOrDefault(sns => sns.PortIndex == settings.PortIndex && sns.Technology.Id == settings.Technology.Id);
                sensor.HighThreshold = settings.HighThreshold;
                sensor.LowThreshold = settings.LowThreshold;
                if (!String.IsNullOrEmpty(settings.Name))
                    sensor.Name = settings.Name;

                await _deviceManager.UpdateDeviceAsync(repo, device, OrgEntityHeader, UserEntityHeader);

                return result.ToInvokeResult();
            }
            else
            {
                return result.ToInvokeResult();
            }
        }

        [HttpPost("/api/device/current/contacts")]
        public async Task<InvokeResult> SetDeviceNotificationUsers([FromBody] ExternalContact[] contacts)
        {
            var sw = Stopwatch.StartNew();
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(CurrentDevice.Id, OrgEntityHeader, UserEntityHeader);
            var result = await _deviceManager.GetDeviceByIdAsync(repo, CurrentDevice.Id, OrgEntityHeader, UserEntityHeader);
            result.Timings.Insert(0, new ResultTiming() { Key = $"Load Repo {repo.Name}", Ms = sw.Elapsed.TotalMilliseconds });
            if (result.Successful)
            {
                var device = result.Result;
                device.LastUpdatedDate = DateTime.UtcNow.ToJSONString();
                device.NotificationContacts = new List<ExternalContact>(contacts);
                return await _deviceManager.UpdateDeviceAsync(repo, device, OrgEntityHeader, UserEntityHeader);
            }
            else
            {
                return result.ToInvokeResult();
            }
        }

        [HttpGet("/api/device/alarms/silence")]
        public async Task<InvokeResult> SilenceAlarmsAsync()
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(CurrentDeviceRepo.Id, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.SilenceAlarmsAsync(repo, CurrentDevice.Id, OrgEntityHeader, UserEntityHeader);
        }

        [HttpGet("/api/device/reset/{newpin}")]
        public async Task<InvokeResult<Device>> SetNewPinAsync(string newpin)
        {
            var fullSw = Stopwatch.StartNew();
            var sw = Stopwatch.StartNew();
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(CurrentDeviceRepo.Id, OrgEntityHeader, UserEntityHeader);
            sw.Stop();
            var result = await _deviceManager.GetDeviceByIdAsync(repo, CurrentDevice.Id, OrgEntityHeader, UserEntityHeader);
            result.Timings.Insert(0, new ResultTiming() { Key = $"Load Repo {repo.Name}", Ms = sw.Elapsed.TotalMilliseconds });
            result.Timings.Add(new ResultTiming() { Key = $"Full device load {result.Result.Name}", Ms = fullSw.Elapsed.TotalMilliseconds });

            if (!result.Successful)
                return result;

            if (String.IsNullOrEmpty(newpin))
            {
                return InvokeResult<Device>.FromError($"Must provide a pin");
            }

            var regEx = new Regex(@"^[A-Za-z0-9]{4,8}$");
            if (!regEx.Match(newpin).Success)
            {
                return InvokeResult<Device>.FromError($"Must provide a pin that is between 4 and 9 characters and must include only letters and numbers.");
            }

            result.Result.DevicePin = newpin;
            result.Result.MustChangePin = false;
            result.Result.PinChangeDate = DateTime.UtcNow.ToJSONString();

            sw.Restart();
            await _deviceManager.UpdateDeviceAsync(repo, result.Result, OrgEntityHeader, UserEntityHeader);
            result.Timings.Add(new ResultTiming() { Key = $"Update device {result.Result.Name}", Ms = fullSw.Elapsed.TotalMilliseconds });
            return InvokeResult<Device>.Create(result.Result);
        }

        [HttpPut("/api/device/properties")]
        public async Task<InvokeResult<Device>> UpdatePropertiesWithPinAsync([FromBody] IEnumerable<AttributeValue> values)
        {
            var fullSw = Stopwatch.StartNew();
            var sw = Stopwatch.StartNew();
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(CurrentDeviceRepo.Id, OrgEntityHeader, UserEntityHeader);
            sw.Stop();
            var result = await _deviceManager.GetDeviceByIdAsync(repo, CurrentDevice.Id, OrgEntityHeader, UserEntityHeader);
            result.Timings.Insert(0, new ResultTiming() { Key = $"Load Repo {repo.Name}", Ms = sw.Elapsed.TotalMilliseconds });
            result.Timings.Add(new ResultTiming() { Key = $"Load Device {result.Result.Name}", Ms = fullSw.Elapsed.TotalMilliseconds });
            sw.Restart();
            foreach (var value in values)
            {
                var existing = result.Result.Properties.FirstOrDefault(prop => prop.Key == value.Key);
                if (existing == null)
                {
                    value.LastUpdated = DateTime.UtcNow.ToJSONString();
                    result.Result.Properties.Add(value);
                }
                else
                {
                    existing.LastUpdated = DateTime.UtcNow.ToJSONString();
                    existing.Value = value.Value;
                }
            }

            await _deviceManager.UpdateDeviceAsync(repo, result.Result, OrgEntityHeader, UserEntityHeader);
            result.Timings.Add(new ResultTiming() { Key = $"Update device {result.Result.Name}", Ms = fullSw.Elapsed.TotalMilliseconds });

            await _remoteConfigurationManager.SendAllPropertiesAsync(CurrentDeviceRepo.Id, CurrentDevice.Id, OrgEntityHeader, UserEntityHeader);

            return result;
        }
    }

    public class SensorSettings
    {
        public int PortIndex { get; set; }
        public EntityHeader Technology { get; set; }
        public double? HighThreshold { get; set; }
        public double? LowThreshold { get; set; }
        public string Name { get; set; }
    }


}
