using GenerativeAI
public class Agent
{
    private string Model { get; set; }
    public string Prompt;
    public string SystemPrompt;

     public Agent( string prompt, string systemPrompt)
    {
        Prompt = prompt ?? throw new ArgumentNullException(nameof(prompt));
        SystemPrompt = systemPrompt ?? throw new ArgumentNullException(nameof(systemPrompt));
    }

    public string GetModel()
    {
        return Model;
    }

    public void SetModel(string value)
    {
        Model = value;
    }
};