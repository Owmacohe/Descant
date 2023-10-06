using System.Linq;
using Editor.Window;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Nodes
{
    public class DescantChoiceNode : DescantNode
    {
        public DescantChoiceNode(
            DescantGraphView graphView,
            Vector2 position)
            : base(graphView, position)
        {
            Type = NodeType.Choice;
        }

        public new void Draw()
        {
            base.Draw();
            
            if (ID < 0)
            {
                ID = graphView.ChoiceNodeID;
                graphView.ChoiceNodeID++;
            }

            style.width = 500;
            
            Port input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            input.portName = "";
            input.name = "Choice";
            inputContainer.Add(input);
            
            input.RegisterCallback<MouseUpEvent>(callback =>
            {
                graphView.CheckAndSave();
                
                input.connections.ElementAt(input.connections.Count() - 1).RegisterCallback<MouseUpEvent>(callback =>
                {
                    graphView.CheckAndSave();
                });
            });

            Button addChoice = new Button();
            addChoice.text = "Add Choice";
            extensionContainer.Add(addChoice);

            addChoice.clicked += () => AddChoice();
            
            RefreshExpandedState();
        }

        public void AddChoice(string choice = "")
        {
            Port output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            output.portName = "";
            output.name = "Choice";
            outputContainer.Add(output);
            
            output.RegisterCallback<MouseUpEvent>(callback =>
            {
                graphView.CheckAndSave();
                
                output.RegisterCallback<MouseUpEvent>(callback =>
                {
                    graphView.CheckAndSave();
                });
            });

            TextField outputField = new TextField();
            outputField.value = choice;
            outputField.multiline = true;
            output.Add(outputField);

            outputField.RegisterValueChangedCallback(callback =>
            {
                graphView.CheckAndSave();
            });

            Button removeChoice = new Button();
            removeChoice.text = "X";
            removeChoice.clicked += () => RemoveChoice(output);
            output.Add(removeChoice);
            
            RefreshExpandedState();
            
            graphView.CheckAndSave();
        }

        void RemoveChoice(Port output)
        {
            graphView.DisconnectPorts(outputContainer, output);
            outputContainer.Remove(output);
            
            graphView.CheckAndSave();
        }
    }
}