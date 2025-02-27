namespace ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;

public interface IOptionalSerialisability
{
    bool ExistanceOriginallyInferred { set; }
    bool ShouldReserialise { get; }
}
