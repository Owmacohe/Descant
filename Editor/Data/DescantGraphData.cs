using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DescantEditor
{
    /// <summary>
    /// Serializable class to hold the data for saving and loading Descant graphs
    /// </summary>
    [Serializable]
    public class DescantGraphData
    {
        public string Name;
        
        /// <summary>
        /// The local path to the file after the Assets folder
        /// (useful for knowing where to save the file when no longer in its directory)
        /// </summary>
        public string Path;
        
        /// <summary>
        /// Whether to autosave this graph when in the Editor
        /// </summary>
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

#if UNITY_EDITOR
        /// <summary>
        /// Saves all the data from this graph to its path
        /// </summary>
        /// <param name="newFile">Whether this is the first time this graph has been saved</param>
        public void Save(bool newFile)
        {
            // Setting the local path if this is the first time
            if (newFile) Path = DescantEditorUtilities.GetCurrentLocalDirectory() + Name + ".desc.json";
            
            // Saving to the full path
            File.WriteAllText(
                Application.dataPath + "/" + Path,
                DescantEditorUtilities.FormatJSON(JsonUtility.ToJson(this)));
        }
#endif

        /// <summary>
        /// Loads and returns a new graph from a full disc path
        /// </summary>
        /// <param name="fullPath">The full disc path to the file</param>
        /// <returns>A loaded Descant graph</returns>
        public static DescantGraphData LoadFromPath(string fullPath)
        {
            return LoadFromString(File.ReadAllText(fullPath));
        }
        
        /// <summary>
        /// Loads and returns a new graph from a JSON-formatted string
        /// </summary>
        /// <param name="str">The string containing all the data for the graph</param>
        /// <returns>A loaded Descant graph</returns>
        public static DescantGraphData LoadFromString(string str)
        {
            return JsonUtility.FromJson<DescantGraphData>(str);
        }

        /// <summary>
        /// Checks through all the graph's connections, making sure none of them are redundant or connect to themselves
        /// </summary>
        /// <param name="checksToPerform">The number of checks that should be performed before we're satisfied that all connections are good</param>
        public void CleanUpConnections(int checksToPerform = 2)
        {
            for (int i = 0; i < Connections.Count; i++)
            {
                for (int j = i + 1; j < Connections.Count; j++)
                {
                    if (Connections[j].Equals(Connections[i]) ||
                        Connections[j].IllegalChoiceFrom() ||
                        Connections[j].ToItself())
                        Connections.RemoveAt(j);
                }
            }

            // Multiple checks are usually performed just to make sure it's all nicely cleaned up
            if (checksToPerform > 1) CleanUpConnections(--checksToPerform);
        }
    }
}