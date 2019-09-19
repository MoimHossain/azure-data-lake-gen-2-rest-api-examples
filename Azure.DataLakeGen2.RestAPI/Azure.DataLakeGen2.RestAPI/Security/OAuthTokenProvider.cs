

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Azure.DataLakeGen2.RestAPI.Security
{
    public class OAuthTokenProvider
    {
        private readonly string tenantId;
        private readonly string clientId;
        private readonly string secret;
        private readonly string scopeUri;
        private const string IdentityEndpoint = "https://login.microsoftonline.com";
        private const string DEFAULT_SCOPE = "https://management.azure.com/";
        private const string MEDIATYPE = "application/x-www-form-urlencoded";

        public OAuthTokenProvider(string tenantId, string clientId, string secret, string scopeUri = DEFAULT_SCOPE)
        {
            this.tenantId = tenantId;
            this.clientId = WebUtility.UrlEncode(clientId);
            this.secret = WebUtility.UrlEncode(secret);
            this.scopeUri = WebUtility.UrlEncode(scopeUri);
        }

        public async Task<Token> GetAccessTokenV2EndpointAsync()
        {
            var url = $"{IdentityEndpoint}/{this.tenantId}/oauth2/v2.0/token";
            var Http = Statics.Http;
            Http.DefaultRequestHeaders.Accept.Clear();
            Http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MEDIATYPE));
            var body = $"grant_type=client_credentials&client_id={clientId}&client_secret={secret}&scope={scopeUri}";
            var response = await Http.PostAsync(url, new StringContent(body, Encoding.UTF8, MEDIATYPE));
            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Token>(tokenResponse);
            }
            return default(Token);
        }

        public class Token
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public int ext_expires_in { get; set; }
        }
    }
}