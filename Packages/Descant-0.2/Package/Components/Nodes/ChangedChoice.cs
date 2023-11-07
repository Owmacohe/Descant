using System;
using Descant.Package.Editor.Nodes;
using Descant.Package.Runtime;
using UnityEngine.Serialization;

namespace Descant.Package.Components
{
    [Serializable, MaxQuantity(float.PositiveInfinity), NodeType(DescantNodeType.Choice)]
    public class ChangedChoice : DescantNodeComponent, IInvokedDescantComponent
    {
        [InlineGroup(1)] public int ChoiceNumber; // base 1
        [InlineGroup(1)] public string ActorName; 
        [InlineGroup(2)] public VariableType VariableType;
        [InlineGroup(2)] public string VariableName;
        [InlineGroup(3)] public ComparisonType ComparisonType;
        [InlineGroup(3)] public float Comparison;
        [InlineGroup(4)] public string ChangeTo;
        
        public ChangedChoice(
            DescantConversationController controller,
            int nodeID,
            int id,
            int choiceNumber, string actorName, VariableType variableType, string variableName, ComparisonType comparisonType, float comparison, string changeTo)
            : base(controller, nodeID, id)
        {
            ChoiceNumber = choiceNumber;
            ActorName = actorName;
            VariableType = variableType;
            VariableName = variableName;
            ComparisonType = comparisonType;
            Comparison = comparison;
            ChangeTo = changeTo;
        }

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

        public void Invoke()
        {
            DescantActor actor = Controller.GetActor(ActorName);
            
            switch (VariableType)
            {
                case VariableType.Statistic:
                    if (Compare(actor.Statistics[VariableName]))
                        Controller.CurrentText[ChoiceNumber] = ChangeTo;
                    break;
                
                case VariableType.Topic:
                    if (actor.Topics.Contains(VariableName))
                        Controller.CurrentText[ChoiceNumber] = ChangeTo;
                    break;
                
                case VariableType.Relationship:
                    if (Compare(actor.Relationships[VariableName]))
                        Controller.CurrentText[ChoiceNumber] = ChangeTo;
                    break;
                
                case VariableType.ReAttempts:
                    if (Compare(actor.ReAttempts))
                        Controller.CurrentText[ChoiceNumber] = ChangeTo;
                    break;
            }
        }
    }
}