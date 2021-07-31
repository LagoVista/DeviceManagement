using LagoVista.Core;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Rpc.Client;
using LagoVista.IoT.DeviceAdmin.Interfaces.Repos;
using LagoVista.IoT.DeviceManagement.Core.Interfaces;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.MediaServices.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LagoVista.IoT.DeviceManagement.Core.Tests.DeviceDataTests
{
    [TestClass]
    public class DeviceManagementTests
    {
        DeviceManager _deviceManager;
        Mock<IDeviceRepositoryManager> _repoMgr = new Mock<IDeviceRepositoryManager>();
        Mock<IDeviceManagementRepo> _devMgmt = new Mock<IDeviceManagementRepo>();
        Mock<IDeviceConfigHelper> _deviceConfigHelper = new Mock<IDeviceConfigHelper>();
        Mock<IAdminLogger> _logger = new Mock<IAdminLogger>();
        Mock<ISecureStorage> _secureStorage = new Mock<ISecureStorage>();
        Mock<IAppConfig> _appConfig = new Mock<IAppConfig>();
        Mock<IDependencyManager> _dependencyManager = new Mock<IDependencyManager>();
        Mock<ISecurity> _security = new Mock<ISecurity>();
        Mock<IDeviceArchiveRepo> _deviceArchiveRepo = new Mock<IDeviceArchiveRepo>();
        Mock<IProxyFactory> _proxyFactory = new Mock<IProxyFactory>();
        Mock<IDeviceExceptionRepo> _deviceExceptionRepo = new Mock<IDeviceExceptionRepo>();
        Mock<IDeviceConnectionEventRepo> _connectionEventRepo = new Mock<IDeviceConnectionEventRepo>();
        Mock<IMediaServicesManager> _mediaServiceManager = new Mock<IMediaServicesManager>();
        Mock<IDeviceTypeRepo> _deviceTypeRepo = new Mock<IDeviceTypeRepo>();
        Mock<IDeviceRepositoryRepo> _deviceRepoRepo = new Mock<IDeviceRepositoryRepo>();

        [TestInitialize]
        public void Init()
        {
            _deviceManager = new DeviceManager(
                _devMgmt.Object,
                _deviceConfigHelper.Object,
                _logger.Object,
                _secureStorage.Object,
                _appConfig.Object,
                _dependencyManager.Object,
                _security.Object,
                _deviceExceptionRepo.Object,
                _deviceArchiveRepo.Object,
                _connectionEventRepo.Object,
                _mediaServiceManager.Object,
                _deviceRepoRepo.Object,
                _deviceTypeRepo.Object,
                _proxyFactory.Object); 
        }
    }
}
