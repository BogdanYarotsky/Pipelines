using Pipelines.Azure.Errors;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace Pipelines.Azure;

public static class AzurePipelineYamlValidator
{
    public static IEnumerable<AzurePipelineYamlError> FindErrors(
        string yaml, AzurePipelineConfiguration? configuration = null)
    {
        if (string.IsNullOrWhiteSpace(yaml))
        {
            yield return new EmptyYaml();
            yield break;
        }
        
        configuration ??= AzurePipelineConfiguration.Default;
        
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
        if (rootNode is not YamlMappingNode rootMapping)
        {
            yield return new InvalidPipelineYamlRoot();
            yield break;
        }
        
        if (rootMapping.Children.TryGetValue("trigger", out var triggerSection))
        {
            foreach (var error in ValidateTriggerSection(triggerSection))
            {
                yield return error;
            }
        }
        else if (configuration.DisableImpliedYamlCiTrigger)
        {
            yield return new MissingTriggerSection();
        }
    }

private static IEnumerable<AzurePipelineYamlError> ValidateTriggerSection(YamlNode node)
{
    switch (node)
    {
        case YamlScalarNode scalar:
        {
            if (scalar.Value != "none")
            {
                yield return new InvalidTriggerDefinition(
                    $"When trigger is a single value, only 'none' is allowed, got: {scalar.Value}");
            }

            break;
        }
        case YamlSequenceNode sequence:
        {
            foreach (var branch in sequence.Children)
            {
                if (branch is not YamlScalarNode)
                {
                    yield return new InvalidTriggerDefinition("Branch names in trigger must be strings");
                    yield break;
                }
            }

            break;
        }
        case YamlMappingNode mapping:
        {
            var validKeys = new HashSet<string> { "batch", "branches", "paths", "tags" };
        
            foreach (var (key, value) in mapping.Children)
            {
                if (key is not YamlScalarNode keyNode)
                {
                    yield return new InvalidTriggerDefinition(
                        $"{key} is not a proper node with a scalar value");
                    
                    continue;
                }

                if (keyNode.Value is null || !validKeys.Contains(keyNode.Value))
                {
                    yield return new InvalidTriggerDefinition(
                        $"Invalid trigger configuration key: {keyNode.Value}. Valid keys are: {string.Join(", ", validKeys)}");
                    
                    continue;
                }

                if (keyNode.Value == "batch")
                {
                    if (value is not YamlScalarNode batchValue || (batchValue.Value != "true" && batchValue.Value != "false"))
                    {
                        yield return new InvalidTriggerDefinition("batch must be true or false");
                    }
                }

                if (keyNode.Value is "branches" or "paths" or "tags")
                {
                    if (value is not YamlMappingNode filterNode)
                    {
                        yield return new InvalidTriggerDefinition(
                            $"{keyNode.Value} must be a mapping with include/exclude");
                        continue;
                    }

                    foreach (var (filterKey, filterValue) in filterNode.Children)
                    {
                        if (filterKey is not YamlScalarNode filterKeyNode)
                        {
                            yield return new InvalidTriggerDefinition(
                                $"{keyNode.Value} filter keys must be strings");
                            continue;
                        }

                        if (filterKeyNode.Value is not "include" and not "exclude")
                        {
                            yield return new InvalidTriggerDefinition(
                                $"{keyNode.Value} filters must use 'include' or 'exclude'");
                            continue;
                        }

                        if (filterValue is not YamlSequenceNode filterValues)
                        {
                            yield return new InvalidTriggerDefinition(
                                $"{keyNode.Value} filter values must be a sequence");
                            continue;
                        }

                        foreach (var pattern in filterValues)
                        {
                            if (pattern is not YamlScalarNode)
                            {
                                yield return new InvalidTriggerDefinition(
                                    $"{keyNode.Value} filter patterns must be strings");
                            }
                        }
                    }
                }
            }

            break;
        }
    }
}
}

public record InvalidTriggerDefinition(string Message) : AzurePipelineYamlError(Message);
public record InvalidPipelineYamlRoot() : AzurePipelineYamlError("Expecting a rootMapping");
public record InvalidYamlSyntax(string Message) : AzurePipelineYamlError(Message);