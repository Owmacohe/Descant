using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Editor.Window;

namespace Editor.Nodes
{
    public class DescantResponseNode : DescantNode
    {
        public DescantResponseNode(
            DescantGraphView graphView,
            Vector2 position)
            : base(graphView, position)
        {
            Type = NodeType.Response;
        }

        public new void Draw()
        {
            base.Draw();
            
            if (ID < 0)
            {
                ID = graphView.ResponseNodeID;
                graphView.ResponseNodeID++;
            }
            
            style.width = 500;

            Port input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            input.portName = "";
            input.name = "Response";
            inputContainer.Add(input);
            
            input.RegisterCallback<MouseUpEvent>(callback =>
            {
                graphView.CheckAndSave();
                
                input.connections.ElementAt(input.connections.Count() - 1).RegisterCallback<MouseUpEvent>(callback =>
                {
                    graphView.CheckAndSave();
                });
            });

            Port output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            output.portName = "";
            output.name = "Response";
            outputContainer.Add(output);
            
            output.RegisterCallback<MouseUpEvent>(callback =>
            {
                graphView.CheckAndSave();
                
                output.connections.ElementAt(output.connections.Count() - 1).RegisterCallback<MouseUpEvent>(callback =>
                {
                    graphView.CheckAndSave();
                });
            });

            TextField response = new TextField();
            response.multiline = true;
            extensionContainer.Add(response);

            response.RegisterValueChangedCallback(callback =>
            {
                graphView.CheckAndSave();
                
            });
            
            RefreshExpandedState();
        }

        public void SetResponse(string response)
        {
            DescantUtilities.FindAllElements<TextField>(this)[1].value = response;
        }
    }
}