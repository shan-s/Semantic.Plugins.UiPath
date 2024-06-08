using Microsoft.Extensions.Options;

namespace LongreachAi.Connectors.UiPath
{
    public partial class Orchestrator
    {
        readonly UiPathOptions _ConfigOptions;
        readonly HttpClient _Client;
        readonly UiPathAuthenticator _Authenticator;
        readonly OrchestratorAPI _Api;
        UiPathAuthenticator.AuthenticationResponse _Authresponse = new();
        DateTime _TokenExpiry = DateTime.MinValue;
        public IEnumerable<FolderDto>? Folders { get; set; }
        public IEnumerable<MachineDto>? MachineTemplates { get; set; }
        public IEnumerable<UserDto>? Users { get; set; }
        public IEnumerable<ProcessDto>? Packages { get; set; }
        public IEnumerable<RoleDto>? Roles { get; set; }
        internal OrchestratorAPI GetApi()
        {   //Refresh token if it is about to expire and set the new token in the httpclient header   
            if (_TokenExpiry.Subtract(DateTime.Now).Minutes < 5)
            {
                _Authresponse = _Authenticator.Authenticate();
                _TokenExpiry = DateTime.Now.AddSeconds(_Authresponse.expires_in);
                _Client.DefaultRequestHeaders.Authorization = new("Bearer",
                                    _Authresponse.access_token);
            }
            return _Api;

        }

        public Orchestrator(UiPathOptions configOptions)
        {
            _ConfigOptions = configOptions ??
                                throw new ArgumentNullException(nameof(configOptions));

            _Authenticator = new UiPathAuthenticator(configOptions);

            _Client = new HttpClient(new HttpClientHandler { UseProxy = _ConfigOptions.UseProxy })
            {
                BaseAddress = new Uri(_ConfigOptions.OrchestratorUrl)
            };
            _Api = new OrchestratorAPI(_Client);
        }
    }

}






