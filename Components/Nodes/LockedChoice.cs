using System;

namespace DescantComponents
{
    [Serializable, MaxQuantity(Single.PositiveInfinity), NodeType(DescantNodeType.Choice)]
    public class LockedChoice : DescantNodeComponent, IInvokedDescantComponent
    {
        [Inline] public string ActorName;
        
        [ParameterGroup("Index of the choice to change (base 1)")] public int ChoiceNumber;
        
        [ParameterGroup("Variable to check")] public VariableType VariableType;
        [ParameterGroup("Variable to check")] public string VariableName;
        
        [ParameterGroup("Comparison to make")] public ComparisonType ComparisonType;
        [ParameterGroup("Comparison to make")] public float Comparison;
        
        public LockedChoice(
            //DescantConversationController controller,
            int nodeID,
            int id,
            int choiceNumber, string actorName, VariableType variableType, string variableName, ComparisonType comparisonType, float comparison)
            : base(nodeID, id)
        {
            ChoiceNumber = choiceNumber;
            ActorName = actorName;
            VariableType = variableType;
            VariableName = variableName;
            ComparisonType = comparisonType;
            Comparison = comparison;
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
            /*
            DescantActor actor = Controller.GetActor(ActorName);
            
            switch (VariableType)
            {
                case VariableType.Statistic:
                    if (Compare(actor.Statistics[VariableName]))
                        Controller.CurrentText.RemoveAt(ChoiceNumber);
                    break;
                
                case VariableType.Topic:
                    if (actor.Topics.Contains(VariableName))
                        Controller.CurrentText.RemoveAt(ChoiceNumber);
                    break;
                
                case VariableType.Relationship:
                    if (Compare(actor.Relationships[VariableName]))
                        Controller.CurrentText.RemoveAt(ChoiceNumber);
                    break;
                
                case VariableType.ReAttempts:
                    if (Compare(actor.ReAttempts))
                        Controller.CurrentText.RemoveAt(ChoiceNumber);
                    break;
            }
            */
        }
    }
}