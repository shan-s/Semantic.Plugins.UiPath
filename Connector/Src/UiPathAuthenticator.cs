namespace LongreachAi.Connectors.UiPath
{
    using System.Net.Http;
    using System.Text.Json;
    using Microsoft.Extensions.Options;
    using RestSharp;

    /// <summary>
    /// Authenticates with UiPath Identity Endpoint. Supports Confidential Client Credentials grant type.
    /// </summary>
    public class UiPathAuthenticator(UiPathOptions configOptions) 
    {
        readonly UiPathOptions _ConfigOptions = configOptions ??
                                throw new ArgumentNullException(nameof(configOptions));
        readonly JsonSerializerOptions _Serializer = new() { PropertyNameCaseInsensitive = true };

        /// <summary>
        /// Authentication Response object
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="token_type"></param>
        /// <param name="expires_in"></param>
        /// <param name="scope"></param>
        public record AuthenticationResponse(string access_token = "", string token_type = "",
                                                    int expires_in = 0, string scope = "");

        /// <summary>
        /// Authenticate with UiPath Identity Endpoint. Returns the AuthenticationResponse object.
        /// </summary>
        /// <returns>AuthenticationResponse</returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="JsonException"></exception>
        public AuthenticationResponse Authenticate()
        {

            HttpClient client = new(new HttpClientHandler { UseProxy = _ConfigOptions.UseProxy })
            {
                BaseAddress = new Uri(_ConfigOptions.IdentityUrl)
            };

            var request = new RestRequest("connect/token", Method.Post)
                .AddHeader("Content-Type", "application/x-www-form-urlencoded")
                .AddParameter("grant_type", "client_credentials", ParameterType.GetOrPost)
                .AddParameter("client_id", _ConfigOptions.ClientId, ParameterType.GetOrPost)
                .AddParameter("client_secret", _ConfigOptions.ClientSecret, ParameterType.GetOrPost)
                .AddParameter("scope", _ConfigOptions.AuthScope, ParameterType.GetOrPost);
            RestResponse response;

            using (RestClient rc = new(client))
            {
                response = rc.Execute(request);
            }

            if (!response.IsSuccessful)
                throw new HttpRequestException($"Failed to authenticate: {response.Content}");

            return JsonSerializer.Deserialize<AuthenticationResponse>(response.Content!, _Serializer)
                   ?? throw new JsonException("Unexpected response form");
        }
    }
}