namespace ScenarioModelling.Objects.SystemObjects.Interfaces;

public interface IOptionalSerialisability
{
    bool ExistanceOriginallyInferred { set; }
    bool ShouldReserialise { get; }
}
