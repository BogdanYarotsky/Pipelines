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
        var errors = FindErrorsWithDefaultConfiguration(yaml);
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
        var errors = FindErrorsWithDefaultConfiguration(yaml);
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
        
        var config = AzurePipelineConfiguration.Default with
        {
            DisableImpliedYamlCiTrigger = true
        };
        
        var errors = FindErrors(yaml, config);
        Assert.AreEqual(1, errors.Count);
        Assert.IsInstanceOfType<MissingTriggerSection>(errors.Single());
    }
    
    [TestMethod]
    public void ValidTriggerSection1()
    {
        const string yaml = """
                            trigger:
                            - main
                            
                            pool:
                              vmImage: 'ubuntu-latest'

                            steps:
                            - script: dotnet build
                            """;
        
        var errors = FindErrorsWithDefaultConfiguration(yaml);
        Assert.AreEqual(0, errors.Count);
    }
    
    [TestMethod]
    public void ValidTriggerSection2()
    {
        const string yaml = """
                            trigger: none

                            pool:
                              vmImage: 'ubuntu-latest'

                            steps:
                            - script: dotnet build
                            """;
        
        var errors = FindErrorsWithDefaultConfiguration(yaml);
        Assert.AreEqual(0, errors.Count);
    }
    
    [TestMethod]
    public void ValidTriggerSection3()
    {
        const string yaml = """
                            trigger:
                              batch: true
                              branches:
                                include:
                                - main
                                - releases/*
                                exclude:
                                - releases/old*
                              paths:
                                include:
                                - src/*
                                exclude:
                                - docs/*
                                - '*.md'
                              tags:
                                include:
                                - v2.*
                                exclude:
                                - v2.0.*

                            pool:
                              vmImage: 'ubuntu-latest'

                            steps:
                            - script: dotnet build
                            """;
        
        var errors = FindErrorsWithDefaultConfiguration(yaml);
        Assert.AreEqual(0, errors.Count);
    }
    
    [TestMethod]
    public void YamlWithInvalidTriggerSection1()
    {
        const string yaml = """
                            trigger: nine

                            pool:
                              vmImage: 'ubuntu-latest'

                            steps:
                            - script: dotnet build
                            """;
        
        var errors = FindErrorsWithDefaultConfiguration(yaml);
        AssertSingleErrorOfType<InvalidTriggerDefinition>(errors);
    }
    
    private static void AssertSingleErrorOfType<T>(IReadOnlyCollection<AzurePipelineYamlError> errors)
    {
        Assert.AreEqual(1, errors.Count);
        Assert.IsInstanceOfType<T>(errors.Single());
    }

    private static List<AzurePipelineYamlError> FindErrorsWithDefaultConfiguration(string yaml) 
        => FindErrors(yaml, AzurePipelineConfiguration.Default);
    
    private static List<AzurePipelineYamlError> FindErrors(string yaml, AzurePipelineConfiguration config)
        => AzurePipelineYamlValidator.FindErrors(yaml, config).ToList();
}