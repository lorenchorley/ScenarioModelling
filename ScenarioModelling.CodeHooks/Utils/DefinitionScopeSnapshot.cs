namespace ScenarioModelling.CodeHooks.Utils;

public class DefinitionScopeSnapshot
{
    public int Index { get; set; }
    public SubgraphScopedHookSynchroniser Scope { get; set; } = null!;
}
