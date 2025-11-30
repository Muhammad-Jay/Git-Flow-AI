using LibGit2Sharp;
using System.CommandLine;
using GitFlowAi.Services;

Console.WriteLine("--- Welcome to GitFlowAI ---");

var rootCommand = new RootCommand("An AI-powered tool to automate Git workflows.");

var autoCommand = new Command("auto", "Automatically analyze changes, commit, or branch based on AI decisions.");

var statusCommand = new Command("status", "Check the status of the current working directory.");

statusCommand.SetHandler(() => RunStatusCheck());
autoCommand.SetHandler(() => RunAutoFlow());

rootCommand.Add(autoCommand);
rootCommand.Add(statusCommand);
return await rootCommand.InvokeAsync(args);

void RunStatusCheck()
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
            Console.WriteLine($"Changes were made on working directory: {workDirectory}");
            Console.WriteLine(diff);
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
            Console.WriteLine($"Changes were made on working directory: {workDirectory}");
            Console.WriteLine(diff);
        }
    }
    catch (RepositoryNotFoundException ex)
    {
        Console.WriteLine($"❌ Git Error: {ex.Message}");
    }
}