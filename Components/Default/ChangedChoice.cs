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
        [Inline] public DescantActor Actor;
        
        [ParameterGroup("Index of the choice to change (base 1)")] public int ChoiceNumber;

        [ParameterGroup("Variable to check")] public VariableType VariableType;
        [ParameterGroup("Variable to check")] public string VariableName;
        
        [ParameterGroup("Comparison to make")] public ComparisonType ComparisonType;
        [ParameterGroup("Comparison to make")] public float Comparison;
        
        [ParameterGroup("Changed value"), NoFiltering] public string ChangeTo;

        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            if ((VariableType.Equals(VariableType.Statistic) && DescantComponentUtilities.CompareVariable(
                    Actor.Statistics[VariableName], Comparison, ComparisonType)) ||
                (VariableType.Equals(VariableType.Topic) && Actor.Topics.Contains(VariableName)) ||
                (VariableType.Equals(VariableType.Relationship) && DescantComponentUtilities.CompareVariable(
                    Actor.Relationships[VariableName], Comparison, ComparisonType)) ||
                (VariableType.Equals(VariableType.DialogueAttempts) && DescantComponentUtilities.CompareVariable(
                    Actor.DialogueAttempts, Comparison, ComparisonType)))
            {
                result.Text[ChoiceNumber - 1] = new KeyValuePair<int, string>(
                    result.Text[ChoiceNumber - 1].Key,
                    ChangeTo);
            }

            return result;
        }
    }
}