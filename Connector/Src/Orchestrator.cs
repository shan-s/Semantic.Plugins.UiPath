namespace LongreachAi.Connectors.UiPath
{
    public class Orchestrator
    {
        readonly UiPathOptions _ConfigOptions = new();
        readonly HttpClient _Client;
        readonly UiPathAuthenticator _Authenticator;
        readonly OrchestratorAPI _Api;
        UiPathAuthenticator.AuthenticationResponse _Authresponse = new();
        DateTime _TokenExpiry = DateTime.MinValue;
        public IEnumerable<FolderDto>? Folders {get;set;}
        public IEnumerable<MachineDto>? MachineTemplates {get;set;}
        public IEnumerable<UserDto>? Users {get;set;}
        public IEnumerable<ProcessDto>? Packages {get;set;}
        public IEnumerable<RoleDto>? Roles {get;set;}
        public OrchestratorAPI Api
        {
            get
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
        }

        public Orchestrator(UiPathOptions configOptions)
        {
            ArgumentNullException.ThrowIfNull(configOptions, nameof(configOptions));
            _ConfigOptions = configOptions;

            _Authenticator = new UiPathAuthenticator(configOptions);

            _Client = new HttpClient(new HttpClientHandler { UseProxy = _ConfigOptions.UseProxy })
            {
                BaseAddress = new Uri(_ConfigOptions.OrchestratorUrl)
            };
            _Api = new OrchestratorAPI(_Client);
        }
    }

}






