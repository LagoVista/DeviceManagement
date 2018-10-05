using LagoVista.Core;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Rpc.Tests.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Rpc.Tests
{
    [TestClass]
    public class Rcp_IDeviceArchiveRepo_Tests
    {
        [ClassInitialize]
        static public void ClassInitialize(TestContext testContext)
        {
            DataFactory.Initialize();
        }

        [TestMethod]
        public async Task AddArchiveAsync( )
        {
            var archiveEntry = new DeviceArchive
            {
                DeviceConfigurationId = Guid.NewGuid().ToId(),
                DeviceConfigurationVersionId  =1.0,
                DeviceId = Guid.NewGuid().ToId(),
                MessageId = Guid.NewGuid().ToId(),
                MetaData = "meta data",
                PartitionKey = Guid.NewGuid().ToId(),
                RowKey = Guid.NewGuid().ToId(),
                PEMMessageId = Guid.NewGuid().ToId(),
                Timestamp = DateTime.UtcNow.ToJSONString()
            };
            await DataFactory.DeviceArchiveRepoProxy.AddArchiveAsync(DataFactory.DeviceRepo, archiveEntry);
        }

        [TestMethod]
        public async Task GetForDateRangeAsync()
        {
            var timeStamp = new DateTime(2018, 10, 1, 1, 0, 0);
            var archiveEntry = new DeviceArchive
            {
                DeviceConfigurationId = Guid.NewGuid().ToId(),
                DeviceConfigurationVersionId = 1.0,
                DeviceId = Guid.NewGuid().ToId(),
                MessageId = Guid.NewGuid().ToId(),
                MetaData = "meta data",
                PartitionKey = Guid.NewGuid().ToId(),
                RowKey = Guid.NewGuid().ToId(),
                PEMMessageId = Guid.NewGuid().ToId(),
                Timestamp = timeStamp.ToJSONString()
            };
            await DataFactory.DeviceArchiveRepoProxy.AddArchiveAsync(DataFactory.DeviceRepo, archiveEntry);

            timeStamp = new DateTime(2018, 10, 1, 2, 0, 0);
            archiveEntry.Timestamp = timeStamp.ToJSONString();
            archiveEntry.RowKey = Guid.NewGuid().ToId();
            await DataFactory.DeviceArchiveRepoProxy.AddArchiveAsync(DataFactory.DeviceRepo, archiveEntry);

            var listRequest = new ListRequest
            {
                PageIndex = 1,
                PageSize = 25,
                StartDate = timeStamp.AddDays(-2).ToJSONString()
            };
            var response = await DataFactory.DeviceArchiveRepoProxy.GetForDateRangeAsync(DataFactory.DeviceRepo, archiveEntry.DeviceId, listRequest);
            Assert.IsTrue(response.Model.Count() > 0);
        }
    }
}


