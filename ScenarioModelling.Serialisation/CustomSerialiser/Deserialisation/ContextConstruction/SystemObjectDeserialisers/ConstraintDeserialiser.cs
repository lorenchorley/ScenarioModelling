﻿using LanguageExt;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.CoreObjects.MetaStateObjects;
using ScenarioModelling.Serialisation.ContextConstruction;
using ScenarioModelling.Serialisation.Expressions.Interpreter;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.SystemObjectDeserialisers.Interfaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.IntermediateSemanticTree;
using ScenarioModelling.Tools.Exceptions;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.SystemObjectDeserialisers;

[MetaStateObjectLike<IDefinitionToObjectDeserialiser, Constraint>]
public class ConstraintDeserialiser(MetaState MetaState, Instanciator Instanciator, ExpressionInterpreter Interpreter) : DefinitionToObjectDeserialiser<Constraint, ConstraintReference>
{
    protected override Option<ConstraintReference> Transform(Definition def, TransformationType type)
    {
        if (def is not ExpressionDefinition expDef)
            return null;

        if (!expDef.Name.Value.IsEqv("Constraint"))
            return null;

        if (type == TransformationType.Property)
            throw new Exception("Constraint should not be properties of other objects");

        Constraint value = Instanciator.New<Constraint>(definition: def);

        def.HasBeenTransformed = true;

        var result = Interpreter.Parse(expDef.Block.ExpressionText.Value);

        if (result.HasErrors)
        {
            //throw new Exception($@"Unable to parse expression ""{expDef.Block.ExpressionText.Value}"" on if node{value.LineInformation} : \n{result.Errors.CommaSeparatedList()}");
            throw new ExpressionException($@"Unable to parse expression ""{expDef.Block.ExpressionText.Value}"" on if node : \n{result.Errors.CommaSeparatedList()}");
            // TODO Add line information ISystemObject.LineInformation
        }

        if (result.ParsedObject is null)
        {
            //throw new Exception($@"Unable to parse expression ""{expDef.Block.ExpressionText.Value}"" on if node{value.LineInformation} : return value not set");
            throw new InternalLogicException($@"Unable to parse expression ""{expDef.Block.ExpressionText.Value}"" on if node : return value not set");
            // TODO Add line information ISystemObject.LineInformation
        }

        value.OriginalConditionText = expDef.Block.ExpressionText.Value;
        value.Condition = result.ParsedObject;

        foreach (var item in expDef.Definitions)
        {
            if (item is NamedDefinition named)
            {
                if (named.Type.Value.IsEqv("Description"))
                {
                    value.Name = named.Name.Value;
                    item.HasBeenTransformed = true;
                    continue;
                }
            }
        }

        return value.GenerateReference();
    }

    public override void BeforeIndividualInitialisation()
    {

    }

    public override void Initialise(Constraint obj)
    {
    }
}

