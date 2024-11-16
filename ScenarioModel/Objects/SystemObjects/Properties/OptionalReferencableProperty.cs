using OneOf;
using ScenarioModel.Objects.SystemObjects.Interfaces;
using ScenarioModel.References.Interfaces;

namespace ScenarioModel.Objects.SystemObjects.Properties;

public abstract class OptionalReferencableProperty<TVal, TRef>
    where TVal : class, IIdentifiable
    where TRef : class, IReference<TVal>
{
    protected OneOf<TVal, TRef>? _valueOrReference = null;
    private readonly System _system;

    public abstract string? Name { get; }

    public OptionalReferencableProperty(System system)
    {
        _system = system;
    }

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

    public TVal? Value
    {
        get => _valueOrReference?.Match<TVal?>(
                value => value,
                reference => default
            );
    }

    public TRef? Reference
    {
        get => _valueOrReference?.Match<TRef?>(
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

    public TVal? ResolvedValue
    {
        get
        {
            if (_valueOrReference == null)
                return null;

            return ((OneOf<TVal, TRef>)_valueOrReference).Match(
                state => state,
                reference => reference.ResolveReference().Match(
                    state => state,
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
