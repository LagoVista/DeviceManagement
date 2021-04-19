using LagoVista.Core;
using LagoVista.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Tests;
using LagoVista.IoT.DeviceManagement.Repos.Repos;
using LagoVista.IoT.Logging.Loggers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.MediaIntegrationTests.MediaTests
{
    [TestClass]
    public class MediaFileRepoTests : ValidationBase
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

        private static async Task RemoveTestContainerAsync()
        {
            var settings = new ConnectionSettings()
            {
                AccountId = System.Environment.GetEnvironmentVariable("AZUREACCOUNTID"),
                AccessKey = System.Environment.GetEnvironmentVariable("AZUREACCESSKEY"),
            };

            var baseuri = $"https://{settings.AccountId}.blob.core.windows.net";

            var uri = new Uri(baseuri);
            var client = new CloudBlobClient(uri, new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(settings.AccountId, settings.AccessKey));

            var container = client.GetContainerReference(GetTestDeviceRepo().GetDeviceMediaStorageName());
            var opContext = new OperationContext();
            await container.DeleteIfExistsAsync();
        }

        [TestMethod]
        public async Task Media_Blob_DownloadTest()
        {
            var mediaRepo = new DeviceMediaRepo(new AdminLogger(new Utils.LogWriter()));
            var fileName = $"{Guid.NewGuid().ToId()}.bin";
            var imageBytes = new byte[4096];
            new Random().NextBytes(imageBytes);

            var result = await mediaRepo.AddMediaAsync(GetTestDeviceRepo(), imageBytes, fileName, "application/jpeg");
            AssertSuccessful(result);

            Console.WriteLine("Media Item Added.");

            var downloadResult = await mediaRepo.GetMediaAsync(GetTestDeviceRepo(), fileName);
            AssertSuccessful(downloadResult.ToInvokeResult());

            Assert.AreEqual(imageBytes.Length, downloadResult.Result.Length);

            for (var idx = 0; idx < imageBytes.Length; ++idx)
            {
                Assert.AreEqual(imageBytes[idx], downloadResult.Result[idx], $"Difference in byte array at:{idx}");
            }
        }

        [TestMethod]
        public async Task Media_Blob_DeleteBlobTest()
        {
            var mediaRepo = new DeviceMediaRepo(new AdminLogger(new Utils.LogWriter()));
            var fileName = $"{Guid.NewGuid().ToId()}.bin";
            var imageBytes = new byte[4096];
            new Random().NextBytes(imageBytes);

            var result = await mediaRepo.AddMediaAsync(GetTestDeviceRepo(), imageBytes, fileName, "application/jpeg");
            AssertSuccessful(result);

            Console.WriteLine("Media Item Added.");

            var deleteResult = await mediaRepo.DeleteMediaAsync(GetTestDeviceRepo(), fileName);
            AssertSuccessful(deleteResult.ToInvokeResult());

            var getResult = await mediaRepo.GetMediaAsync(GetTestDeviceRepo(), fileName);
            AssertInvalidError(getResult.ToInvokeResult(), "The remote server returned an error: (404) Not Found.", "Unhandled Excpetion");
        }

        [TestMethod]
        public async Task Media_Blob_UploadTest()
        {
            var mediaRepo = new DeviceMediaRepo(new AdminLogger(new Utils.LogWriter()));
            var fileName = $"{Guid.NewGuid().ToId()}.bin";
            var imageBytes = new byte[4096];
            new Random().NextBytes(imageBytes);

            var result = await mediaRepo.AddMediaAsync(GetTestDeviceRepo(), imageBytes, fileName, "application/jpeg");
            AssertSuccessful(result);
        }

        [ClassCleanup]
        public static async Task TestCleanup()
        {
            await RemoveTestContainerAsync();
        }
    }
}
