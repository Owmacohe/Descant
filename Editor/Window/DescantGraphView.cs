#if UNITY_EDITOR
using System.Collections.Generic;
using Descant.Components;
using Descant.Utilities;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Descant.Editor
{
    /// <summary>
    /// The custom GraphView component of the editor, housing the nodes, groups, and connections
    /// </summary>
    public class DescantGraphView : GraphView
    {
        /// <summary>
        /// The current custom Descant EditorWindow
        /// </summary>
        public DescantEditor Editor;
        
        /// <summary>
        /// The number used to instantiate each new DescantChoiceNode with a unique ID
        /// (so that it can be told apart from other nodes with the same type and name)
        /// </summary>
        public int ChoiceNodeID { get; set; }
        
        /// <summary>
        /// The number used to instantiate each new DescantResponseNode with a unique ID
        /// (so that it can be told apart from other nodes with the same type and name)
        /// </summary>
        public int ResponseNodeID { get; set; }
        
        /// <summary>
        /// The number used to instantiate each new DescantEndNode with a unique ID
        /// (so that it can be told apart from other nodes with the same type and name)
        /// </summary>
        public int EndNodeID { get; set; }
        
        /// <summary>
        /// The number used to instantiate each new DescantNodeGroup with a unique ID
        /// (so that it can be told apart from other groups with the same name)
        /// </summary>
        public int GroupID { get; set; }

        /// <summary>
        /// The list of all DescantChoiceNodes in the graph
        /// </summary>
        public List<DescantChoiceNode> ChoiceNodes = new List<DescantChoiceNode>();
        
        /// <summary>
        /// The list of all DescantResponseNodes in the graph
        /// </summary>
        public List<DescantResponseNode> ResponseNodes = new List<DescantResponseNode>();
        
        /// <summary>
        /// The list of all DescantStartNodes in the graph
        /// </summary>
        public DescantStartNode StartNode;
        
        /// <summary>
        /// The list of all DescantEndNodes in the graph
        /// </summary>
        public List<DescantEndNode> EndNodes = new List<DescantEndNode>();
        
        /// <summary>
        /// The list of all DescantNodeGroups in the graph
        /// </summary>
        public List<DescantNodeGroup> Groups = new List<DescantNodeGroup>();
        
        IManipulator startNodeManipulator; // The 'Add Start Node' contextual menu manipulator
        List<IManipulator> contextMenuManipulators = new List<IManipulator>(); // The list of contextual menu manipulators for nodes

        #region Constructor
        
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="editor">The editor window that this graph view is a part of</param>
        public DescantGraphView(DescantEditor editor)
        {
            Editor = editor;
            
            AddGridBackground(); // Initializing the background
            AddManipulators(); // Initializing the context manu manipulators
            DescantEditorUtilities.AddStyleSheet(styleSheets, "DescantGraphEditorStyleSheet"); // Initializing the stylesheet
            
            // Adding a callback for when the mouse leaves the graph view
            RegisterCallback(new EventCallback<MouseLeaveEvent>(callback =>
            {
                Editor.CheckAndSave(); // Check for autosave
            }));

            // Generating all the data that has been previously loaded into the editor
            DescantGraph data = Editor.data;

            ChoiceNodeID = data.ChoiceNodeID;
            ResponseNodeID = data.ResponseNodeID;
            EndNodeID = data.EndNodeID;
            GroupID = data.GroupID;

            // Generating the DescantChoiceNodes
            foreach (var i in data.ChoiceNodes)
            {
                var temp = CreateChoiceNode(i.Position, i.Name, i.ID);

                for (int ii = 0; ii < i.Choices.Count; ii++)
                    temp.AddChoice(ii + 1, i.Choices[ii]);
                
                for (int ij = 0; ij < i.NodeComponents.Count; ij++)
                    temp.AddComponent(DescantComponentUtilities.GetTrimmedTypeName(
                        i.NodeComponents[ij].GetType()), ij, i.NodeComponents[ij]);

                AddElement(temp);
            }

            // Generating the DescantResponseNodes
            foreach (var j in data.ResponseNodes)
            {
                var temp = CreateResponseNode(j.Position, j.Name, j.ID);
                DescantEditorUtilities.FindAllElements<TextField>(temp)[1].value = j.Response;
                
                for (int ji = 0; ji < j.NodeComponents.Count; ji++)
                    temp.AddComponent(DescantComponentUtilities.GetTrimmedTypeName(
                        j.NodeComponents[ji].GetType()), ji, j.NodeComponents[ji]);
                
                AddElement(temp);
            }

            // Generating the DescantStartNode (creating a new one if there isn't one present in the file)
            if (data.StartNode != null)
            {
                DescantStartNode temp = CreateStartNode(
                    data.StartNode.Position,
                    data.StartNode.Name,
                    data.StartNode.ID
                );
                
                for (int k = 0; k < data.StartNode.NodeComponents.Count; k++)
                    temp.AddComponent(DescantComponentUtilities.GetTrimmedTypeName(
                        data.StartNode.NodeComponents[k].GetType()), k, data.StartNode.NodeComponents[k]);
                
                AddElement(temp);
            }
            else AddElement(CreateStartNode(new Vector2(50, 70)));
            
            this.RemoveManipulator(startNodeManipulator);
            
            // Generating the DescantEndNodes
            foreach (var l in data.EndNodes)
            {
                DescantEndNode temp = CreateEndNode(l.Position, l.Name, l.ID);
                
                for (int li = 0; li < l.NodeComponents.Count; li++)
                    temp.AddComponent(DescantComponentUtilities.GetTrimmedTypeName(
                        l.NodeComponents[li].GetType()), li, l.NodeComponents[li]);
                
                AddElement(temp);
            }

            // Generating the DescantNodeGroups
            foreach (var m in data.Groups)
            {
                var temp = CreateNodeGroup(m.Position, m.Name, m.ID);

                for (int mi = 0; mi < m.Nodes.Count; mi++)
                    temp.AddElement(FindNode(m.Nodes[mi], m.NodeIDs[mi]));
                
                AddElement(temp);
            }
            
            // Generating the connections between DescantNodes
            foreach (var n in data.Connections)
            {
                if (n.To != "null")
                {
                    var from = FindNode(n.From, n.FromID);
                    var to = FindNode(n.To, n.ToID);

                    int fromPortIndex = from.Type.Equals(DescantNodeType.Start) ? 0 : 1;
                    if (from.Type.Equals(DescantNodeType.Choice)) fromPortIndex = n.ChoiceIndex;

                    Port fromPort = DescantEditorUtilities.FindAllElements<Port>(from)[fromPortIndex];
                    Port toPort = DescantEditorUtilities.FindFirstElement<Port>(to);

                    Edge temp = fromPort.ConnectTo(toPort);
                    Add(temp);
                
                    temp.RegisterCallback<MouseUpEvent>(callback =>
                    {
                        Editor.CheckAndSave(); // Check for autosave
                    });   
                }
            }
        }
        
        #endregion

        /// <summary>
        /// Initializes the grid background for the Descant graph
        /// </summary>
        void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();
            gridBackground.StretchToParentSize();
            
            Insert(0, gridBackground);
        }
        
        /// <summary>
        /// Adds all the GraphView manipulators (e.g. zooming, selection, etc.)
        /// as well as the node and group context manu manipulators
        /// </summary>
        void AddManipulators()
        {
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            
            AddContextMenuManipulators();
        }
        
        # region Contextual Menus
        
        /// <summary>
        /// Adds all the node and group context menu manipulators
        /// </summary>
        public void AddContextMenuManipulators()
        {
            this.AddManipulator(CreateNodeContextualMenu("Add Choice Node", DescantNodeType.Choice));
            this.AddManipulator(CreateNodeContextualMenu("Add Response Node", DescantNodeType.Response));
            this.AddManipulator(CreateNodeContextualMenu("Add Start Node", DescantNodeType.Start));
            this.AddManipulator(CreateNodeContextualMenu("Add End Node", DescantNodeType.End));

            this.AddManipulator(CreateNodeGroupContextualMenu());
        }
        
        /// <summary>
        /// Removes all the node and group context menu manipulators
        /// </summary>
        public void RemoveContextMenuManipulators()
        {
            foreach (var i in contextMenuManipulators)
                this.RemoveManipulator(i);
            
            contextMenuManipulators.Clear();

            startNodeManipulator = null;
        }
        
        /// <summary>
        /// Creates a node contextual menu item for some type of DescantNode
        /// </summary>
        /// <param name="actionTitle">The title for the dropdown option</param>
        /// <param name="type">The type of node to be added when clicked</param>
        /// <returns>The new node contextual menu, ready for addition to the context-sensitive dropdown</returns>
        IManipulator CreateNodeContextualMenu(string actionTitle, DescantNodeType type)
        {
            ContextualMenuManipulator context = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(actionTitle,
                    actionEvent =>
                    {
                        switch (type)
                        {
                            case DescantNodeType.Choice:
                                AddElement(CreateChoiceNode(contentViewContainer.WorldToLocal(
                                    actionEvent.eventInfo.localMousePosition)));
                                break;
                            
                            case DescantNodeType.Response:
                                AddElement(CreateResponseNode(contentViewContainer.WorldToLocal(
                                    actionEvent.eventInfo.localMousePosition)));
                                break;
                            
                            case DescantNodeType.Start:
                                AddElement(CreateStartNode(contentViewContainer.WorldToLocal(
                                    actionEvent.eventInfo.localMousePosition)));
                                break;

                            case DescantNodeType.End:
                                AddElement(CreateEndNode(contentViewContainer.WorldToLocal(
                                    actionEvent.eventInfo.localMousePosition)));
                                break;

                        }
                        
                        Editor.CheckAndSave(); // Check for autosave
                    })
            );
            
            // If this manipulator is for the DescantStartNode, we should save it
            if (startNodeManipulator == null && type.Equals(DescantNodeType.Start)) startNodeManipulator = context;
            
            // Adding the contextual manipulator to the list of contextual manipulators
            contextMenuManipulators.Add(context);

            return context;
        }
        
        /// <summary>
        /// Creates a contextual menu item for a DescantNodeGroup
        /// </summary>
        /// <returns></returns>
        IManipulator CreateNodeGroupContextualMenu()
        {
            ContextualMenuManipulator context = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Add Group",
                    actionEvent =>
                    {
                        AddElement(CreateNodeGroup(contentViewContainer.WorldToLocal(
                            actionEvent.eventInfo.localMousePosition)));
                        Editor.CheckAndSave(); // Check for autosave
                    })
            );
            
            // Adding the contextual manipulator to the list of contextual manipulators
            contextMenuManipulators.Add(context);

            return context;
        }
        
        #endregion
        
        #region Node/Group creation

        /// <summary>
        /// Creates a new DescantChoiceNode
        /// </summary>
        /// <param name="nodePosition">The position at which to create the node</param>
        /// <param name="nodeName">The custom name of the node (default if ignored)</param>
        /// <param name="nodeID">The ID of the node (default if ignored)</param>
        /// <returns>The newly-created node</returns>
        DescantChoiceNode CreateChoiceNode(Vector2 nodePosition, string nodeName = "", int nodeID = -1)
        {
            var choiceNode = new DescantChoiceNode(this, nodePosition)
            {
                Name = nodeName,
                ID = nodeID
            };

            choiceNode.Draw();
            
            ChoiceNodes.Add(choiceNode);

            return choiceNode;
        }
        
        /// <summary>
        /// Creates a new DescantResponseNode
        /// </summary>
        /// <param name="nodePosition">The position at which to create the node</param>
        /// <param name="nodeName">The custom name of the node (default if ignored)</param>
        /// <param name="nodeID">The ID of the node (default if ignored)</param>
        /// <returns>The newly-created node</returns>
        DescantResponseNode CreateResponseNode(Vector2 nodePosition, string nodeName = "", int nodeID = -1)
        {
            var responseNode = new DescantResponseNode(this, nodePosition)
            {
                Name = nodeName,
                ID = nodeID
            };

            responseNode.Draw();
            
            ResponseNodes.Add(responseNode);

            return responseNode;
        }
        
        /// <summary>
        /// Creates a new DescantStartNode
        /// </summary>
        /// <param name="nodePosition">The position at which to create the node</param>
        /// <param name="nodeName">The custom name of the node (default if ignored)</param>
        /// <param name="nodeID">The ID of the node (0 if ignored)</param>
        /// <returns>The newly-created node</returns>
        DescantStartNode CreateStartNode(Vector2 nodePosition, string nodeName = "", int nodeID = 0)
        {
            StartNode = new DescantStartNode(this, nodePosition)
            {
                Name = nodeName,
                ID = nodeID
            };

            StartNode.Draw();
            
            this.RemoveManipulator(startNodeManipulator);
            contextMenuManipulators.Remove(startNodeManipulator);

            return StartNode;
        }
        
        /// <summary>
        /// Creates a new DescantEndNode
        /// </summary>
        /// <param name="nodePosition">The position at which to create the node</param>
        /// <param name="nodeName">The custom name of the node (default if ignored)</param>
        /// <param name="nodeID">The ID of the node (default if ignored)</param>
        /// <returns>The newly-created node</returns>
        DescantEndNode CreateEndNode(Vector2 nodePosition, string nodeName = "", int nodeID = -1)
        {
            var endNode = new DescantEndNode(this, nodePosition)
            {
                Name = nodeName,
                ID = nodeID
            };

            endNode.Draw();
            
            EndNodes.Add(endNode);

            return endNode;
        }
        
        /// <summary>
        /// Creates a new DescantNodeGroup
        /// </summary>
        /// <param name="groupPosition">The position at which to create the group</param>
        /// <param name="groupName">The custom name of the group (default if ignored)</param>
        /// <param name="groupID">The ID of the group (default if ignored)</param>
        /// <returns>The newly-created group</returns>
        DescantNodeGroup CreateNodeGroup(Vector2 groupPosition, string groupName = "", int groupID = -1)
        {
            var group = new DescantNodeGroup(this, groupPosition)
            {
                Name = groupName,
                ID = groupID
            };

            group.Draw();
            
            // If there are currently-selected nodes at the time of the group's creation,
            // they get automatically made members of the group
            foreach (var i in selection)
                if (i.GetType() == typeof(DescantChoiceNode) ||
                    i.GetType() == typeof(DescantResponseNode) ||
                    i.GetType() == typeof(DescantStartNode) ||
                    i.GetType() == typeof(DescantEndNode))
                    group.AddElement((GraphElement)i);

            Groups.Add(group);
            
            return group;
        }
        
        #endregion
        
        #region Ports
        
        /// <summary>
        /// Disconnects all the ports for some DescantNode container
        /// </summary>
        /// <param name="container">The input or output container of the DescantNode
        ///                         from which the ports will be disconnected</param>
        /// <param name="onlyThisPort">The only port that needs to be disconnected (all ports if ignored)</param>
        public void DisconnectPorts(VisualElement container, Port onlyThisPort = null)
        {
            foreach (Port i in container.Children())
                if (i.connected && (onlyThisPort == null || i.Equals(onlyThisPort)))
                    DeleteElements(i.connections);
        }
        
        /// <summary>
        /// Overridden method to make sure that connections can only be made between DescantNodes that are:
        /// a) Not of the same type and
        /// b) Not in the same direction
        /// </summary>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort == port) return;
                if (startPort.node == port.node) return;
                if (startPort.direction == port.direction) return;
                
                compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }
        
        #endregion
        
        #region Nodes

        /// <summary>
        /// Searches through all the DescantNodes in the graph and finds a matching one
        /// </summary>
        /// <param name="nodeType">The type (as a string) of the DescantNode</param>
        /// <param name="nodeID">The ID of the DescantNode</param>
        /// <returns>The DescantNode in thr graph with the matching criteria (null if not found)</returns>
        DescantNode FindNode(string nodeType, int nodeID)
        {
            foreach (var i in ChoiceNodes)
                if (NodeMatches(i, nodeType, nodeID))
                    return i;
            
            foreach (var j in ResponseNodes)
                if (NodeMatches(j, nodeType, nodeID))
                    return j;

            if (NodeMatches(StartNode, nodeType, nodeID)) return StartNode;
            
            foreach (var k in EndNodes)
                if (NodeMatches(k, nodeType, nodeID))
                    return k;

            return null;
        }

        /// <summary>
        /// Checks to see if a given DescantNode matches a type and ID
        /// </summary>
        /// <param name="node">the DescantNode to check</param>
        /// <param name="nodeType">The type (as a string) to check it against</param>
        /// <param name="nodeID">the ID to check it against</param>
        /// <returns></returns>
        bool NodeMatches(DescantNode node, string nodeType, int nodeID)
        {
            return node.Type.ToString() == nodeType && node.ID == nodeID;
        }
        
        #endregion
    }
}
#endif