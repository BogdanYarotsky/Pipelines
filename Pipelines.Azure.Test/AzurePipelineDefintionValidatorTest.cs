namespace Pipelines.Azure.Test;

[TestClass]
public class AzurePipelineYmlValidatorTest
{
    [TestMethod]
    public void ValidSimpleYmlProducesNoErrors()
    {
        const string validYml = """
        trigger:
        - main
        
        pool:
          vmImage: 'ubuntu-latest'
        
        steps:
        - script: dotnet build
        """;
        var errors = FindErrorsWithDefaultDevOpsConfiguration(validYml);
        Assert.AreEqual(0, errors.Count);
    }

    private static List<AzurePipelineYmlError> FindErrorsWithDefaultDevOpsConfiguration(string yaml) 
        => AzurePipelineYmlValidator.FindErrors(yaml, AzureDevOpsPipelineConfiguration.Default).ToList();
}


public class AzureDevOpsPipelineConfiguration
{
    private AzureDevOpsPipelineConfiguration()
    {
    }

    public static AzureDevOpsPipelineConfiguration Default = new AzureDevOpsPipelineConfiguration();
}

public static class AzurePipelineYmlValidator
{
    public static IEnumerable<AzurePipelineYmlError> FindErrors(string yaml, AzureDevOpsPipelineConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(yaml))
        {
            yield return AzurePipelineYmlError.NoYaml();
            yield break;
        }
        
        
    }

}

public record AzurePipelineYmlError(string Message)
{
    // todo - simple hierarchy?
    public static AzurePipelineYmlError NoYaml() => new AzurePipelineYmlError("empty");


};
