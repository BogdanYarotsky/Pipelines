namespace Pipelines.Azure.Errors;

public record EmptyYaml() : AzurePipelineYamlError(
    "There is nothing to validate. Null or empty string was passed to the validator.");