using GitFlowAi.Config;
using GitFlowAi.Models;
using GitFlowAi.Services;
using GitFlowAi.Utilities;
using GitFlowAi.Server;
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

                if (isDirty && !string.IsNullOrWhiteSpace(diff))
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
        public static async Task InitialRun()
        {
            try
            {
                Console.WriteLine("--------------------------------------------------");
                var manager = new SecretManager();
                string apiKey = manager.GetGeminiApiKey();
            
                // Console.WriteLine($"API Key Status: {(string.IsNullOrEmpty(apiKey) ? "MISSING" : "FOUND")}");
                
                var genClient = new GeminiService(apiKey);
            
                _service.PrintCurrentBranch();
            
                // Task.Run is used to ensure the LibGit2Sharp call is offloaded from the UI thread,
                (bool isDirty, string? diff) = await Task.Run(() => _service.GetRepoStatusAndDiff());
            
                if (isDirty)
                {
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
                            _service.CreateBranchAndCommit(response.BranchName, response.CommitMessage, response.FilePaths);
                            Console.WriteLine($"Commit: {response.CommitMessage}");
                            Console.WriteLine($"Reason: {response.Explanation}");
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
                    Console.WriteLine("Directory is Clean. No changes ware made.");
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
        /// Analyze the current working Directory, and suggest commit messages and branch names.
        /// </summary>
        public static async Task RunAnalyzeCommand()
        {
            var manager = new SecretManager();
            string apiKey = manager.GetGeminiApiKey();
            var genClient = new GeminiService(apiKey);
            
            string message = "------------------------------------------------------" + "\n" +
                             "|     Please select one of the options below.        |" + "\n" + 
                             "------------------------------------------------------";
            LineSeparator.Line();
            try
            {
                (bool isDirty, string? diff) = await Task.Run(() => _service.GetRepoStatusAndDiff());

                if (isDirty)
                {
                    string fileContent = File.ReadAllText($"{_workDirectory}/Templates/Analysis/AnalysisPreview.html");
                    
                    AiAnalysis aiAnalysis = await genClient.GetAnalysis(diff);
                    
                    await WebServer.StartServer(fileContent.Replace("{{aiResponse}}", aiAnalysis.FinalSummary));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            finally
            {
                LineSeparator.Line();
            }
        }
        
        /// <summary>
        /// Stage the current working Directory. (git add .)
        /// </summary>
        public static void RunStageCommand()
        {
            try
            {
                LineSeparator.Line();

                _service.Stage();

                Console.WriteLine("Directory staged.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            finally
            {
                LineSeparator.Line();
            }
            
        }
        
        public static void RunInitCommand()
        { 
            string configFolderName = Constants.Constants.ConfigFolderName;
            string configDir = Constants.Constants.ConfigDir;
            string configFilePath = Constants.Constants.ConfigFilePath;
            string readmeFilePath = Constants.Constants.ReadmeFilePath;
            string envFilePath = Constants.Constants.EnvFilePath;

            try
            {
                LineSeparator.Line();
                // Check if folder already exists, and create a new one if it doesn't. return a boolean for error.
                bool isDirNotExists = _service.CheckCurrentDirectory(configDir);

                if (isDirNotExists)
                {
                    string systemInstruction = Constants.Constants.SYSTEMINSTRUCTION;

                    Console.WriteLine(
                        $"Initializing a {configFolderName} folder in the current working directory. \n {configDir}");

                    _service.CreateFile(configFilePath);
                    _service.CreateFile(envFilePath);
                    _service.CreateFile(readmeFilePath, systemInstruction);
                    _service.PrintCurrentBranch();
                    _service.GetRepoStatus();
                }
                else
                {

                    _service.PrintCurrentBranch();
                    _service.GetRepoStatus();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            finally
            {
                LineSeparator.Line();
            }
            
           
        }
    }
}