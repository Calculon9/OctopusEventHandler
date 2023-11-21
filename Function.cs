using Amazon.Lambda.Core;
using Newtonsoft.Json;
using System.Text;
// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace OctopusEventHandler;

public class Function
{
    /// <summary>
    /// Octopus Event handler
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task FunctionHandler(OctopusEvent input, ILambdaContext context)
    {
        //Create JSON data
        //var jsonData = JsonConvert.SerializeObject(input);
        var dateTime = input.Payload.Event.Occurred.ToString("f");
        var message = input.Payload.Event.Message;

        var text = $"Time: {dateTime}\nMessage: {message}";
        var slackText = new SlackText(text);
        var jsonData = JsonConvert.SerializeObject(slackText);
        var httpContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

        using (HttpClient client = new HttpClient())
        {
            var webhookUrl = "https://hooks.slack.com/services/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";

            try
            {
                var response = await client.PostAsync(webhookUrl, httpContent);
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Response Content: " + responseContent);
                }
                else
                {
                    Console.WriteLine("Error: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }
    }


}
public class OctopusEvent
{
    [JsonProperty("Payload")]
    public Payload Payload { get; set; }
}

public class Payload
{
    [JsonProperty("Event")]
    public OctopusEventInfo Event { get; set; }
}

public class OctopusEventInfo
{
    [JsonProperty("Occurred")]
    public DateTime Occurred { get; set; }
    [JsonProperty("Message")]
    public string Message { get; set; }
    [JsonProperty("MessageHtml")]
    public string MessageHtml { get; set; }
}

public class SlackText
{
    public SlackText(string _text)
    {
        text = _text;
    }
    public string text { get; set; }
}