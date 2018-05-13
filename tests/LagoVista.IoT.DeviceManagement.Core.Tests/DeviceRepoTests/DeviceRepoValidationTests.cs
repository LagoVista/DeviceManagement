using LagoVista.Core.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Tests.DeviceRepoTests
{
    [TestClass]
    public class DeviceRepoValidationTests : DeviceRepoTestBase
    {
        [TestMethod]
        public void DeviceRepo_Validation_Insert_Valid()
        {
            var repo = GetValidRepo();
            repo.DeviceArchiveStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            repo.DeviceStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            repo.PEMStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            AssertSuccessful(Validator.Validate(repo, Actions.Create));
        }

        [TestMethod]
        public void DeviceRepo_Validation_Update_SecureIDs_Valid()
        {
            var repo = GetValidRepo();
            repo.DeviceArchiveStorageSettingsSecureId = "abc123";
            repo.DeviceStorageSecureSettingsId = "def435";
            repo.PEMStorageSettingsSecureId = "hig532";
            AssertSuccessful(Validator.Validate(repo, Actions.Update));
        }

        [TestMethod]
        public void DeviceRepo_Validation_Update_ChangeSettings_Valid()
        {
            var repo = GetValidRepo();
            repo.DeviceArchiveStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            repo.DeviceStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            repo.PEMStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            AssertSuccessful(Validator.Validate(repo, Actions.Update));
        }

        [TestMethod]
        public void DeviceRepo_Validation_Insert_MissingArchive_InValid()
        {
            var repo = GetValidRepo();
            repo.DeviceArchiveStorageSettings = null;
            repo.DeviceStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            repo.PEMStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            AssertInvalidError(Validator.Validate(repo, Actions.Create), "Device Archive Storage Settings are Required on Insert.");
        }

        [TestMethod]
        public void DeviceRepo_Validation_Insert_Missing_DeviceStorage_InValid()
        {
            var repo = GetValidRepo();
            repo.DeviceArchiveStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            repo.DeviceStorageSettings = null;
            repo.PEMStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            AssertInvalidError(Validator.Validate(repo, Actions.Create), "Device Storage Settings are Required on Insert.");
        }

        [TestMethod]
        public void DeviceRepo_Validation_Insert_Missing_PEMStorage_InValid()
        {
            var repo = GetValidRepo();
            repo.DeviceArchiveStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            repo.DeviceStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            repo.PEMStorageSettings = null;
            AssertInvalidError(Validator.Validate(repo, Actions.Create), "PEM Storage Settings Are Required on Insert.");
        }

        [TestMethod]
        public void DeviceRepo_Validation_Update_MissingArchive_InValid()
        {
            var repo = GetValidRepo();
            repo.DeviceArchiveStorageSettings = null;
            repo.DeviceStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            repo.PEMStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            AssertInvalidError(Validator.Validate(repo, Actions.Update), "Device Archive Storage Settings Or SecureId are Required when updating.");
        }

        [TestMethod]
        public void DeviceRepo_Validation_Update_Missing_DeviceStorage_InValid()
        {
            var repo = GetValidRepo();
            repo.DeviceArchiveStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            repo.DeviceStorageSettings = null;
            repo.PEMStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            AssertInvalidError(Validator.Validate(repo, Actions.Update), "Device Storage Settings Or Secure Id are Required when updating.");
        }

        [TestMethod]
        public void DeviceRepo_Validation_Update_Missing_PEMStorage_InValid()
        {
            var repo = GetValidRepo();
            repo.DeviceArchiveStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            repo.DeviceStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            repo.PEMStorageSettings = null;
            AssertInvalidError(Validator.Validate(repo, Actions.Update), "PEM Storage Settings or Secure Id Are Required when updating.");
        }


        [TestMethod]
        public void DeviceRepo_Validation_AzureIoTHub_Insert_Valid()
        {
            var repo = GetValidRepo();
            repo.ResourceName = "AzureIoTHub";
            repo.AccessKeyName = "AzureIoTHub";
            repo.AccessKey = "dGhpc2lzbXlrZXlwbGFzZXZpZW1lc29tZWRhdGE=";

            repo.RepositoryType = LagoVista.Core.Models.EntityHeader<Models.RepositoryTypes>.Create(Models.RepositoryTypes.AzureIoTHub);
            repo.DeviceArchiveStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            repo.DeviceStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            repo.PEMStorageSettings = new LagoVista.Core.Models.ConnectionSettings(); ;
            AssertSuccessful(Validator.Validate(repo, Actions.Create));
        }

        [TestMethod]
        public void DeviceRepo_Validation_AzureIoTHub_Update_Valid_SecureId()
        {
            var repo = GetValidRepo();
            repo.ResourceName = "AzureIoTHub";
            repo.AccessKeyName = "AzureIoTHub";
            repo.AccessKey = "dGhpc2lzbXlrZXlwbGFzZXZpZW1lc29tZWRhdGE=";

            repo.RepositoryType = LagoVista.Core.Models.EntityHeader<Models.RepositoryTypes>.Create(Models.RepositoryTypes.AzureIoTHub);
            repo.DeviceArchiveStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            repo.DeviceStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            repo.PEMStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            AssertSuccessful(Validator.Validate(repo, Actions.Update));
        }

        [TestMethod]
        public void DeviceRepo_Validation_AzureIoTHub_Update_Valid_AccessKey()
        {
            var repo = GetValidRepo();
            repo.ResourceName = "AzureIoTHub";
            repo.AccessKeyName = "AzureIoTHub";
            repo.SecureAccessKeyId = "1234565";

            repo.RepositoryType = LagoVista.Core.Models.EntityHeader<Models.RepositoryTypes>.Create(Models.RepositoryTypes.AzureIoTHub);
            repo.DeviceArchiveStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            repo.DeviceStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            repo.PEMStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            AssertSuccessful(Validator.Validate(repo, Actions.Update));
        }


        [TestMethod]
        public void DeviceRepo_Validation_AzureIoTHub_Insert_MissingResourceName_InValid()
        {
            var repo = GetValidRepo();
            repo.ResourceName = null;
            repo.AccessKeyName = "AzureIoTHub";
            repo.AccessKey = "dGhpc2lzbXlrZXlwbGFzZXZpZW1lc29tZWRhdGE=";

            repo.RepositoryType = LagoVista.Core.Models.EntityHeader<Models.RepositoryTypes>.Create(Models.RepositoryTypes.AzureIoTHub);
            repo.DeviceArchiveStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            repo.DeviceStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            repo.PEMStorageSettings = new LagoVista.Core.Models.ConnectionSettings(); ;
            AssertInvalidError(Validator.Validate(repo, Actions.Create), "Resource name which is the name of our Azure IoT Hub is a required field.");
        }

        [TestMethod]
        public void DeviceRepo_Validation_AzureIoTHub_MissingAccessKey_InValid()
        {
            var repo = GetValidRepo();
            repo.ResourceName = "AzureIoTHub";
            repo.AccessKeyName = "AzureIoTHub";
            repo.AccessKey = null;

            repo.RepositoryType = LagoVista.Core.Models.EntityHeader<Models.RepositoryTypes>.Create(Models.RepositoryTypes.AzureIoTHub);
            repo.DeviceArchiveStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            repo.DeviceStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            repo.PEMStorageSettings = new LagoVista.Core.Models.ConnectionSettings(); ;
            AssertInvalidError(Validator.Validate(repo, Actions.Create), "Access Key is a Required field when adding a repository of type Azure IoT Hub");
        }


        [TestMethod]
        public void DeviceRepo_Validation_AzureIoTHub_InvalidAccessKey_InValid()
        {
            var repo = GetValidRepo();
            repo.ResourceName = "AzureIoTHub";
            repo.AccessKeyName = "AzureIoTHub";
            repo.AccessKey = "%!$%GA Fq dfadfr";

            repo.RepositoryType = LagoVista.Core.Models.EntityHeader<Models.RepositoryTypes>.Create(Models.RepositoryTypes.AzureIoTHub);
            repo.DeviceArchiveStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            repo.DeviceStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            repo.PEMStorageSettings = new LagoVista.Core.Models.ConnectionSettings(); ;
            AssertInvalidError(Validator.Validate(repo, Actions.Create), "Access Key does not appear to be a Base 64 String.");
        }

        [TestMethod]
        public void DeviceRepo_Validation_AzureIoTHub_MissingAccessKeyName_InValid()
        {
            var repo = GetValidRepo();
            repo.ResourceName = "AzureIoTHub";
            repo.AccessKeyName = null;
            repo.AccessKey = "dGhpc2lzbXlrZXlwbGFzZXZpZW1lc29tZWRhdGE=";

            repo.RepositoryType = LagoVista.Core.Models.EntityHeader<Models.RepositoryTypes>.Create(Models.RepositoryTypes.AzureIoTHub);
            repo.DeviceArchiveStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            repo.DeviceStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            repo.PEMStorageSettings = new LagoVista.Core.Models.ConnectionSettings(); ;
            AssertInvalidError(Validator.Validate(repo, Actions.Create), "Access Key name is a Required field.");
        }

        [TestMethod]
        public void DeviceRepo_Validation_AzureIoTHub_MissingAccessKeyName_Update_MissingKeyAndSecureId_InValid()
        {
            var repo = GetValidRepo();
            repo.ResourceName = "AzureIoTHub";
            repo.AccessKeyName = "accesskeyname";
            repo.AccessKey = null;
            repo.SecureAccessKeyId = null;

            repo.RepositoryType = LagoVista.Core.Models.EntityHeader<Models.RepositoryTypes>.Create(Models.RepositoryTypes.AzureIoTHub);
            repo.DeviceArchiveStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            repo.DeviceStorageSettings = new LagoVista.Core.Models.ConnectionSettings();
            repo.PEMStorageSettings = new LagoVista.Core.Models.ConnectionSettings(); ;
            AssertInvalidError(Validator.Validate(repo, Actions.Update), "Access Key or ScureAccessKeyId is a Required when updating a repo of Azure IoT Hub.");
        }


    }
}
