namespace LongreachAi.Connectors.UiPath
{
    public class UiPathOptions
    {
        public const string SectionName = "UiPathOptions";
        public string OrchestratorUrl { get; set; } = "";
        public string IdentityUrl { get; set; } = "https://cloud.uipath.com/identity_/";
        public string ClientId { get; set; } = "";
        public string ClientSecret { get; set; } = "";
        public bool UseProxy { get; set; } = true;
        public string AuthScope { get; set; } = @"OR.License OR.Robots OR.Machines OR.Execution 
        OR.Assets OR.Queues OR.Jobs OR.Users OR.Folders";
    }

}
