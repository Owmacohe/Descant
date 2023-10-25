using System;
using Editor.Nodes;
using Runtime;

namespace Components
{
    public enum ComparisonType { Statistic, Topic, Relationship, ReAttempts }
    public enum OperationType { LessThan, LessThanOrEqualTo, EqualTo, GreaterThanOrEqualTo, GreaterThan, NotEqualTo }
    public enum ValueChangeType { IncreaseBy, DecreaseBy, Set }
    public enum ListChangeType { Add, Remove }
    
    [Serializable]
    public abstract class DescantNodeComponent : DescantComponent
    {
        public int NodeID;
        
        protected DescantNodeComponent(
            DescantConversationController controller,
            int nodeID,
            int id,
            float max)
            : base(controller, id, max)
        {
            NodeID = nodeID;
        }
        
        public override string ToString()
        {
            return base.ToString() + " (" + ID + ")";
        }
    }
}