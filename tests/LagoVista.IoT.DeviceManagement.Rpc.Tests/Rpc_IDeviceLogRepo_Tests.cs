using LagoVista.Core;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Rpc.Tests.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Rpc.Tests
{
    [TestClass]
    public class Rpc_IDeviceLogRepo_Tests
    {
        [ClassInitialize]
        static public void ClassInitialize(TestContext testContext)
        {
            DataFactory.Initialize();
        }

        [TestMethod]
        public async Task AddLogEntryAsync()
        {
            var logEntry = new DeviceLog
            {
                DateStamp = DateTime.UtcNow.ToJSONString(),
                DeviceId = Guid.NewGuid().ToId(),
                Entry = "entry",
                EntryType = "entrytype",
                MetaData = "meta",
                RowKey = Guid.NewGuid().ToId(),
                Source = "source"
            };
            await DataFactory.DeviceLogRepo.AddLogEntryAsync(DataFactory.DeviceRepo, logEntry);
        }

        [TestMethod]
        public async Task GetForDateRangeAsync()
        {
            var timeStamp = new DateTime(2018, 10, 1, 1, 0, 0);
            var logEntry = new DeviceLog
            {
                DateStamp = DateTime.UtcNow.ToJSONString(),
                DeviceId = Guid.NewGuid().ToId(),
                Entry = "entry",
                EntryType = "entrytype",
                MetaData = "meta",
                RowKey = Guid.NewGuid().ToId(),
                Source = "source"
            };
            await DataFactory.DeviceLogRepo.AddLogEntryAsync(DataFactory.DeviceRepo, logEntry);

            timeStamp = new DateTime(2018, 10, 1, 2, 0, 0);
            logEntry.DateStamp = timeStamp.ToJSONString();
            logEntry.RowKey = Guid.NewGuid().ToId();
            await DataFactory.DeviceLogRepo.AddLogEntryAsync(DataFactory.DeviceRepo, logEntry);

            var listRequest = new ListRequest
            {
                PageIndex = 1,
                PageSize = 25,
                StartDate = timeStamp.AddDays(-2).ToJSONString()
            };
            var response = await DataFactory.DeviceLogRepo.GetForDateRangeAsync(DataFactory.DeviceRepo, logEntry.DeviceId, listRequest);
            Assert.IsTrue(response.Successful);
            Assert.IsTrue(response.Model.Count() > 0);
        }
    }
}
