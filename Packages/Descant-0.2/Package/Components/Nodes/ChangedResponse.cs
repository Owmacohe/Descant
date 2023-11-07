using System;
using Descant.Package.Editor.Nodes;
using Descant.Package.Runtime;
using UnityEngine.Serialization;

namespace Descant.Package.Components
{
    [Serializable, MaxQuantity(float.PositiveInfinity), NodeType(DescantNodeType.Response)]
    public class ChangedResponse : DescantNodeComponent, IInvokedDescantComponent
    {
        [InlineGroup(0)] public string ActorName;
        [InlineGroup(1)] public VariableType VariableType;
        [InlineGroup(1)] public string VariableName;
        [InlineGroup(2)] public ComparisonType ComparisonType;
        [InlineGroup(2)] public float Comparison;
        [InlineGroup(3)] public string ChangeTo;
        
        public ChangedResponse(
            DescantConversationController controller,
            int nodeID,
            int id, string actorName, VariableType variableType, string variableName, ComparisonType comparisonType, float comparison, string changeTo)
            : base(controller, nodeID, id)
        {
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
                        Controller.CurrentText[0] = ChangeTo;
                    break;
                
                case VariableType.Topic:
                    if (actor.Topics.Contains(VariableName))
                        Controller.CurrentText[0] = ChangeTo;
                    break;
                
                case VariableType.Relationship:
                    if (Compare(actor.Relationships[VariableName]))
                        Controller.CurrentText[0] = ChangeTo;
                    break;
                
                case VariableType.ReAttempts:
                    if (Compare(actor.ReAttempts))
                        Controller.CurrentText[0] = ChangeTo;
                    break;
            }
        }
    }
}