using LibGit2Sharp;
using System.CommandLine;
using GitFlowAi.Services;
using GitFlowAi.Config;

Console.WriteLine("--- Welcome to GitFlowAI ---");

var rootCommand = new RootCommand("An AI-powered tool to automate Git workflows.");

var autoCommand = new Command("auto", "Automatically analyze changes, commit, or branch based on AI decisions.");

var statusCommand = new Command("status", "Check the status of the current working directory.");

statusCommand.SetHandler(() => RunStatusCheck());
autoCommand.SetHandler(() => RunAutoFlow());

rootCommand.Add(autoCommand);
rootCommand.Add(statusCommand);
return await rootCommand.InvokeAsync(args);

async void RunStatusCheck()
{
    try
    {
        string workDirectory = Environment.CurrentDirectory;
        var service = new GitService(workDirectory);
        Agent newAgent = new Agent("what is this", "you are an expert");
        
        newAgent.SetModel("gemini-flash-002");
        string model = newAgent.GetModel();
        Console.WriteLine($"model: {model}");
        
        service.PrintCurrentBranch();
        service.GetRepoStatus();
        
        (bool isDirty, string? diff) = service.GetRepoStatusAndDiff();

        if (isDirty)
        {
            Console.WriteLine($"Changes were made on working directory: {workDirectory}");
            var manager = new SecretManager();
            string apiKey = manager.GetGeminiApiKey();
            
            var genClient = new GeminiService(apiKey);

            string response = await genClient.GenerateDecision(diff);

        }
        else
        {
            Console.WriteLine("Directory is Clean");
            Console.WriteLine("No changes were made");
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        throw;
    }
}

void RunAutoFlow()
{
    try
    {
        string workDirectory = Environment.CurrentDirectory;

        var service = new GitService(workDirectory);
        
        service.PrintCurrentBranch();
        service.GetRepoStatus();
        
        (bool isDirty, string? diff) = service.GetRepoStatusAndDiff();

        if (isDirty)
        {
            string formatedDiff = diff.Length > 0 ? diff?.Substring(0, 400) : "";
            Console.WriteLine($"Changes were made on working directory: {workDirectory}");
            Console.WriteLine(formatedDiff);
            Console.WriteLine("--------------------------------------------------");
        }
    }
    catch (RepositoryNotFoundException ex)
    {
        Console.WriteLine($"❌ Git Error: {ex.Message}");
    }
}