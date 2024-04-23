#if UNITY_EDITOR
using Descant.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Descant.Editor
{
    /// <summary>
    /// Purely in-Editor VisualElement used to group and name different collections of DescantNodes
    /// </summary>
    public class DescantNodeGroup : Group
    {
        /// <summary>
        /// The unique identifier for this group,
        /// used to differentiate it from others of the name type and/or name
        /// </summary>
        public int ID { get; set; }
        
        /// <summary>
        /// The custom name of this group,
        /// which is independent from its type or ID
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Used to update the ID and save when changes have been made
        /// </summary>
        DescantGraphView GraphView;
    
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="graphView">The GraphView for this editor window</param>
        /// <param name="position">The position to spawn the group at</param>
        public DescantNodeGroup(
            DescantGraphView graphView,
            Vector2 position)
        {
            GraphView = graphView;
            SetPosition(new Rect(position, Vector2.zero));
        }

        /// <summary>
        /// Initializes this group's VisualElements
        /// </summary>
        public void Draw()
        {
            // If there's no custom name yet present, we set a default one
            if (Name == "") Name = "NodeGroup";
            
            // If this group is just being created, we set its ID
            if (ID < 0)
            {
                ID = GraphView.GroupID;
                GraphView.GroupID++;
            }

            // Initializing the group's custom name
            title = Name;
            TextField customName = DescantEditorUtilities.FindFirstElement<TextField>(this);
            customName.value = Name;
            
            // Adding a callback for when the custom name text is changed
            customName.RegisterValueChangedCallback(callback =>
            {
                // Filtering the text for the field as it's being written
                TextField target = (TextField) callback.target;
                target.value = DescantUtilities.FilterText(target.value);
                
                GraphView.Editor.CheckAndSave(); // Check for autosave
            });
        
            // Initializing the node removal button
            Button removeGroup = new Button();
            removeGroup.text = "X";
            removeGroup.clicked += RemoveGroup;
            headerContainer.Add(removeGroup);
            
            /*
            // Adding a callback for when the group is removed
            RegisterCallback(new EventCallback<MouseLeaveEvent>(callback =>
            {
                GraphView.Editor.CheckAndSave(); // Check for autosave
            }));
            */

            // Aligning the group's name to the center
            headerContainer.style.flexDirection = FlexDirection.Row;
            headerContainer.style.alignItems = Align.Center;
            headerContainer.style.justifyContent = Justify.Center;
            
            // Initializing the comments
            TextField comments = new TextField("Comments:");
            comments.AddToClassList("comments");
            comments.multiline = true;
            contentContainer.Add(comments);
            
            comments.RegisterValueChangedCallback(evt =>
            {
                GraphView.Editor.CheckAndSave();
            });
        }

        /// <summary>
        /// Method to remove the group from the hierarchy, and perform all the necessary checks afterwards
        /// </summary>
        void RemoveGroup()
        {
            DescantEditorUtilities.RemoveElement(this); // Actually removing the group
            
            // Absolutely making sure that this group is no longer preserved in the DescantGraphView's list
            GraphView.Groups.Remove(this);
            
            GraphView.Editor.CheckAndSave(); // Check for autosave
        }
    }
}
#endif