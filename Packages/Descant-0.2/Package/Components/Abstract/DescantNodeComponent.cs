using System;
using Descant.Package.Editor.Nodes;
using Descant.Package.Runtime;

namespace Descant.Package.Components
{
    public enum VariableType { Statistic, Topic, Relationship, ReAttempts }
    public enum ComparisonType { LessThan, LessThanOrEqualTo, EqualTo, GreaterThanOrEqualTo, GreaterThan, NotEqualTo }
    public enum OperationType { IncreaseBy, DecreaseBy, Set }
    public enum ListChangeType { Add, Remove }
    
    [Serializable, NodeType(DescantNodeType.Any)]
    public abstract class DescantNodeComponent : DescantComponent
    {
        public int NodeID;
        
        protected DescantNodeComponent(
            DescantConversationController controller,
            int nodeID,
            int id)
            : base(controller, id)
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