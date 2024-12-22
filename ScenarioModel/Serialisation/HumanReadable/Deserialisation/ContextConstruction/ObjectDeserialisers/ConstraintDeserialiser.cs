﻿using LanguageExt;
using ScenarioModel.ContextConstruction;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Expressions.Interpreter;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.References;
using ScenarioModel.Serialisation.HumanReadable.Deserialisation.ContextConstruction.ObjectDeserialisers.Interfaces;
using ScenarioModel.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

namespace ScenarioModel.Serialisation.HumanReadable.Deserialisation.ContextConstruction.ObjectDeserialisers;

[ObjectLike<IDefinitionToObjectDeserialiser, Constraint>]
public class ConstraintDeserialiser(System System, Instanciator Instanciator) : DefinitionToObjectDeserialiser<Constraint, ConstraintReference>
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

        ExpressionInterpreter interpreter = new();

        var result = interpreter.Parse(expDef.Block.ExpressionText.Value);

        if (result.HasErrors)
        {
            //throw new Exception($@"Unable to parse expression ""{expDef.Block.ExpressionText.Value}"" on if node{value.LineInformation} : \n{result.Errors.CommaSeparatedList()}");
            throw new Exception($@"Unable to parse expression ""{expDef.Block.ExpressionText.Value}"" on if node : \n{result.Errors.CommaSeparatedList()}");
            // TODO Add line information ISystemObject.LineInformation
        }

        if (result.ParsedObject is null)
        {
            //throw new Exception($@"Unable to parse expression ""{expDef.Block.ExpressionText.Value}"" on if node{value.LineInformation} : return value not set");
            throw new Exception($@"Unable to parse expression ""{expDef.Block.ExpressionText.Value}"" on if node : return value not set");
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

