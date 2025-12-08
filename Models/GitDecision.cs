using System.Text.Json.Serialization;
using Json.Schema.Generation;

namespace GitFlowAi.Models
{
    public enum GitAction
    {
        [JsonPropertyName("SKIP")]
        Skip, // No action needed or changes are too vague.
        
        [JsonPropertyName("COMMIT")]
        Commit, // Direct commit to current branch.
        
        [JsonPropertyName("BRANCH")]
        Branch // Create a new branch and commit to it.
    }

    public class GitDecision
    {
        [JsonPropertyName("action")]
        [Required]
        public GitAction Action { get; set; }

        // The commit message (Required if Action is COMMIT or BRANCH)
        [JsonPropertyName("commitMessage")]
        public string CommitMessage { get; set; } = string.Empty;
        
        // The branch name (Required if Action is BRANCH)
        [JsonPropertyName("branchName")]
        public string BranchName { get; set; } = string.Empty;

        // Array of file paths included in the proposed changes/commit
        [JsonPropertyName("filePaths")]
        public List<string> FilePaths { get; set; } = new List<string>();

        // Optional: A brief human-readable explanation of why the action was chosen.
        [JsonPropertyName("explanation")]
        public string Explanation { get; set; } = string.Empty;
    }

    public enum RiskAssessment
    {
        [JsonPropertyName("LOW")]
        Low,
        
        [JsonPropertyName("MEDIUM")]
        Medium,
        
        [JsonPropertyName("HIGH")]
        High,
    }

    public record FilePath
    {
        [JsonPropertyName("path")] 
        public string Path { get; set; } = string.Empty;
        
        [JsonPropertyName("analysisReport")] 
        public string AnalysisReport { get; set; } = string.Empty;
        
        [JsonPropertyName("riskAssessment")]
        public RiskAssessment RiskAssessment { get; set; }

        [JsonPropertyName("suggestions")][Description("A list of suggestions if necessary")]
        public List<string>? Suggestion { get; set; } = new List<string>();
    }

    public class AiAnalysis
    {
        [JsonPropertyName("analysisResults")] 
        public List<FilePath> AnalysisResults { get; set; } = new List<FilePath>();
        
        [JsonPropertyName("finalSummary")]
        public string FinalSummary { get; set; } = string.Empty;
    }
}

