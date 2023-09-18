using System.Linq;
using Editor.Window;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Nodes
{
    public class DescantChoiceNode : DescantActionNode
    {
        int initialChoices;

        public DescantChoiceNode(DescantGraphView graphView, Vector2 position) : base(graphView, position)
        {
            Type = ActionNodeType.Choice;
            initialChoices = 3;

            ID = graphView.ChoiceNodeID;
            graphView.ChoiceNodeID++;
        }

        public new void Draw()
        {
            base.Draw();

            style.width = 500;
            
            Port input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            input.portName = "";
            input.name = "Choice";
            inputContainer.Add(input);

            for (int i = 0; i < initialChoices; i++)
                AddChoice();

            Button addChoice = new Button();
            addChoice.text = "Add Choice";
            extensionContainer.Add(addChoice);

            addChoice.clicked += AddChoice;
            
            RefreshExpandedState();
        }

        void AddChoice()
        {
            Port output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            output.portName = "";
            output.name = "Choice";
            outputContainer.Add(output);

            TextField outputField = new TextField();
            outputField.multiline = true;
            //outputField.label = "Choice text";
            output.Add(outputField);

            Button removeChoice = new Button();
            removeChoice.text = "X";
            removeChoice.clicked += () => RemoveChoice(output);
            output.Add(removeChoice);
        }

        void RemoveChoice(Port output)
        {
            graphView.DisconnectPorts(outputContainer, output);
            outputContainer.Remove(output);
        }
    }
}