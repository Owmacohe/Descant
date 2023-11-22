#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DescantComponents;
using UnityEngine;
using UnityEngine.UIElements;

namespace DescantEditor
{
    public class DescantNodeComponentVisualElement : VisualElement
    {
        public string Name { get; }
        public DescantNodeComponent Component { get; }
        
        DescantGraphView graphView;
        DescantNode node;
        
        List<VisualElement> paramRows;
        List<string> paramRowGroups;
        VisualElement collabsible;

        public DescantNodeComponentVisualElement(DescantGraphView graph, DescantNode descantNode, string name, DescantNodeComponent component)
        {
            graphView = graph;
            node = descantNode;
            Name = name;

            if (component == null) Component = (DescantNodeComponent) Activator.CreateInstance(GetComponentType());
            else Component = component;
        }

        public void Draw()
        {
            AddToClassList("node_component");
            
            paramRows = new List<VisualElement>();
            paramRowGroups = new List<string>();

            VisualElement top_row = new VisualElement();
            top_row.AddToClassList("node_component_row");
            Add(top_row);

            VisualElement top_row_left = new VisualElement();
            top_row_left.AddToClassList("node_component_row");
            top_row.Add(top_row_left);
            paramRows.Add(top_row_left);
            paramRowGroups.Add("");
            
            Button removeComponent = new Button();
            removeComponent.text = "X";
            removeComponent.clicked += RemoveComponent;
            top_row_left.Add(removeComponent);
            
            TextElement name = new TextElement();
            name.text = Name;
            top_row_left.Add(name);

            collabsible = new VisualElement();
            Add(collabsible);

            ScrollView paramRow1 = new ScrollView();
            paramRow1.mode = ScrollViewMode.Horizontal;
            paramRow1.horizontalScrollerVisibility = ScrollerVisibility.Auto;
            paramRow1.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            paramRow1.AddToClassList("node_component_row");
            collabsible.Add(paramRow1);
            paramRows.Add(paramRow1);
            paramRowGroups.Add("");
            
            // TODO: make it scrollable

            string[] ignoreFields = { "Collapsed" };
            
            foreach (var i in GetComponentType().GetFields())
            {
                if (!ignoreFields.Contains(i.Name))
                {
                    string label = i.Name;
                    
                    string formattedType = i.FieldType.ToString();
                    formattedType = formattedType.Substring(formattedType.LastIndexOf('.') + 1);

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
                    
                    var componentValue = i.GetValue(Component);
                    
                    if (i.FieldType == typeof(float) || i.FieldType == typeof(int) || i.FieldType == typeof(string))
                    {
                        TextField temp = new TextField();
                        temp.label = label;
                        CheckAndAddParamLine(temp, i);

                        if (componentValue != null) temp.value = componentValue.ToString();

                        temp.RegisterValueChangedCallback(callback =>
                        {
                            i.SetValue(Component, DescantComponentUtilities.GetComponentParameterValue(
                                callback.newValue,
                                i.FieldType
                            ));
                            
                            graphView.Editor.CheckAndSave(); // Check for autosave
                        });
                    }
                    else if (i.FieldType == typeof(bool))
                    {
                        Toggle temp = new Toggle();
                        temp.label = label;
                        CheckAndAddParamLine(temp, i);

                        if (componentValue != null) temp.value = (bool)componentValue;
                        
                        temp.RegisterValueChangedCallback(callback =>
                        {
                            i.SetValue(Component, callback.newValue);
                            
                            graphView.Editor.CheckAndSave(); // Check for autosave
                        });
                    }
                    else if (i.FieldType.IsSubclassOf(typeof(Enum)))
                    {
                        List<string> enumValues = new List<string>();
                        
                        foreach (var j in i.FieldType.GetFields())
                            enumValues.Add(j.ToString().Substring(j.ToString().LastIndexOf(' ') + 1));
                        
                        PopupField<string> temp = new PopupField<string>(
                            enumValues.GetRange(1, enumValues.Count - 1), 0);
                        temp.label = label;
                        CheckAndAddParamLine(temp, i);
                        
                        if (componentValue != null) temp.value = componentValue.ToString();
                        
                        temp.RegisterValueChangedCallback(callback =>
                        {
                            string enumName = i.FieldType.Name;

                            switch (enumName.Substring(enumName.LastIndexOf('.') + 1))
                            {
                                case "VariableType":
                                    i.SetValue(Component, DescantComponentUtilities.ParseEnum<VariableType>(callback.newValue));
                                    break;
                                
                                case "ComparisonType":
                                    i.SetValue(Component, DescantComponentUtilities.ParseEnum<ComparisonType>(callback.newValue));
                                    break;
                                
                                case "OperationType":
                                    i.SetValue(Component, DescantComponentUtilities.ParseEnum<OperationType>(callback.newValue));
                                    break;
                                
                                case "ListChangeType":
                                    i.SetValue(Component, DescantComponentUtilities.ParseEnum<ListChangeType>(callback.newValue));
                                    break;
                            }

                            graphView.Editor.CheckAndSave(); // Check for autosave
                        });
                    }
                }
            }

            if (paramRows.Count == 2 && paramRow1.childCount <= 1)
            {
                VisualElement row1 = paramRows[1];
                paramRows.RemoveAt(1);
                DescantEditorUtilities.RemoveElement(row1);
            }
            else
            {
                Button collapseComponent = new Button();
                collapseComponent.text = "v";
                collapseComponent.clicked += () =>
                {
                    ToggleCollapseComponent(collapseComponent);
                };
                top_row.Add(collapseComponent);
            
                if (Component.Collapsed)
                    ToggleCollapseComponent(collapseComponent);
            }
        }

        void CheckAndAddParamLine(VisualElement elem, FieldInfo info)
        {
            try
            {
                string unused = ((InlineAttribute) info.GetCustomAttributes(
                    typeof(InlineAttribute),
                    true
                ).FirstOrDefault())!.ToString();
                
                paramRows[0].Insert(2, elem);
            }
            catch
            {
                try
                {
                    string group = ((ParameterGroupAttribute) info.GetCustomAttributes(
                        typeof(ParameterGroupAttribute),
                        true
                    ).FirstOrDefault())!.Group;
                
                    if (!paramRowGroups.Contains(group))
                    {
                        VisualElement groupRow = new VisualElement();
                        groupRow.AddToClassList("node_component_group");
                        collabsible.Add(groupRow);
                        
                        TextElement groupName = new TextElement();
                        groupName.text = group;
                        groupName.AddToClassList("node_component_group_name");
                        groupRow.Add(groupName);
                        
                        ScrollView paramRow = new ScrollView();
                        paramRow.mode = ScrollViewMode.Horizontal;
                        paramRow.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
                        paramRow.verticalScrollerVisibility = ScrollerVisibility.Hidden;
                        paramRow.AddToClassList("node_component_row");
                        groupRow.Add(paramRow);
                        paramRows.Add(paramRow);
                        paramRowGroups.Add(group);
                    }
            
                    paramRows[paramRowGroups.IndexOf(group)].Add(elem);
                }
                catch
                {
                    paramRows[1].Add(elem);
                }   
            }
        }

        void ToggleCollapseComponent(Button collapseComponent)
        {
            if (collabsible.visible)
            {
                collapseComponent.text = "^";
                collabsible.visible = false;
                collabsible.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);

                Component.Collapsed = true;
            }
            else
            {
                collapseComponent.text = "v";
                collabsible.visible = true;
                collabsible.style.display = new StyleEnum<DisplayStyle>();

                Component.Collapsed = false;
            }
        }

        void RemoveComponent()
        {
            node.Components[Name]--;

            if (node.Components[Name] < DescantComponentUtilities.GetNodeComponentMaximum(Name))
            {
                node.ComponentDropdown.choices.Add(Name);
                node.ComponentDropdown.choices.Sort();
            }
            
            DescantEditorUtilities.RemoveElement(this);
            
            graphView.Editor.CheckAndSave(); // Check for autosave
        }

        Type GetComponentType()
        {
            List<Type> types = DescantComponentUtilities.GetAllNodeComponentTypes();
            return types[DescantComponentUtilities.TrimmedNodeComponentTypes(types).IndexOf(Name)];
        }
    }
}
#endif