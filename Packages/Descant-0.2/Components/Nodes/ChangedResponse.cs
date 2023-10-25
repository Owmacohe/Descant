using System;
using Editor.Nodes;
using Runtime;

namespace Components
{
    [Serializable]
    public class ChangedResponse : DescantNodeComponent, IInvokedDescantComponent, IResponseNodeComponent
    {
        public DescantActor Actor;
        public ComparisonType TypeComparison;
        public string Name;
        public OperationType TypeOperation;
        public float Requirement;
        public string Change;
        
        public ChangedResponse(
            DescantConversationController controller,
            int nodeID,
            int id, DescantActor actor, ComparisonType typeComparison, string name, OperationType typeOperation, float requirement, string change)
            : base(controller, nodeID, id, float.PositiveInfinity)
        {
            Actor = actor;
            TypeComparison = typeComparison;
            Name = name;
            TypeOperation = typeOperation;
            Requirement = requirement;
            Change = change;
        }

        public bool Compare(float a)
        {
            switch (TypeOperation)
            {
                case OperationType.LessThan:
                    return a < Requirement;
                
                case OperationType.LessThanOrEqualTo:
                    return a <= Requirement;
                
                case OperationType.EqualTo:
                    return a == Requirement;
                
                case OperationType.GreaterThanOrEqualTo:
                    return a >= Requirement;
                
                case OperationType.GreaterThan:
                    return a > Requirement;
                
                case OperationType.NotEqualTo:
                    return a != Requirement;
                
                default: return false;
            }
        }

        public void Invoke()
        {
            switch (TypeComparison)
            {
                case ComparisonType.Statistic:
                    if (Compare(Actor.Statistics[Name]))
                        Controller.CurrentText[0] = Change;
                    break;
                
                case ComparisonType.Topic:
                    if (Actor.Topics.Contains(Name))
                        Controller.CurrentText[0] = Change;
                    break;
                
                case ComparisonType.Relationship:
                    if (Compare(Actor.Relationships[Name]))
                        Controller.CurrentText[0] = Change;
                    break;
                
                case ComparisonType.ReAttempts:
                    if (Compare(Actor.ReAttempts))
                        Controller.CurrentText[0] = Change;
                    break;
            }
        }
    }
}