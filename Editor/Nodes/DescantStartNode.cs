#if UNITY_EDITOR
using System.Linq;
using Descant.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Descant.Editor
{
    /// <summary>
    /// DescantNode used to indicate the beginning of a DescantGraph
    /// </summary>
    public class DescantStartNode : DescantNode
    {
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="graphView">The GraphView for this editor window</param>
        /// <param name="position">The position to spawn the node at</param>
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
            
            style.minWidth = 350;
            
            // Initializing the output port
            Port output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            output.portName = "";
            output.name = "Start";
            outputContainer.Add(output);
            
            // Adding a callback for when the port is released
            // (presumably after a new connection has been made)
            output.RegisterCallback<MouseUpEvent>(callback =>
            {
                GraphView.Editor.CheckAndSave(); // Check for autosave

                if (output.connected)
                {
                    // Adding a callback to the new connection itself, to trigger when it is deleted
                    output.connections.ElementAt(output.connections.Count() - 1)
                        .RegisterCallback<MouseUpEvent>(callback =>
                    {
                        GraphView.Editor.CheckAndSave(); // Check for autosave
                    });
                }
            });
            
            // Refreshing the extensionContainer after new elements have been added to it
            RefreshExpandedState();
        }
    }
}
#endif