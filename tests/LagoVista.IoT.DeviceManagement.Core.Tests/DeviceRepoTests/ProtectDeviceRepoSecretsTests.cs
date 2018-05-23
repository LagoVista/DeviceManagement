using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.DeviceManagement.Repos;
using LagoVista.IoT.Logging.Loggers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;
using LagoVista.Core.Validation;
using Newtonsoft.Json;

namespace LagoVista.IoT.DeviceManagement.Core.Tests.DeviceRepoTests
{
    [TestClass]
    public class ProtectDeviceRepoSecretsTests : DeviceRepoTestBase
    {
        Mock<IDeviceManagementSettings> _deviceMgmtSettings = new Mock<IDeviceManagementSettings>();
        Mock<IDeviceRepositoryRepo> _deviceRepositoryRepo = new Mock<IDeviceRepositoryRepo>();
        Mock<ISecureStorage> _secureStorage = new Mock<ISecureStorage>();
        Mock<IAppConfig> _appConfig = new Mock<IAppConfig>();
        Mock<IDependencyManager> _dependencyManager = new Mock<IDependencyManager>();
        Mock<ISecurity> _security = new Mock<ISecurity>();
        IAdminLogger _adminLogger;

        EntityHeader _user = EntityHeader.Create("3367B1522AF441F39238A85A80B94D33", "User");
        EntityHeader _org = EntityHeader.Create("5567B1522AF441F39238A85A80B94D33", "User");

        ConnectionSettings _defaultTableStorageSettings = new ConnectionSettings() { AccountId = "act123", AccessKey = "access123" };
        ConnectionSettings _deviceStorageSettings = new ConnectionSettings() { Uri = "urlofresource", ResourceName = "act123", AccessKey = "access123" };

        String _defaultTSJson;
        String _defaultDeviceStorageJson;


        [TestInitialize()]
        public void Init()
        {
            _secureStorage.Setup<Task<InvokeResult<string>>>(srs => srs.AddSecretAsync(It.IsAny<string>())).ReturnsAsync(InvokeResult<string>.Create("passingid"));

            _deviceMgmtSettings.Setup<IConnectionSettings>(dms => dms.DefaultDeviceTableStorage).Returns(_defaultTableStorageSettings);
            _deviceMgmtSettings.Setup<IConnectionSettings>(dms => dms.DefaultDeviceStorage).Returns(_deviceStorageSettings);

            _defaultTSJson = JsonConvert.SerializeObject(_defaultTableStorageSettings);
            _defaultDeviceStorageJson = JsonConvert.SerializeObject(_deviceStorageSettings);
        }

        private DeviceRepositoryManager GetRepoManager()
        {
            _adminLogger = new AdminLogger(new Utils.LogWriter());

            return new DeviceRepositoryManager(_deviceMgmtSettings.Object, _deviceRepositoryRepo.Object, _adminLogger, _secureStorage.Object, _appConfig.Object, _dependencyManager.Object, _security.Object);
        }

        

        [TestMethod]
        public async Task DeviceRepo_ProtectSecret_DefaultSave_Insert()
        {
            var repoManager = GetRepoManager();
            var repo = GetValidRepo();

            var result = await repoManager.AddDeviceRepositoryAsync(repo, _org, _user);
            AssertSuccessful(result);
        }

        [TestMethod]
        public async Task DeviceRepo_ProtectSecret_DefaultSave_Update()
        {
            var repoManager = GetRepoManager();
            var repo = GetValidRepo();
            repo.DeviceArchiveStorageSettingsSecureId = "6C67B1522AF441F39238A85A80B94D39";
            repo.PEMStorageSettingsSecureId = "6C67B1522AF441F39238A85A80B94D39";
            repo.DeviceStorageSecureSettingsId = "6C67B1522AF441F39238A85A80B94D39";

            var result = await repoManager.UpdateDeviceRepositoryAsync(repo, _org, _user);
            AssertSuccessful(result);
        }


        [TestMethod]
        public async Task DeviceRepo_ProtectSecret_SaveDeviceStorageSettings_Insert()
        {
            var repoManager = GetRepoManager();
            var repo = GetValidRepo();
            var result = await repoManager.AddDeviceRepositoryAsync(repo, _org, _user);
            _secureStorage.Verify(srs => srs.AddSecretAsync(_defaultDeviceStorageJson), Times.Once);
            _deviceRepositoryRepo.Verify(drp => drp.AddDeviceRepositoryAsync(It.Is<DeviceRepository>(rep => rep.DeviceStorageSecureSettingsId == "passingid" && rep.DeviceStorageSettings == null)), Times.Once);
            AssertSuccessful(result);
            Assert.IsNull(repo.DeviceStorageSettings);
        }


        [TestMethod]
        public async Task DeviceRepo_ProtectSecret_SavePEMStorageSettings_Insert()
        {
            var repoManager = GetRepoManager();
            var repo = GetValidRepo();
            var result = await repoManager.AddDeviceRepositoryAsync(repo, _org, _user);
            _secureStorage.Verify(srs => srs.AddSecretAsync(_defaultTSJson), Times.Exactly(2));
            _deviceRepositoryRepo.Verify(drp => drp.AddDeviceRepositoryAsync(It.Is<DeviceRepository>(rep => rep.PEMStorageSettingsSecureId == "passingid" && rep.PEMStorageSettings == null)), Times.Once);
            AssertSuccessful(result);
            Assert.IsNull(repo.PEMStorageSettings);
        }

        [TestMethod]
        public async Task DeviceRepo_ProtectSecret_SaveDeviceArchiveStorageSettings_Insert()
        {
            var repoManager = GetRepoManager();
            var repo = GetValidRepo();
            var result = await repoManager.AddDeviceRepositoryAsync(repo, _org, _user);
            _secureStorage.Verify(srs => srs.AddSecretAsync(_defaultTSJson), Times.Exactly(2));
            _deviceRepositoryRepo.Verify(drp => drp.AddDeviceRepositoryAsync(It.Is<DeviceRepository>(rep => rep.DeviceArchiveStorageSettingsSecureId == "passingid" && rep.DeviceArchiveStorageSettings == null)), Times.Once);
            AssertSuccessful(result);
            Assert.IsNull(repo.DeviceArchiveStorageSettings);
        }



        [TestMethod]
        public async Task DeviceRepo_ProtectSecret_SaveDeviceStorageSettings_Update()
        {
            var repoManager = GetRepoManager();
            var repo = GetValidRepo();
            repo.DeviceArchiveStorageSettingsSecureId = "settingid1";
            repo.PEMStorageSettingsSecureId = "settingid2";
            repo.DeviceStorageSecureSettingsId = "settingid3";

            var result = await repoManager.UpdateDeviceRepositoryAsync(repo, _org, _user);
            _secureStorage.Verify(srs => srs.AddSecretAsync(It.IsAny<String>()), Times.Never);
            _deviceRepositoryRepo.Verify(drp => drp.UpdateDeviceRepositoryAsync(It.Is<DeviceRepository>(rep => rep.DeviceStorageSecureSettingsId == "settingid3" && rep.DeviceStorageSettings == null)), Times.Once);
            AssertSuccessful(result);
            Assert.IsNull(repo.DeviceStorageSettings);
        }


        [TestMethod]
        public async Task DeviceRepo_ProtectSecret_SavePEMStorageSettings_Update()
        {
            var repoManager = GetRepoManager();
            var repo = GetValidRepo();
            repo.DeviceArchiveStorageSettingsSecureId = "settingid1";
            repo.PEMStorageSettingsSecureId = "settingid2";
            repo.DeviceStorageSecureSettingsId = "settingid3";

            var result = await repoManager.UpdateDeviceRepositoryAsync(repo, _org, _user);
            _secureStorage.Verify(srs => srs.AddSecretAsync(It.IsAny<String>()), Times.Never);
            _deviceRepositoryRepo.Verify(drp => drp.UpdateDeviceRepositoryAsync(It.Is<DeviceRepository>(rep => rep.PEMStorageSettingsSecureId == "settingid2" && rep.PEMStorageSettings == null)), Times.Once);
            AssertSuccessful(result);
            Assert.IsNull(repo.PEMStorageSettings);
        }

        [TestMethod]
        public async Task DeviceRepo_ProtectSecret_SaveDeviceArchiveStorageSettings_Update()
        {
            var repoManager = GetRepoManager();
            var repo = GetValidRepo();
            repo.DeviceArchiveStorageSettingsSecureId = "settingid1";
            repo.PEMStorageSettingsSecureId = "settingid2";
            repo.DeviceStorageSecureSettingsId = "settingid3";

            var result = await repoManager.UpdateDeviceRepositoryAsync(repo, _org, _user);
            _secureStorage.Verify(srs => srs.AddSecretAsync(It.IsAny<String>()), Times.Never);
            _deviceRepositoryRepo.Verify(drp => drp.UpdateDeviceRepositoryAsync(It.Is<DeviceRepository>(rep => rep.DeviceArchiveStorageSettingsSecureId == "settingid1" && rep.DeviceArchiveStorageSettings == null)), Times.Once);
            AssertSuccessful(result);
            Assert.IsNull(repo.DeviceArchiveStorageSettings);
        }



        [TestMethod]
        public async Task DeviceRepo_ProtectSecret_SaveDeviceStorageSettingsChanged_Update()
        {
            var repoManager = GetRepoManager();
            var repo = GetValidRepo();
            repo.DeviceArchiveStorageSettingsSecureId = "settingid1";
            repo.PEMStorageSettingsSecureId = "settingid2";
            repo.DeviceStorageSecureSettingsId = "settingid3";
            repo.DeviceStorageSettings = _deviceStorageSettings;

            var result = await repoManager.UpdateDeviceRepositoryAsync(repo, _org, _user);
            _secureStorage.Verify(srs => srs.AddSecretAsync(_defaultDeviceStorageJson), Times.Once);
            _deviceRepositoryRepo.Verify(drp => drp.UpdateDeviceRepositoryAsync(It.Is<DeviceRepository>(rep => rep.DeviceStorageSecureSettingsId == "passingid" && rep.DeviceStorageSettings == null)), Times.Once);
            AssertSuccessful(result);
            Assert.IsNull(repo.DeviceStorageSettings);
        }


        [TestMethod]
        public async Task DeviceRepo_ProtectSecret_SavePEMStorageSettingsChanged_Update()
        {
            var repoManager = GetRepoManager();
            var repo = GetValidRepo();
            repo.DeviceArchiveStorageSettingsSecureId = "settingid1";
            repo.PEMStorageSettingsSecureId = "settingid2";
            repo.DeviceStorageSecureSettingsId = "settingid3";
            repo.PEMStorageSettings = _defaultTableStorageSettings;

            var result = await repoManager.UpdateDeviceRepositoryAsync(repo, _org, _user);
            _secureStorage.Verify(srs => srs.AddSecretAsync(_defaultTSJson), Times.Once);
            _deviceRepositoryRepo.Verify(drp => drp.UpdateDeviceRepositoryAsync(It.Is<DeviceRepository>(rep => rep.PEMStorageSettingsSecureId == "passingid" && rep.PEMStorageSettings == null)), Times.Once);
            AssertSuccessful(result);
            Assert.IsNull(repo.PEMStorageSettings);
        }

        [TestMethod]
        public async Task DeviceRepo_ProtectSecret_SaveDeviceArchiveStorageSettingsChanged_Update()
        {
            var repoManager = GetRepoManager();
            var repo = GetValidRepo();
            repo.DeviceArchiveStorageSettingsSecureId = "settingid1";
            repo.PEMStorageSettingsSecureId = "settingid2";
            repo.DeviceStorageSecureSettingsId = "settingid3";
            repo.DeviceArchiveStorageSettings = _defaultTableStorageSettings;

            var result = await repoManager.UpdateDeviceRepositoryAsync(repo, _org, _user);
            _secureStorage.Verify(srs => srs.AddSecretAsync(_defaultTSJson), Times.Once);
            _deviceRepositoryRepo.Verify(drp => drp.UpdateDeviceRepositoryAsync(It.Is<DeviceRepository>(rep => rep.DeviceArchiveStorageSettingsSecureId == "passingid" && rep.DeviceArchiveStorageSettings == null)), Times.Once);
            AssertSuccessful(result);
            Assert.IsNull(repo.DeviceArchiveStorageSettings);
        }
    }
}
