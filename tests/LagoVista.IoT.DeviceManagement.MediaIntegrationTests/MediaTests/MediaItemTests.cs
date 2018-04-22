using LagoVista.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                Key = "testrepo",
                Id = "890C3F4F480C4FF283F7C9B16CB5F368"
            };

            return deviceRepo;
        }
    }
}
