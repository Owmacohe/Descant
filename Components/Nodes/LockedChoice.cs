using System;

namespace DescantComponents
{
    [Serializable, MaxQuantity(Single.PositiveInfinity), NodeType(DescantNodeType.Choice)]
    public class LockedChoice : DescantNodeComponent
    {
        [Inline] public string ActorName;
        
        [ParameterGroup("Index of the choice to change (base 1)")] public int ChoiceNumber;
        
        [ParameterGroup("Variable to check")] public VariableType VariableType;
        [ParameterGroup("Variable to check")] public string VariableName;
        
        [ParameterGroup("Comparison to make")] public ComparisonType ComparisonType;
        [ParameterGroup("Comparison to make")] public float Comparison;

        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            DescantActor actor = DescantComponentUtilities.GetActor(result.Actors, ActorName);

            if (actor == null) return result;

            if ((VariableType.Equals(VariableType.Statistic) && CompareVariable(
                    actor.StatisticValues[actor.StatisticKeys.IndexOf(VariableName)], Comparison, ComparisonType)) ||
                (VariableType.Equals(VariableType.Topic) && actor.Topics.Contains(VariableName)) ||
                (VariableType.Equals(VariableType.Relationship) && CompareVariable(
                    actor.RelationshipValues[actor.RelationshipKeys.IndexOf(VariableName)], Comparison, ComparisonType)) ||
                (VariableType.Equals(VariableType.ConversationAttempts) && CompareVariable(
                    actor.ConversationAttempts, Comparison, ComparisonType)))
            {
                result.Choices.RemoveAt(ChoiceNumber - 1);
            }
            
            return result;
        }
    }
}