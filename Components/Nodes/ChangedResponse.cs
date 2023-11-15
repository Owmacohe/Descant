using System;
using System.Collections.Generic;
using DescantUtilities;

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

        public override List<string> Invoke(List<string> choices)
        {
            /*
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
            */
            
            return choices;
        }

        public override void FixedUpdate()
        {
            
        }
    }
}