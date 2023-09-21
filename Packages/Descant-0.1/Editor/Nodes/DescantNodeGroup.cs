using Editor.Window;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Nodes
{
    public class DescantNodeGroup : Group
    {
        public int ID { get; set; }
        public string Name { get; set; }
        
        DescantGraphView graphView;
    
        public DescantNodeGroup(
            DescantGraphView graphView,
            Vector2 position)
        {
            this.graphView = graphView;
            SetPosition(new Rect(position, Vector2.zero));
        }

        public void Draw()
        {
            if (Name == "") Name = "NodeGroup";
            
            if (ID < 0)
            {
                ID = graphView.GroupID;
                graphView.GroupID++;
            }

            title = Name;
            DescantUtilities.FindFirstElement<TextField>(this).value = Name;
            
            DescantUtilities.FindFirstElement<TextField>(this).RegisterValueChangedCallback(callback =>
            {
                TextField target = (TextField) callback.target;
                target.value = DescantUtilities.FilterText(target.value);
                
                graphView.CheckAndSave();
            });
        
            Button removeGroup = new Button();
            removeGroup.text = "X";
            removeGroup.clicked += RemoveGroup;
            headerContainer.Add(removeGroup);

            headerContainer.style.flexDirection = FlexDirection.Row;
            headerContainer.style.alignItems = Align.Center;
            headerContainer.style.justifyContent = Justify.Center;
            
            // TODO: moving callback
        }

        void RemoveGroup()
        {
            DescantUtilities.RemoveElement(this);
            
            graphView.Groups.Remove(this);
            
            graphView.CheckAndSave();
        }
    }
}