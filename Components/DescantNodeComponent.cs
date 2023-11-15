using System;
using System.Collections.Generic;
using DescantUtilities;

namespace DescantComponents
{
    public enum VariableType { Statistic, Topic, Relationship, ReAttempts }
    public enum ComparisonType { LessThan, LessThanOrEqualTo, EqualTo, GreaterThanOrEqualTo, GreaterThan, NotEqualTo }
    public enum OperationType { IncreaseBy, DecreaseBy, Set }
    public enum ListChangeType { Add, Remove }
    
    [Serializable]
    public abstract class DescantNodeComponent // TODO: find a way to have components interact with the controller at runtime
    {
        public bool Collapsed;

        public abstract List<string> Invoke(List<string> choices);
        public abstract void FixedUpdate();
        
        public override string ToString()
        {
            string temp = "";
            
            foreach (var i in GetType().GetFields())
                temp += " " + i.GetValue(this);

            return GetType() + " (" + (temp.Length > 1 ? temp.Substring(1) : "") + ")";
        }
    }
    
    public class MaxQuantityAttribute : Attribute
    {
        public readonly float Quantity;
        
        public MaxQuantityAttribute(float quantity) => Quantity = quantity;
    }

    public class NodeTypeAttribute : Attribute
    {
        public readonly DescantNodeType Type;
        
        public NodeTypeAttribute(DescantNodeType type) => Type = type;
    }
    
    public class InlineAttribute : Attribute { }
    
    public class ParameterGroupAttribute : Attribute
    {
        public readonly string Group;
        
        public ParameterGroupAttribute(string group) => Group = group;
    }
}