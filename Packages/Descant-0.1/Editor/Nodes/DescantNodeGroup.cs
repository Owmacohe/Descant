using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DescantNodeGroup : Group
{
    public DescantNodeGroup(Vector2 position)
    {
        SetPosition(new Rect(position, Vector2.zero));
    }

    public void Draw()
    {
        title = "Node Group";
        
        Button removeGroup = new Button();
        removeGroup.text = "X";
        removeGroup.clicked += RemoveGroup;
        headerContainer.Add(removeGroup);

        headerContainer.style.flexDirection = FlexDirection.Row;
        headerContainer.style.alignItems = Align.Center;
        headerContainer.style.justifyContent = Justify.Center;
    }
    
    void RemoveGroup()
    {
        parent.Remove(this);
    }
}