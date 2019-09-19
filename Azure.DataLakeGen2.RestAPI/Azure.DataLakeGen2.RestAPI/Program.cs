

using Azure.DataLakeGen2.RestAPI.FileSystem;
using Azure.DataLakeGen2.RestAPI.Security;
using System;

namespace Azure.DataLakeGen2.RestAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            var clientId = "#############";
            var tenantId = "#############";
            var secret = "####################";
            var scope = "https://storage.azure.com/.default";
            var storageAccountName = "##############";
            var fileSystemName = "myexamplehdfs";

            var tokenProvider = new OAuthTokenProvider(tenantId, clientId, secret, scope);
            var hdfs = new FileSystemApi(storageAccountName, tokenProvider);

            //var response = hdfs.CreateFileSystemAsync(fileSystemName).Result;

            //hdfs.CreateDirectoryAsync(fileSystemName, "/demo").Wait();
            hdfs.CreateFileAsync(fileSystemName, "/demo/example.txt").Wait();

            var acls = new AclEntry[]
            {
                new AclEntry(
                    AclScope.Access,
                    AclType.Group,
                    "2dec2374-3c51-4743-b247-ad6f80ce4f0b",
                    (GrantType.Read | GrantType.Execute)),
                new AclEntry(
                    AclScope.Access,
                    AclType.Group,
                    "62049695-0418-428e-a5e4-64600d6d68d8",
                    (GrantType.Read | GrantType.Write | GrantType.Execute)),
                new AclEntry(
                    AclScope.Default,
                    AclType.Group,
                    "62049695-0418-428e-a5e4-64600d6d68d8",
                    (GrantType.Read | GrantType.Write | GrantType.Execute))
            };

            //hdfs.SetAccessControlAsync(fileSystemName, "/", acls).Wait();
            Console.ReadLine();
        }
    }
}
