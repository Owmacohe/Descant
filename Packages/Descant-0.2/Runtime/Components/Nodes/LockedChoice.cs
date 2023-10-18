using System;
using Editor.Nodes;

namespace Runtime.Components.Nodes
{
    public class LockedChoice : DescantNodeComponent, IInvokedDescantComponent
    {
        public int Choice { get; } // base 0
        public DescantActor Actor { get; }
        public ComparisonType TypeComparison { get; }
        public string Name { get; }
        public OperationType TypeOperation { get; }
        public float Requirement { get; }
        
        public LockedChoice(DescantConversationController controller, int id, int choice, DescantActor actor, ComparisonType typeComparison, string name, OperationType typeOperation, float requirement)
            : base(controller, id, DescantNodeType.Choice, float.PositiveInfinity)
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