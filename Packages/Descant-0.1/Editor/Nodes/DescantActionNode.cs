using System.Linq;
using Editor.Window;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace Editor.Nodes
{
    public enum ActionNodeType { Choice, Response }

    public class DescantActionNode : Node
    {
        public ActionNodeType Type { get; protected set; }
        public int ID { get; protected set; }
        protected DescantGraphView graphView;

        protected DescantActionNode(DescantGraphView graphView, Vector2 position)
        {
            this.graphView = graphView;
            SetPosition(new Rect(position, Vector2.zero));
        }

        protected void Draw()
        {
            titleContainer.style.height = 70;

            VisualElement names = new VisualElement();
            names.AddToClassList("names");
            titleContainer.Insert(0, names);

            TextElement nodeName = new TextElement();
            nodeName.text = Type.ToString()[0].ToString();
            nodeName.AddToClassList("node-name");
            names.Add(nodeName);
            
            TextField customName = new TextField();
            customName.value = Type + " Node";
            //customName.label = "Custom name";
            names.Add(customName);
            
            Button removeNode = new Button();
            removeNode.text = "X";
            removeNode.clicked += RemoveNode;
            titleContainer.Insert(1, removeNode);
        }

        void RemoveNode()
        {
            graphView.DisconnectPorts(inputContainer);
            graphView.DisconnectPorts(outputContainer);
            parent.Remove(this);
        }
    }
}