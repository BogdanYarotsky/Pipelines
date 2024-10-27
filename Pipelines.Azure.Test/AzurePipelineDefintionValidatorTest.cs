namespace Pipelines.Azure.Test;

[TestClass]
public class AzurePipelineYmlValidatorTest
{
    [TestMethod]
    public void InvalidAzurePipelineDefinitionThrowsError()
    {
        const string invalidYml = "Hello world!!!";
        var errors = AzurePipelineYmlValidator.FindErrors(invalidYml).ToList();
        Assert.AreNotEqual(0, errors.Count);
    }
}


public static class AzurePipelineYmlValidator
{
    public static IEnumerable<AzurePipelineYmlError> FindErrors(string yaml)
    {
        yield return new AzurePipelineYmlError();
    }
}

public record AzurePipelineYmlError();
