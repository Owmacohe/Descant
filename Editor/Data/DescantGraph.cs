using System;
using System.Collections.Generic;
using Descant.Components;
using UnityEngine;
using UnityEngine.Serialization;

namespace Descant.Editor
{
    /// <summary>
    /// Serializable class to hold the data for saving and loading Descant graphs
    /// </summary>
    [Serializable, CreateAssetMenu(menuName = "Descant/Graph")]
    public class DescantGraph : ScriptableObject
    {
        #region Properties
        
        /// <summary>
        /// Whether to autosave the graph when in the Editor
        /// </summary>
        [HideInInspector] public bool Autosave;

        /// <summary>
        /// Whether to show or hide the advanced features when in the editor
        /// </summary>
        [HideInInspector] public bool Advanced;

        /// <summary>
        /// The position that the graph view has been panned to
        /// </summary>
        [HideInInspector] public Vector3 PannedPosition;

        /// <summary>
        /// The scale that the graph view has been scrolled to
        /// </summary>
        [HideInInspector] public Vector3 ScrolledScale;

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
        [HideInInspector] public int ChoiceNodeID;
        
        /// <summary>
        /// A unique ID number that will be applied to the next-created ResponseNode
        /// </summary>
        [HideInInspector] public int ResponseNodeID;

        /// <summary>
        /// A unique ID number that will be applied to the next-created IfNode
        /// </summary>
        [HideInInspector] public int IfNodeID;
        
        /// <summary>
        /// A unique ID number that will be applied to the next-created EndNode
        /// </summary>
        [HideInInspector] public int EndNodeID;
        
        /// <summary>
        /// A unique ID number that will be applied to the next-created NodeGroup
        /// </summary>
        [HideInInspector] public int GroupID;
        
        /// <summary>
        /// The ChoiceNodes in the graph
        /// </summary>
        public List<DescantChoiceNodeData> ChoiceNodes;
        
        /// <summary>
        /// The ResponseNodes in the graph
        /// </summary>
        public List<DescantResponseNodeData> ResponseNodes;

        /// <summary>
        /// The IfNodes in the graph
        /// </summary>
        public List<DescantIfNodeData> IfNodes;
        
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
        [HideInInspector] public List<DescantGroupData> Groups;
        
        /// <summary>
        /// The connections between Nodes in the graph
        /// </summary>
        [HideInInspector] public List<DescantConnectionData> Connections;
        
        #endregion

        /// <summary>
        /// Parameterized constructor
        /// (most of the DescantGroupData's properties are set after
        /// it has been initialized, as part of the saving process)
        /// </summary>
        public DescantGraph()
        {
            Autosave = false;
            Advanced = true;
            PannedPosition = Vector3.zero;
            ScrolledScale = Vector3.one;
            Typewriter = true;
            TypewriterSpeed = 1;
            ChoiceNodes = new List<DescantChoiceNodeData>();
            ResponseNodes = new List<DescantResponseNodeData>();
            IfNodes = new List<DescantIfNodeData>();
            
            // We assume that we'll replace this later, but just in case we don't, we create a default StartNode
            StartNode = new DescantStartNodeData(
                "StartNode",
                "Start",
                Vector2.zero,
                new List<DescantComponent>(),
                ""
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

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(base.GetHashCode());
            hashCode.Add(Autosave);
            hashCode.Add(Advanced);
            hashCode.Add(PannedPosition);
            hashCode.Add(ScrolledScale);
            hashCode.Add(Typewriter);
            hashCode.Add(TypewriterSpeed);
            hashCode.Add(ChoiceNodeID);
            hashCode.Add(ResponseNodeID);
            hashCode.Add(EndNodeID);
            hashCode.Add(GroupID);
            hashCode.Add(ChoiceNodes);
            hashCode.Add(ResponseNodes);
            hashCode.Add(IfNodes);
            hashCode.Add(StartNode);
            hashCode.Add(EndNodes);
            hashCode.Add(Groups);
            hashCode.Add(Connections);
            return hashCode.ToHashCode();
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
            
            foreach (var k in IfNodes)
                temp += "\n\t" + k;

            temp += "\n\t" + StartNode;
            
            foreach (var l in EndNodes)
                temp += "\n\t" + l;
            
            foreach (var m in Groups)
                temp += "\n\t" + m;
            
            foreach (var n in Connections)
                temp += "\n\t" + n;

            return GetType() + " (" + name + " " + Autosave + " " + Advanced + " " + Typewriter + " " + TypewriterSpeed + " " +
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
                        Connections[j].IllegalChoiceOrIf() ||
                        Connections[j].ToItself() ||
                        Connections[j].To.Equals("null"))
                        Connections.RemoveAt(j);
                }
            }

            // Multiple checks are usually performed just to make sure it's all nicely cleaned up
            if (checksToPerform > 1) CleanUpConnections(--checksToPerform);
        }
    }
}