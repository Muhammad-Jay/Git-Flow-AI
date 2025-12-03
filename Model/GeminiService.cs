using Mscc.GenerativeAI;
using System;


namespace GitFlowAi.Services
{
    public class GeminiService
    {
        public readonly GoogleAI Client;
        public readonly string AiModel = Model.Gemini20Flash;
        
        public  GeminiService(string apiKey)
        {
            Client = new GoogleAI(apiKey);
        }

        public async Task<string> GenerateDecision(string diff)
        {
            string systemInstruction = "You are GitFlowAI, an expert Git analyst. Your task is to analyze the provided unified code diff and decide on the next Git action. Based ONLY on the diff, determine if the changes warrant a new feature branch, a commit (and generate a concise, conventional commit message), or if they should be stashed. Respond ONLY with a JSON object that adheres to the following C# class structure for direct deserialization: public class AIDecision { public string Action { get; set; } // e.g., 'Commit', 'NewBranch', 'Stash' public string Message { get; set; } // Commit message or reason public string? SuggestedBranchName { get; set; } }";
            
            string userPrompt = $"Analyze the following unified diff and respond with the necessary JSON object:\n\n{diff}";

            var model = Client.GenerativeModel(
                model: AiModel
            );

            var response = await model.GenerateContent(prompt: userPrompt);
            
            Console.WriteLine(response.Text);
            return response.Text ?? "";
        }
    }
}