using System;
using System.Collections.Generic;
using System.IO;
using Editor.Window;
using UnityEngine;

namespace Editor.Data
{
    [Serializable]
    public class DescantGraphData
    {
        public string Name;
        public string Path;
        public bool Autosave;
        
        public int ChoiceNodeID;
        public int ResponseNodeID;
        public int EndNodeID;
        public int GroupID;
        
        public List<DescantChoiceNodeData> ChoiceNodes;
        public List<DescantResponseNodeData> ResponseNodes;
        public DescantStartNodeData StartNode;
        public List<DescantEndNodeData> EndNodes;
        
        public List<DescantGroupData> Groups;
        
        public List<DescantConnectionData> Connections;

        public DescantGraphData(string name)
        {
            Name = name;
            Path = "";
            Autosave = false;
            ChoiceNodes = new List<DescantChoiceNodeData>();
            ResponseNodes = new List<DescantResponseNodeData>();
            StartNode = null;
            EndNodes = new List<DescantEndNodeData>();
            Groups = new List<DescantGroupData>();
            Connections = new List<DescantConnectionData>();
        }

        public override bool Equals(object other)
        {
            return Equals((DescantGraphData)other);
        }

        public bool Equals(DescantGraphData other)
        {
            return ToString() == other.ToString();
        }

        public override string ToString()
        {
            string temp = "";

            foreach (var i in ChoiceNodes)
                temp += "\n\t" + i;
            
            foreach (var j in ResponseNodes)
                temp += "\n\t" + j;

            temp += "\n\t" + StartNode;
            
            foreach (var k in EndNodes)
                temp += "\n\t" + k;
            
            foreach (var l in Groups)
                temp += "\n\t" + l;
            
            foreach (var m in Connections)
                temp += "\n\t" + m;
            
            return GetType() + " (" + Name + " " + Autosave + " " +
                   ChoiceNodeID + " " + ResponseNodeID + " " + EndNodeID + " " + GroupID + ")" +
                   "\n" + Path +
                   (temp.Length > 1 ? temp : "");
        }

        public void Save(bool newFile)
        {
            if (newFile) Path = DescantUtilities.GetCurrentDirectory() + Name + ".desc";
            
            File.WriteAllText(Path, DescantUtilities.FormatJSON(JsonUtility.ToJson(this)));
        }

        public static DescantGraphData Load(string fullPath)
        {
            return JsonUtility.FromJson<DescantGraphData>(File.ReadAllText(fullPath));
        }

        public void CleanUpConnections()
        {
            for (int i = 0; i < Connections.Count; i++)
                for (int j = i + 1; j < Connections.Count; j++)
                    if (Connections[i].Equals(Connections[j]) ||
                        Connections[i].DirectionsEqual(Connections[j]) ||
                        Connections[j].ToItself())
                        Connections.RemoveAt(j);
        }
    }
}