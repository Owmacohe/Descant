using Editor.Window;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Editor.Nodes
{
    public class DescantEndNode : DescantScopeNode
    {
        public int ID { get; }
        
        public DescantEndNode(DescantGraphView graphView, Vector2 position) : base(graphView, position)
        {
            Type = ScopeNodeType.End;
            
            ID = graphView.EndNodeID;
            graphView.EndNodeID++;
        }
        
        public new void Draw()
        {
            base.Draw();
            
            style.width = 250;
            
            Port input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            input.portName = "";
            input.name = "End";
            inputContainer.Add(input);
        }
    }
}