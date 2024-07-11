using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using LongreachAi.Semantic.Plugins;
using LongreachAi.Connectors.UiPath;

// Load the configuration
var config = new ConfigurationBuilder()
    .AddJsonFile("config/appsettings.json", optional: true, reloadOnChange: true)
    .Build();

var kernelSettings = config.GetSection(KernelSettings.SectionName).Get<KernelSettings>()
                     ?? throw new Exception("KernelSettings not found in appsettings.json");

var builder = Kernel.CreateBuilder();
builder.Services.AddLogging(c => c.SetMinimumLevel(LogLevel.Information));
builder.Services.AddChatCompletionService(kernelSettings);
builder.Services.AddOptions<UiPathOptions>().Bind(config.GetSection(UiPathOptions.SectionName));
builder.Plugins.AddFromType<UiPathPlugin>();
builder.Plugins.AddFromType<GlobalContextPlugin>();

Kernel kernel = builder.Build();

// Load prompt from resource
using StreamReader reader = new(Assembly.GetExecutingAssembly().GetManifestResourceStream("Demo.Resources.Chat.yaml")!);
KernelFunction prompt = kernel.CreateFunctionFromPromptYaml(
    reader.ReadToEnd(),
    promptTemplateFactory: new HandlebarsPromptTemplateFactory()
);

// Create the chat history
ChatHistory chatMessages = [];

// Loop till we are cancelled
while (true)
{
    // Get user input
    System.Console.Write("User > ");
    chatMessages.AddUserMessage(Console.ReadLine()!);

    // Get the chat completions
    OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
    };

    var result = kernel.InvokeStreamingAsync<StreamingChatMessageContent>(
        prompt,
        arguments: new(openAIPromptExecutionSettings) {
            { "messages", chatMessages }
        });

    // Print the chat completions
    ChatMessageContent? chatMessageContent = null;
    await foreach (var content in result)
    {
        System.Console.Write(content);
        if (chatMessageContent == null)
        {
            System.Console.Write("Assistant > ");
            chatMessageContent = new ChatMessageContent(
                content.Role ?? AuthorRole.Assistant,
                content.ModelId!,
                content.Content!,
                content.InnerContent,
                content.Encoding,
                content.Metadata);
        }
        else
        {
            chatMessageContent.Content += content;
        }
    }
    System.Console.WriteLine();
    if (chatMessageContent != null)
    {
        chatMessages.AddMessage(chatMessageContent.Role, chatMessageContent.Content!);
    }
}
