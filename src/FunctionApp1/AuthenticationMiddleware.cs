using System.Linq;
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using BackendFunctionApp.Extensions;

namespace FunctionApp1;

public class AuthenticationMiddleware : IFunctionsWorkerMiddleware
{

    private readonly IExtensionMethodsWrapper _extensionMethodsWrapper;

    public AuthenticationMiddleware(IExtensionMethodsWrapper extensionMethodsWrapper)
    {
        _extensionMethodsWrapper = extensionMethodsWrapper;
    }
    
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        string headers = context.BindingContext.BindingData["Headers"]?.ToString();
        var httpHeaders = headers == null ? null : System.Text.Json.JsonSerializer.Deserialize<HttpHeaders>(headers);

        if (httpHeaders.Authorization == "foo")
            await next(context);
        else
        {
            await _extensionMethodsWrapper.CreateJsonResponse(context, HttpStatusCode.Unauthorized, new { Message = "Token is not valid" });
        }
    }    
}


public class HttpHeaders
{
    public virtual string Authorization { get; set; }
}

public static class FunctionContextExtentions
{
    /// <summary>
    /// Set response data for request.
    /// https://github.com/Azure/azure-functions-dotnet-worker/issues/414
    /// </summary>
    /// <param name="functionContext"></param>
    /// <param name="responseData"></param>
    public static void SetResponseData(this FunctionContext functionContext, HttpResponseData responseData)
    {
        var feature = functionContext.Features.FirstOrDefault(f => f.Key.Name == "IFunctionBindingsFeature").Value;
        if (feature == null) throw new Exception("Required binding feature is not present.");
        var pinfo = feature.GetType().GetProperty("InvocationResult");
        pinfo.SetValue(feature, responseData);
    }
}