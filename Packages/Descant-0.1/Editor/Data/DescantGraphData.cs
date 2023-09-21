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
        public bool Autosave;
        
        public int ChoiceNodeID;
        public int ResponseNodeID;
        public int EndNodeID;
        public int GroupID;
        
        public List<ChoiceNodeData> ChoiceNodes;
        public List<ResponseNodeData> ResponseNodes;
        public StartNodeData StartNode;
        public List<EndNodeData> EndNodes;
        
        public List<GroupData> Groups;
        
        public List<ConnectionData> Connections;

        public DescantGraphData(string name)
        {
            Name = name;
            Autosave = false;
            ChoiceNodes = new List<ChoiceNodeData>();
            ResponseNodes = new List<ResponseNodeData>();
            StartNode = null;
            EndNodes = new List<EndNodeData>();
            Groups = new List<GroupData>();
            Connections = new List<ConnectionData>();
        }

        public void Save()
        {
            // TODO: fix path
            File.WriteAllText(
                Application.dataPath + "/" + Name + ".desc",
                DescantUtilities.FormatJSON(JsonUtility.ToJson(this)));
        }

        public void ClearConnectionDuplicates()
        {
            for (int i = 0; i < Connections.Count; i++)
                for (int j = i + 1; j < Connections.Count; j++)
                    if (Connections[i].Equals(Connections[j]))
                        Connections.RemoveAt(j);
        }
    }
}