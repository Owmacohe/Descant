using System;
using System.Collections.Generic;
using Editor.Nodes;

namespace Runtime.Components.Nodes
{
    public enum LockedChoiceType { Topic, Statistic, Timed, ReAttempts, Relationship }
    
    public class LockedChoice : DescantNodeComponent
    {
        public int Choice { get; } // base 1
        public LockedChoiceType Type { get; }
        public ValueType ValueRequirement { get; }
        public string StringRequirement { get; }
        
        public LockedChoice(DescantGraphController controller, int id, int choice, LockedChoiceType type, ValueType valueRequirement, string stringRequirement = "")
            : base(controller, id, DescantNodeType.Choice, float.PositiveInfinity)
        {
            Choice = choice;
            Type = type;
            ValueRequirement = valueRequirement;
            StringRequirement = stringRequirement;
        }

        public bool Locked(ValueType checkingValue, List<string> checkingString = null)
        {
            return checkingString == null
                ? (float)checkingValue < (float)ValueRequirement
                : !checkingString.Contains(StringRequirement);
        }
    }
}