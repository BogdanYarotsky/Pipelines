namespace Pipelines.Azure.Errors;

public record MissingTriggerSection() : AzurePipelineYamlError(
    "If in Azure DevOps it was configured for pipelines to require an explicit trigger defined in yaml. Add fix suggestion here.");