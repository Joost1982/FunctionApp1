using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Moq;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Immutable;

namespace FunctionApp1.Tests;

public class AuthenticationMiddlewareTests
{
    private readonly AuthenticationMiddleware _sut;
    private readonly Mock<IExtensionMethodsWrapper> _extensionMethodsWrapper = new Mock<IExtensionMethodsWrapper>();

    public AuthenticationMiddlewareTests()
    {
        _sut = new AuthenticationMiddleware(_extensionMethodsWrapper.Object);
        _extensionMethodsWrapper
        .Setup(e => e.CreateJsonResponse(It.IsAny<FunctionContext>(), It.IsAny<HttpStatusCode>(), It.IsAny<It.IsAnyType>()));
    }

    private Mock<FunctionContext> CreateFunctionContextMock(string headerToken)
    {
        var context = new Mock<FunctionContext>();
        var bindingMetaData = new Mock<BindingMetadata>();
        bindingMetaData.Setup(b => b.Direction).Returns(BindingDirection.In);
        bindingMetaData.Setup(b => b.Name).Returns("req");
        bindingMetaData.Setup(b => b.Type).Returns("httpTrigger");
        var functionDefinition = new Mock<FunctionDefinition>();
        functionDefinition.Setup(f => f.InputBindings).Returns(
                   new Dictionary<string, BindingMetadata>() { { "req", bindingMetaData.Object } }.ToImmutableDictionary());
        context.Setup(c => c.FunctionDefinition).Returns(functionDefinition.Object);

        var bindingContext = new Mock<BindingContext>();
        string header = "{\"Authorization\":\"" + headerToken + "\"}";
        bindingContext.Setup(b => b.BindingData).Returns(new Dictionary<string, object>() { { "Headers", header } });
        context.Setup(c => c.BindingContext).Returns(bindingContext.Object);

        return context;
    } 

    [Fact]
    public async Task Invoke_WithInvalidAuthorizationHeader_InvokesCreateJsonResponseWithUnauthorizedSettings()
    {
        // Arrange
        var context = CreateFunctionContextMock("blabla"); // invalid header
        FunctionExecutionDelegate functionExecutionDel = async (context) => await Task.FromResult(context);

        // Act
        await _sut.Invoke(context.Object, functionExecutionDel);

        // Assert
        _extensionMethodsWrapper.Verify(e => e.CreateJsonResponse(
                It.Is<FunctionContext>(s => s.Equals(s)),
                It.Is<HttpStatusCode>(s => s.Equals(HttpStatusCode.Unauthorized)),
                It.Is<Object>(s => JsonConvert.SerializeObject(s).Equals("{\"Message\":\"Token is not valid\"}"))), Times.Once);
    }

    [Fact]
    public async Task Invoke_WithValidAuthorizationHeader_DoesNotInvokeCreateJsonResponse()
    {
        // Arrange
        var context = CreateFunctionContextMock("foo"); // valid header
        FunctionExecutionDelegate functionExecutionDel = async (context) => await Task.FromResult(context);

        // Act
        await _sut.Invoke(context.Object, functionExecutionDel);

        // Assert
        _extensionMethodsWrapper.Verify(e => e.CreateJsonResponse(
                It.Is<FunctionContext>(s => s.Equals(s)),
                It.Is<HttpStatusCode>(s => s.Equals(HttpStatusCode.Unauthorized)),
                It.Is<Object>(s => JsonConvert.SerializeObject(s).Equals("{\"Message\":\"Token is not valid\"}"))), Times.Never);
    }
}

