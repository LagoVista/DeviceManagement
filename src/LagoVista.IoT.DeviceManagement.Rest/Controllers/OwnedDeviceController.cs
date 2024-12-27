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
using LagoVista.UserAdmin.Interfaces.Managers;
using LagoVista.IoT.DeviceAdmin.Interfaces.Repos;
using LagoVista.IoT.DeviceManagement.Core.Interfaces;
using LagoVista.Core.Models.UIMetaData;
using System.Security.Cryptography;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;

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
        private readonly ISmsSender _smsSender;
        private readonly IDeviceConfigHelper _deviceConfigHelper;

        public OwnedDeviceLoginController(IDeviceRepositoryManager repoManager, ISmsSender smsSender, IDeviceConfigHelper deviceConfigHelper, IDeviceOwnerRepo deviceOwnerRepo, IAdminLogger adminLogger, IDeviceManager deviceManager, SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
            _adminLogger = adminLogger ?? throw new ArgumentNullException(nameof(adminLogger));
            _deviceManager = deviceManager ?? throw new ArgumentNullException(nameof(deviceManager));
            _deviceOwnerRepo = deviceOwnerRepo ?? throw new ArgumentNullException(nameof(deviceOwnerRepo));
            _smsSender = smsSender ?? throw new ArgumentNullException(nameof(smsSender));
            _deviceConfigHelper = deviceConfigHelper ?? throw new ArgumentNullException(nameof(deviceConfigHelper));
        }

        [HttpGet("/api/device/{orgid}/{devicerepoid}/{id}/{pin}/view")]
        public async Task<InvokeResult<Device>> GetDeviceWithPineAsync(string orgId, string devicerepoid, string id, string pin)
        {
            var org = EntityHeader.Create(orgId, "PIN Device Access");
            var user = EntityHeader.Create(Guid.Empty.ToId(), "PIN Device Access");
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, org, user, pin);
            var result = await _deviceManager.GetDeviceByIdWithPinAsync(repo, id, pin, org, user);
            return result;
        }

        [HttpGet("/api/device/{orgid}/{devicerepoid}/{id}/{pin}/signin")]
        public async Task<InvokeResult<DeviceOwnerUser>> GetDeviceUserAsync(string orgId, string devicerepoid, string id, string pin)
        {
            var fullSw = Stopwatch.StartNew();
            var sw = Stopwatch.StartNew();
            var org = EntityHeader.Create(orgId, "PIN Device Access");
            var user = EntityHeader.Create(Guid.Empty.ToId(), "PIN Device Access");
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(devicerepoid, org, user, pin);
            org = repo.OwnerOrganization;

            _adminLogger.Trace($"[OwnedDeviceLoginController__GetDeviceAsync] Got device repo: {repo.Name}.");
            sw.Stop();
            var result = await _deviceManager.GetDeviceByIdWithPinAsync(repo, id, pin, org, user);

            if (!result.Successful)
                return InvokeResult<DeviceOwnerUser>.FromInvokeResult(result.ToInvokeResult());

            var homePages = await _deviceConfigHelper.GetHomePagesAsync(result.Result.DeviceConfiguration.Id, org, user);

            result.Timings.Add(new ResultTiming() { Key = $"Full device load {result.Result.Name}", Ms = fullSw.Elapsed.TotalMilliseconds });

            if (!EntityHeader.IsNullOrEmpty(result.Result.DeviceOwner))
            {
                await _signInManager.SignOutAsync();

                var owner = await _deviceOwnerRepo.FindByIdAsync(result.Result.DeviceOwner.Id);
                owner.CurrentDevice = result.Result.ToEntityHeader();
                owner.CurrentRepo = repo.ToEntityHeader();
                owner.CurrentDeviceId = result.Result.DeviceId;                
                owner.CurrentDeviceConfig = result.Result.DeviceConfiguration;
                owner.HomePage = homePages.CustomPage;
                owner.MobileHomePage = homePages.CustomMobilePage;

                var appUser = new AppUser()
                {
                    Email = owner.EmailAddress,
                    PhoneNumber = owner.PhoneNumber,
                    SecurityStamp = owner.Id,
                    EmailConfirmed = true,
                    LoginType = LoginTypes.DeviceOwner,
                    PhoneNumberConfirmed = true,
                    CurrentRepo = owner.CurrentRepo,
                    CurrentDevice = owner.CurrentDevice,
                    CurrentDeviceId = owner.CurrentDeviceId,
                    CurrentDeviceConfig = owner.CurrentDeviceConfig,
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
                    CurrentDeviceConfig = result.Result.DeviceConfiguration,
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
                    CurrentDeviceConfig = owner.CurrentDeviceConfig,
                    CurrentRepo = owner.CurrentRepo,
                    HomePage = homePages.CustomPage,
                    MobileHomePage = homePages.CustomMobilePage,
                    OwnerOrganization = org,
            };

                await _signInManager.SignInAsync(owner, true);

                return InvokeResult<DeviceOwnerUser>.Create(deviceUser);
            }
        }

        const string CODE_HASH_COOKIE_NAME = "authvalue";

        [HttpGet("/api/deviceowner/phone/{phoneNumber}/sendcode")]
        public async Task<InvokeResult> SendCode(string phoneNumber)
        {
            phoneNumber = phoneNumber.CleanPhoneNumber();
            var code = Random.Shared.Next(100000, 999999);
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                var str = $"{phoneNumber}:{code}";
                var buffer = System.Text.ASCIIEncoding.ASCII.GetBytes(str);
                var hashedBytes = md5.ComputeHash(buffer);
                var b64 = Convert.ToBase64String(buffer);
                Response.Cookies.Append(CODE_HASH_COOKIE_NAME, b64, new Microsoft.AspNetCore.Http.CookieOptions()
                {
                    Secure = true,
                    HttpOnly = true,
                    SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None,
                });
            }

            return await _smsSender.SendAsync(phoneNumber.CleanPhoneNumber(), $"Please enter the following code {code} to create an account");
        }
    }

    // Device Owner Base has attribute for authenticated.
    public class OwnedDeviceController : DeviceOwnerBaseController
    {
        private readonly IDeviceRepositoryManager _repoManager;
        private readonly IDeviceManager _deviceManager;
        private readonly IRemoteConfigurationManager _remoteConfigurationManager;
        private readonly IDeviceOwnerRepo _deviceOwnerRepo;
        private readonly ISmsSender _smsSender;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IDeviceTypeRepo _deviceTypeRepo;
        private readonly IDeviceConfigHelper _deviceConfigHelper;


        const string CODE_HASH_COOKIE_NAME = "authvalue";

        public OwnedDeviceController(IDeviceRepositoryManager repoManager, ISmsSender smsSender, IDeviceOwnerRepo deviceOwnerRepo, IDeviceManager deviceManager, IRemoteConfigurationManager remoteConfigurationManager, 
                                     IDeviceConfigHelper deviceConfigHelper, IAdminLogger adminLogger, IDeviceTypeRepo deviceTypeRepo, SignInManager<AppUser> signInManager) : base(adminLogger)
        {
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
            _deviceManager = deviceManager ?? throw new ArgumentNullException(nameof(deviceManager));
            _remoteConfigurationManager = remoteConfigurationManager ?? throw new ArgumentNullException(nameof(remoteConfigurationManager));
            _deviceOwnerRepo = deviceOwnerRepo ?? throw new ArgumentNullException(nameof(deviceOwnerRepo));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _smsSender = smsSender ?? throw new ArgumentNullException(nameof(smsSender));
            _deviceTypeRepo = deviceTypeRepo ?? throw new ArgumentNullException(nameof(deviceTypeRepo));
            _deviceConfigHelper = deviceConfigHelper ?? throw new ArgumentNullException(nameof(deviceConfigHelper));
        }

        [HttpGet("/api/deviceowner/account/{phoneNumber}/{code}/login")]
        public async Task<InvokeResult<DeviceOwnerUser>> LoginExistingAccount(string phoneNumber, string code)
        {
            if (!Request.Cookies.ContainsKey(CODE_HASH_COOKIE_NAME))
            {
                Console.WriteLine("Cookie not present.");
                return InvokeResult<DeviceOwnerUser>.FromError("Invalid Code.");
            }

            var existingHash = Request.Cookies[CODE_HASH_COOKIE_NAME];

            Console.WriteLine($"EXISTING HASH: {existingHash}"); ;

            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                var str = $"{phoneNumber}:{code}";
                var buffer = System.Text.ASCIIEncoding.ASCII.GetBytes(str);
                var hashedBytes = md5.ComputeHash(buffer);
                var newHash = Convert.ToBase64String(buffer);

                Console.WriteLine($"Existing:   {existingHash}");
                Console.WriteLine($"Calculated: {newHash}");

                if (existingHash != newHash)
                    return InvokeResult<DeviceOwnerUser>.FromError("Invalid Code.");
            }

            var deviceUser = await _deviceOwnerRepo.FindByPhoneNumberAsync(phoneNumber);
                        
            deviceUser.CurrentDevice = CurrentDevice;
            deviceUser.CurrentDeviceId = CurrentDeviceId;
            deviceUser.CurrentRepo = CurrentDeviceRepo;
            deviceUser.OwnerOrganization = OrgEntityHeader;
            deviceUser.Password = null;
            deviceUser.PasswordHash = null;
            deviceUser.PasswordConfirm = null;
            await _signInManager.SignInAsync(deviceUser.ToAppUser(), true);

            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(CurrentDeviceRepo.Id, OrgEntityHeader, UserEntityHeader);
            var result = await _deviceManager.GetDeviceByIdAsync(repo, CurrentDevice.Id, OrgEntityHeader, UserEntityHeader);
            result.Result.DeviceOwner = deviceUser.ToEntityHeader();

            var homePages = await _deviceConfigHelper.GetHomePagesAsync(result.Result.DeviceConfiguration.Id, OrgEntityHeader, UserEntityHeader);
            deviceUser.HomePage = homePages.CustomPage;
            deviceUser.MobileHomePage = homePages.CustomMobilePage;

            return InvokeResult<DeviceOwnerUser>.Create(deviceUser);
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
                
                if(settings.HighThreshold.HasValue)
                    sensor.HighThreshold = settings.HighThreshold;
                
                if(settings.LowThreshold.HasValue)
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

        [HttpPost("/api/device/current/contact")]
        public async Task<InvokeResult<ExternalContact[]>> AddDeviceNotificationUsers([FromBody] ExternalContact contact)
        {
            var sw = Stopwatch.StartNew();
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(CurrentDeviceRepo.Id, OrgEntityHeader, UserEntityHeader);
            var result = await _deviceManager.GetDeviceByIdAsync(repo, CurrentDevice.Id, OrgEntityHeader, UserEntityHeader);
            result.Timings.Insert(0, new ResultTiming() { Key = $"Load Repo {repo.Name}", Ms = sw.Elapsed.TotalMilliseconds });
            if (result.Successful)
            {
                var device = result.Result;
                device.LastUpdatedDate = DateTime.UtcNow.ToJSONString();
                device.NotificationContacts.Add(contact);
                await _deviceManager.UpdateDeviceAsync(repo, device, OrgEntityHeader, UserEntityHeader);
                return InvokeResult<ExternalContact[]>.Create(device.NotificationContacts.ToArray());
            }
            else
            {
                return InvokeResult<ExternalContact[]>.FromInvokeResult(result.ToInvokeResult());
            }
        }


        [HttpGet("/api/device/current/contact/factory")]
        public DetailResponse<ExternalContact> CreateExteranlContact()
        {
            return DetailResponse<ExternalContact>.Create();
        }

        [HttpPut("/api/device/current/contact")]
        public async Task<InvokeResult<ExternalContact[]>> UpdateDeviceNotificationUsers([FromBody] ExternalContact contact)
        {
            var sw = Stopwatch.StartNew();
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(CurrentDeviceRepo.Id, OrgEntityHeader, UserEntityHeader);
            var result = await _deviceManager.GetDeviceByIdAsync(repo, CurrentDevice.Id, OrgEntityHeader, UserEntityHeader);
            result.Timings.Insert(0, new ResultTiming() { Key = $"Load Repo {repo.Name}", Ms = sw.Elapsed.TotalMilliseconds });
            if (result.Successful)
            {
                var device = result.Result;
                device.LastUpdatedDate = DateTime.UtcNow.ToJSONString();
                var existing = device.NotificationContacts.FirstOrDefault(cnt => cnt.Id == contact.Id);
                if (existing == null)
                    return InvokeResult<ExternalContact[]>.FromError("Could not find contact to update");

                var idx = device.NotificationContacts.IndexOf(existing);
                device.NotificationContacts[idx] = contact;
                await _deviceManager.UpdateDeviceAsync(repo, device, OrgEntityHeader, UserEntityHeader);
                return InvokeResult<ExternalContact[]>.Create(device.NotificationContacts.ToArray());
            }
            else
            {
                return InvokeResult<ExternalContact[]>.FromInvokeResult(result.ToInvokeResult());
            }
        }

        [HttpDelete("/api/device/current/contact/{id}")]
        public async Task<InvokeResult<ExternalContact[]>> RemoveDeviceNotificationUsers(string id)
        {
            var sw = Stopwatch.StartNew();
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(CurrentDeviceRepo.Id, OrgEntityHeader, UserEntityHeader);
            var result = await _deviceManager.GetDeviceByIdAsync(repo, CurrentDevice.Id, OrgEntityHeader, UserEntityHeader);
            result.Timings.Insert(0, new ResultTiming() { Key = $"Load Repo {repo.Name}", Ms = sw.Elapsed.TotalMilliseconds });
            if (result.Successful)
            {
                var device = result.Result;
                device.LastUpdatedDate = DateTime.UtcNow.ToJSONString();
                var existing = device.NotificationContacts.FirstOrDefault(cnt => cnt.Id == id);
                if (existing == null)
                    return InvokeResult<ExternalContact[]>.FromError("Could not find contact to update");

                device.NotificationContacts.Remove(existing);
                await _deviceManager.UpdateDeviceAsync(repo, device, OrgEntityHeader, UserEntityHeader);
                return InvokeResult<ExternalContact[]>.Create(device.NotificationContacts.ToArray());
            }
            else
            {
                return InvokeResult<ExternalContact[]>.FromInvokeResult(result.ToInvokeResult());
            }
        }

        [HttpGet("/api/device/alarms/silence")]
        public async Task<InvokeResult> SilenceAlarmsAsync()
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(CurrentDeviceRepo.Id, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.SilenceAlarmsAsync(repo, CurrentDevice.Id, OrgEntityHeader, UserEntityHeader);
        }


        [HttpGet("/api/device/error/{errorid}/silence")]
        public async Task<InvokeResult> SilenceErrorAsync(string errorid)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(CurrentDeviceRepo.Id, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.SilenceErrorAsync(repo, CurrentDevice.Id, errorid, OrgEntityHeader, UserEntityHeader);
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

        [HttpGet("/api/deviceowner/search/phone/{phoneNumber}")]
        public async Task<InvokeResult> LookupPhoneNumber(string phoneNumber)
        {
            phoneNumber = phoneNumber.CleanPhoneNumber();
            var user = await _deviceOwnerRepo.FindByPhoneNumberAsync(phoneNumber);
            if (user == null)
                return InvokeResult.FromError("User not found");

            return InvokeResult.Success;
        }

        [HttpGet("/api/deviceowner/account/{phoneNumber}/{code}/create")]
        public async Task<InvokeResult<DeviceOwnerUser>> CreateAccount(string phoneNumber, string code)
        {
            if (!Request.Cookies.ContainsKey(CODE_HASH_COOKIE_NAME))
            {
                Console.WriteLine("Cookie not present.");
                return InvokeResult<DeviceOwnerUser>.FromError("Invalid Code.");
            }

            phoneNumber = phoneNumber.CleanPhoneNumber();

            var existingHash = Request.Cookies[CODE_HASH_COOKIE_NAME];

            Console.WriteLine($"EXISTING HASH: {existingHash}"); ;

            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                var str = $"{phoneNumber}:{code}";
                var buffer = System.Text.ASCIIEncoding.ASCII.GetBytes(str);
                var hashedBytes = md5.ComputeHash(buffer);
                var newHash = Convert.ToBase64String(buffer);

                Console.WriteLine($"Existing:   {existingHash}");
                Console.WriteLine($"Calculated: {newHash}");

                if (existingHash != newHash)
                    return InvokeResult<DeviceOwnerUser>.FromError("Invalid code.");               
            }

            var timeStamp = DateTime.UtcNow.ToJSONString();

            var deviceUser = new DeviceOwnerUser()
            {            
                Key = phoneNumber,
                UserName = phoneNumber,
                PhoneNumber = phoneNumber,
                FirstName = String.Empty,
                LastName = String.Empty,
                Name = "TBD",
                CurrentDevice = CurrentDevice,
                CurrentDeviceId = CurrentDeviceId,
                CurrentRepo = CurrentDeviceRepo,
                CurrentDeviceConfig = CurrentDeviceConfig,
                OwnerOrganization = OrgEntityHeader,
                LastUpdatedDate = timeStamp,
                CreationDate = timeStamp,
            };

            deviceUser.CreatedBy = EntityHeader.Create(deviceUser.Id, "REGISTRATION");
            deviceUser.LastUpdatedBy = deviceUser.CreatedBy;

            var appuser = deviceUser.ToAppUser();

            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(CurrentDeviceRepo.Id, OrgEntityHeader, UserEntityHeader);
            var result = await _deviceManager.GetDeviceByIdAsync(repo, CurrentDevice.Id, OrgEntityHeader, UserEntityHeader);
            result.Result.DeviceOwner = deviceUser.ToEntityHeader();

            var homePages = await _deviceConfigHelper.GetHomePagesAsync(result.Result.DeviceConfiguration.Id, OrgEntityHeader, UserEntityHeader);
            deviceUser.HomePage = homePages.CustomPage;
            deviceUser.MobileHomePage = homePages.CustomMobilePage;

            await _deviceManager.UpdateDeviceAsync(repo, result.Result, OrgEntityHeader, UserEntityHeader);

            var deviceType = await _deviceTypeRepo.GetDeviceTypeAsync(result.Result.DeviceType.Id); 

            await _deviceOwnerRepo.AddUserAsync(deviceUser);
            await _deviceOwnerRepo.AddOwnedDeviceAsync(OrgEntityHeader.Id, deviceUser.Id, new DeviceOwnerDevices()
            {
                Device = result.Result.ToEntityHeader(),
                DeviceId = result.Result.DeviceId,
                DeviceRepository = result.Result.DeviceRepository,
                Product = deviceType.Product
            }); 

            await _signInManager.SignInAsync(appuser, true);

            return InvokeResult<DeviceOwnerUser>.Create(deviceUser);
        }

        [HttpGet("/api/device/product/associate")]
        public async Task<InvokeResult> AssociateCurrentDevicWithUser(bool confirm)
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(CurrentDeviceRepo.Id, OrgEntityHeader, UserEntityHeader);
            var result = await _deviceManager.GetDeviceByIdAsync(repo, CurrentDevice.Id, OrgEntityHeader, UserEntityHeader);
            if (!EntityHeader.IsNullOrEmpty(result.Result.DeviceOwner) && !confirm)
                return InvokeResult.FromError("Device is already owned.");
            
            result.Result.DeviceOwner = UserEntityHeader;

            await _deviceManager.UpdateDeviceAsync(repo, result.Result, OrgEntityHeader, UserEntityHeader);

            var deviceType = await _deviceTypeRepo.GetDeviceTypeAsync(result.Result.DeviceType.Id);

            await _deviceOwnerRepo.AddOwnedDeviceAsync(OrgEntityHeader.Id, UserEntityHeader.Id, new DeviceOwnerDevices()
            {
                Device = result.Result.ToEntityHeader(),
                DeviceId = result.Result.DeviceId,
                DeviceRepository = result.Result.DeviceRepository,
                Product = deviceType.Product
            });

            return InvokeResult.Success;
        }

        [HttpPost("/api/device/owner")]
        public async Task<InvokeResult> SetOwnerInfo([FromBody] DeviceOwnerUserUpdateFields updates)
        {
            var owner = await _deviceOwnerRepo.FindByIdAsync(CurrentUserId);
            owner.FirstName = updates.FirstName;
            owner.LastName = updates.LastName;
            owner.EmailAddress = updates.Email;
            return await _deviceOwnerRepo.UpdateUserAsync(owner);
        }


        [HttpGet("/api/device/current")]
        public async Task<InvokeResult<Device>> GetCurrentDevice()
        {
            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(CurrentDeviceRepo.Id, OrgEntityHeader, UserEntityHeader);
            return await _deviceManager.GetDeviceByIdAsync(repo, CurrentDevice.Id, OrgEntityHeader, UserEntityHeader);            
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

    public class DeviceOwnerUserUpdateFields
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class SensorSettings
    {
        public int PortIndex { get; set; }
        public EntityHeader Technology { get; set; }
        public double? HighThreshold { get; set; }
        public double? LowThreshold { get; set; }
        public string Name { get; set; }
    }

    public static class PhoneExtensions
    {
        public static string CleanPhoneNumber(this string phoneNumber)
        {
            if (String.IsNullOrEmpty(phoneNumber))
                return String.Empty;

            return phoneNumber.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
        }
    }


}
