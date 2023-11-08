#if UNITY_EDITOR
using System.Linq;
using DescantEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DescantEditor
{
    /// <summary>
    /// DescantNode used to indicate the end of a DescantGraph
    /// </summary>
    public class DescantEndNode : DescantNode
    {
        public DescantEndNode(
            DescantGraphView graphView,
            Vector2 position)
            : base(graphView, position)
        {
            Type = DescantNodeType.End;
        }
        
        /// <summary>
        /// Initializes this node's VisualElements
        /// </summary>
        public new void Draw()
        {
            base.Draw(); // Making sure that the parent has been drawn
            
            // If this node is just being created, we set its ID
            if (ID < 0)
            {
                ID = GraphView.EndNodeID;
                GraphView.EndNodeID++;
            }
            
            style.width = 250;
            
            // Initializing the input port
            Port input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            input.portName = "";
            input.name = "End";
            inputContainer.Add(input);
            
            // Adding a callback for when the port is released
            // (presumably after a new connection has been made)
            input.RegisterCallback<MouseUpEvent>(callback =>
            {
                GraphView.Editor.CheckAndSave(); // Check for autosave

                if (input.connected)
                {
                    // Adding a callback to the new connection itself, to trigger when it is deleted
                    input.connections.ElementAt(input.connections.Count() - 1)
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