using GitFlowAi.Services;

namespace GitFlowAi.Config
{
    public class SecretManager
    {
        static readonly string CurrentWorkingDirectory = Directory.GetCurrentDirectory();
        private const string ApiKeyVariableName = "GEMINI_API_KEY";

        public string GetGeminiApiKey()
        {
            string? apiKey = Environment.GetEnvironmentVariable(ApiKeyVariableName);

            if (string.IsNullOrEmpty(apiKey))
            {
                apiKey = GetGeminiEnvironmentVariables();
                if (!string.IsNullOrEmpty(apiKey))
                {
                    return apiKey;
                }
            }
            
            Console.WriteLine("Please set it in your ~/.bashrc or ~/.zshrc file to proceed.");
            Console.WriteLine($"Please run (export {ApiKeyVariableName}='Your_Api_Key') or \n add the key to the env in the .gitflowai directory");
            throw new InvalidOperationException($"API Key '{ApiKeyVariableName}' is missing.");
        }
        
        private string GetGeminiEnvironmentVariables()
        {
            GitService service = new GitService(CurrentWorkingDirectory);
            string[]? envVariables = service.GetEnvVariables();
            string env = "";

            if (envVariables == null) return "";
            
            foreach (var variable in envVariables)
            {
                if (variable.StartsWith(ApiKeyVariableName))
                {
                    env = variable.Split("=").Length > 0 ? variable.Split("=").Last().Trim() : "";
                    if (env.StartsWith('"'))
                    {
                        Console.WriteLine($"Env variable cannot start with a double quotes, \n {env}");
                        env = "";
                    }
                }
            }

            return env;
        }
    }
}