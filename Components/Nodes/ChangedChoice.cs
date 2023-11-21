using System;
using System.Collections.Generic;
using UnityEngine;

namespace DescantComponents
{
    [Serializable, MaxQuantity(float.PositiveInfinity), NodeType(DescantNodeType.Choice)]
    public class ChangedChoice : DescantNodeComponent
    {
        [Inline] public string ActorName;
        
        [ParameterGroup("Index of the choice to change (base 1)")] public int ChoiceNumber;

        [ParameterGroup("Variable to check")] public VariableType VariableType;
        [ParameterGroup("Variable to check")] public string VariableName;
        
        [ParameterGroup("Comparison to make")] public ComparisonType ComparisonType;
        [ParameterGroup("Comparison to make")] public float Comparison;
        
        [ParameterGroup("Changed value")] public string ChangeTo;

        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            DescantActor actor = DescantComponentUtilities.GetActor(result.Actors, ActorName);

            if ((VariableType.Equals(VariableType.Statistic) && DescantComponentUtilities.CompareVariable(
                    actor.StatisticValues[actor.StatisticKeys.IndexOf(VariableName)], Comparison, ComparisonType)) ||
                (VariableType.Equals(VariableType.Topic) && actor.Topics.Contains(VariableName)) ||
                (VariableType.Equals(VariableType.Relationship) && DescantComponentUtilities.CompareVariable(
                    actor.RelationshipValues[actor.RelationshipKeys.IndexOf(VariableName)], Comparison, ComparisonType)) ||
                (VariableType.Equals(VariableType.ConversationAttempts) && DescantComponentUtilities.CompareVariable(
                    actor.ConversationAttempts, Comparison, ComparisonType)))
            {
                result.Choices[ChoiceNumber - 1] = new KeyValuePair<int, string>(
                    result.Choices[ChoiceNumber - 1].Key,
                    ChangeTo);
            }

            return result;
        }
    }
}