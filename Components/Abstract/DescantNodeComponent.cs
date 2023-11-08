using System;

namespace DescantComponents
{
    public enum DescantNodeType { Choice, Response, Start, End, Any }
    public enum VariableType { Statistic, Topic, Relationship, ReAttempts }
    public enum ComparisonType { LessThan, LessThanOrEqualTo, EqualTo, GreaterThanOrEqualTo, GreaterThan, NotEqualTo }
    public enum OperationType { IncreaseBy, DecreaseBy, Set }
    public enum ListChangeType { Add, Remove }
    
    [Serializable, NodeType(DescantNodeType.Any)]
    public abstract class DescantNodeComponent : DescantComponent
    {
        public int NodeID;
        
        protected DescantNodeComponent(
            int nodeID,
            int id)
            : base(id)
        {
            NodeID = nodeID;
        }
        
        public override string ToString()
        {
            return base.ToString() + " (" + ID + ")";
        }
    }
    
    public class NodeTypeAttribute : Attribute
    {
        public readonly DescantNodeType Type;
        
        public NodeTypeAttribute(DescantNodeType type) => Type = type;
    }
}