using Pipelines.Azure.Errors;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace Pipelines.Azure;

public static class AzurePipelineYamlValidator
{
    public static IEnumerable<AzurePipelineYamlError> FindErrors(
        string yaml, AzurePipelineConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(yaml))
        {
            yield return new EmptyYaml();
            yield break;
        }
        
        var yamlStream = new YamlStream();
        YamlException? yamlException = null;
        
        try
        {
            using var reader = new StringReader(yaml);
            yamlStream.Load(reader);
        }
        catch (YamlException ex)
        {
            yamlException = ex;
        }

        if (yamlException is not null)
        {
            yield return new InvalidYamlSyntax(yamlException.Message);
            yield break;
        }
        
        var rootNode = yamlStream.Documents[0].RootNode;

        // Validate root is mapping
        if (rootNode is not YamlMappingNode rootMapping)
        {
            yield return new InvalidYamlRoot();
            yield break;
        }
        
        if (rootMapping.Children.TryGetValue("trigger", out var triggerNode))
        {
            foreach (var error in ValidateTriggerNode(triggerNode))
            {
                yield return error;
            }
        }
        else if (configuration.DisableImpliedYamlCiTrigger)
        {
            yield return new MissingTriggerSection();
        }
    }

    private static IEnumerable<AzurePipelineYamlError> ValidateTriggerNode(YamlNode triggerNode)
    {
        yield break;
    }
}

public record InvalidYamlRoot() : AzurePipelineYamlError("Expecting a rootMapping");
public record InvalidYamlSyntax(string Message) : AzurePipelineYamlError(Message);