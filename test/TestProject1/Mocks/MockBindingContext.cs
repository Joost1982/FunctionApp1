using Microsoft.Azure.Functions.Worker;

namespace FunctionApp1.Tests.Mocks;

public class MockBindingContext : BindingContext
{

    public MockBindingContext()
    {
        string header = "{\"Authorization\":\"WxLU3NF4CVoQpwVgjcGzPdrXMVYVZ3Bo\"}";
        BindingData = new Dictionary<string, object>() { { "Headers", header } };
    }

    public override IReadOnlyDictionary<string, object> BindingData { get;  }
}
