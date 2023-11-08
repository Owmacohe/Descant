#if UNITY_EDITOR
using System.Linq;
using DescantEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DescantEditor
{
    /// <summary>
    /// DescantNode used to indicate NPC response/statement
    /// </summary>
    public class DescantResponseNode : DescantNode
    {
        public DescantResponseNode(
            DescantGraphView graphView,
            Vector2 position)
            : base(graphView, position)
        {
            Type = DescantNodeType.Response;
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
                ID = GraphView.ResponseNodeID;
                GraphView.ResponseNodeID++;
            }
            
            style.width = 500;

            // Initializing the input port
            Port input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            input.portName = "";
            input.name = "Response";
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

            // Initializing the output port
            Port output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            output.portName = "";
            output.name = "Response";
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

            // Initializing the big response text field
            TextField response = new TextField();
            response.multiline = true;
            response.AddToClassList("response");
            extensionContainer.Insert(0, response);

            // Adding a callback for when the response text is changed
            response.RegisterValueChangedCallback(callback =>
            {
                GraphView.Editor.CheckAndSave(); // Check for autosave
            });
            
            RefreshExpandedState();
        }
    }
}
#endif