using System;
using System.Collections.Generic;

namespace DescantComponents
{
    public enum VariableType { Statistic, Topic, Relationship, ConversationAttempts }
    public enum ComparisonType { LessThan, LessThanOrEqualTo, EqualTo, GreaterThanOrEqualTo, GreaterThan, NotEqualTo }
    public enum OperationType { IncreaseBy, DecreaseBy, Set }
    public enum ListChangeType { Add, Remove }

    public class DescantNodeInvokeResult
    {
        public List<KeyValuePair<int, string>> Choices;
        public List<DescantActor> Actors;

        public DescantNodeInvokeResult(List<KeyValuePair<int, string>> choices, List<DescantActor> actors)
        {
            Choices = choices;
            Actors = actors;
        }
    }
    
    [Serializable]
    public abstract class DescantNodeComponent // TODO: find a way to have components interact with the controller at runtime
    {
        public bool Collapsed;

        public virtual DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            return result;
        }

        public virtual bool FixedUpdate()
        {
            return true;
        }
        
        public virtual bool Update()
        {
            return true;
        }

        public override bool Equals(object other)
        {
            return Equals((DescantNodeComponent)other);
        }

        public bool Equals(DescantNodeComponent other)
        {
            return ToString().Equals(other.ToString());
        }
        
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