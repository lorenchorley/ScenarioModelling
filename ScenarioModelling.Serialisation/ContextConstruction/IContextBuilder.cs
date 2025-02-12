using LanguageExt.Common;
using ScenarioModelling.CoreObjects;

namespace ScenarioModelling.Serialisation.ContextConstruction;

public interface IContextBuilder<TInputs> where TInputs : IContextBuilderInputs
{
    Result<Context> RefreshContextWithInputs(TInputs inputs);
    void Transform(TInputs inputs);
    void InitialiseObjects();
}