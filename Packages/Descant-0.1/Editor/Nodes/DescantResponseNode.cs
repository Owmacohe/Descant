using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Editor.Window;

namespace Editor.Nodes
{
    public class DescantResponseNode : DescantActionNode
    {
        public DescantResponseNode(DescantGraphView graphView, Vector2 position) : base(graphView, position)
        {
            Type = ActionNodeType.Response;
            
            ID = graphView.ResponseNodeID;
            graphView.ResponseNodeID++;
        }

        public new void Draw()
        {
            base.Draw();
            
            style.width = 500;

            Port input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            input.portName = "";
            input.name = "Response";
            inputContainer.Add(input);
            
            Port output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            output.portName = "";
            output.name = "Response";
            outputContainer.Add(output);
            
            TextField response = new TextField();
            response.multiline = true;
            //response.label = "Response text";
            extensionContainer.Add(response);
            
            RefreshExpandedState();
        }
    }
}