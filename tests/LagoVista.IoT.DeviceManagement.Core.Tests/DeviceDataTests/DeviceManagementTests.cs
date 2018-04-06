using LagoVista.Core.Interfaces;
using LagoVista.IoT.DeviceManagement.Core.Interfaces;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.Logging.Loggers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Tests.DeviceDataTests
{
    [TestClass]
    public class DeviceManagementTests
    {
        DeviceManager _deviceManager;
        Mock<IDeviceRepositoryManager> _repoMgr = new Mock<IDeviceRepositoryManager>();
        Mock<IDeviceManagementRepo> _devMgmt = new Mock<IDeviceManagementRepo>();
        Mock<IDeviceManagementConnector> _deviceConnectorService = new Mock<IDeviceManagementConnector>();
        Mock<IDeviceConfigHelper> _deviceConfigHelper = new Mock<IDeviceConfigHelper>();
        Mock<IAdminLogger> _logger = new Mock<IAdminLogger>();
        Mock<ISecureStorage> _secureStorage = new Mock<ISecureStorage>();
        Mock<IAppConfig> _appConfig = new Mock<IAppConfig>();
        Mock<IDependencyManager> _dependencyManager = new Mock<IDependencyManager>();
        Mock<ISecurity> _security = new Mock<ISecurity>();

        [TestInitialize]
        public void Init()
        {
            _deviceManager = new DeviceManager(_devMgmt.Object, _deviceConnectorService.Object, _deviceConfigHelper.Object, _logger.Object,
                _secureStorage.Object, _appConfig.Object, _dependencyManager.Object, _security.Object);
        }
    }
}
