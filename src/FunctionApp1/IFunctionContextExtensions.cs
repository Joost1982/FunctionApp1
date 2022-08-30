using BackendFunctionApp.Extensions;
using Microsoft.Azure.Functions.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp1;

public interface IExtensionMethodsWrapper
{
    public async Task CreateJsonResponse<T>(FunctionContext functionContext, System.Net.HttpStatusCode statusCode, T data) { }
}

public class ExtensionMethodsWrapper : IExtensionMethodsWrapper
{
    public async Task CreateJsonResponse<T>(FunctionContext functionContext, System.Net.HttpStatusCode statusCode, T data)
    {
        await functionContext.CreateJsonResponse(statusCode, data);
    }
}
