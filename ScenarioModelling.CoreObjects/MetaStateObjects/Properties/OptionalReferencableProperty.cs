using Newtonsoft.Json;
using OneOf;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;
using System.Diagnostics;

namespace ScenarioModelling.CoreObjects.MetaStateObjects.Properties;

[DebuggerDisplay("{DebugInfo}")]
public abstract class OptionalReferencableProperty<TValue, TReference>
    where TValue : class, IMetaStateObject<TReference>
    where TReference : class, IReference<TValue>
{
    protected OneOf<TValue, TReference>? _valueOrReference = null;
    private readonly MetaState _metaState;

    public abstract string? Name { get; }

    public string DebugInfo
    {
        get
        {
            if (!_valueOrReference.HasValue)
                return "null";

            return _valueOrReference.Value.Match(
                (TValue state) => $"{state.GetType().Name} : {state.Name}",
                (TReference reference) => $"{reference.TypeName} reference : {reference.Name}"
            );
        }
    }

    public OptionalReferencableProperty(MetaState metaState)
    {
        _metaState = metaState;
    }

    [JsonIgnore]
    public bool IsSet => _valueOrReference != null;

    public void SetValue(TValue? value)
    {
        if (value == null)
        {
            _valueOrReference = default;
        }
        else
        {
            _valueOrReference = value;
        }
    }

    public void SetReference(TReference? reference)
    {
        if (reference == null)
        {
            _valueOrReference = default;
        }
        else
        {
            _valueOrReference = reference;
        }
    }

    public TReference? GetOrGenerateReference()
    {
        if (_valueOrReference == null)
            return null;

        return _valueOrReference?.Match(
                value => value.GenerateReference(),
                reference => reference
            );
    }

    [JsonIgnore]
    public TValue? ValueOnly
    {
        get => _valueOrReference?.Match(
                value => value,
                reference => default
            );
    }

    [JsonIgnore]
    public TReference? ReferenceOnly
    {
        get => _valueOrReference?.Match(
                state => default,
                reference => reference
            );
    }

    public TResult Match<TResult>(Func<TValue, TResult> value, Func<TReference, TResult> reference, Func<TResult> isNull)
    {
        if (_valueOrReference == null)
            return isNull();

        return _valueOrReference.Value.Match(value, reference);
    }

    [JsonIgnore]
    public TValue? ResolvedValue
    {
        get
        {
            if (_valueOrReference == null)
                return null;

            return ((OneOf<TValue, TReference>)_valueOrReference).Match(
                state => state,
                reference => reference.ResolveReference().Match(
                    state =>
                    {
                        _valueOrReference = state; // Cache the resolved value
                        return state;
                    },
                    () => throw new Exception($"{typeof(TValue).Name} reference '{reference}' could not be resolved.")
                )
            );
        }
    }

    public bool IsEqv(OptionalReferencableProperty<TValue, TReference> other) // TODO Move to extension method
    {
        if ((_valueOrReference == null && other._valueOrReference != null) ||
            (_valueOrReference != null && other._valueOrReference == null))
        {
            return false;
        }

        if (_valueOrReference == null && other._valueOrReference == null)
        {
            return true;
        }

        return _valueOrReference!.Value.Match(
            state => other._valueOrReference!.Value.Match(
                otherState => state.IsEqv(otherState),
                otherReference => false),
            reference => other._valueOrReference!.Value.Match(
                otherState => false,
                otherReference => reference.IsEqv(otherReference)));
    }
}
