using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Moq;
using System.Net;

namespace FunctionApp1.Tests
{
    public class Function1Tests
    {
        [Fact()]
        public async Task Run_GetRequest_ReturnsOkMessageAndStatusCode()
        {
            // Arrange
            var context = new Mock<FunctionContext>();
            var request = new Mock<HttpRequestData>(context.Object);
            request.Setup(r => r.CreateResponse()).Returns(() =>
            {
                var response = new Mock<HttpResponseData>(context.Object);
                response.SetupProperty(r => r.StatusCode, HttpStatusCode.BadRequest); // deliberately different from OK
                response.SetupProperty(r => r.Headers, new HttpHeadersCollection());
                response.SetupProperty(r => r.Body, new MemoryStream());
                return response.Object;
            });
            
            var function = new Function1();

            // Act
            HttpResponseData response = function.Run(request.Object, context.Object);
            response.Body.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(response.Body);
            var responseBody = await reader.ReadToEndAsync();
            
            // Assert
            Assert.Equal("Welcome to Azure Functions!", responseBody);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}