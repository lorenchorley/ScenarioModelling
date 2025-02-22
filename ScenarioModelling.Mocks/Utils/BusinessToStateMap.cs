using ScenarioModelling.CoreObjects;

namespace ScenarioModelling.Mocks.Utils;
internal class BusinessToStateMap
{
    private readonly MetaState _metaState;
    private readonly Dictionary<string, object> _stateNameToBusinessValueTranslations = new();
    private readonly Dictionary<string, Func<object, string>> _businessValueToStateNameTranslatorsByBusinessTypeName = new();
    private readonly Dictionary<string, string> _businessNameToStatefulObjectName = new();
    private readonly Dictionary<string, string> _stateMachineNameToBusinessTypeNames = new();
    private readonly Dictionary<string, BusinessTypeProfile> _businessNameToBusinessTypes = new();

    internal BusinessToStateMap(MetaState metaState)
    {
        _metaState = metaState;
    }

    public void RegisterBusinessType<TBusinessValue>(string businessTypeName) where TBusinessValue : notnull
    {
        _businessNameToBusinessTypes.Add(businessTypeName, new() { Name = businessTypeName, ValueType = typeof(TBusinessValue) });
    }

    internal void AddValueToStateTranslator<TBusinessValue>(string businessTypeName, Func<TBusinessValue, string> businessValueToStateTranslator) where TBusinessValue : notnull
    {
        if (!_businessNameToBusinessTypes.TryGetValue(businessTypeName, out BusinessTypeProfile businessTypeProfile))
            throw new KeyNotFoundException($"Business type '{businessTypeName}' is not registered");

        Func<object, string> objectTranslator = (object o) => businessValueToStateTranslator((TBusinessValue)o);
        _businessValueToStateNameTranslatorsByBusinessTypeName.Add(businessTypeName, objectTranslator);
    }

    internal void AddStateToValueTranslation<TBusinessValue>(string stateName, string businessTypeName, TBusinessValue businessValue) where TBusinessValue : notnull
    {
        var state = _metaState.States.FirstOrDefault(s => s.Name.IsEqv(stateName));
        var stateMachine = state.StateMachine;

        if (!_businessNameToBusinessTypes.TryGetValue(businessTypeName, out BusinessTypeProfile businessTypeProfile))
            throw new KeyNotFoundException($"Business type '{businessTypeName}' is not registered");

        if (typeof(TBusinessValue) != businessTypeProfile.ValueType)
            throw new KeyNotFoundException($"Business type '{businessTypeName}' is not registered with the value type '{typeof(TBusinessValue)}'");

        // Check that the business type is registered with the correct value type by checking that the state machine corresponds to the correct business type
        if (_stateMachineNameToBusinessTypeNames.TryGetValue(stateMachine.Name, out string registeredBusinessType))
        {
            if (registeredBusinessType != businessTypeName)
                throw new KeyNotFoundException($"State machine '{stateMachine.Name}' is already registered with business type '{registeredBusinessType}'");
        }
        else
        {
            // If the business type/state machine pair is not registered, register it
            _stateMachineNameToBusinessTypeNames.Add(stateMachine.Name, businessTypeName);
        }

        _stateNameToBusinessValueTranslations.Add(stateName, businessValue);
    }

    internal string GetStatefulObjectName(string businessTypeName)
    {
        if (!_businessNameToBusinessTypes.TryGetValue(businessTypeName, out BusinessTypeProfile businessTypeProfile))
            throw new KeyNotFoundException($"Business type '{businessTypeName}' is not registered");

        if (!_businessNameToStatefulObjectName.TryGetValue(businessTypeName, out string statefulObjectName))
            throw new KeyNotFoundException($"No stateful object name found for business type '{businessTypeName}'");

        return statefulObjectName;
    }

    internal string GetStateName<TBusinessValue>(string businessTypeName, TBusinessValue businessValue) where TBusinessValue : notnull
    {
        if (!_businessNameToBusinessTypes.TryGetValue(businessTypeName, out BusinessTypeProfile businessTypeProfile))
            throw new KeyNotFoundException($"Business type '{businessTypeName}' is not registered");

        if (!_businessValueToStateNameTranslatorsByBusinessTypeName.TryGetValue(businessTypeName, out var translator))
            throw new KeyNotFoundException($"No translator found for business type '{businessTypeName}'");

        return translator(businessValue);
    }

    internal TBusinessValue GetValue<TBusinessValue>(string businessTypeName, string stateName) where TBusinessValue : notnull
    {
        if (!_businessNameToBusinessTypes.TryGetValue(businessTypeName, out BusinessTypeProfile businessTypeProfile))
            throw new KeyNotFoundException($"Business type '{businessTypeName}' is not registered");

        if (businessTypeProfile.ValueType != typeof(TBusinessValue))
            throw new KeyNotFoundException($"Business type '{businessTypeName}' is not registered with the value type '{typeof(TBusinessValue)}'");

        if (!_stateNameToBusinessValueTranslations.TryGetValue(stateName, out object businessValue))
            throw new KeyNotFoundException($"No business value found for state '{stateName}'");

        if (!(businessValue is TBusinessValue))
            throw new KeyNotFoundException($"Business value for state '{stateName}' is not of the correct type '{typeof(TBusinessValue)}'");

        return (TBusinessValue)businessValue;
    }

    internal void AddStateToValueTranslation(string businessTypeName, string statefulObjectName)
    {
        _businessNameToStatefulObjectName.Add(businessTypeName, statefulObjectName);
    }
}