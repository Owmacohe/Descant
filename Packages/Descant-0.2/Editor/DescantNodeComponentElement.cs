using System;
using System.Collections.Generic;
using System.Linq;
using Components;
using Editor.Window;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    public class DescantNodeComponentElement : VisualElement
    {
        public string Name { get; }
        
        DescantGraphView GraphView;
        
        public DescantNodeComponentElement(DescantGraphView graphView, string name)
        {
            GraphView = graphView;
            Name = name;
        }

        public void Draw()
        {
            AddToClassList("node_component");
            
            Button removeComponent = new Button();
            removeComponent.text = "X";
            removeComponent.clicked += RemoveComponent;
            Add(removeComponent);
            
            TextElement name = new TextElement();
            name.text = Name;
            
            Add(name);

            List<Type> types = DescantUtilities.GetAllNodeComponentTypes();
            int fields = 0;

            foreach (var i in types[DescantUtilities.TrimmedNodeComponentTypes(types).IndexOf(Name)].GetFields())
            {
                if (i.Name is not "ID" and not "MaxQuantity" and not "NodeID")
                {
                    string label = i.FieldType.ToString();
                    label = label.Substring(label.LastIndexOf('.') + 1);

                    switch (label)
                    {
                        case "Int32":
                            label = "int";
                            break;
                        
                        case "Single":
                            label = "float";
                            break;
                        
                        case "String":
                            label = "string";
                            break;
                    }
                    
                    label = i.Name + "\n<i>(" + label + ")</i>";
                    
                    if (i.FieldType == typeof(float) || i.FieldType == typeof(int) || i.FieldType == typeof(string))
                    {
                        TextField temp = new TextField();
                        temp.label = label;
                        Add(temp);

                        temp.UnregisterValueChangedCallback(callback =>
                        {
                            GraphView.Editor.CheckAndSave(); // Check for autosave
                        });
                        
                        fields++;
                    }
                    else if (i.FieldType.IsSubclassOf(typeof(Enum)))
                    {
                        List<string> enumValues = new List<string>();
                        
                        foreach (var j in i.FieldType.GetFields())
                            enumValues.Add(j.ToString().Substring(j.ToString().LastIndexOf(' ') + 1));
                        
                        PopupField<string> temp = new PopupField<string>(
                            enumValues.GetRange(1, enumValues.Count - 1), 0);
                        temp.label = label;
                        Add(temp);
                        
                        temp.UnregisterValueChangedCallback(callback =>
                        {
                            GraphView.Editor.CheckAndSave(); // Check for autosave
                        });
                        
                        fields += 2;
                    }
                }
            }
        }

        void RemoveComponent()
        {
            DescantUtilities.RemoveElement(this);
            
            GraphView.Editor.CheckAndSave(); // Check for autosave
        }
    }
}