using LibGit2Sharp;

namespace GitFlowAi.Services
{
    public class GitService
    {
        private readonly Repository? _repository;
        
        public GitService(string workingDirectory)
        {
            // 1. Discover the repository path
            string? repoPath = Repository.Discover(workingDirectory);

            if (string.IsNullOrEmpty(repoPath))
            {
                throw new RepositoryNotFoundException($"Error: Not in a Git repository. Starting directory: {workingDirectory}");
            }

            Console.WriteLine($"Repository: {repoPath}");

            // 2. Initialize the LibGit2Sharp Repository object
            try
            {
                _repository = new Repository(repoPath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to open Git repository: {ex.Message}");
            }
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
                
                    // Patch unifiedPatch = _repository.Diff.Compare<Patch>();
                
                    // Console.WriteLine(unifiedPatch.Content);
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
        
    }
}