using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using FunctionApp1.Tests.Mocks;

namespace FunctionApp1.Tests;

public class AuthenticationMiddlewareTests
{
    [Fact()]
    public async Task Invoke_WithInvalidAuthorizationHeader_FunctionContextHasUnauthorizedResponseCodeAndJsonResponse()
    {
        // Arrange
        var functionDefinition = new MockFunctionDefinition(new Dictionary<string, BindingMetadata>() { { "req", new MockBindingMetadata() } });
        var context = new MockFunctionContext(functionDefinition);
        FunctionExecutionDelegate functionExecutionDel = async (context) => await Task.FromResult(context);
     
        var middleWare = new AuthenticationMiddleware();

        // Act
        await middleWare.Invoke(context, functionExecutionDel);

        // Assert

        // extract response data from context... to do ('act' part throws exception, first fix that)
    }
}

