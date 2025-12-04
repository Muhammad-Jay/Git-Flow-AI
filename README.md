# GitFlowAI
The Intelligent Assistant for Consistent Git Workflows.

GitFlowAI is a powerful, opinionated .NET Global Tool designed to eliminate the friction and guesswork from your daily Git routine. It acts as an expert pair programmer, applying the analytical capabilities of generative AI directly to your uncommitted code changes to recommend the optimal next action.

### The Goal
The primary goal of this tool is to inject consistency and intelligence into the crucial commit stage loop. Manual decisions whether to create a new branch, what to name a commit, or if changes should be deferred are slow, subjective, and prone to error.

### GitFlowAI provides three core values:

**Automated Consistency**: It analyzes the scope and intent of your unified diff to automatically generate concise, standards-compliant Conventional Commit messages. This enforces clean history logs without manual effort.

**Workflow Guidance**: Based on the complexity and impact of the changes, the AI suggests whether the current delta warrants a new feature branch, a quick fix commit, or if it's best placed in the stash.

**Enhanced Efficiency**: By streamlining decision-making and ensuring your commits are high-quality from the start, it allows you to maintain your flow state and keep your focus on coding, not on tooling overhead.

### How It Works
The tool reads the output of your local git diff. This raw data is passed to the Gemini API, which processes the contextual code changes. Utilizing forced-structured output, the AI returns a single, actionable JSON object containing a clean action (Commit, NewBranch, Stash) and the corresponding message or reason.

This is intelligent automation designed to enforce best practices and save developer time.

### Getting Started
The tool requires a valid API key to function. We recommend setting this as an environment variable named GEMINI_API_KEY for secure access.

```
# Installation (will be updated once published to NuGet)
dotnet tool install -g GitFlowAi.Cli

# Ensure your API key is set
export GEMINI_API_KEY="YOUR_KEY"

# Analyze your current changes
gitflowai analyze
```