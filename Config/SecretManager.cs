namespace GitFlowAi.Config
{
    public class SecretManager
    {
        private const string ApiKeyVariableName = "GEMINI_API_KEY";

        public string GetGeminiApiKey()
        {
            // Environment.GetEnvironmentVariable is the standard way to read environment variables in .NET
            string? apiKey = Environment.GetEnvironmentVariable(ApiKeyVariableName);

            // if (string.IsNullOrEmpty(apiKey))
            // {
            //     Console.WriteLine($"\n🚨 CRITICAL ERROR: The '{ApiKeyVariableName}' environment variable is not set.");
            //     Console.WriteLine("Please set it in your ~/.bashrc or ~/.zshrc file to proceed.");
            //     Console.WriteLine($"Please run (export {ApiKeyVariableName}='Your_Api_Key').");
            //     throw new InvalidOperationException($"API Key '{ApiKeyVariableName}' is missing.");
            // }

            return "AIzaSyCWtuh49iVezvkbAxh7cZnpS_U60ud-79Q";
        }
    }
}