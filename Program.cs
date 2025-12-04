using System.CommandLine;
using GitFlowAi.Handlers;

namespace GitFlowAi
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            Command rootCommand = new RootCommand("An AI-powered tool to automate Git workflows.");
            Command autoCommand = new Command("auto", "Automatically analyze changes, commit, or branch based on AI decisions.");
            Command statusCommand = new Command("status", "Check the status of the current working directory.");
            Command runCommand = new Command("run", "Ai check the current directory and commit changes or create branched and commit base on the changes.");
            Command analyzeCommand = new Command("analyze", "Analyze changes, commit, or branch and give feedback base on the current changes");
            Command stageCommand = new Command("stage", "Stage the current working directory.");
            
            statusCommand.SetHandler( () => CommandHandlers.RunStatusCheck());
            autoCommand.SetHandler( () => CommandHandlers.RunAutoFlow());
            runCommand.SetHandler(() => CommandHandlers.InitialRun());
            analyzeCommand.SetHandler(() => CommandHandlers.RunAnalyzeCommand());
            stageCommand.SetHandler(() => CommandHandlers.RunStageCommand());

            rootCommand.Add(autoCommand);
            rootCommand.Add(statusCommand);
            rootCommand.Add(runCommand);
            rootCommand.Add(analyzeCommand);
            rootCommand.Add(stageCommand);
            
            return await rootCommand.InvokeAsync(args);
        }
        
        
    }
}