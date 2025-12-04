using Mscc.GenerativeAI;
using System.Text.Json.Serialization;
using static GitFlowAi.Constants.Constants;


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
    
    public class GeminiService
    {
        public readonly GoogleAI Client;
        public readonly string AiModel = Model.Gemini20Flash;
        
        public  GeminiService(string apiKey)
        {
            Client = new GoogleAI(apiKey);
        }

        public async Task<string> GenerateDecision(string? diff)
        {
            // string systemInstruction = SYSTEMINSTRUCTION;
            string userPrompt = $"Analyze the following unified diff and respond with the necessary JSON object:\n\n{diff}";
            
            try
            {
                var model = Client.GenerativeModel(
                    model: AiModel
                );

                var response = await Task.Run(() => model.GenerateContent(userPrompt));
                Console.WriteLine(response);
                
                return response.Text ?? "Sorry, i can't response now.";
            }
            catch (Exception e)
            {
                    Console.WriteLine(e.Message);
                    return $"{e.Message}";
            }
        }
    }
}