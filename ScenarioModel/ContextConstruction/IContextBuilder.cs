using LanguageExt.Common;

namespace ScenarioModel.ContextConstruction;

public interface IContextBuilder<TInputs> where TInputs : IContextBuilderInputs
{
    Result<Context> Build(TInputs inputs);
    void Transform(TInputs inputs);
    void CreateObjectsFromUnresolvableReferences();
    void NameUnnamedObjects();
    void ValidateObjects();
}