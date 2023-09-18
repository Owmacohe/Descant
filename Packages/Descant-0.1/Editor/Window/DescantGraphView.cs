using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Editor.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Window
{
    public class DescantGraphView : GraphView
    {
        public int ChoiceNodeID { get; set; }
        public int ResponseNodeID { get; set; }
        public int EndNodeID { get; set; }

        IManipulator startNodeManipulator;
        List<IManipulator> contextMenuManipulators = new List<IManipulator>();

        public DescantGraphView()
        {
            AddGridBackground();
            AddManipulators();
            
            AddStyleSheet();
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort == port) return;
                if (startPort.node == port.node) return;
                if (startPort.direction == port.direction) return;
                if (startPort.name == port.name) return;
                
                compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();
            gridBackground.StretchToParentSize();
            
            Insert(0, gridBackground);
        }

        public void RemoveContextMenuManipulators()
        {
            foreach (var i in contextMenuManipulators)
                this.RemoveManipulator(i);
            
            contextMenuManipulators.Clear();

            startNodeManipulator = null;
        }

        public void AddContextMenuManipulators()
        {
            this.AddManipulator(CreateActionNodeContextualMenu("Add Choice Node", ActionNodeType.Choice));
            this.AddManipulator(CreateActionNodeContextualMenu("Add Response Node", ActionNodeType.Response));
            this.AddManipulator(CreateScopeNodeContextualMenu("Add Start Node", ScopeNodeType.Start));
            this.AddManipulator(CreateScopeNodeContextualMenu("Add End Node", ScopeNodeType.End));

            this.AddManipulator(CreateGroupContextualMenu());
        }
        
        void AddManipulators()
        {
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            AddContextMenuManipulators();
        }

        IManipulator CreateActionNodeContextualMenu(string actionTitle, ActionNodeType type)
        {
            ContextualMenuManipulator context = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(actionTitle,
                actionEvent => AddElement(type.Equals(ActionNodeType.Choice)
                    ? CreateChoiceNode(actionEvent.eventInfo.localMousePosition)
                    : CreateResponseNode(actionEvent.eventInfo.localMousePosition))
                )
            );
            
            contextMenuManipulators.Add(context);

            return context;
        }
        
        IManipulator CreateScopeNodeContextualMenu(string actionTitle, ScopeNodeType type)
        {
            ContextualMenuManipulator context = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(actionTitle,
                actionEvent => AddElement(type.Equals(ScopeNodeType.Start)
                    ? CreateStartNode(actionEvent.eventInfo.localMousePosition)
                    : CreateEndNode(actionEvent.eventInfo.localMousePosition))
                )
            );

            if (startNodeManipulator == null) startNodeManipulator = context;
            contextMenuManipulators.Add(context);

            return context;
        }
        
        IManipulator CreateGroupContextualMenu()
        {
            ContextualMenuManipulator context = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Add Group",
                    actionEvent => AddElement(CreateGroup(actionEvent.eventInfo.localMousePosition))
                )
            );
            
            contextMenuManipulators.Add(context);

            return context;
        }

        DescantChoiceNode CreateChoiceNode(Vector2 position)
        {
            DescantChoiceNode choiceNode = new DescantChoiceNode(this, position);
            choiceNode.Draw();

            return choiceNode;
        }
        
        DescantResponseNode CreateResponseNode(Vector2 position)
        {
            DescantResponseNode responseNode = new DescantResponseNode(this, position);
            responseNode.Draw();

            return responseNode;
        }
        
        DescantStartNode CreateStartNode(Vector2 position)
        {
            DescantStartNode startNode = new DescantStartNode(this, position);
            startNode.Draw();
            
            this.RemoveManipulator(startNodeManipulator);
            contextMenuManipulators.Remove(startNodeManipulator);

            return startNode;
        }
        
        DescantEndNode CreateEndNode(Vector2 position)
        {
            DescantEndNode endNode = new DescantEndNode(this, position);
            endNode.Draw();

            return endNode;
        }
        
        DescantNodeGroup CreateGroup(Vector2 position)
        {
            DescantNodeGroup group = new DescantNodeGroup(position);
            group.Draw();
            
            foreach (var i in selection)
                if (i.GetType() == typeof(DescantChoiceNode) ||
                    i.GetType() == typeof(DescantResponseNode) ||
                    i.GetType() == typeof(DescantStartNode) ||
                    i.GetType() == typeof(DescantEndNode))
                    group.AddElement((GraphElement)i);

            return group;
        }

        void AddStyleSheet()
        {
            StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("Packages/Descant/Assets/DescantStyleSheet.uss");
            styleSheets.Add(styleSheet);
        }
        
        public void DisconnectPorts(VisualElement container, Port onlyThisPort = null)
        {
            foreach (Port i in container.Children())
                if (i.connected && (onlyThisPort == null || i.Equals(onlyThisPort)))
                    DeleteElements(i.connections);
        }
        
        public string FilterText(string text)
        {
            string special = "/\\`~!@#$%^*()+={}[]|;:'\",.<>?";
            
            text = text.Trim();

            for (int i = 0; i < text.Length; i++)
                if (special.Contains(text[i]))
                    text = text.Remove(i);

            return text;
        }
    }
}