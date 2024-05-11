#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Descant.Components;
using Descant.Utilities;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Descant.Editor
{
    public class DescantNodeComponentVisualElement : VisualElement
    {
        /// <summary>
        /// The Component that this visual element is representing
        /// </summary>
        public DescantComponent Component { get; }

        /// <summary>
        /// A callback that triggers once the VisualElements for this Component have all been drawn
        /// </summary>
        public event Action Drawn;
        
        string componentName; // The trimmed name corresponding to this component
        int index; // The index of this Component within its DescantNode's list of Components
        
        DescantGraphView graphView; // The DescantGraphView (used for saving)
        DescantNode node; // The node that this Component is attached to
        
        List<VisualElement> paramRows; // The list of rows of parameters for this Component
        List<string> paramRowGroups; // The list of names of rows of parameters for this Component

        TextField order;
        VisualElement collapsible; // The section of the Component that is collapsible
        Button collapsibleButton; // The button used to collapse/expand the Component

        /// <summary>
        /// The Component visual element representation constructor
        /// </summary>
        /// <param name="graph">The DescantGraphView (used for saving)</param>
        /// <param name="descantNode">The node that the Component is attached to</param>
        /// <param name="componentName">The name of the Component</param>
        /// <param name="index">The starting index of this Component within its DescantNode's list of Components</param>
        /// <param name="component">The Component that the visual element is representing</param>
        public DescantNodeComponentVisualElement(
            DescantGraphView graph,
            DescantNode descantNode,
            string componentName,
            int index,
            DescantComponent component)
        {
            graphView = graph;
            node = descantNode;
            this.componentName = componentName;
            this.index = index;

            // If the Component is null (i.e. it's being created for the first time), we create a new instance of it
            if (component == null)
            {
                List<Type> types = DescantComponentUtilities.GetComponentTypes();
                var temp = types[DescantComponentUtilities.GetTrimmedComponentTypes(types).IndexOf(this.componentName)];
                
                Component = (DescantComponent) Activator.CreateInstance(temp);
            }
            else Component = component; // Otherwise we just use the previously saved copy
        }
        
        #region Draw

        /// <summary>
        /// Initializes this Component's VisualElements
        /// </summary>
        public void Draw()
        {
            AddToClassList("node_component");
            
            paramRows = new List<VisualElement>();
            paramRowGroups = new List<string>();

            // Initializing the top name and button row
            VisualElement top_row = new VisualElement();
            top_row.AddToClassList("node_component_row");
            Add(top_row);

            // Initializing the left-aligned section of the top row
            VisualElement top_row_left = new VisualElement();
            top_row_left.AddToClassList("node_component_row");
            top_row.Add(top_row_left);
            paramRows.Add(top_row_left);
            paramRowGroups.Add("");

            // Adding the order field
            order = new TextField();
            order.AddToClassList("node_component_row_order");
            order.multiline = false;
            order.value = index.ToString();
            top_row_left.Add(order);

            if (Component.GetType() != typeof(RandomizedChoice))
            {
                // Adding a small callback to prevent large and unnecessary indices to be entered into the order field
                order.RegisterValueChangedCallback(evt =>
                {
                    try
                    {
                        int parsed = int.Parse(order.value);

                        if (parsed < 0) order.value = "0";
                        else if (parsed >= node.VisualComponents.Count)
                            order.value = (node.VisualComponents.Count - 1).ToString();
                    }
                    catch { }
                });

                // Callback to parse and rearrange the Components once the user presses enter
                // in the order field after entering a new index for the Component
                order.RegisterCallback(new EventCallback<KeyDownEvent>(evt =>
                {
                    if (evt.keyCode.Equals(KeyCode.Return))
                    {
                        int parsed;

                        try
                        {
                            parsed = int.Parse(order.value);

                            if (parsed < 0) parsed = 0;
                            else if (parsed >= node.VisualComponents.Count)
                            {
                                parsed = node.VisualComponents.Count - 1;

                                if (parsed == index) order.value = parsed.ToString();
                            }
                        }
                        catch
                        {
                            parsed = 0;
                        }

                        if (parsed != index) node.RearrangeComponent(this, parsed);
                    }
                }));
            }
            // Hiding the index if it's a RandomizedNode
            else order.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            
            // Adding the name
            TextElement name = new TextElement();
            name.AddToClassList("node_component_name");
            name.text = componentName;
            top_row_left.Add(name);

            // Adding the collapsible section
            collapsible = new VisualElement();
            Add(collapsible);
            
            PropertyField test = new PropertyField();
            Add(test);

            // Creating the first (default) row
            ScrollView paramRow1 = new ScrollView();
            paramRow1.mode = ScrollViewMode.Horizontal;
            paramRow1.horizontalScrollerVisibility = ScrollerVisibility.Auto;
            paramRow1.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            paramRow1.AddToClassList("node_component_row");
            collapsible.Add(paramRow1);
            paramRows.Add(paramRow1);
            paramRowGroups.Add("");

            // The public fields from the action Component object that should be ignored
            string[] ignoreFields = { "Collapsed" };
            
            // Checking through all the Component's parameters (public variables)
            foreach (var i in Component.GetType().GetFields())
            {
                string label = i.Name; // The name of the field
                
                if (!ignoreFields.Contains(label))
                {
                    // Removing all the prefixes to the parameter type
                    // (to make it easier to check below)
                    string formattedType = i.FieldType.ToString();
                    formattedType = formattedType.Substring(formattedType.LastIndexOf('.') + 1);
                    
                    // Adding an indicator to mark what type of parameter this is
                    switch (formattedType)
                    {
                        case "Int32":
                            label += " <i>(int)</i>";
                            break;
                        
                        case "Single":
                            label += " <i>(float)</i>";
                            break;
                        
                        case "String":
                            label += " <i>(string)</i>";
                            break;
                    }
                    
                    var componentValue = i.GetValue(Component); // The current value of this parameter
                    
                    // If the parameter type is one that can just be filled in with a field...
                    if (i.FieldType == typeof(string))
                    {
                        // Adding a new field
                        TextField field = new TextField();
                        field.label = label;
                        CheckAndAddParamLine(field, i);

                        // Setting the field's value
                        if (componentValue != null) field.value = componentValue.ToString();

                        // When the field's value is changed...
                        field.RegisterValueChangedCallback(callback =>
                        {
                            // Checking to see if the field should be filtered, and if so, doing that
                            try
                            {
                                string unused = ((NoFilteringAttribute) i.GetCustomAttributes(
                                    typeof(NoFilteringAttribute),
                                    true
                                ).FirstOrDefault())!.ToString();
                            }
                            catch
                            {
                                field.value = DescantUtilities.FilterText(field.value, i.FieldType != typeof(string));
                            }

                            try
                            {
                                // Saving the change to the Component object
                                i.SetValue(Component, DescantComponentUtilities.GetComponentParameterValue(
                                    callback.newValue,
                                    i.FieldType
                                ));
                            }
                            catch
                            {
                                DescantUtilities.ErrorMessage(GetType(), "Wrong text format!");
                            }

                            graphView.Editor.CheckAndSave(); // Check for autosave
                        });
                    }
                    // If the parameter type is a float...
                    else if (i.FieldType == typeof(float))
                    {
                        FloatField field = new FloatField();
                        field.label = label;
                        CheckAndAddParamLine(field, i);
                        
                        // Setting the field's value
                        if (componentValue != null) field.value = (float) componentValue;
                        
                        // When the field's value is changed...
                        field.RegisterValueChangedCallback(callback =>
                        {
                            // Saving the change to the Component object
                            i.SetValue(Component, callback.newValue);
                            
                            graphView.Editor.CheckAndSave(); // Check for autosave
                        });
                        
                    }
                    // If the parameter type is an int...
                    else if (i.FieldType == typeof(int))
                    {
                        IntegerField field = new IntegerField();
                        field.label = label;
                        CheckAndAddParamLine(field, i);
                        
                        // Setting the field's value
                        if (componentValue != null) field.value = (int) componentValue;
                        
                        // When the field's value is changed...
                        field.RegisterValueChangedCallback(callback =>
                        {
                            // Saving the change to the Component object
                            i.SetValue(Component, callback.newValue);
                            
                            graphView.Editor.CheckAndSave(); // Check for autosave
                        });
                        
                    }
                    // If the parameter type is a boolean...
                    else if (i.FieldType == typeof(bool))
                    {
                        // Adding a new toggle
                        Toggle field = new Toggle();
                        field.label = label;
                        CheckAndAddParamLine(field, i);

                        // Setting the toggle's value
                        if (componentValue != null) field.value = (bool) componentValue;
                        
                        // When the field's value is changed...
                        field.RegisterValueChangedCallback(callback =>
                        {
                            // Saving the change to the Component object
                            i.SetValue(Component, callback.newValue);
                            
                            graphView.Editor.CheckAndSave(); // Check for autosave
                        });
                    }
                    // If the parameter type is a Vector2...
                    else if (i.FieldType == typeof(Vector2))
                    {
                        // Adding a new field
                        Vector2Field field = new Vector2Field();
                        field.label = label;
                        CheckAndAddParamLine(field, i);

                        // Setting the field's value
                        if (componentValue != null) field.value = (Vector2) componentValue;
                        
                        // When the field's value is changed...
                        field.RegisterValueChangedCallback(callback =>
                        {
                            // Saving the change to the Component object
                            i.SetValue(Component, callback.newValue);
                            
                            graphView.Editor.CheckAndSave(); // Check for autosave
                        });
                    }
                    // If the parameter type is a Vector3...
                    else if (i.FieldType == typeof(Vector3))
                    {
                        // Adding a new field
                        Vector3Field field = new Vector3Field();
                        field.label = label;
                        CheckAndAddParamLine(field, i);

                        // Setting the field's value
                        if (componentValue != null) field.value = (Vector3) componentValue;
                        
                        // When the field's value is changed...
                        field.RegisterValueChangedCallback(callback =>
                        {
                            // Saving the change to the Component object
                            i.SetValue(Component, callback.newValue);
                            
                            graphView.Editor.CheckAndSave(); // Check for autosave
                        });
                    }
                    // If the parameter type is a Color...
                    else if (i.FieldType == typeof(Color))
                    {
                        // Adding a new field
                        ColorField field = new ColorField();
                        field.label = label;
                        CheckAndAddParamLine(field, i);

                        // Setting the field's value
                        if (componentValue != null) field.value = (Color) componentValue;
                        
                        // When the field's value is changed...
                        field.RegisterValueChangedCallback(callback =>
                        {
                            // Saving the change to the Component object
                            i.SetValue(Component, callback.newValue);
                            
                            graphView.Editor.CheckAndSave(); // Check for autosave
                        });
                    }
                    // If the parameter type is an Object...
                    else if (i.FieldType.IsSubclassOf(typeof(UnityEngine.Object)))
                    {
                        // Adding a new field
                        ObjectField field = new ObjectField();
                        field.objectType = i.FieldType;
                        field.allowSceneObjects = false;
                        field.label = label;
                        CheckAndAddParamLine(field, i);

                        // Setting the field's value
                        if (componentValue != null) field.value = (UnityEngine.Object) componentValue;
                        
                        // When the field's value is changed...
                        field.RegisterValueChangedCallback(callback =>
                        {
                            // Saving the change to the Component object
                            i.SetValue(Component, callback.newValue);
                            
                            graphView.Editor.CheckAndSave(); // Check for autosave
                        });
                    }
                    // Lastly, if the parameter type is an enumerator...
                    else if (i.FieldType.IsSubclassOf(typeof(Enum)))
                    {
                        List<string> enumValues = new List<string>();
                        
                        // Getting the full list of the enum's options
                        foreach (var j in i.FieldType.GetFields())
                            enumValues.Add(j.ToString().Substring(j.ToString().LastIndexOf(' ') + 1));

                        EnumField field = new EnumField();
                        field.Init((Enum) componentValue);
                        field.label = label;
                        CheckAndAddParamLine(field, i);
                        
                        // When the field's value is changed...
                        field.RegisterValueChangedCallback(callback =>
                        {
                            // Saving the change to the Component object
                            i.SetValue(Component, callback.newValue);

                            graphView.Editor.CheckAndSave(); // Check for autosave
                        });
                    }
                    
                    // If the parameter type isn't any of the above types, we simply ignore it
                }
            }

            // If there are no parameters (or there are only inline ones), we can remove the default row
            if (paramRows.Count == 2 && paramRow1.childCount <= 1)
            {
                VisualElement row1 = paramRows[1];
                paramRows.RemoveAt(1);
                DescantEditorUtilities.RemoveElement(row1);
            }
            // If there are, we add a button to toggle the collapsible section
            else
            {
                collapsibleButton = new Button();
                collapsibleButton.tooltip = "collapse/expand";
                collapsibleButton.text = "v";
                collapsibleButton.clicked += ToggleCollapseComponent;
                top_row_left.Insert(1, collapsibleButton);
            
                if (Component.Collapsed) ToggleCollapseComponent();
            }
            
            // Adding the removal button
            Button removeButton = new Button();
            removeButton.tooltip = "remove";
            removeButton.text = "X";
            removeButton.clicked += RemoveComponent;
            top_row.Add(removeButton);
            
            Drawn?.Invoke();
        }
        
        #endregion

        #region Utility methods
        
        /// <summary>
        /// Quick method to set this Component's index within its DescantNode's list of Components
        /// </summary>
        /// <param name="index">The new index to be set</param>
        public void SetOrder(int index)
        {
            this.index = index;
            order.value = index.ToString();
        }
        
        /// <summary>
        /// Somewhat involved method for adding new elements to Component parameter rows
        /// </summary>
        /// <param name="elem">The new parameter/element being added</param>
        /// <param name="info">The parameter's FieldInfo (the info from the field in the DescantNodeComponent)</param>
        void CheckAndAddParamLine(VisualElement elem, FieldInfo info)
        {
            // First we check to se if this parameter is inline
            try
            {
                string unused = ((InlineAttribute) info.GetCustomAttributes(
                    typeof(InlineAttribute),
                    true
                ).FirstOrDefault())!.ToString();
                
                // If it is, we just inset it into the top row (between the Component name and the collapse button)
                paramRows[0].Insert(2, elem);
            }
            catch
            {
                // Checking to see if it has a parameter group
                try
                {
                    string group = ((ParameterGroupAttribute) info.GetCustomAttributes(
                        typeof(ParameterGroupAttribute),
                        true
                    ).FirstOrDefault())!.Group;
                
                    // If the group doesn't already exist, we create it
                    if (!paramRowGroups.Contains(group))
                    {
                        VisualElement groupRow = new VisualElement();
                        groupRow.AddToClassList("node_component_group");
                        collapsible.Add(groupRow);
                        
                        TextElement groupName = new TextElement();
                        groupName.text = group;
                        groupName.AddToClassList("node_component_group_name");
                        groupRow.Add(groupName);
                        
                        // Making the param row be scrollable if needed
                        ScrollView paramRow = new ScrollView();
                        paramRow.mode = ScrollViewMode.Horizontal;
                        paramRow.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
                        paramRow.verticalScrollerVisibility = ScrollerVisibility.Hidden;
                        paramRow.AddToClassList("node_component_row");
                        groupRow.Add(paramRow);
                        
                        // Adding the row to the list of rows
                        paramRows.Add(paramRow);
                        paramRowGroups.Add(group);
                    }
            
                    paramRows[paramRowGroups.IndexOf(group)].Add(elem); // Finally adding the element to the row
                }
                // If there's no parameter group, we just add it to the default group
                catch { paramRows[1].Add(elem); }   
            }
        }

        /// <summary>
        /// Quick method to collapse/expand this Component
        /// </summary>
        void ToggleCollapseComponent()
        {
            if (collapsible.visible)
            {
                collapsibleButton.text = "^";
                
                // Making sure it doesn't appear with USS
                collapsible.visible = false;
                collapsible.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);

                Component.Collapsed = true;
            }
            else
            {
                collapsibleButton.text = "v";
                
                // Making sure it appears with USS
                collapsible.visible = true;
                collapsible.style.display = new StyleEnum<DisplayStyle>();

                Component.Collapsed = false;
            }
        }

        /// <summary>
        /// Method called when this Component removes itself
        /// </summary>
        void RemoveComponent()
        {
            node.VisualComponents.Remove(this);
            node.ComponentCounts[componentName]--; // Decrementing the Component's node's count for this particular type of Component

            // Now that we've removed it from the count, can future nodes of this type be added?
            if (node.ComponentCounts[componentName] < DescantComponentUtilities.GetComponentMaximum(componentName))
            {
                node.ComponentDropdown.choices.Add(componentName);
                node.ComponentDropdown.choices.Sort();
            }
            
            node.UpdateComponents();
            
            DescantEditorUtilities.RemoveElement(this); // Removing the actual Component from the hierarchy
            
            graphView.Editor.CheckAndSave(); // Check for autosave
        }

        #endregion
    }
}
#endif