using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FunctionApp1;

public class Program
{
    public static void Main()
    {
        var host = new HostBuilder()
            //.ConfigureFunctionsWorkerDefaults()   // without Middleware
            .ConfigureFunctionsWorkerDefaults(configure =>
            {
                configure.UseMiddleware<AuthenticationMiddleware>();
            })
            .ConfigureServices(services =>
            {
                services.AddScoped<IExtensionMethodsWrapper, ExtensionMethodsWrapper>();
            })
            .Build();

        host.Run();
    }
}