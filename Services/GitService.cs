using LibGit2Sharp;
using GitFlowAi.Utilities;

namespace GitFlowAi.Services
{
    public class GitService
    {
        private readonly Repository? _repository;
        
        public GitService(string workingDirectory)
        {
            string? repoPath = Repository.Discover(workingDirectory);

            if (string.IsNullOrEmpty(repoPath))
            {
                throw new RepositoryNotFoundException($"Error: Not in a Git repository. Starting directory: {workingDirectory}");
            }
            
            try
            {
                _repository = new Repository(repoPath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to open Git repository: {ex.Message}");
            }
        }
        
        private Signature? GetDefaultSignature()
        {
            if (_repository == null)
            {
                Console.WriteLine("No Git repository found.");
                return null;
            }
            // Try to build the signature from the repository's configuration
            Signature configSignature = _repository.Config.BuildSignature(DateTimeOffset.Now);

            if (configSignature != null)
            {
                Console.WriteLine($"Using Git config signature: {configSignature.Name} <{configSignature.Email}>");
                return configSignature;
            }
            
            // Fallback if configuration is missing or invalid
            string fallbackName = "GitFlowAI Automaton";
            string fallbackEmail = "ai@gitflow.com";
            
            Console.WriteLine($"Could not read user configuration. Using fallback signature: {fallbackName} <{fallbackEmail}>");
            return new Signature(fallbackName, fallbackEmail, DateTimeOffset.Now);
        }
        
        public void PrintCurrentBranch()
        {
            if (_repository != null)
            {
                Console.WriteLine($"Current Branch: {_repository.Head.FriendlyName}");
            }
        }

        public  (bool IsDirty, string? Diff) GetRepoStatusAndDiff()
        {
            RepositoryStatus status = _repository.RetrieveStatus();

            if (!status.IsDirty || _repository == null)
            {
                return (true, string.Empty);
            }
            
            Patch unifiedPatch = _repository.Diff.Compare<Patch>();
            
            return (true, unifiedPatch);
        }

        public void GetRepoStatus()
        {
            if ( _repository != null)
            {
                RepositoryStatus status = _repository.RetrieveStatus();

                if (status.IsDirty)
                {
                    Console.WriteLine("Changes were made on working directory");
                }
                else
                {
                    Console.WriteLine("Directory is Clean");
                    Console.WriteLine("No changes were made");
                }
            }
            else
            {
                Console.WriteLine("Failed to open Git repository.");
            }
        }

        public void CommitChanges(string message, List<string> filePaths)
        {
            if(_repository == null) return;
            
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Commit message cannot be empty.");
            }

            List<string> options = new List<string>();
            options.Add("Proceed");
            options.Add("Exit (Esc)");

            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("|     Please select one of the options below.        |");
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine(" ");
            Console.WriteLine($"are you sure you want to commit changes using this message? ({message})");

            int selectedIndex = TerminalPrompt.PromptForSelection(options);
            string selectedOption = options[selectedIndex];

            if (selectedOption == "Proceed")
            {
                AddFiles(filePaths);
            
                Signature? signature = GetDefaultSignature();

                if (signature == null) return;
                
                Signature author = signature;
                Signature committer = signature;

                _repository.Commit(message, author, committer);
            
                Console.WriteLine($"Successfully committed {filePaths.Count} files.");
            }
            else
            {
                Console.WriteLine("Process terminated.");
            }
        }

        public void AddFiles(List<string> filePaths)
        {
            if(_repository == null) return;

            if (filePaths.Count == 0)
            {
                Stage();
            }
            else
            {
                Console.WriteLine("Adding path...");
                foreach (string path in filePaths)
                {
                    _repository.Index.Add(path);
                    Console.WriteLine($"Path: {path}");
                }
                _repository.Index.Write();
            }
            Console.WriteLine("--------------------------------------------------");
        }

        public async void Stage()
        {
            (bool isDirty, string? diff) = await Task.Run(() => GetRepoStatusAndDiff());
                
            if (isDirty && !string.IsNullOrWhiteSpace(diff))
            {
                Commands.Stage(_repository, "*");
            }
            else
            {
                Console.WriteLine("Working directory is clean. Nothing to do.");
            }
        }

        public Branch? CreateBranch(string branchName)
        {
            if (_repository == null) return null;
            Branch newBranch = _repository.Branches.Add(branchName, _repository.Head.Tip);
            Console.WriteLine($"   -> Created new branch: {branchName}");
            return newBranch;
        }

        public void CheckoutBranch(Branch newBranch)
        {
            if(_repository == null) return;
            
            Commands.Checkout(_repository, newBranch);
            Console.WriteLine($"   -> Switched to branch: {newBranch.FriendlyName}");
        }

        /// <summary>
        /// Creates a new branch, checks it out, stages the specified files, and commits.
        /// This method is triggered when AI recommends the 'BRANCH' action.
        /// </summary>
        public async void CreateBranchAndCommit(string branchName, string commitMessage, List<string> filePaths)
        {
            if(_repository == null) return;
            if (string.IsNullOrWhiteSpace(branchName) || string.IsNullOrWhiteSpace(commitMessage))
            {
                throw new ArgumentException("Branch name and commit message cannot be empty for BRANCH action.");
            }
            
            if (branchName.Contains(' '))
            {
                throw new ArgumentException($"Invalid branch name '{branchName}'. Use kebab-case (e.g., feature/new-task).");
            }
            
            Branch? newBranch = await Task.Run(() => CreateBranch(branchName));

            if (newBranch == null) return;
            
            await Task.Run(() => CheckoutBranch(newBranch));
           
            CommitChanges(commitMessage, filePaths);
        }
        
        public bool CheckCurrentDirectory(string dir)
        {
            try
            {
                if (!Directory.Exists(dir))
                {
                    Console.WriteLine($"Directory {dir} does not exist.");
                    Console.WriteLine("Creating Directory...");
                    Directory.CreateDirectory(dir);
                    return true;
                }
                else
                {
                    Console.WriteLine($"Directory {dir} already exists.");
                    return false;
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        
        public void CreateFile(string path, string? content = "")
        {
            try
            {
                Console.WriteLine($"Creating Config file...\n {path}"); 
                File.WriteAllText(path, content);
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public string[]? GetEnvVariables()
        {
            if (_repository == null) return null;
            
            string configFolderName = ".gitflowai";
            string envFileName = ".env";
            string currentWorkingDirectory = Directory.GetCurrentDirectory();
            string envPath = Path.Combine(currentWorkingDirectory, configFolderName, envFileName);
            
            try
            {
                List<string> envs = new List<string>();
                
                string configEnvContent = File.ReadAllText(envPath);

                if (!string.IsNullOrEmpty(configEnvContent))
                {
                    foreach (string env in configEnvContent.Split("\n"))
                    {
                        envs.Add(env.Trim());
                    }
                    return envs.ToArray();
                }
                else
                {
                    return null;
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        
    }
}