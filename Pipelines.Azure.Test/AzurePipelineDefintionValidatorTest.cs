namespace Pipelines.Azure.Test;

[TestClass]
public class AzurePipelineYamlValidatorTest
{
    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("       \n")]
    public void NoYamlYieldsAnError(string yaml)
    {
        var errors = FindErrorsWithDefaultDevOpsConfiguration(yaml);
        Assert.AreEqual(1, errors.Count);
        Assert.IsInstanceOfType<AzurePipelineNoYaml>(errors.Single());
    }
    
    [TestMethod]
    public void YamlWithNoTriggerSectionAndDefaultDevOpsConfigMeansNoErrors()
    {
        const string yaml = """
                            pool:
                              vmImage: 'ubuntu-latest'
                            
                            steps:
                            - script: dotnet build
                            """;
        var errors = FindErrorsWithDefaultDevOpsConfiguration(yaml);
        Assert.AreEqual(0, errors.Count);
    }
    
    [TestMethod]
    public void YamlWithNoTriggerSectionAndDisableImpliedYamlCiTrigger1Error()
    {
        const string yaml = """
                                pool:
                                  vmImage: 'ubuntu-latest'

                                steps:
                                - script: dotnet build
                                """;
        
        var config = AzureDevOpsPipelineConfiguration.Default with
        {
            DisableImpliedYamlCiTrigger = true
        };
        
        var errors = FindErrors(yaml, config);
        Assert.AreEqual(1, errors.Count);
        Assert.IsInstanceOfType<AzurePipelineMissingTriggerSection>(errors.Single());
    }

    private static List<AzurePipelineYamlError> FindErrorsWithDefaultDevOpsConfiguration(string yaml) 
        => FindErrors(yaml, AzureDevOpsPipelineConfiguration.Default);
    
    private static List<AzurePipelineYamlError> FindErrors(string yaml, AzureDevOpsPipelineConfiguration config)
        => AzurePipelineYamlValidator.FindErrors(yaml, config).ToList();
}


public record AzureDevOpsPipelineConfiguration
{
    public bool DisableImpliedYamlCiTrigger { get; init; }
    private AzureDevOpsPipelineConfiguration() { }

    public static readonly AzureDevOpsPipelineConfiguration Default = new();
}

public static class AzurePipelineYamlValidator
{
    public static IEnumerable<AzurePipelineYamlError> FindErrors(
        string yaml, AzureDevOpsPipelineConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(yaml))
        {
            yield return new AzurePipelineNoYaml();
            yield break;
        }
        
        // todo - get first section
        
    }

}

public record AzurePipelineYamlError(string Message);

public record AzurePipelineNoYaml() : AzurePipelineYamlError(
    "There is nothing to validate. Null or empty string was passed to the validator");

public record AzurePipelineMissingTriggerSection() : AzurePipelineYamlError(
    "If in Azure DevOps it was configured for pipelines to require an explicit trigger defined in yaml...");
