using LibGit2Sharp;
using System.CommandLine;
using GitFlowAi.Services;
using GitFlowAi.Config;
using GitFlowAi.Models;

namespace GitFlowAi
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            Console.WriteLine("--- Welcome to GitFlowAI ---");

            var rootCommand = new RootCommand("An AI-powered tool to automate Git workflows.");
            var autoCommand = new Command("auto", "Automatically analyze changes, commit, or branch based on AI decisions.");
            var statusCommand = new Command("status", "Check the status of the current working directory.");
            var runCommand = new Command("run", "Ai check the current directory and commit changes or create branched and commit base on the changes.");
            
            statusCommand.SetHandler( () => RunStatusCheck());
            autoCommand.SetHandler( () => RunAutoFlow());
            runCommand.SetHandler(() => InitialRun());

            rootCommand.Add(autoCommand);
            rootCommand.Add(statusCommand);
            rootCommand.Add(runCommand);
            
            return await rootCommand.InvokeAsync(args);
        }
        
        /// <summary>
        /// Checks the status of the current working directory.
        /// Changed from async void to async Task.
        /// </summary>
        async static Task RunStatusCheck()
        {
            try
            {
                var workDirectory = Environment.CurrentDirectory;
                var service = new GitService(workDirectory);
                
                service.PrintCurrentBranch();
                
                // Task.Run is used here to potentially offload CPU-bound work, though it's often optional
                // for I/O bound methods which should be async internally.
                (bool isDirty, string? diff) = await Task.Run(() => service.GetRepoStatusAndDiff());

                if (isDirty)
                {
                    Console.WriteLine($"Changes were made on working directory: {workDirectory}");
                }
                else
                {
                    Console.WriteLine("Directory is Clean");
                    Console.WriteLine("No changes were made");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred during status check: {e.Message}");
            }
        }

        /// <summary>
        /// Runs the AI-powered automatic Git flow (analyze and decide).
        /// Changed from async void to async Task.
        /// </summary>
        async static Task RunAutoFlow()
        {
            try
            {
                string workDirectory = Environment.CurrentDirectory;

                var service = new GitService(workDirectory);
                var manager = new SecretManager();
                string apiKey = manager.GetGeminiApiKey();
            
                Console.WriteLine($"API Key Status: {(string.IsNullOrEmpty(apiKey) ? "MISSING" : "FOUND")}");
                
                var genClient = new GeminiService(apiKey);
            
                service.PrintCurrentBranch();
            
                // Task.Run is used to ensure the LibGit2Sharp call is offloaded from the UI thread,
                // which is good practice for CPU-bound operations in command line tools.
                (bool isDirty, string? diff) = await Task.Run(() => service.GetRepoStatusAndDiff());
            
                if (isDirty)
                {
                    Console.WriteLine($"Changes were made on working directory: {workDirectory}");
                    Console.WriteLine("--------------------------------------------------");

                    // The actual AI call, which should be asynchronous internally in GeminiService
                    GitDecision response = await genClient.GenerateDecision(diff);

                    Console.WriteLine($"AI Response: {response}");

                    switch (response.Action)
                    {
                        case GitAction.COMMIT:
                            service.CommitChanges(response.CommitMessage, response.FilePaths);
                            break;
                        case GitAction.BRANCH:
                            break;
                        case GitAction.SKIP:
                            Console.WriteLine($"{response.Explanation}");
                            break;
                        default:
                            Console.WriteLine("No Action needed.");
                            break;
                    }
                    
                    string formatedDiff = diff.Length > 0 ? diff.Substring(0, Math.Min(diff.Length, 100)) + (diff.Length > 100 ? "..." : "") : "No Diff";
                    Console.WriteLine($"Preview of Changes Sent to AI: {formatedDiff}");
                }
                else
                {
                    Console.WriteLine("Directory is Clean. No changes to process.");
                }
            }
            catch (RepositoryNotFoundException ex)
            {
                Console.WriteLine($"❌ Git Error: {ex.Message} (Not a Git repository)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ An unexpected error occurred: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Handles the full AI workflow for checking and committing/branching.
        /// Returns Task to be awaited by the command line interface.
        /// </summary>
        async static Task InitialRun()
        {
            try
            {
                Console.WriteLine("|______Running Initial AI Workflow______|");
                
                string workDirectory = Environment.CurrentDirectory;
                var service = new GitService(workDirectory);
                
                (bool isDirty, string? diff) = await Task.Run(() => service.GetRepoStatusAndDiff());
                
                if (isDirty)
                {
                    Console.WriteLine("Reading changes made on current directory...");
                    // Placeholder for the full logic:
                    // 1. Get API Key
                    // 2. Initialize GeminiService
                    // 3. Get AI decision (commit message or new branch name)
                    // 4. Execute Git command (commit or branch + commit)
                    Console.WriteLine("Diff found. Processing with AI...");
                    await RunAutoFlow(); // Reusing the AutoFlow logic for demonstration
                }
                else
                {
                    Console.WriteLine("Working directory is clean. Nothing to do.");
                }
            }
            catch (RepositoryNotFoundException ex)
            {
                Console.WriteLine($"❌ Git Error: {ex.Message} (Not a Git repository)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ An unexpected error occurred in InitialRun: {ex.Message}");
            }
        }
    }
}