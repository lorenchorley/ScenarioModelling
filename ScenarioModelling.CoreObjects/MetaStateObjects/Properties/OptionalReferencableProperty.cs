using Newtonsoft.Json;
using OneOf;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;
using ScenarioModelling.CoreObjects.References;

namespace ScenarioModelling.CoreObjects.MetaStateObjects.Properties;

public abstract class OptionalReferencableProperty<TVal, TRef>
    where TVal : class, ISystemObject<TRef>
    where TRef : class, IReference<TVal>
{
    protected OneOf<TVal, TRef>? _valueOrReference = null;
    private readonly MetaState _system;

    public abstract string? Name { get; }

    public OptionalReferencableProperty(MetaState system)
    {
        _system = system;
    }

    [JsonIgnore]
    public bool IsSet => _valueOrReference != null;

    public void SetValue(TVal? value)
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

    public void SetReference(TRef? reference)
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

    public TRef? GetOrGenerateReference()
    {
        if (_valueOrReference == null)
            return null;

        return _valueOrReference?.Match(
                value => value.GenerateReference(),
                reference => reference
            );
    }

    [JsonIgnore]
    public TVal? ValueOnly
    {
        get => _valueOrReference?.Match(
                value => value,
                reference => default
            );
    }

    [JsonIgnore]
    public TRef? ReferenceOnly
    {
        get => _valueOrReference?.Match(
                state => default,
                reference => reference
            );
    }

    public TResult Match<TResult>(Func<TVal, TResult> value, Func<TRef, TResult> reference, Func<TResult> isNull)
    {
        if (_valueOrReference == null)
            return isNull();

        return _valueOrReference.Value.Match(value, reference);
    }

    [JsonIgnore]
    public TVal? ResolvedValue
    {
        get
        {
            if (_valueOrReference == null)
                return null;

            return ((OneOf<TVal, TRef>)_valueOrReference).Match(
                state => state,
                reference => reference.ResolveReference().Match(
                    state =>
                    {
                        _valueOrReference = state; // Cache the resolved value
                        return state;
                    },
                    () => throw new Exception($"{typeof(TVal).Name} reference '{reference}' could not be resolved.")
                )
            );
        }
    }

    public bool IsEqv(OptionalReferencableProperty<TVal, TRef> other) // TODO Move to extension method
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
