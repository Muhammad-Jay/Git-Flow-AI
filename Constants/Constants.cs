namespace GitFlowAi.Constants
{
    public static class Constants
    {
        public static readonly string SYSTEMINSTRUCTION = "You are **GitFlowAI**, an expert Git analyst. Your task is to analyze the provided unified code diff and decide on the next Git action. \n" +
                                                " Based ONLY on the diff, determine if the changes warrant a new feature branch, a commit (and generate a concise, conventional commit message) \n, " +
                                                "or if they should be stashed. Respond ONLY with a JSON object that adheres to the following C# class structure for direct deserialization: \n " +
                                                "public class AIDecision { public string Action { get; set; } // e.g., 'Commit', 'NewBranch', 'Stash' public string Message { get; set; } // Commit message or reason public string? SuggestedBranchName { get; set; } }";

        public const string SystemAnalysisReportInstructions = "You're a sophisticated code Review and Analysis AI, your purpose is to access the provided Git Diff and generate a detailed, strucured analysis report in JSON format." +
                                                               "Do Not take action; only analyze";
        
        public static readonly string WorkDirectory = Directory.GetCurrentDirectory();
        public static readonly string ConfigFolderName = ".gitflowai";
        public static readonly string ConfigFileName = ".config";
        public static readonly string ReadmeFileName = "GITFLOWAI.md";
        public static readonly string EnvFileName = ".env";
        
        public static readonly string ConfigDir = Path.Combine(WorkDirectory, ConfigFolderName);
        public static readonly string ConfigFilePath = Path.Combine(ConfigDir, ConfigFileName);
        public static readonly string ReadmeFilePath = Path.Combine(ConfigDir, ReadmeFileName);
        public static readonly string EnvFilePath = Path.Combine(ConfigDir, EnvFileName);

        public static string HtmlTemplate = @"";
    }
}