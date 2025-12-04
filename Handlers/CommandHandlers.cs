using GitFlowAi.Config;
using GitFlowAi.Models;
using GitFlowAi.Services;
using LibGit2Sharp;

namespace GitFlowAi.Handlers
{
    public class CommandHandlers
    {
        private static string _workDirectory = Environment.CurrentDirectory;
        private static GitService _service = new GitService(_workDirectory);
        
        /// <summary>
        /// Checks the status of the current working directory.
        /// Changed from async void to async Task.
        /// </summary>
        public static async Task RunStatusCheck()
        {
            try
            {
                Console.WriteLine("--------------------------------------------------");
                
                _service.PrintCurrentBranch();
                
                // Task.Run is used here to potentially offload CPU-bound work, though it's often optional
                // for I/O bound methods which should be async internally.
                (bool isDirty, string? diff) = await Task.Run(() => _service.GetRepoStatusAndDiff());

                if (isDirty)
                {
                    Console.WriteLine($"Changes were made on working directory: {_workDirectory}");
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
        public static async Task RunAutoFlow()
        {
            try
            {
                Console.WriteLine("--------------------------------------------------");
                var manager = new SecretManager();
                string apiKey = manager.GetGeminiApiKey();
            
                Console.WriteLine($"API Key Status: {(string.IsNullOrEmpty(apiKey) ? "MISSING" : "FOUND")}");
                
                var genClient = new GeminiService(apiKey);
            
                _service.PrintCurrentBranch();
            
                // Task.Run is used to ensure the LibGit2Sharp call is offloaded from the UI thread,
                (bool isDirty, string? diff) = await Task.Run(() => _service.GetRepoStatusAndDiff());
            
                if (isDirty)
                {
                    Console.WriteLine($"Changes were made on working directory: {_workDirectory}");

                    // The actual AI call, which should be asynchronous internally in GeminiService
                    GitDecision response = await genClient.GenerateDecision(diff);

                    Console.WriteLine($"AI Response: {response}");

                    switch (response.Action)
                    {
                        case GitAction.Commit:
                            _service.CommitChanges(response.CommitMessage, response.FilePaths);
                            Console.WriteLine($"Commit: {response.CommitMessage}");
                            Console.WriteLine($"Reason: {response.Explanation}");
                            break;
                        case GitAction.Branch:
                            break;
                        case GitAction.Skip:
                            Console.WriteLine($"{response.Explanation}");
                            break;
                        default:
                            Console.WriteLine("No Action needed.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Directory is Clean. No changes to process.");
                }
            }
            catch (RepositoryNotFoundException ex)
            {
                Console.WriteLine($"Git Error: {ex.Message} (Not a Git repository)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Handles the full AI workflow for checking and committing/branching.
        /// Returns Task to be awaited by the command line interface.
        /// </summary>
        public static async Task InitialRun()
        {
            try
            {
                Console.WriteLine("--------------------------------------------------");
                
                (bool isDirty, string? diff) = await Task.Run(() => _service.GetRepoStatusAndDiff());
                
                if (isDirty)
                {
                    Console.WriteLine("Diff found. Processing with AI...");
                    await RunAutoFlow();
                }
                else
                {
                    Console.WriteLine("Working directory is clean. Nothing to do.");
                }
            }
            catch (RepositoryNotFoundException ex)
            {
                Console.WriteLine($"Git Error: {ex.Message} (Not a Git repository)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred in InitialRun: {ex.Message}");
            }
        }

        /// <summary>
        /// Analyze the current working Directory, and suggest commit messages and branch names.
        /// </summary>
        public static async void RunAnalyzeCommand()
        {
            Console.WriteLine("--------------------------------------------------");
            
        }
        
        /// <summary>
        /// Stage the current working Directory. (git add .)
        /// </summary>
        public static void RunStageCommand()
        {
            try
            {
                Console.WriteLine("--------------------------------------------------");
               
                _service.Stage();

                Console.WriteLine("Directory staged.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            
        }
    }
}