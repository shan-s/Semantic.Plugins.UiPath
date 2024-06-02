using Microsoft.Extensions.Configuration;
using LongreachAi.Connectors.UiPath;

namespace LongreachAi.Semantic.Plugins;
public class UiPathPlugin
{
readonly OrchestratorQuery _QueryClient;
readonly OrchestratorAction _ActionClient;
public UiPathPlugin()
{
     var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        var options = config.GetSection(UiPathOptions.SectionName).Get<UiPathOptions>()!;
        var orch = new Orchestrator(options);
        _QueryClient = new OrchestratorQuery(orch);
        _ActionClient = new OrchestratorAction(orch);
}

 

}
