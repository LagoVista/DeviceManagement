// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 7c4b34a6567a16c3f002ec1855a21f182adbb39dfa1e3071b6e8cc2f180351b5
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.DeviceManagement.Repos.Repos;
using LagoVista.IoT.Logging.Loggers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Tests.DeviceRepoTests
{
    [TestClass]
    public class DeviceRepoIntegrationTests
    {
        private string _accountId;
        private string _accountKey;
        private string _uri;

        IDeviceManagementRepo _devMgtRepo;


        const string ORG_ID = "C8AD4589F26842E7A1AEFBAEFC979C9B";
        const string REPO_ID = "5CF12598069C400B8B91A3AE8604EA4A";

        [TestInitialize]
        public void TestInit()
        {
            _accountId = Environment.GetEnvironmentVariable("TEST_DOCDB_ACCOUNTID");
            _accountKey = Environment.GetEnvironmentVariable("TEST_DOCDB_ACCOUTKEY");

            if (String.IsNullOrEmpty(_accountId)) throw new ArgumentNullException("Please add TEST_AZURESTORAGE_ACCOUNTID as an environnment variable");
            if (String.IsNullOrEmpty(_accountKey)) throw new ArgumentNullException("Please add TEST_AZURESTORAGE_ACCESSKEY as an environnment variable");

            _uri = $"https://{_accountId}.documents.azure.com:443";

            _devMgtRepo = new DeviceManagementRepo(new AdminLogger(new Utils.LogWriter()));
        }

        [TestMethod]
        public async Task GetDevices()
        {
            var repo = new DeviceRepository();
            repo.DeviceStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            repo.DeviceStorageSettings.Uri = _uri;
            repo.DeviceStorageSettings.ResourceName = "dev";
            repo.DeviceStorageSettings.AccessKey = _accountKey;
            repo.Id = REPO_ID;
            var devices = await _devMgtRepo.GetDevicesForRepositoryAsync(repo, ORG_ID, new LagoVista.Core.Models.UIMetaData.ListRequest() { PageIndex = 1, PageSize = 100 });
            foreach (var device in devices.Model)
            {
                Console.WriteLine(device.DeviceName);
            }
        }

    }
}
