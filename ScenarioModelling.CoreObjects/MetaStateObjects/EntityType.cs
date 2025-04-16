using Newtonsoft.Json;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects.Properties;
using ScenarioModelling.CoreObjects.Visitors;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using System.Diagnostics;

namespace ScenarioModelling.CoreObjects.MetaStateObjects;

/// <summary>
/// Types exist only to allow grouping and reuse of entities (that would then have the same state type and aspects)
/// </summary>
[DebuggerDisplay(@"EntityType : {Name}")]
[MetaStateObjectLike<IMetaStateObject, EntityType>]
public record EntityType : IMetaStateObject<EntityTypeReference>, IOptionalSerialisability
{
    private MetaState _system = null!;

    [JsonIgnore]
    public Type Type => typeof(EntityType);

    public string Name { get; set; } = "";

    public StateMachineProperty StateMachine { get; private set; }

    public bool ExistanceOriginallyInferred { get; set; } = false;
    public bool ShouldReserialise
    {
        get
        {
            if (ExistanceOriginallyInferred)
                return false;

            //if (StateMachine.IsSet)
            //    return false;

            return true;
        }
    }

    // AspectType ?

    private EntityType()
    {

    }

    public EntityType(MetaState system)
    {
        _system = system;

        // Add this to the system
        //system.EntityTypes.Add(this);

        StateMachine = new(system);
    }

    public void InitialiseAfterDeserialisation(MetaState system)
    {
        _system = system;
    }

    public OneOfMetaStateObject ToOneOf()
        => new OneOfMetaStateObject(this);

    public EntityTypeReference GenerateReference()
        => new EntityTypeReference(_system) { Name = Name };

    public object Accept(IMetaStateVisitor visitor)
        => visitor.VisitEntityType(this);
}
