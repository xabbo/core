namespace Xabbo.Core.Tests;

public sealed class ConditionalFact : FactAttribute
{
    public ConditionalFact(string environmentVariable) {
        if (Environment.GetEnvironmentVariable(environmentVariable) is null) {
            Skip = $"Ignoring because the '{environmentVariable}' environment variable is not set.";
        }
    }
}
