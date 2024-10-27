namespace Pipelines.Azure;

public record AzurePipelineYamlError(string Message)
{
    public override string ToString() => Message;
};