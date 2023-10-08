using System.Linq;
using Editor.Window;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Nodes
{
    /// <summary>
    /// DescantNode used to indicate the beginning of a DescantGraph
    /// </summary>
    public class DescantStartNode : DescantNode
    {
        public DescantStartNode(
            DescantGraphView graphView,
            Vector2 position)
            : base(graphView, position)
        {
            Type = DescantNodeType.Start;
        }
        
        /// <summary>
        /// Initializes this node's VisualElements
        /// </summary>
        public new void Draw()
        {
            base.Draw(); // Making sure that the parent has been drawn
            
            style.width = 250;
            
            // Initializing the output port
            Port output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            output.portName = "";
            output.name = "Start";
            outputContainer.Add(output);
            
            // Adding a callback for when the port is released
            // (presumably after a new connection has been made)
            output.RegisterCallback<MouseUpEvent>(callback =>
            {
                GraphView.CheckAndSave(); // Check for autosave
                
                // Adding a callback to the new connection itself, to trigger when it is deleted
                output.connections.ElementAt(output.connections.Count() - 1)
                    .RegisterCallback<MouseUpEvent>(callback =>
                {
                    GraphView.CheckAndSave(); // Check for autosave
                });
            });
        }
    }
}