#if UNITY_EDITOR
using System.Linq;
using Descant.Components;
using Descant.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Descant.Editor
{
    /// <summary>
    /// DescantNode used to indicate a junction in the dialogue flow, based on some comparison
    /// </summary>
    public class DescantIfNode : DescantNode
    {
        public IfComponent IfComponent;
        
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="graphView">The GraphView for this editor window</param>
        /// <param name="position">The position to spawn the node at</param>
        public DescantIfNode(DescantGraphView graphView, Vector2 position) : base(graphView, position)
        {
            Type = DescantNodeType.If;
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
                ID = GraphView.IfNodeID;
                GraphView.IfNodeID++;
            }

            style.width = 500;
            
            // Initializing the input port
            Port input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            input.portName = "";
            input.name = "If";
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
            
            // Initializing the true output's port
            Port outputTrue = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            outputTrue.portName = "";
            outputTrue.name = "If";
            outputContainer.Add(outputTrue);
            
            // Adding a callback for when the port is released
            // (presumably after a new connection has been made)
            outputTrue.RegisterCallback<MouseUpEvent>(callback =>
            {
                GraphView.Editor.CheckAndSave(); // Check for autosave

                if (outputTrue.connected)
                {
                    // Adding a callback to the new connection itself, to trigger when it is deleted
                    outputTrue.connections.ElementAt(outputTrue.connections.Count() - 1)
                        .RegisterCallback<MouseUpEvent>(callback =>
                        {
                            GraphView.Editor.CheckAndSave(); // Check for autosave
                        });
                }
            });

            TextElement outputTrueText = new TextElement();
            outputTrueText.text = "True";
            outputTrue.Add(outputTrueText);
            
            // Initializing the false output's port
            Port outputFalse = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            outputFalse.portName = "";
            outputFalse.name = "If";
            outputContainer.Add(outputFalse);
            
            // Adding a callback for when the port is released
            // (presumably after a new connection has been made)
            outputFalse.RegisterCallback<MouseUpEvent>(callback =>
            {
                GraphView.Editor.CheckAndSave(); // Check for autosave

                if (outputFalse.connected)
                {
                    // Adding a callback to the new connection itself, to trigger when it is deleted
                    outputFalse.connections.ElementAt(outputFalse.connections.Count() - 1)
                        .RegisterCallback<MouseUpEvent>(callback =>
                        {
                            GraphView.Editor.CheckAndSave(); // Check for autosave
                        });
                }
            });
            
            TextElement outputFalseText = new TextElement();
            outputFalseText.text = "False";
            outputFalse.Add(outputFalseText);
            
            // Creating a DescantNodeComponentVisualElement to represent
            // the comparison that is made when the if node is reached
            DescantNodeComponentVisualElement temp = new DescantNodeComponentVisualElement(
                GraphView, 
                this,
                "IfComponent",
                0,
                IfComponent,
                true
            );
            
            temp.Draw();
            extensionContainer.Insert(0, temp);
            
            // Refreshing the extensionContainer after new elements have been added to it
            RefreshExpandedState();
        }
    }
}
#endif