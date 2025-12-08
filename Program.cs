using System.CommandLine;
using GitFlowAi.Handlers;

namespace GitFlowAi
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            Command rootCommand = new RootCommand("An AI-powered tool to automate Git workflows.");
            Command runCommand = new Command("run", "Automatically analyze changes, commit, or branch based on AI decisions.");
            Command statusCommand = new Command("status", "Check the status of the current working directory.");
            Command analyzeCommand = new Command("analyze", "Analyze changes, commit, or branch and give feedback base on the current changes");
            Command stageCommand = new Command("stage", "Stage the current working directory.");
            Command initCommand = new Command("init", "Initialize a GitFlowAi repository in the current working directory.");
            
            statusCommand.SetHandler( () => CommandHandlers.RunStatusCheck());
            runCommand.SetHandler(() => CommandHandlers.InitialRun());
            analyzeCommand.SetHandler(() => CommandHandlers.RunAnalyzeCommand());
            stageCommand.SetHandler(() => CommandHandlers.RunStageCommand());
            initCommand.SetHandler(() => CommandHandlers.RunInitCommand());
            
            rootCommand.Add(statusCommand);
            rootCommand.Add(runCommand);
            rootCommand.Add(analyzeCommand);
            rootCommand.Add(stageCommand);
            rootCommand.Add(initCommand);
            
            return await rootCommand.InvokeAsync(args);
        }
        
        
    }
}