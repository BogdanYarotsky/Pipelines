using Pipelines.Azure.Errors;

namespace Pipelines.Azure;

public static class AzurePipelineYamlValidator
{
    public static IEnumerable<AzurePipelineYamlError> FindErrors(
        string yaml, AzureDevOpsPipelineConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(yaml))
        {
            yield return new EmptyYaml();
            yield break;
        }
        
        // todo - get first section
        
    }

}