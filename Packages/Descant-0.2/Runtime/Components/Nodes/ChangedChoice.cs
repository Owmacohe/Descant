using System;
using Editor.Nodes;

namespace Runtime.Components.Nodes
{
    public class ChangedChoice : DescantNodeComponent, IInvokedDescantComponent
    {
        public int Choice { get; } // base 1
        public DescantActor Actor { get; }
        public ComparisonType TypeComparison { get; }
        public string Name { get; }
        public OperationType TypeOperation { get; }
        public float Requirement { get; }
        public string Change { get; }
        
        public ChangedChoice(DescantConversationController controller, int id, int choice, DescantActor actor, ComparisonType typeComparison, string name, OperationType typeOperation, float requirement, string change)
            : base(controller, id, DescantNodeType.Choice, float.PositiveInfinity)
        {
            Choice = choice;
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
                        Controller.CurrentText[Choice] = Change;
                    break;
                
                case ComparisonType.Topic:
                    if (Actor.Topics.Contains(Name))
                        Controller.CurrentText[Choice] = Change;
                    break;
                
                case ComparisonType.Relationship:
                    if (Compare(Actor.Relationships[Name]))
                        Controller.CurrentText[Choice] = Change;
                    break;
                
                case ComparisonType.ReAttempts:
                    if (Compare(Actor.ReAttempts))
                        Controller.CurrentText[Choice] = Change;
                    break;
            }
        }
    }
}