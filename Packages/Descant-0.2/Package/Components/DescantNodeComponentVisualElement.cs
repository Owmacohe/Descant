using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Descant.Package.Components;
using Descant.Package.Editor.Nodes;
using Descant.Package.Editor.Window;
using UnityEngine;
using UnityEngine.UIElements;

namespace Descant.Package.Components
{
    public class DescantNodeComponentVisualElement : VisualElement
    {
        public string Name { get; }
        
        DescantGraphView GraphView;
        DescantNode Node;
        List<VisualElement> ParamRows;
        
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

            VisualElement top_row = new VisualElement();
            top_row.AddToClassList("node_component_row");
            Add(top_row);
            ParamRows.Add(top_row);
            
            Button removeComponent = new Button();
            removeComponent.text = "X";
            removeComponent.clicked += RemoveComponent;
            top_row.Add(removeComponent);
            
            TextElement name = new TextElement();
            name.text = Name;
            top_row.Add(name);

            ScrollView paramRow1 = new ScrollView();
            paramRow1.mode = ScrollViewMode.Horizontal;
            paramRow1.horizontalScrollerVisibility = ScrollerVisibility.Auto;
            paramRow1.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            paramRow1.AddToClassList("node_component_row");
            Add(paramRow1);
            ParamRows.Add(paramRow1);

            TextElement row1Label = new TextElement();
            row1Label.text = "1";
            row1Label.AddToClassList("node_component_row_label");
            paramRow1.Add(row1Label);

            List<Type> types = DescantUtilities.GetAllNodeComponentTypes();

            foreach (var i in types[DescantUtilities.TrimmedNodeComponentTypes(types).IndexOf(Name)].GetFields())
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
                }
            }

            if (ParamRows.Count == 2 && paramRow1.childCount <= 1)
            {
                VisualElement row1 = ParamRows[1];
                ParamRows.RemoveAt(1);
                DescantUtilities.RemoveElement(row1);
            }
        }

        void CheckAndAddParamLine(VisualElement elem, FieldInfo info)
        {
            try
            {
                int line = (((InlineGroupAttribute) info.GetCustomAttributes(
                    typeof(InlineGroupAttribute),
                    true
                ).FirstOrDefault())!).Line;
                
                if (line >= ParamRows.Count)
                {
                    for (int i = 0; i < (line + 1) - ParamRows.Count; i++)
                    {
                        ScrollView paramRow = new ScrollView();
                        paramRow.mode = ScrollViewMode.Horizontal;
                        paramRow.horizontalScrollerVisibility = ScrollerVisibility.Auto;
                        paramRow.verticalScrollerVisibility = ScrollerVisibility.Hidden;
                        paramRow.AddToClassList("node_component_row");
                        Add(paramRow);
                        ParamRows.Add(paramRow);
                        
                        TextElement rowLabel = new TextElement();
                        rowLabel.text = (ParamRows.Count - 1).ToString();
                        rowLabel.AddToClassList("node_component_row_label");
                        paramRow.Add(rowLabel);
                    }
                }
            
                ParamRows[line].Add(elem);
            }
            catch
            {
                ParamRows[1].Add(elem);
            }
        }

        void RemoveComponent()
        {
            Node.Components[Name]--;

            if (Node.Components[Name] < DescantUtilities.GetNodeComponentMaximum(Name))
            {
                Node.ComponentDropdown.choices.Add(Name);
                Node.ComponentDropdown.choices.Sort();
            }
            
            DescantUtilities.RemoveElement(this);
            
            GraphView.Editor.CheckAndSave(); // Check for autosave
        }
    }
}