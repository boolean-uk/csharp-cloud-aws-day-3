using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System.Net;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWSLambda_Day_3;

public class Function
{
    /// <summary>
    /// Lambda function to handle API requests.
    /// </summary>
    /// <param name="input">The API Gateway request.</param>
    /// <param name="context">Lambda execution context.</param>
    /// <returns>API Gateway response.</returns>
    public string FunctionHandler(ILambdaContext context)
    {
        return "Hello Nigel";
    }
}
