using LibGit2Sharp;
using System.CommandLine;

Console.WriteLine("--- Welcome to GitFlowAI ---");

var rootCommand = new RootCommand("An AI-powered tool to automate Git workflows.");


var autoCommand = new Command("auto", "Automatically analyze changes, commit, or branch based on AI decisions.")
{
    
};

autoCommand.SetHandler(() =>
{
    Console.WriteLine("--- GitFlowAI Auto-Run Initiated ---");
     var  dir = Console.ReadLine();
    if (dir == null || dir == "")
    {
        Console.WriteLine("You have to enter a path.");
    }

    var agentResponse = new Agent("gemini", "who am i?", "any");
    Console.WriteLine($"Path: {agentResponse.Prompt}");
    PrintHelp();
    
});

rootCommand.Add(autoCommand);
return await rootCommand.InvokeAsync(args);


void PrintHelp()
{
    Console.WriteLine("Usage: GitFlowAI [OPTIONS]+");
}