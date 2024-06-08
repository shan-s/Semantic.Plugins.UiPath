namespace LongreachAi.Semantic.Plugins;

using Microsoft.Extensions.Configuration;
using LongreachAi.Connectors.UiPath;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public class UiPathPlugin(IOptions<UiPathOptions> options)
{
        readonly Orchestrator orch = new (options.Value);

        [KernelFunction("GetFolders")]
        [Description(@"Get all folders.
        Returns a Json string with folder details")]
        public string GetFolders()
        {
                return orch.GetFolders().ToJson();
        }



}
