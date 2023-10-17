using Editor.Nodes;

namespace Runtime.Components.Nodes
{
    public abstract class DescantNodeComponent : DescantComponent
    {
        DescantNodeType Type { get; }
        
        protected DescantNodeComponent(DescantGraphController controller, int id, DescantNodeType type, float max)
            : base(controller, id, max)
        {
            Type = type;
        }
    }
}