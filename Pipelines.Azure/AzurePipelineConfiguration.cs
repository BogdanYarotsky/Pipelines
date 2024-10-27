namespace Pipelines.Azure;

public record AzurePipelineConfiguration
{
    // todo - read from DevOps using web API? :)
    public bool DisableImpliedYamlCiTrigger { get; init; }

    private AzurePipelineConfiguration() { }
    
    public static readonly AzurePipelineConfiguration Default = new()
    {
        DisableImpliedYamlCiTrigger = false
    };
}