using Amazon.CDK;

namespace Ghanavats.DotnetAws.IaC;

internal static class Program
{
    public static void Main(string[] args)
    {
        var app = new App();
        StackInitializer.Apply(app);
        app.Synth();
    }
}
