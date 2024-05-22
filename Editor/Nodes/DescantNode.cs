#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Descant.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Descant.Components;

namespace Descant.Editor
{
    /// <summary>
    /// Descant graph node parent class
    /// </summary>
    public abstract class DescantNode : Node
    {
        /// <summary>
        /// The custom name of this node
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The unique identifier ID for this node,
        /// used to differentiate it from others of the name type and/or name
        /// </summary>
        public int ID { get; set; }
        
        /// <summary>
        /// The type of this node
        /// </summary>
        public DescantNodeType Type { get; protected set; }

        /// <summary>
        /// The number of components of each type that are attached to this node
        /// </summary>
        public Dictionary<string, int> ComponentCounts { get; }
        
        /// <summary>
        /// The dropdown VisualElement for this node
        /// </summary>
        public PopupField<string> ComponentDropdown { get; private set; }

        /// <summary>
        /// Used by Descant graph nodes to update their IDs and save when changes have been made to them
        /// </summary>
        protected DescantGraphView GraphView;

        /// <summary>
        /// A list of all DescantNodeComponentVisualElement components currently
        /// generated and added in the Descant Graph Editor
        /// </summary>
        [HideInInspector] public List<DescantNodeComponentVisualElement> VisualComponents =
            new List<DescantNodeComponentVisualElement>();

        VisualElement componentParent = new VisualElement(); // A VisualElement to hold all of the added components
        
        #region Initialization
        
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="graphView">The GraphView for this editor window</param>
        /// <param name="position">The position to spawn the node at</param>
        protected DescantNode(DescantGraphView graphView, Vector2 position)
        {
            ComponentCounts = new Dictionary<string, int>();
            
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
                target.value = DescantUtilities.FilterText(target.value);
                
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
            
                /*
                // Adding a callback for when the node is removed
                RegisterCallback(new EventCallback<MouseLeaveEvent>(callback =>
                {
                    GraphView.Editor.CheckAndSave(); // Check for autosave
                }));
                */
            }
            
            // Getting a formatted list of all the types of Components that can be attached to this node 
            List<string> nodeComponentNames = DescantComponentUtilities.GetTrimmedComponentTypes(
                DescantComponentUtilities.GetComponentTypes()
                .Where(type =>
                {
                    bool show = true;

                    // Making sure that any Components marked to not be shown aren't
                    try
                    {
                        string unused = ((DontShowInEditorAttribute) type.GetCustomAttributes(
                            typeof(DontShowInEditorAttribute),
                            true
                        ).FirstOrDefault())!.ToString();

                        show = false;
                    }
                    catch { }
                    
                    var tempType = (((NodeTypeAttribute) type.GetCustomAttributes(
                        typeof(NodeTypeAttribute),
                        true
                    ).FirstOrDefault())!).Type;
                    
                    return show && (tempType.Equals(DescantNodeType.Any) || Type.Equals(tempType));
                }).ToList());
            
            // Initializing the dropdown with the appropriate options from above
            ComponentDropdown = new PopupField<string>(nodeComponentNames, 0);
            ComponentDropdown.AddToClassList("node_component_dropdown");
            ComponentDropdown.value = "Add Component";
            extensionContainer.Add(ComponentDropdown);

            // When the dropdown value is changed, we create a new Component and reset the dropdown
            ComponentDropdown.RegisterValueChangedCallback(callback =>
            {
                string componentName = callback.newValue;

                if (componentName != "Add Component")
                    AddComponent(componentName, VisualComponents.Count, null);
                
                ComponentDropdown.value = "Add Component";
                
                GraphView.Editor.CheckAndSave(); // Check for autosave
            });

            // Initializing the component parent
            componentParent = new VisualElement();
            componentParent.AddToClassList("component_parent");
            extensionContainer.Add(componentParent);

            // Initializing the comments
            TextField comments = new TextField("Comments:");
            comments.AddToClassList("comments");
            comments.multiline = true;
            extensionContainer.Add(comments);

            comments.RegisterValueChangedCallback(evt =>
            {
                GraphView.Editor.CheckAndSave();
            });
        }

        /// <summary>
        /// Overridden BuildContextualMenu method to make sure the Delete dropdown calls RemoveNode
        /// </summary>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Delete", actionEvent => { RemoveNode(); });
        }
        
        #endregion
        
        #region Components

        /// <summary>
        /// Creates and adds a new DescantNodeComponentVisualElement to this node
        /// </summary>
        /// <param name="componentName">The name of the Component type to add</param>
        /// <param name="componentIndex">The index among Components that this new one should be added at</param>
        /// <param name="component">
        /// The DescantNodeComponent object that the VisualElement will be representing
        /// (if null, the DescantNodeComponentVisualElement will create a new instance of it)
        /// </param>
        public void AddComponent(string componentName, int componentIndex, DescantComponent component)
        {
            // Updating the count for this Component type
            if (!ComponentCounts.ContainsKey(componentName)) ComponentCounts.Add(componentName, 1);
            else ComponentCounts[componentName]++;
            
            // If we've hit the maximum, we remove the type from the list
            if (ComponentCounts[componentName] >= DescantComponentUtilities.GetComponentMaximum(componentName))
                ComponentDropdown.choices.RemoveAt(ComponentDropdown.choices.IndexOf(componentName));
            
            // Creating a new VisualElement
            DescantNodeComponentVisualElement temp = new DescantNodeComponentVisualElement(
                GraphView, 
                this,
                componentName,
                componentIndex,
                component
            );
            
            // Adding to the DescantNodeComponentVisualElement list and updating the order
            // (in case there are any RandomizedNodes in there)
            VisualComponents.Add(temp);
            temp.Drawn += UpdateComponents;
            
            componentParent.Add(temp);
            temp.Draw();
                    
            RefreshExpandedState();
        }

        /// <summary>
        /// Repositions a DescantNodeComponentVisualElement's place within the lsit
        /// </summary>
        /// <param name="component">The Component VisualElement to be repositioned</param>
        /// <param name="index">The index it should be moved it</param>
        /// <param name="update">
        /// Whether or not to update all other Components in the list in regards to this change (true by default)
        /// </param>
        public void RearrangeComponent(DescantNodeComponentVisualElement component, int index, bool update = true)
        {
            // removing and inserting it in the list
            VisualComponents.Remove(component);
            VisualComponents.Insert(index, component);
            
            componentParent.Insert(index, component); // Removing and inserting it in the Node

            if (update) UpdateComponents();
        }

        /// <summary>
        /// Checks through the DescantNodeComponentVisualElements, making sure that
        /// RandomizedChoices are at the end, and that all of their order indices are set correctly
        /// </summary>
        public void UpdateComponents()
        {
            for (int i = 0; i < VisualComponents.Count; i++)
                if (i < VisualComponents.Count - 1 && VisualComponents[i].Component.GetType() == typeof(RandomizedChoice))
                    RearrangeComponent(VisualComponents[i], VisualComponents.Count - 1, false);
            
            for (int j = 0; j < VisualComponents.Count; j++)
            {
                VisualComponents[j].SetOrder(j);
            }
        }
        
        #endregion
        
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
            else if (Type.Equals(DescantNodeType.If)) GraphView.IfNodes.Remove((DescantIfNode)this);
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