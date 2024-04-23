#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Descant.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Descant.Editor
{
    /// <summary>
    /// DescantNode used to indicate player choice/statement, with a list of possible choices to make
    /// </summary>
    public class DescantChoiceNode : DescantNode
    {
        // A list of the TextElements indicating the numbers of the choices
        List<TextElement> choiceNumbers = new List<TextElement>();
        
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="graphView">The GraphView for this editor window</param>
        /// <param name="position">The position to spawn the node at</param>
        public DescantChoiceNode(
            DescantGraphView graphView,
            Vector2 position)
            : base(graphView, position)
        {
            Type = DescantNodeType.Choice;
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
                ID = GraphView.ChoiceNodeID;
                GraphView.ChoiceNodeID++;
            }

            style.width = 500;
            
            // Initializing the input port
            Port input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            input.portName = "";
            input.name = "Choice";
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

            // Initializing the button to add new possible choices
            Button addChoice = new Button();
            addChoice.text = "Add Choice";
            addChoice.AddToClassList("add_choice");
            addChoice.clicked += () => AddChoice(choiceNumbers.Count + 1);
            extensionContainer.Insert(0, addChoice);
            
            // Refreshing the extensionContainer after new elements have been added to it
            RefreshExpandedState();
        }

        /// <summary>
        /// Adds a new possible choice to the list in the node
        /// </summary>
        /// <param name="index">The base-1 index of this choice</param>
        /// <param name="choice">The text that should go in this choice's field</param>
        public void AddChoice(int index, string choice = "")
        {
            // Initializing the new choice's output port
            Port output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            output.portName = "";
            output.name = "Choice";
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

            // Initializing the new choice's text field
            TextField outputField = new TextField();
            outputField.value = choice;
            outputField.multiline = true;
            output.Add(outputField);

            // Adding a callback for when the choice text is changed
            outputField.RegisterValueChangedCallback(callback =>
            {
                GraphView.Editor.CheckAndSave(); // Check for autosave
            });

            // Initializing the new choice's removal button
            Button removeChoice = new Button();
            removeChoice.text = "X";
            removeChoice.clicked += () => RemoveChoice(output);
            output.Add(removeChoice);
            
            TextElement choiceNumber = new TextElement();
            choiceNumber.text = index.ToString();
            choiceNumber.AddToClassList("choice_number");
            output.Add(choiceNumber);
            
            choiceNumbers.Add(choiceNumber);
            
            // Refreshing the extensionContainer after new elements have been added to it
            RefreshExpandedState();
            
            if (choice.Equals("")) GraphView.Editor.CheckAndSave(); // Check for autosave
        }

        /// <summary>
        /// Method to remove a choice from the node, and perform all the necessary checks afterwards
        /// </summary>
        /// <param name="output">The port for the choice to be removed</param>
        void RemoveChoice(Port output)
        {
            // Disconnecting all the connections to the port
            GraphView.DisconnectPorts(outputContainer, output);
            
            choiceNumbers.RemoveAt(outputContainer.IndexOf(output));

            for (int i = 0; i < choiceNumbers.Count; i++)
                choiceNumbers[i].text = (i + 1).ToString();
            
            outputContainer.Remove(output); // Actually removing the choice
            
            GraphView.Editor.CheckAndSave(); // Check for autosave
        }
    }
}
#endif