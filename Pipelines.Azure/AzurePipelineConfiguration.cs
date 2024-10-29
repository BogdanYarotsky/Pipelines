namespace Pipelines.Azure;

public record AzurePipelineConfiguration
{
    public bool DisableImpliedYamlCiTrigger { get; init; }

    private AzurePipelineConfiguration() { }
    
    public static readonly AzurePipelineConfiguration Default = new()
    {
        DisableImpliedYamlCiTrigger = false
    };
}