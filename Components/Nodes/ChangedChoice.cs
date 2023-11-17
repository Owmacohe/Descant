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

        public bool Compare(float a)
        {
            switch (ComparisonType)
            {
                case ComparisonType.LessThan:
                    return a < Comparison;
                
                case ComparisonType.LessThanOrEqualTo:
                    return a <= Comparison;
                
                case ComparisonType.EqualTo:
                    return a == Comparison;
                
                case ComparisonType.GreaterThanOrEqualTo:
                    return a >= Comparison;
                
                case ComparisonType.GreaterThan:
                    return a > Comparison;
                
                case ComparisonType.NotEqualTo:
                    return a != Comparison;
                
                default: return false;
            }
        }

        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            DescantActor actor = DescantComponentUtilities.GetActor(result.Actors, ActorName);

            switch (VariableType)
            {
                case VariableType.Statistic:
                    if (Compare(actor.StatisticValues[actor.StatisticKeys.IndexOf(VariableName)]))
                        result.Choices[ChoiceNumber - 1] = new KeyValuePair<int, string>(
                            result.Choices[ChoiceNumber - 1].Key,
                            ChangeTo);
                    break;
                
                case VariableType.Topic: // TODO: hide other options if the user selects topic in the editor
                    if (actor.Topics.Contains(VariableName))
                        result.Choices[ChoiceNumber - 1] = new KeyValuePair<int, string>(
                            result.Choices[ChoiceNumber - 1].Key,
                            ChangeTo);
                    break;
                
                case VariableType.Relationship:
                    if (Compare(actor.RelationshipValues[actor.RelationshipKeys.IndexOf(VariableName)]))
                        result.Choices[ChoiceNumber - 1] = new KeyValuePair<int, string>(
                            result.Choices[ChoiceNumber - 1].Key,
                            ChangeTo);
                    break;
                
                case VariableType.ConversationAttempts:
                    if (Compare(actor.ConversationAttempts))
                        result.Choices[ChoiceNumber - 1] = new KeyValuePair<int, string>(
                            result.Choices[ChoiceNumber - 1].Key,
                            ChangeTo);
                    break;
            }

            return result;
        }
    }
}