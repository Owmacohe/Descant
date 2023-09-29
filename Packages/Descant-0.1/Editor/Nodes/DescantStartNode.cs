using System.Linq;
using Editor.Window;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Nodes
{
    public class DescantStartNode : DescantNode
    {
        public DescantStartNode(
            DescantGraphView graphView,
            Vector2 position)
            : base(graphView, position)
        {
            Type = NodeType.Start;
        }
        
        public new void Draw()
        {
            base.Draw();
            
            style.width = 250;
            
            Port output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            output.portName = "";
            output.name = "Start";
            outputContainer.Add(output);
            
            output.RegisterCallback<MouseUpEvent>(callback =>
            {
                graphView.CheckAndSave();
                
                output.connections.ElementAt(output.connections.Count() - 1).RegisterCallback<MouseUpEvent>(callback =>
                {
                    graphView.CheckAndSave();
                });
            });
        }
    }
}