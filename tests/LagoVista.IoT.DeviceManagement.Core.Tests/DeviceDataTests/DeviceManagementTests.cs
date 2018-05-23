using LagoVista.Core.Interfaces;
using LagoVista.Core.Networking.AsyncMessaging;
using LagoVista.IoT.DeviceManagement.Core.Interfaces;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.Logging.Loggers;
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
        Mock<IAsyncCoupler<IAsyncResponse>> _asyncCoupler = new Mock<IAsyncCoupler<IAsyncResponse>>();
        Mock<IAsyncProxyFactory> _asyncProxyFactory = new Mock<IAsyncProxyFactory>();
        Mock<IAsyncRequestHandler> _asyncRequestSender = new Mock<IAsyncRequestHandler>();

        [TestInitialize]
        public void Init()
        {
            _deviceManager = new DeviceManager(_devMgmt.Object, _deviceConfigHelper.Object, _logger.Object,
                _secureStorage.Object, _appConfig.Object, _dependencyManager.Object, _security.Object, 
                _asyncProxyFactory.Object, _asyncCoupler.Object, _asyncRequestSender.Object);
        }
    }
}
