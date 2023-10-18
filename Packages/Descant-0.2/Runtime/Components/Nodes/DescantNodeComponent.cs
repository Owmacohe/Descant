using Editor.Nodes;

namespace Runtime.Components.Nodes
{
    public enum ComparisonType { Statistic, Topic, Relationship, ReAttempts }
    public enum OperationType { LessThan, LessThanOrEqualTo, EqualTo, GreaterThanOrEqualTo, GreaterThan, NotEqualTo }
    public enum ValueChangeType { IncreaseBy, DecreaseBy, Set }
    public enum ListChangeType { Add, Remove }
    
    public abstract class DescantNodeComponent : DescantComponent
    {
        DescantNodeType Type { get; }
        
        protected DescantNodeComponent(DescantConversationController controller, int id, DescantNodeType type, float max)
            : base(controller, id, max)
        {
            Type = type;
        }
    }
}