
public class Agent
{
    public string Model;
    public string Prompt;
    public string SystemPrompt;

    public Agent(string model, string prompt, string systemPrompt)
    {
        Model = model; 
        Prompt = prompt;
        SystemPrompt = systemPrompt;
    }
};