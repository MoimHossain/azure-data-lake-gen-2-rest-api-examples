


using Azure.DataLakeGen2.RestAPI.Security;
using Azure.DataLakeGen2.RestAPI.Supports;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Azure.DataLakeGen2.RestAPI.FileSystem
{
    public class FileSystemApi
    {
        private readonly string storageAccountName;
        private readonly OAuthTokenProvider tokenProvider;
        private readonly Uri baseUri;
        private const string ACK_HEADER_NAME = "x-ms-acl";
        private const string API_VERSION_HEADER_NAME = "x-ms-version";
        private const string API_VERSION_HEADER_VALUE = "2018-11-09";
        private int Timeout = 100;

        public FileSystemApi(string storageAccountName, OAuthTokenProvider tokenProvider)
        {
            this.storageAccountName = storageAccountName;
            this.tokenProvider = tokenProvider;
            this.baseUri = new Uri($"https://{this.storageAccountName}.dfs.core.windows.net");
        }

        public async Task<bool> CreateFileSystemAsync(
            string fileSystemName)
        {
            var tokenInfo = await tokenProvider.GetAccessTokenV2EndpointAsync();

            var jsonContent = new StringContent(string.Empty);
            var headers = Statics.Http.DefaultRequestHeaders;
            headers.Clear();
            headers.Add("Authorization", $"Bearer {tokenInfo.access_token}");
            headers.Add(API_VERSION_HEADER_NAME, API_VERSION_HEADER_VALUE);
            var response = await Statics.Http.PutAsync($"{baseUri}{WebUtility.UrlEncode(fileSystemName)}?resource=filesystem", jsonContent);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CreateDirectoryAsync(string fileSystemName, string fullPath)
        {
            var tokenInfo = await tokenProvider.GetAccessTokenV2EndpointAsync();

            var jsonContent = new StringContent(string.Empty);
            var headers = Statics.Http.DefaultRequestHeaders;
            headers.Clear();
            headers.Add("Authorization", $"Bearer {tokenInfo.access_token}");
            headers.Add(API_VERSION_HEADER_NAME, API_VERSION_HEADER_VALUE);
            var response = await Statics.Http.PutAsync($"{baseUri}{WebUtility.UrlEncode(fileSystemName)}{fullPath}?resource=directory", jsonContent);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CreateEmptyFileAsync(string fileSystemName, string path, string fileName)
        {
            var tokenInfo = await tokenProvider.GetAccessTokenV2EndpointAsync();

            var jsonContent = new StringContent(string.Empty);
            var headers = Statics.Http.DefaultRequestHeaders;
            headers.Clear();
            headers.Add("Authorization", $"Bearer {tokenInfo.access_token}");
            headers.Add(API_VERSION_HEADER_NAME, API_VERSION_HEADER_VALUE);
            var response = await Statics.Http.PutAsync($"{baseUri}{WebUtility.UrlEncode(fileSystemName)}{path}{fileName}?resource=file", jsonContent);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CreateFileAsync(string filesystem, string path, 
            string fileName, Stream stream)
        {           
            var operationResult = await this.CreateEmptyFileAsync(filesystem, path, fileName);
            if (operationResult)
            {
                var tokenInfo = await tokenProvider.GetAccessTokenV2EndpointAsync();
                var headers = Statics.Http.DefaultRequestHeaders;
                headers.Clear();
                headers.Add("Authorization", $"Bearer {tokenInfo.access_token}");
                headers.Add(API_VERSION_HEADER_NAME, API_VERSION_HEADER_VALUE);

                using (var streamContent = new StreamContent(stream))
                {
                    var resourceUrl = $"{baseUri}{filesystem}{path}{fileName}?action=append&timeout={this.Timeout}&position=0";
                    var msg = new HttpRequestMessage(new HttpMethod("PATCH"), resourceUrl);
                    msg.Content = streamContent;
                    var response = await Statics.Http.SendAsync(msg);

                    //flush the buffer to commit the file
                    var flushUrl = $"{baseUri}{filesystem}{path}{fileName}?action=flush&timeout={this.Timeout}&position={msg.Content.Headers.ContentLength}";
                    var flushMsg = new HttpRequestMessage(new HttpMethod("PATCH"), flushUrl);
                    response = await Statics.Http.SendAsync(flushMsg);

                    return response.IsSuccessStatusCode;
                }
            }
            return false;
        }

        public async Task<bool> SetAccessControlAsync(string fileSystemName, string path, AclEntry[] acls)
        {
            var targetPath = $"{WebUtility.UrlEncode(fileSystemName)}{path}";
            var tokenInfo = await tokenProvider.GetAccessTokenV2EndpointAsync();

            var jsonContent = new StringContent(string.Empty);
            var headers = Statics.Http.DefaultRequestHeaders;
            headers.Clear();
            headers.Add("Authorization", $"Bearer {tokenInfo.access_token}");
            headers.Add(API_VERSION_HEADER_NAME, API_VERSION_HEADER_VALUE);
            headers.Add(ACK_HEADER_NAME, string.Join(',', acls.Select(a => a.ToString()).ToArray()));
            var response = await Statics.Http.PatchAsync($"{baseUri}{targetPath}?action=setAccessControl", jsonContent);

            return response.IsSuccessStatusCode;
        }
    }
}
