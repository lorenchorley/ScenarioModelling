using LanguageExt;
using ScenarioModel.SystemObjects.States;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction;

public class DefinitionAutoNamer(string Prefix)
{
    private readonly List<INameful> _definitionsToName = new();

    public T AddUnnamedDefinition<T>(T definition) where T : INameful
    {
        _definitionsToName.Add(definition);

        return definition;
    }

    public void NameUnnamedObjects<T>(IEnumerable<T> completeList) where T : INameful
    {
        int index = 1;
        var existingNames = completeList.Select(d => d.Name).ToHashSet();

        foreach (var definition in _definitionsToName)
        {
            string nameProposition = $"{Prefix}{index}";

            while (existingNames.Contains(nameProposition))
            {
                index++;
                nameProposition = $"{Prefix}{index}";
            }

            definition.Name = nameProposition;
            index++;
        }
    }
}
