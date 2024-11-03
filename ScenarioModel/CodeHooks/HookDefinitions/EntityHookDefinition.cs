using ScenarioModel.Objects.SystemObjects.Entities;
using ScenarioModel.References;
using System.Security.Cryptography.X509Certificates;

namespace ScenarioModel.CodeHooks.HookDefinitions;

public class EntityHookDefinition(System System, string Name)
{
    private Entity? _entity = null;

    public string? StateName { get; private set; }

    public EntityHookDefinition SetState(string stateName)
    {
        StateName = stateName;

        if (!string.IsNullOrEmpty(StateName))
            GetEntity().State.Set(new StateReference() { StateName = StateName });

        return this;
    }

    internal Entity GetEntity()
    {
        return _entity = _entity ?? new Entity(System)
        {
            Name = Name,
        };
    }
}
