namespace Pipelines.Azure.Test;

[TestClass]
public class AzurePipelineValidationTest
{
    [TestMethod]
    public void InvalidAzurePipelineDefinitionThrowsError()
    {
        var invalidYaml = "Hello world!!!";
        
        Assert.ThrowsException<AzurePipelineInvalidDefinitionException>(
            () => AzurePipeline.Execute(invalidYaml));
    }
}

public class AzurePipelineInvalidDefinitionException : Exception
{
}

public static class AzurePipeline
{
    public static void Execute(string yaml)
    {
        throw new AzurePipelineInvalidDefinitionException();
    }
}