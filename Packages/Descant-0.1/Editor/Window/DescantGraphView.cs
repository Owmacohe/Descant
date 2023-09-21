using System.Collections.Generic;
using Editor.Data;
using Editor.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Window
{
    public class DescantGraphView : GraphView
    {
        public DescantEditor Editor;
        
        public int ChoiceNodeID { get; set; }
        public int ResponseNodeID { get; set; }
        public int EndNodeID { get; set; }
        public int GroupID { get; set; }

        public List<DescantChoiceNode> ChoiceNodes = new List<DescantChoiceNode>();
        public List<DescantResponseNode> ResponseNodes = new List<DescantResponseNode>();
        public DescantStartNode StartNode;
        public List<DescantEndNode> EndNotes = new List<DescantEndNode>();
        
        public List<DescantNodeGroup> Groups = new List<DescantNodeGroup>();

        IManipulator startNodeManipulator;
        List<IManipulator> contextMenuManipulators = new List<IManipulator>();

        public DescantGraphView(DescantEditor editor)
        {
            Editor = editor;
            
            AddGridBackground();
            AddManipulators();
            
            AddStyleSheet();

            DescantGraphData data = Editor.data;

            ChoiceNodeID = data.ChoiceNodeID;
            ResponseNodeID = data.ResponseNodeID;
            EndNodeID = data.EndNodeID;
            GroupID = data.GroupID;

            foreach (var i in data.ChoiceNodes)
            {
                var temp = CreateChoiceNode(i.Position, i.Name, i.ID);

                foreach (var ii in i.Choices)
                    temp.AddChoice(ii);
                
                AddElement(temp);
            }

            foreach (var j in data.ResponseNodes)
            {
                var temp = CreateResponseNode(j.Position, j.Name, j.ID);
                temp.SetResponse(j.Response);
                
                AddElement(temp);
            }

            if (data.StartNode != null)
                AddElement(CreateStartNode(
                    data.StartNode.Position,
                    data.StartNode.Name,
                    data.StartNode.ID
                ));
            else AddElement(CreateStartNode(new Vector2(50, 70)));
            
            this.RemoveManipulator(startNodeManipulator);
            
            foreach (var k in data.EndNodes)
                AddElement(CreateEndNode(k.Position, k.Name, k.ID));
            
            foreach (var l in data.Groups)
            {
                var temp = CreateGroup(l.Position, l.Name, l.ID);

                for (int li = 0; li < l.Nodes.Count; li++)
                    temp.AddElement(FindNode(l.Nodes[li], l.NodeIDs[li]));
                
                AddElement(temp);
            }
            
            foreach (var m in data.Connections)
            {
                var from = FindNode(m.From, m.FromID);
                var to = FindNode(m.To, m.ToID);

                int fromPortIndex = from.Type.Equals(NodeType.Start) ? 0 : 1;
                if (from.Type.Equals(NodeType.Choice)) fromPortIndex = m.ChoiceIndex;

                Port fromPort = DescantUtilities.FindAllElements<Port>(from)[fromPortIndex];
                Port toPort = DescantUtilities.FindFirstElement<Port>(to);

                Add(fromPort.ConnectTo(toPort));
            }
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
            this.AddManipulator(CreateNodeContextualMenu("Add Choice Node", NodeType.Choice));
            this.AddManipulator(CreateNodeContextualMenu("Add Response Node", NodeType.Response));
            this.AddManipulator(CreateNodeContextualMenu("Add Start Node", NodeType.Start));
            this.AddManipulator(CreateNodeContextualMenu("Add End Node", NodeType.End));

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
        
        IManipulator CreateNodeContextualMenu(string actionTitle, NodeType type)
        {
            ContextualMenuManipulator context = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(actionTitle,
                    actionEvent =>
                    {
                        switch (type)
                        {
                            case NodeType.Choice:
                                AddElement(CreateChoiceNode(actionEvent.eventInfo.localMousePosition));
                                break;
                            
                            case NodeType.Response:
                                AddElement(CreateResponseNode(actionEvent.eventInfo.localMousePosition));
                                break;
                            
                            case NodeType.Start:
                                AddElement(CreateStartNode(actionEvent.eventInfo.localMousePosition));
                                break;

                            case NodeType.End:
                                AddElement(CreateEndNode(actionEvent.eventInfo.localMousePosition));
                                break;

                        }
                        
                        CheckAndSave();
                    })
            );
            
            if (startNodeManipulator == null && type.Equals(NodeType.Start)) startNodeManipulator = context;
            contextMenuManipulators.Add(context);

            return context;
        }
        
        IManipulator CreateGroupContextualMenu()
        {
            ContextualMenuManipulator context = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Add Group",
                    actionEvent =>
                    {
                        AddElement(CreateGroup(actionEvent.eventInfo.localMousePosition));
                        CheckAndSave();
                    })
            );
            
            contextMenuManipulators.Add(context);

            return context;
        }

        DescantChoiceNode CreateChoiceNode(Vector2 nodePosition, string nodeName = "", int nodeID = -1)
        {
            var choiceNode = new DescantChoiceNode(this, nodePosition);

            choiceNode.Name = nodeName;
            choiceNode.ID = nodeID;

            choiceNode.Draw();
            
            ChoiceNodes.Add(choiceNode);

            return choiceNode;
        }
        
        DescantResponseNode CreateResponseNode(Vector2 nodePosition, string nodeName = "", int nodeID = -1)
        {
            var responseNode = new DescantResponseNode(this, nodePosition);
            
            responseNode.Name = nodeName;
            responseNode.ID = nodeID;

            responseNode.Draw();
            
            ResponseNodes.Add(responseNode);

            return responseNode;
        }
        
        DescantStartNode CreateStartNode(Vector2 nodePosition, string nodeName = "", int nodeID = -1)
        {
            StartNode = new DescantStartNode(this, nodePosition);
            
            StartNode.Name = nodeName;
            StartNode.ID = nodeID;

            StartNode.Draw();
            
            this.RemoveManipulator(startNodeManipulator);
            contextMenuManipulators.Remove(startNodeManipulator);

            return StartNode;
        }
        
        DescantEndNode CreateEndNode(Vector2 nodePosition, string nodeName = "", int nodeID = -1)
        {
            var endNode = new DescantEndNode(this, nodePosition);
            
            endNode.Name = nodeName;
            endNode.ID = nodeID;

            endNode.Draw();
            
            EndNotes.Add(endNode);

            return endNode;
        }
        
        DescantNodeGroup CreateGroup(Vector2 groupPosition, string groupName = "", int groupID = -1)
        {
            var group = new DescantNodeGroup(this, groupPosition);

            group.Name = groupName;
            group.ID = groupID;

            group.Draw();
            
            foreach (var i in selection)
                if (i.GetType() == typeof(DescantChoiceNode) ||
                    i.GetType() == typeof(DescantResponseNode) ||
                    i.GetType() == typeof(DescantStartNode) ||
                    i.GetType() == typeof(DescantEndNode))
                    group.AddElement((GraphElement)i);

            Groups.Add(group);
            
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

        DescantNode FindNode(string nodeName, int nodeID)
        {
            foreach (var i in ChoiceNodes)
                if (NodeMatches(i, nodeName, nodeID))
                    return i;
            
            foreach (var j in ResponseNodes)
                if (NodeMatches(j, nodeName, nodeID))
                    return j;

            if (NodeMatches(StartNode, nodeName, nodeID)) return StartNode;
            
            foreach (var k in EndNotes)
                if (NodeMatches(k, nodeName, nodeID))
                    return k;

            return null;
        }

        bool NodeMatches(DescantNode node, string nodeName, int nodeID)
        {
            return node.Name == nodeName && node.ID == nodeID;
        }

        public void CheckAndSave()
        {
            if (Editor.AutoSave != null && Editor.AutoSave.value) Editor.Save();
        }
    }
}