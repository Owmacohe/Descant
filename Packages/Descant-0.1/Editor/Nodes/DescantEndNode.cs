using System.Linq;
using Editor.Window;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Nodes
{
    public class DescantEndNode : DescantNode
    {
        public DescantEndNode(
            DescantGraphView graphView,
            Vector2 position)
            : base(graphView, position)
        {
            Type = NodeType.End;
        }
        
        public new void Draw()
        {
            base.Draw();
            
            if (ID < 0)
            {
                ID = graphView.EndNodeID;
                graphView.EndNodeID++;
            }
            
            style.width = 250;
            
            Port input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            input.portName = "";
            input.name = "End";
            inputContainer.Add(input);
            
            input.RegisterCallback<MouseUpEvent>(callback =>
            {
                graphView.CheckAndSave();
                
                input.connections.ElementAt(input.connections.Count() - 1).RegisterCallback<MouseUpEvent>(callback =>
                {
                    graphView.CheckAndSave();
                }); 
            });
        }
    }
}