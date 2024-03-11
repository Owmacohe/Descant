using System;
using System.Collections.Generic;
using DescantComponents;
using UnityEngine;

namespace DescantEditor
{
    /// <summary>
    /// Serializable class to hold the data for saving and loading Descant graphs
    /// </summary>
    [Serializable, CreateAssetMenu]
    public class DescantGraph : ScriptableObject
    {
        /// <summary>
        /// Whether to autosave the graph when in the Editor
        /// </summary>
        [HideInInspector] public bool Autosave;

        /// <summary>
        /// Whether or not to type out ResponseNode text one character at a time
        /// </summary>
        public bool Typewriter;
        
        /// <summary>
        /// The speed of the typewriter (higher value = faster typing speed)
        /// (see DescantDialogueUI::Type() for its application)
        /// </summary>
        public float TypewriterSpeed;
        
        /// <summary>
        /// A unique ID number that will be applied to the next-created ChoiceNode
        /// </summary>
        public int ChoiceNodeID;
        
        /// <summary>
        /// A unique ID number that will be applied to the next-created ResponseNode
        /// </summary>
        public int ResponseNodeID;
        
        /// <summary>
        /// A unique ID number that will be applied to the next-created EndNode
        /// </summary>
        public int EndNodeID;
        
        /// <summary>
        /// A unique ID number that will be applied to the next-created NodeGroup
        /// </summary>
        public int GroupID;
        
        /// <summary>
        /// The ChoiceNodes in the graph
        /// </summary>
        public List<DescantChoiceNodeData> ChoiceNodes;
        
        /// <summary>
        /// The ResponseNodes in the graph
        /// </summary>
        public List<DescantResponseNodeData> ResponseNodes;
        
        /// <summary>
        /// The StartNode in the graph
        /// </summary>
        public DescantStartNodeData StartNode;
        
        /// <summary>
        /// The EndNodes in the graph
        /// </summary>
        public List<DescantEndNodeData> EndNodes;
        
        /// <summary>
        /// The NodeGroups in the graph
        /// </summary>
        public List<DescantGroupData> Groups;
        
        /// <summary>
        /// The connections between Nodes in the graph
        /// </summary>
        public List<DescantConnectionData> Connections;

        /// <summary>
        /// Parameterized constructor
        /// (most of the DescantGroupData's properties are set after
        /// it has been initialized, as part of the saving process)
        /// </summary>
        public DescantGraph()
        {
            Autosave = false;
            Typewriter = true;
            TypewriterSpeed = 1;
            ChoiceNodes = new List<DescantChoiceNodeData>();
            ResponseNodes = new List<DescantResponseNodeData>();
            
            // We assume that we'll replace this later, but just in case we don't, we create a default StartNode
            StartNode = new DescantStartNodeData(
                "StartNode",
                "Start",
                Vector2.zero,
                new List<DescantComponent>()
            );
            
            EndNodes = new List<DescantEndNodeData>();
            Groups = new List<DescantGroupData>();
            Connections = new List<DescantConnectionData>();
        }

        /// <summary>
        /// Overridden Equals method
        /// </summary>
        /// <param name="other">The object being compared against</param>
        /// <returns>Whether the other object has the same data as this one</returns>
        public override bool Equals(object other)
        {
            try
            {
                _ = (DescantGraph) other;
            }
            catch
            {
                return false;
            }
            
            return Equals((DescantGraph)other);
        }

        #if UNITY_EDITOR
        /// <summary>
        /// Custom Equals method
        /// </summary>
        /// <param name="other">The data object being compared against</param>
        /// <returns>Whether the other DescantGraphData has the same data as this one</returns>
        public bool Equals(DescantGraph other)
        {
            return ToString() == other.ToString();
        }
        #endif

        /// <summary>
        /// Overridden ToString method
        /// </summary>
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

            return GetType() + " (" + name + " " + Autosave + " " +
                   ChoiceNodeID + " " + ResponseNodeID + " " + EndNodeID + " " + GroupID + ")" +
                   (temp.Length > 1 ? temp : "");
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