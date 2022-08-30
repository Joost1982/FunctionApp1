using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.Functions.Worker.Converters;
using Microsoft.Extensions.DependencyInjection;


namespace BackendFunctionApp.Extensions;
    
public static class FunctionContextExtentions
{
    private static readonly List<string> FUNCS_EXCLUDED_FROM_MIDDLEWARE = new() { "RenderSwaggerUI", "RenderSwaggerDocument" };

    /// <summary>
    /// Check that current inputbinding or current endpoint function in HttpTrigger or not.
    /// </summary>
    /// <param name="functionContext"></param>
    /// <returns>return true or false.</returns>
    public static bool IsHttpTriggerFunction(this FunctionContext functionContext)
    {
        if (functionContext.FunctionDefinition.InputBindings != null &&
            functionContext.FunctionDefinition.InputBindings.ContainsKey("req") &&
            !FUNCS_EXCLUDED_FROM_MIDDLEWARE.Contains(functionContext.FunctionDefinition.Name))
        {
            var bindingMetaData = functionContext.FunctionDefinition.InputBindings["req"];
            return bindingMetaData.Direction == BindingDirection.In && "httpTrigger".Equals(bindingMetaData.Type, StringComparison.InvariantCultureIgnoreCase);

        }
        return false;
    }

    /// <summary>
    /// This method extract requestdata from functioncontext.
    /// https://github.com/Azure/azure-functions-dotnet-worker/issues/414
    /// </summary>
    /// <param name="functionContext"></param>
    /// <returns>return HttpRequestData.</returns>
    public static HttpRequestData GetHttpRequestData(this FunctionContext functionContext)
    {
        try
        {
            KeyValuePair<Type, object> keyValuePair = functionContext.Features.SingleOrDefault(f => f.Key.Name == "IFunctionBindingsFeature");
            object functionBindingsFeature = keyValuePair.Value;
            Type type = functionBindingsFeature.GetType();
            var inputData = type.GetProperties().Single(p => p.Name == "InputData").GetValue(functionBindingsFeature) as IReadOnlyDictionary<string, object>;
            return inputData?.Values.SingleOrDefault(o => o is HttpRequestData) as HttpRequestData;
        }
        catch
        {
            return null;
        }
    }

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

    /// <summary>
    /// Create response from function context and with specified object and status code.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="functionContext"></param>
    /// <param name="statusCode"></param>
    /// <param name="data"></param>
    /// <returns>return task.</returns>
    public static async Task CreateJsonResponse<T>(this FunctionContext functionContext, System.Net.HttpStatusCode statusCode, T data)
    {
        //var request = functionContext.GetHttpRequestData();
        // since Azure.Functions.Worker 1.8 part of Azure Function codebase, so custom extension method GetHttpRequestData() no longer needed
        var request = await functionContext.GetHttpRequestDataAsync(); 
        if (request != null)
        {
            var response = request.CreateResponse(statusCode);
            await response.WriteAsJsonAsync(data);
            response.StatusCode = statusCode;
            functionContext.SetResponseData(response);
        }
    }
}