using GenerativeAI;
using GenerativeAI.Types;
using static GitFlowAi.Constants.Constants;


namespace GitFlowAi.Models
{
    public class GeminiService
    {
        public readonly GoogleAi Client;
        
        public GeminiService(string apiKey)
        {
            if (apiKey == null) throw new ArgumentNullException(nameof(apiKey));
            Client = new GoogleAi(apiKey);
        }

        public async Task<GitDecision> GenerateDecision(string? diff)
        {
            string systemInstruction = SYSTEMINSTRUCTION;
            string userPrompt = $"Analyze the following unified diff and respond with the necessary JSON object:\n\n{diff}";
            
            try
            {
                var model = Client.CreateGenerativeModel("models/gemini-2.0-flash");
                var request = new GenerateContentRequest();

                var allModels = await Client.ListModelsAsync();

                Console.WriteLine($"Models: {allModels}");
                
                request.UseJsonMode<GitDecision>();
                request.AddText($"{systemInstruction}\n {userPrompt}");
                
                var response = await model.GenerateContentAsync<GitDecision>(request);
                
                Console.WriteLine(response);
                
                var jsonObject = response.ToObject<GitDecision>();
                
                if (jsonObject == null)
                {
                    return new GitDecision { 
                        Action = GitAction.Skip, 
                        CommitMessage = "ai-error", 
                        Explanation = "AI returned an empty response. Skipping action." 
                    };
                }

                return jsonObject;
            }
            catch (Exception e)
            {
                    Console.WriteLine("> Error: " + e.Message);
                    return new GitDecision { 
                        Action = GitAction.Skip, 
                        CommitMessage = "ai-error", 
                        Explanation = "AI returned an empty response. Skipping action." 
                    };
            }
        }
    }
}