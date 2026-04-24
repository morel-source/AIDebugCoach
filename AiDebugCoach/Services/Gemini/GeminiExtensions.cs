using AiDebugCoach.Configuration;
using GenerativeAI;

namespace AiDebugCoach.Services.Gemini;

public static class GeminiExtensions
{
    public static void AddGemini(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IAiService, GeminiService>();

        builder.Services.Configure<GeminiOptions>(
            builder.Configuration.GetSection(GeminiOptions.SectionName));

        var options = builder.Configuration.GetSection(GeminiOptions.SectionName).Get<GeminiOptions>();

        if (string.IsNullOrEmpty(options?.ApiKey))
            throw new Exception("Gemini API Key is missing!");

        builder.Services.AddSingleton(new GenerativeModel(model: options.ModelName!, apiKey: options.ApiKey));
    }
}