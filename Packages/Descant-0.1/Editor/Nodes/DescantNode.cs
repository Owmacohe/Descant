using Editor.Window;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Nodes
{
    public enum NodeType { Choice, Response, Start, End }
    
    public class DescantNode : Node
    {
        public NodeType Type { get; protected set; }
        public int ID { get; set; }
        public string Name { get; set; }
        
        protected DescantGraphView graphView;
        
        protected DescantNode(
            DescantGraphView graphView,
            Vector2 position)
        {
            this.graphView = graphView;
            SetPosition(new Rect(position, Vector2.zero));
        }
        
        protected void Draw()
        {
            if (Name == "") Name = Type + "Node";
            
            titleContainer.style.height = 70;

            VisualElement names = new VisualElement();
            names.AddToClassList("names");
            titleContainer.Insert(0, names);

            TextElement nodeName = new TextElement();
            nodeName.text = Type.ToString()[0].ToString();
            nodeName.AddToClassList("node-name");
            names.Add(nodeName);
            
            TextField customName = new TextField();
            customName.value = Name;
            names.Add(customName);

            customName.RegisterValueChangedCallback(callback =>
            {
                TextField target = (TextField) callback.target;
                target.value = DescantUtilities.FilterText(target.value);
                
                graphView.CheckAndSave();
            });

            if (GetType() != typeof(DescantStartNode))
            {
                Button removeNode = new Button();
                removeNode.text = "X";
                removeNode.clicked += RemoveNode;
                titleContainer.Insert(1, removeNode);
            
                RegisterCallback(new EventCallback<MouseLeaveEvent>(callback =>
                {
                    graphView.CheckAndSave();
                }));   
            }
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Delete", actionEvent => { RemoveNode(); });
        }
        
        void RemoveNode()
        {
            graphView.DisconnectPorts(inputContainer);
            graphView.DisconnectPorts(outputContainer);
            
            DescantUtilities.RemoveElement(this);

            if (Type.Equals(NodeType.Choice)) graphView.ChoiceNodes.Remove((DescantChoiceNode)this);
            else if (Type.Equals(NodeType.Response)) graphView.ResponseNodes.Remove((DescantResponseNode)this);
            
            if (Type.Equals(NodeType.Start))
            {
                graphView.RemoveContextMenuManipulators();
                graphView.AddContextMenuManipulators();
            }
            
            foreach (var i in graphView.Groups)
                i.UpdateGeometryFromContent();
            
            graphView.CheckAndSave();
        }
    }
}