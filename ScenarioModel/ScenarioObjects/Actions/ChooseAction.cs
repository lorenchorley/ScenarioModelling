
namespace ScenarioModel.SystemObjects.Entities;

public class ChooseAction : IScenarioAction
{
    public string Name { get; set; } = "";
    public List<string> Choices { get; set; } = new();

    public IEnumerable<string> TargetNodeNames => throw new NotImplementedException();
}
