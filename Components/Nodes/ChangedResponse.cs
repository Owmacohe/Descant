using System;
using System.Collections.Generic;
using UnityEngine;

namespace DescantComponents
{
    [Serializable, MaxQuantity(float.PositiveInfinity), NodeType(DescantNodeType.Response)]
    public class ChangedResponse : DescantNodeComponent
    {
        [Inline] public string ActorName;
        
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
                result.Choices[0] = new KeyValuePair<int, string>(result.Choices[0].Key, ChangeTo);
            }
            
            return result;
        }
    }
}