namespace Pipelines.Azure;

public record AzureDevOpsPipelineConfiguration
{
    public bool DisableImpliedYamlCiTrigger { get; init; }

    private AzureDevOpsPipelineConfiguration() { }
    
    public static readonly AzureDevOpsPipelineConfiguration Default = new()
    {
        DisableImpliedYamlCiTrigger = false
    };
}