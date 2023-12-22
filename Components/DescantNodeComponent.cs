using System;
using System.Collections.Generic;

namespace DescantComponents
{
    public enum VariableType { Statistic, Topic, Relationship, DialogueAttempts }
    public enum ComparisonType { LessThan, LessThanOrEqualTo, EqualTo, GreaterThanOrEqualTo, GreaterThan, NotEqualTo }
    public enum OperationType { IncreaseBy, DecreaseBy, Set }
    public enum ListChangeType { Add, Remove }

    public class DescantNodeInvokeResult
    {
        public List<KeyValuePair<int, string>> Text;
        public List<DescantActor> Actors;
        public string PlayerPortrait;
        public bool PlayerPortraitEnabled;
        public string NPCPortrait;
        public bool NPCPortraitEnabled;

        public DescantNodeInvokeResult(
            List<KeyValuePair<int, string>> text,
            List<DescantActor> actors,
            string playerPortrait,
            bool playerPortraitEnabled,
            string npcPortrait,
            bool npcPortraitEnabled)
        {
            Text = text;
            Actors = actors;
            PlayerPortrait = playerPortrait;
            PlayerPortraitEnabled = playerPortraitEnabled;
            NPCPortrait = npcPortrait;
            NPCPortraitEnabled = npcPortraitEnabled;
        }
    }
    
    [Serializable]
    public abstract class DescantNodeComponent
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
            try
            {
                var unused = (DescantNodeComponent) other;
            }
            catch
            {
                return false;
            }
            
            return Equals((DescantNodeComponent)other);
        }

        public bool Equals(DescantNodeComponent other)
        {
            return ToString().Equals(other.ToString());
        }
        
        protected static bool CompareVariable(float variable, float comparison, ComparisonType comparisonType)
        {
            switch (comparisonType)
            {
                case ComparisonType.LessThan:
                    return variable < comparison;
                
                case ComparisonType.LessThanOrEqualTo:
                    return variable <= comparison;
                
                case ComparisonType.EqualTo:
                    return variable == comparison;
                
                case ComparisonType.GreaterThanOrEqualTo:
                    return variable >= comparison;
                
                case ComparisonType.GreaterThan:
                    return variable > comparison;
                
                case ComparisonType.NotEqualTo:
                    return variable != comparison;
                
                default: return false;
            }
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
    
    public class NoFilteringAttribute : Attribute { }
}