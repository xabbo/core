
namespace Xabbo.Core.Tests;

public sealed class ConditionalTheory : TheoryAttribute
{
    public ConditionalTheory(string environmentVariable) {
        if (Environment.GetEnvironmentVariable(environmentVariable) is null) {
            Skip = $"Ignoring because the '{environmentVariable}' environment variable is not set.";
        }
    }
}
