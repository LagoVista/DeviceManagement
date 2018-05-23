using LagoVista.Core.Models;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Repos.Repos;
using LagoVista.IoT.Logging.Loggers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.MediaIntegrationTests.MediaTests
{
    [TestClass]
    public class MediaItemTests
    {
        /* 
 * NOte if this fails run again, it will delete the test contianer after initial run, and sometimes we get a delete conflict, should never happen in production,
 * this is really an integration test, not sure how much time should be put in to fixing. */

        private static DeviceRepository GetTestDeviceRepo()
        {
            var deviceRepo = new DeviceRepository()
            {
                DeviceArchiveStorageSettings = new ConnectionSettings()
                {
                    AccountId = System.Environment.GetEnvironmentVariable("AZUREACCOUNTID"),
                    AccessKey = System.Environment.GetEnvironmentVariable("AZUREACCESSKEY"),
                },
                Key = "defaultrepo",
                Id = "784128c7cc214a4ca80957c2334a00c1"
            };

            return deviceRepo;
        }

        [TestMethod]
        public async Task GetMediaItems()
        {
            var mock = new Mock<IDeviceArchiveReportUtils>();
            var itemRepo = new DeviceMediaItemRepo(new AdminLogger(new Utils.LogWriter()));
            var items = await itemRepo.GetMediaItemsForDeviceAsync(GetTestDeviceRepo(), "A79528c7cc214a4ca80957c2334a0033", new LagoVista.Core.Models.UIMetaData.ListRequest() { });
            Assert.IsTrue(items.Successful);
        }
    }
}
