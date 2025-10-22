// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 054efa1c107e955870af61ecafe8f4036573c1bfc8e4c0ddbf827c972a3f3d2c
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.IoT.DeviceManagement.Core.Models;
using System;
using LagoVista.Core;
using LagoVista.Core.Models;

namespace LagoVista.IoT.DeviceManagement.Core.Tests.DeviceRepoTests
{
    public class DeviceRepoTestBase : ValidationBase
    {
        protected DeviceRepository GetValidRepo()
        {
            EntityHeader _user = EntityHeader.Create("3367B1522AF441F39238A85A80B94D33", "User");
            EntityHeader _org = EntityHeader.Create("5567B1522AF441F39238A85A80B94D33", "User");

            var repo = new DeviceRepository()
            {
                Id = "6C67B1522AF441F39238A85A80B94D39",
                CreatedBy = _user,
                CreationDate = DateTime.UtcNow.ToJSONString(),
                LastUpdatedBy = _user,
                LastUpdatedDate = DateTime.UtcNow.ToJSONString(),
                OwnerOrganization = _org,
                Name = "My Good Repo",
                Key = "mygoodrepo",
                Subscription = EntityHeader.Create("id123", "idtext"),
                DeviceCapacity = EntityHeader.Create("dev123", "capac"),
                StorageCapacity = EntityHeader.Create("storage", "storage"),
                RepositoryType = EntityHeader<RepositoryTypes>.Create(RepositoryTypes.NuvIoT)
            };

            return repo;
        }
    }
}
