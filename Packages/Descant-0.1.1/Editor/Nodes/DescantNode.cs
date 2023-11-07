#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using DescantComponents;

namespace DescantEditor
{
    /// <summary>
    /// Enum to list all of the types of DescantNodes
    /// </summary>
    public enum DescantNodeType { Choice, Response, Start, End, Any }
    
    /// <summary>
    /// Descant graph node parent class
    /// </summary>
    public abstract class DescantNode : Node
    {
        /// <summary>
        /// The custom name of this node,
        /// which is independent from its type or ID
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The unique identifier for this node,
        /// used to differentiate it from others of the name type and/or name
        /// </summary>
        public int ID { get; set; }
        
        public DescantNodeType Type { get; protected set; }
        
        // TODO: component IDs

        public Dictionary<string, int> Components { get; }
        public PopupField<string> ComponentDropdown { get; private set; }

        /// <summary>
        /// Used by Descant graph nodes to update their IDs and save when changes have been made to them
        /// </summary>
        protected DescantGraphView GraphView;
        
        protected DescantNode(
            DescantGraphView graphView,
            Vector2 position)
        {
            Components = new Dictionary<string, int>();
            
            GraphView = graphView;
            SetPosition(new Rect(position, Vector2.zero));
        }
        
        /// <summary>
        /// Initializes this node's VisualElements
        /// </summary>
        protected void Draw()
        {
            // If there's no custom name yet present, we set a default one
            if (Name == "") Name = Type + "Node";
            
            titleContainer.style.height = 70;

            // Initializing the name section
            VisualElement names = new VisualElement();
            names.AddToClassList("names");
            titleContainer.Insert(0, names);

            // Initializing the single character which signified what type of node this is
            TextElement nodeType = new TextElement();
            nodeType.text = Type.ToString()[0].ToString();
            nodeType.AddToClassList("node-name");
            names.Add(nodeType);
            
            // Initializing the custom name field
            TextField customName = new TextField();
            customName.value = Name;
            names.Add(customName);

            // Adding a callback for when the custom name text is changed
            customName.RegisterValueChangedCallback(callback =>
            {
                // Filtering the text for the field as it's being written
                TextField target = (TextField) callback.target;
                target.value = DescantEditorUtilities.FilterText(target.value);
                
                GraphView.Editor.CheckAndSave(); // Check for autosave
            });

            // Initializing the node removal button
            // (only non-Start nodes can be removed)
            if (GetType() != typeof(DescantStartNode))
            {
                Button removeNode = new Button();
                removeNode.text = "X";
                removeNode.clicked += RemoveNode;
                titleContainer.Insert(1, removeNode);
            
                // Adding a callback for when the node is removed
                RegisterCallback(new EventCallback<MouseLeaveEvent>(callback =>
                {
                    GraphView.Editor.CheckAndSave(); // Check for autosave
                }));
            }

            /*
            List<Type> nodeComponents = DescantComponentUtilities.GetAllNodeComponentTypes();
            
            List<string> nodeComponentNames = DescantComponentUtilities.TrimmedNodeComponentTypes(nodeComponents
                .Where(type =>
                    {
                        var tempType = (((NodeTypeAttribute) type.GetCustomAttributes(
                            typeof(NodeTypeAttribute),
                            true
                        ).FirstOrDefault())!).Type;

                        return tempType.Equals(DescantNodeType.Any) || Type.Equals(tempType);
                    })
                .ToList());

            ComponentDropdown = new PopupField<string>(nodeComponentNames, 0);
            ComponentDropdown.AddToClassList("node_component_dropdown");
            ComponentDropdown.value = "Add Component";
            extensionContainer.Add(ComponentDropdown);

            ComponentDropdown.RegisterValueChangedCallback(callback =>
            {
                string componentName = callback.newValue;

                if (componentName != "Add Component")
                {
                    if (!Components.ContainsKey(componentName)) Components.Add(componentName, 1);
                    else Components[componentName]++;

                    if (Components[componentName] >= DescantComponentUtilities.GetNodeComponentMaximum(componentName))
                        ComponentDropdown.choices.RemoveAt(ComponentDropdown.choices.IndexOf(componentName));
                    
                    DescantNodeComponentVisualElement temp = new DescantNodeComponentVisualElement(
                        GraphView, 
                        this,
                        componentName
                    );
                    
                    extensionContainer.Add(temp);
                    temp.Draw();
                    
                    RefreshExpandedState();
                }
                
                ComponentDropdown.value = "Add Component";
                
                GraphView.Editor.CheckAndSave(); // Check for autosave
            });
            */
        }

        /// <summary>
        /// Overridden BuildContextualMenu method to make sure the Delete dropdown calls RemoveNode
        /// </summary>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Delete", actionEvent => { RemoveNode(); });
        }
        
        /// <summary>
        /// Method to remove the node from the hierarchy, and perform all the necessary checks afterwards
        /// </summary>
        void RemoveNode()
        {
            // Disconnecting all the connections to the ports
            GraphView.DisconnectPorts(inputContainer);
            GraphView.DisconnectPorts(outputContainer);
            
            DescantEditorUtilities.RemoveElement(this); // Actually removing the node

            // Absolutely making sure that this node is no longer preserved in the DescantGraphView's lists
            if (Type.Equals(DescantNodeType.Choice)) GraphView.ChoiceNodes.Remove((DescantChoiceNode)this);
            else if (Type.Equals(DescantNodeType.Response)) GraphView.ResponseNodes.Remove((DescantResponseNode)this);
            else if (Type.Equals(DescantNodeType.End)) GraphView.EndNodes.Remove((DescantEndNode)this);
            
            // If by some strange chance the Start node does get removed, it should be allowed to be added back in
            if (Type.Equals(DescantNodeType.Start))
            {
                GraphView.RemoveContextMenuManipulators();
                GraphView.AddContextMenuManipulators();
            }
            
            // Updating all the groups to their proper sizes
            foreach (var i in GraphView.Groups)
                i.UpdateGeometryFromContent();
            
            GraphView.Editor.CheckAndSave(); // Check for autosave
        }
    }
}
#endif