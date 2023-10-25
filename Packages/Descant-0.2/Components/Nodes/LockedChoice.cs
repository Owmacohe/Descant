using System;
using Editor.Nodes;
using Runtime;

namespace Components
{
    [Serializable]
    public class LockedChoice : DescantNodeComponent, IInvokedDescantComponent, IChoiceNodeComponent
    {
        public int Choice; // base 0
        public DescantActor Actor;
        public ComparisonType TypeComparison;
        public string Name;
        public OperationType TypeOperation;
        public float Requirement;
        
        public LockedChoice(
            DescantConversationController controller,
            int nodeID,
            int id,
            int choice, DescantActor actor, ComparisonType typeComparison, string name, OperationType typeOperation, float requirement)
            : base(controller, nodeID, id, float.PositiveInfinity)
        {
            Choice = choice;
            Actor = actor;
            TypeComparison = typeComparison;
            Name = name;
            TypeOperation = typeOperation;
            Requirement = requirement;
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
                        Controller.CurrentText.RemoveAt(Choice);
                    break;
                
                case ComparisonType.Topic:
                    if (Actor.Topics.Contains(Name))
                        Controller.CurrentText.RemoveAt(Choice);
                    break;
                
                case ComparisonType.Relationship:
                    if (Compare(Actor.Relationships[Name]))
                        Controller.CurrentText.RemoveAt(Choice);
                    break;
                
                case ComparisonType.ReAttempts:
                    if (Compare(Actor.ReAttempts))
                        Controller.CurrentText.RemoveAt(Choice);
                    break;
            }
        }
    }
}