#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DescantComponents;
using DescantEditor;
using UnityEngine.UIElements;

namespace DescantEditor
{
    public class DescantNodeComponentVisualElement : VisualElement
    {
        public string Name { get; }
        
        DescantGraphView GraphView;
        DescantNode Node;
        List<VisualElement> ParamRows;
        List<string> ParamRowGroups;
        VisualElement Collabsible;

        public DescantNodeComponentVisualElement(DescantGraphView graphView, DescantNode node, string name)
        {
            GraphView = graphView;
            Node = node;
            Name = name;
        }

        public void Draw()
        {
            AddToClassList("node_component");
            
            ParamRows = new List<VisualElement>();
            ParamRowGroups = new List<string>();

            VisualElement top_row = new VisualElement();
            top_row.AddToClassList("node_component_row");
            Add(top_row);

            VisualElement top_row_left = new VisualElement();
            top_row_left.AddToClassList("node_component_row");
            top_row.Add(top_row_left);
            ParamRows.Add(top_row_left);
            ParamRowGroups.Add("");
            
            Button removeComponent = new Button();
            removeComponent.text = "X";
            removeComponent.clicked += RemoveComponent;
            top_row_left.Add(removeComponent);
            
            TextElement name = new TextElement();
            name.text = Name;
            top_row_left.Add(name);
            
            Button collapseComponent = new Button();
            collapseComponent.text = "v";
            collapseComponent.clicked += delegate
            {
                if (Collabsible.visible)
                {
                    collapseComponent.text = "^";
                    Collabsible.visible = false;
                    Collabsible.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                }
                else
                {
                    collapseComponent.text = "v";
                    Collabsible.visible = true;
                    Collabsible.style.display = new StyleEnum<DisplayStyle>();
                }
            };
            top_row.Add(collapseComponent);

            Collabsible = new VisualElement();
            Add(Collabsible);

            ScrollView paramRow1 = new ScrollView();
            paramRow1.mode = ScrollViewMode.Horizontal;
            paramRow1.horizontalScrollerVisibility = ScrollerVisibility.Auto;
            paramRow1.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            paramRow1.AddToClassList("node_component_row");
            Collabsible.Add(paramRow1);
            ParamRows.Add(paramRow1);
            ParamRowGroups.Add("");

            List<Type> types = DescantComponentUtilities.GetAllNodeComponentTypes();

            foreach (var i in types[DescantComponentUtilities.TrimmedNodeComponentTypes(types).IndexOf(Name)].GetFields())
            {
                if (i.Name is not "ID" and not "MaxQuantity" and not "NodeID")
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
                    
                    if (i.FieldType == typeof(float) || i.FieldType == typeof(int) || i.FieldType == typeof(string))
                    {
                        TextField temp = new TextField();
                        temp.label = label;
                        CheckAndAddParamLine(temp, i);

                        temp.UnregisterValueChangedCallback(callback =>
                        {
                            GraphView.Editor.CheckAndSave(); // Check for autosave
                        });
                    }
                    else if (i.FieldType == typeof(bool))
                    {
                        Toggle temp = new Toggle();
                        temp.label = label;
                        CheckAndAddParamLine(temp, i);
                        
                        temp.UnregisterValueChangedCallback(callback =>
                        {
                            GraphView.Editor.CheckAndSave(); // Check for autosave
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
                        
                        temp.UnregisterValueChangedCallback(callback =>
                        {
                            GraphView.Editor.CheckAndSave(); // Check for autosave
                        });
                    }
                    else if (i.FieldType == typeof(Event))
                    {
                        // TODO
                    }
                }
            }

            if (ParamRows.Count == 2 && paramRow1.childCount <= 1)
            {
                VisualElement row1 = ParamRows[1];
                ParamRows.RemoveAt(1);
                DescantEditorUtilities.RemoveElement(row1);
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
                
                ParamRows[0].Insert(2, elem);
            }
            catch
            {
                try
                {
                    string group = ((ParameterGroupAttribute) info.GetCustomAttributes(
                        typeof(ParameterGroupAttribute),
                        true
                    ).FirstOrDefault())!.Group;
                
                    if (!ParamRowGroups.Contains(group))
                    {
                        VisualElement groupRow = new VisualElement();
                        groupRow.AddToClassList("node_component_group");
                        Collabsible.Add(groupRow);
                        
                        TextElement groupName = new TextElement();
                        groupName.text = group;
                        groupName.AddToClassList("node_component_group_name");
                        groupRow.Add(groupName);
                        
                        ScrollView paramRow = new ScrollView();
                        paramRow.mode = ScrollViewMode.Horizontal;
                        paramRow.horizontalScrollerVisibility = ScrollerVisibility.Auto;
                        paramRow.verticalScrollerVisibility = ScrollerVisibility.Hidden;
                        paramRow.AddToClassList("node_component_row");
                        groupRow.Add(paramRow);
                        ParamRows.Add(paramRow);
                        ParamRowGroups.Add(group);
                    }
            
                    ParamRows[ParamRowGroups.IndexOf(group)].Add(elem);
                }
                catch
                {
                    ParamRows[1].Add(elem);
                }   
            }
        }

        void RemoveComponent()
        {
            Node.Components[Name]--;

            if (Node.Components[Name] < DescantComponentUtilities.GetNodeComponentMaximum(Name))
            {
                Node.ComponentDropdown.choices.Add(Name);
                Node.ComponentDropdown.choices.Sort();
            }
            
            DescantEditorUtilities.RemoveElement(this);
            
            GraphView.Editor.CheckAndSave(); // Check for autosave
        }
    }
}
#endif