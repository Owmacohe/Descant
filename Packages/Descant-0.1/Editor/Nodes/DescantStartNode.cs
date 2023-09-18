using Editor.Window;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Editor.Nodes
{
    public class DescantStartNode : DescantScopeNode
    {
        public DescantStartNode(DescantGraphView graphView, Vector2 position) : base(graphView, position)
        {
            Type = ScopeNodeType.Start;
        }
        
        public new void Draw()
        {
            base.Draw();
            
            style.width = 250;
            
            Port output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            output.portName = "";
            output.name = "Start";
            outputContainer.Add(output);
        }
    }
}