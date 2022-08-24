using Microsoft.Azure.Functions.Worker;

namespace FunctionApp1.Tests.Mocks;
public class MockBindingMetadata : BindingMetadata
{
    public MockBindingMetadata(string type, BindingDirection direction, string name)
    {
        Type = type;
        Direction = direction;
        Name = name;
    }

    public MockBindingMetadata()
    {
        Type = "httpTrigger";
        Direction = BindingDirection.In;
        Name = "req";
    }

    public override string Type { get; }

    public override BindingDirection Direction { get; }

    public override string Name { get; }
}
