using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EsepWebhook;

public class Function
{
    private static readonly HttpClient client = new HttpClient();
    
    public async Task<string> FunctionHandler(object input, ILambdaContext context)
    {
        context.Logger.LogInformation($"FunctionHandler received: {input}");
        
        var json = JsonSerializer.Deserialize<JsonElement>(input.ToString());
        
        string issueUrl = json.GetProperty("issue").GetProperty("html_url").GetString();
        string payload = $"{{\"text\":\"Issue Created: {issueUrl}\"}}";

        var webRequest = new HttpRequestMessage(HttpMethod.Post, Environment.GetEnvironmentVariable("SLACK_URL"))
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };
    
        var response = await client.SendAsync(webRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
            
        return responseContent;
    }
}