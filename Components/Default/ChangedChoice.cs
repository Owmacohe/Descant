// Please see https://omch.tech/descant/#changedchoice for documentation

using System;
using System.Collections.Generic;
using UnityEngine;
using Descant.Utilities;

namespace Descant.Components
{
    [Serializable, MaxQuantity(float.PositiveInfinity), NodeType(DescantNodeType.Choice)]
    public class ChangedChoice : DescantComponent
    {
        [Inline] public string ActorName;
        
        [ParameterGroup("Index of the choice to change (base 1)")] public int ChoiceNumber;

        [ParameterGroup("Variable to check")] public VariableType VariableType;
        [ParameterGroup("Variable to check")] public string VariableName;
        
        [ParameterGroup("Comparison to make")] public ComparisonType ComparisonType;
        [ParameterGroup("Comparison to make")] public float Comparison;
        
        [ParameterGroup("Changed value"), NoFiltering] public string ChangeTo;

        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            DescantActor actor = DescantComponentUtilities.GetActor(this, result.Actors, ActorName);

            if (actor == null) return result;

            if ((VariableType.Equals(VariableType.Statistic) && CompareVariable(
                    actor.StatisticValues[actor.StatisticKeys.IndexOf(VariableName)], Comparison, ComparisonType)) ||
                (VariableType.Equals(VariableType.Topic) && actor.Topics.Contains(VariableName)) ||
                (VariableType.Equals(VariableType.Relationship) && CompareVariable(
                    actor.RelationshipValues[actor.RelationshipKeys.IndexOf(VariableName)], Comparison, ComparisonType)) ||
                (VariableType.Equals(VariableType.DialogueAttempts) && CompareVariable(
                    actor.DialogueAttempts, Comparison, ComparisonType)))
            {
                result.Text[ChoiceNumber - 1] = new KeyValuePair<int, string>(
                    result.Text[ChoiceNumber - 1].Key,
                    ChangeTo);
            }

            return result;
        }
    }
}