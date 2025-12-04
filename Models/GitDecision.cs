using System.CommandLine.Help;
using System.Text.Json.Serialization;
using Google.Protobuf.WellKnownTypes;

namespace GitFlowAi.Models
{
    public enum GitAction
    {
        [JsonPropertyName("SKIP")]
        SKIP, // No action needed or changes are too vague.
        
        [JsonPropertyName("COMMIT")]
        COMMIT, // Direct commit to current branch.
        
        [JsonPropertyName("BRANCH")]
        BRANCH // Create a new branch and commit to it.
    }

    public class GitDecision
    {
        [JsonPropertyName("action")]
        public GitAction Action { get; set; }

        // 2. The commit message (Required if Action is COMMIT or BRANCH)
        [JsonPropertyName("commitMessage")]
        public string CommitMessage { get; set; } = string.Empty;
        
        // 3. The branch name (Required if Action is BRANCH)
        // NOTE: This property must be non-static.
        [JsonPropertyName("branchName")]
        public string BranchName { get; set; } = string.Empty;

        // 4. Array of file paths included in the proposed changes/commit
        [JsonPropertyName("filePaths")]
        public List<string> FilePaths { get; set; } = new List<string>();

        // 5. Optional: A brief human-readable explanation of why the action was chosen.
        [JsonPropertyName("explanation")]
        public string Explanation { get; set; } = string.Empty;
    }
}

