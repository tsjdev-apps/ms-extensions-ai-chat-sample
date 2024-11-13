using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using MicrosoftExtensionsAiSample.Utils;
using OpenAI;
using System.ClientModel;

IChatClient? chatClient = null;

// get the chat host
string chatHost =
    ConsoleHelper.SelectFromOptions(
        [Statics.OllamaKey, Statics.OpenAiKey,
        Statics.AzureOpenAiKey]);

switch (chatHost)
{
    // use OLLAMA
    case Statics.OllamaKey:

        // get the OLLAMA model name
        string ollamaModelName =
            ConsoleHelper.GetString("Enter your Ollama model name:");

        // create the OLLAMA chat client
        chatClient = new OllamaChatClient(
            new Uri("http://localhost:11434/"), ollamaModelName);

        break;

    // use OpenAI
    case Statics.OpenAiKey:

        // get the OpenAI API key
        string openAiKey =
            ConsoleHelper.GetString("Enter your OpenAI API key:");

        // get the OpenAI model name
        string openAiModel =
            ConsoleHelper.SelectFromOptions(
                [Statics.Gpt4oMiniModelName, Statics.Gpt4oModelName,
                Statics.Gpt4TurboModelName, Statics.Gpt4ModelName]);

        // create the OpenAI chat client
        chatClient = new OpenAIClient(
            openAiKey)
            .AsChatClient(openAiModel);

        break;

    // use Azure OpenAI
    case Statics.AzureOpenAiKey:

        // get the Azure OpenAI endpoint
        string azureOpenAiEndpoint =
            ConsoleHelper.GetString("Enter your Azure OpenAI endpoint:");

        // get the Azure OpenAI API key
        string azureOpenAiKey =
            ConsoleHelper.GetString("Enter your Azure OpenAI API key:");

        // get the Azure OpenAI model name
        string azureOpenAiModel =
            ConsoleHelper.GetString("Enter your Azure OpenAI chat model name:");

        // create the Azure OpenAI chat client
        chatClient = new AzureOpenAIClient(
            new Uri(azureOpenAiEndpoint),
            new ApiKeyCredential(azureOpenAiKey))
            .AsChatClient(azureOpenAiModel);

        break;
}

// check if the chat client is valid
if (chatClient is null)
{
    ConsoleHelper.DisplayError("Invalid chat host selected.");
    return;
}

// show the header
ConsoleHelper.ShowHeader();

// create a list of chat messages
List<ChatMessage> chatMessages = [];

// loop forever
while (true)
{
    // get the user message
    string userMessage =
        ConsoleHelper.GetString("Enter your message:", false);

    // add the user message to the chat messages
    chatMessages.Add(new ChatMessage(ChatRole.User, userMessage));

    Console.WriteLine();
    Console.WriteLine("AI:");

    // send the chat messages to the model and stream the response messages
    await foreach (var update in chatClient.CompleteStreamingAsync(chatMessages))
    {
        Console.Write(update);
    }

    Console.WriteLine();
    Console.WriteLine();
}