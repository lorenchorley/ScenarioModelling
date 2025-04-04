﻿namespace ScenarioModelling.Tools.Collections.Graph.Validation;

public abstract class DirectedGraphValidationResult<T> where T : IDirectedGraphNode<T>
{
    public static DirectedGraphValidationResult<T> BrokenLinks(List<(IDirectedGraphNode<T> node, string intendedTarget)> value) => new BrokenLinksResult<T>(value);
    public static DirectedGraphValidationResult<T> Valid() => new ValidResult<T>();
}
