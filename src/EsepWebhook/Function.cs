using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EsepWebhook;

public class Function
{   
    public string FunctionHandler(object input, ILambdaContext context)
    {   
        var json = JsonSerializer.Deserialize<JsonElement>(input.ToString());
        
        string issue = json.GetProperty("issue").GetProperty("html_url").GetString();
        string payload = $"{{\"text\":\"🙂 Issue Created: {issue}\"}}";

        var client = new HttpClient();
        var webRequest = new HttpRequestMessage(HttpMethod.Post, Environment.GetEnvironmentVariable("SLACK_URL"))
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };
    
        var response = client.Send(webRequest);
        using var reader = new StreamReader(response.Content.ReadAsStream());
            
        return reader.ReadToEnd();
    }
}