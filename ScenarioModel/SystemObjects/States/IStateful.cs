namespace ScenarioModel.SystemObjects.States;

public interface IStateful
{
    string Name { get; }
    State? State { get; }
}
