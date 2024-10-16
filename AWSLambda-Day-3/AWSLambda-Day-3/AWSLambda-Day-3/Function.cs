using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System.Net;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWSLambda_Day_3;

public class Function
{
    public string FunctionHandler(ILambdaContext context)
    {
        return "Meow meow øyvind";
    }
    /// <summary>
    /// Lambda function to handle API requests.
    /// </summary>
    /// <param name="input">The API Gateway request.</param>
    /// <param name="context">Lambda execution context.</param>
    /// <returns>API Gateway response.</returns>
    /*public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
    {
        string method = input.HttpMethod.ToUpper();
        string responseBody = string.Empty;
        int statusCode = (int)HttpStatusCode.OK;

        switch (method)
        {
            case "GET":
                // Handle GET request
                string path = input.PathParameters != null && input.PathParameters.ContainsKey("name")
                    ? input.PathParameters["name"]
                    : "world";

                responseBody = $"Hello, {path}!";
                break;

            case "POST":
                // Handle POST request
                if (!string.IsNullOrEmpty(input.Body))
                {
                    try
                    {
                        var data = JsonSerializer.Deserialize<Dictionary<string, string>>(input.Body);
                        if (data != null && data.ContainsKey("message"))
                        {
                            responseBody = $"You sent: {data["message"]}";
                        }
                        else
                        {
                            statusCode = (int)HttpStatusCode.BadRequest;
                            responseBody = "Invalid request body.";
                        }
                    }
                    catch (JsonException)
                    {
                        statusCode = (int)HttpStatusCode.BadRequest;
                        responseBody = "Error parsing JSON.";
                    }
                }
                else
                {
                    statusCode = (int)HttpStatusCode.BadRequest;
                    responseBody = "Request body is empty.";
                }
                break;

            default:
                statusCode = (int)HttpStatusCode.MethodNotAllowed;
                responseBody = "Method not supported.";
                break;
        }

        return new APIGatewayProxyResponse
        {
            StatusCode = statusCode,
            Body = responseBody,
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        };
    }
    */

}