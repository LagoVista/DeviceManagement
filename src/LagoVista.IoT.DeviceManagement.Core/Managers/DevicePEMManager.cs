using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.Core.Managers;
using LagoVista.Core.Interfaces;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.DeviceManagement.Core.Repos;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public class DevicePEMManager : ManagerBase, IDevicePEMManager
    {
        IDevicePEMIndexesRepo _devicePEMIndexRepo;
        IDevicePEMRepo _devicePEMRepo;

        public DevicePEMManager(IDevicePEMRepo devicePEMRepo, IDevicePEMIndexesRepo devicePEMIndexRepo, IAdminLogger logger, IAppConfig appConfig, IDependencyManager dependencyManager, ISecurity security) : base(logger, appConfig, dependencyManager, security)
        {
            _devicePEMIndexRepo = devicePEMIndexRepo;
            _devicePEMRepo = devicePEMRepo;
        }

        public Task<IEnumerable<DevicePEMIndex>> GetPEMIndexesforDeviceAsync(string deviceId, EntityHeader org, EntityHeader user, int maxReturnCount = 100, String dateStampAfter = "")
        {
            //TODO: Add Security, will need to confirm the user has access to view PEM lists AND has access to this device.
            //TODO: Sorry someone will need to clean this up.

            return _devicePEMIndexRepo.GetPEMIndexForDeviceAsync(deviceId, maxReturnCount, dateStampAfter);
        }

        public async Task<InvokeResult<string>> GetPEMAsync(string pemURI, EntityHeader org, EntityHeader user)
        {
            //TODO: Add Security, will be authorized but we don't know if the user can get THIS pem record.

            return new InvokeResult<string>()
            {
                Result = await _devicePEMRepo.GetPEMAsync(pemURI)
            };
        }
    }
}
