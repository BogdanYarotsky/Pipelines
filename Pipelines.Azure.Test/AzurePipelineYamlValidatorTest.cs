using Pipelines.Azure.Errors;

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
        Assert.IsInstanceOfType<EmptyYaml>(errors.Single());
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
        Assert.IsInstanceOfType<MissingTriggerSection>(errors.Single());
    }

    private static List<AzurePipelineYamlError> FindErrorsWithDefaultDevOpsConfiguration(string yaml) 
        => FindErrors(yaml, AzureDevOpsPipelineConfiguration.Default);
    
    private static List<AzurePipelineYamlError> FindErrors(string yaml, AzureDevOpsPipelineConfiguration config)
        => AzurePipelineYamlValidator.FindErrors(yaml, config).ToList();
}