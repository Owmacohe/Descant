using System;
using System.Collections.Generic;
using UnityEngine;

namespace Editor.Data
{
    /// <summary>
    /// Serializable class to hold the data for saving and loading Descant groups
    /// </summary>
    [Serializable]
    public class DescantGroupData
    {
        public string Name;
        public int ID;
        public Vector2 Position;
        public List<string> Nodes;
        public List<int> NodeIDs;

        public DescantGroupData(string name, int id, Vector2 position, List<string> nodes, List<int> nodeIDs)
        {
            Name = name;
            ID = id;
            Position = position;
            Nodes = nodes;
            NodeIDs = nodeIDs;
        }

        public override bool Equals(object other)
        {
            return Equals((DescantGroupData)other);
        }
        
        public bool Equals(DescantGroupData other)
        {
            return
                Name == other.Name &&
                ID == other.ID &&
                Position == other.Position &&
                DescantUtilities.AreListsEqual(Nodes, other.Nodes) &&
                DescantUtilities.AreListsEqual(NodeIDs, other.NodeIDs);
        }
        
        public override string ToString()
        {
            string temp = "";

            for (int i = 0; i < Nodes.Count; i++)
                temp += " " + NodeIDs[i] + Nodes[i];
            
            return GetType() + " (" + ID + Name + " " + Position + ") (" +
                   (temp.Length > 1 ? temp.Substring(1) : "") + ")";
        }
    }
    
    /// <summary>
    /// Serializable class to hold the data for saving and loading Descant node connections
    /// </summary>
    [Serializable]
    public class DescantConnectionData
    {
        public string From;
        public int FromID;
        public string To;
        public int ToID;
        public int ChoiceIndex;

        public DescantConnectionData(string from, int fromID, string to, int toID, int choiceIndex = 0)
        {
            From = from;
            FromID = fromID;
            To = to;
            ToID = toID;
            ChoiceIndex = choiceIndex;
        }

        public override bool Equals(object other)
        {
            return Equals((DescantConnectionData)other);
        }

        public bool Equals(DescantConnectionData other)
        {
            return
                From == other.From && FromID == other.FromID &&
                To == other.To && ToID == other.ToID &&
                ChoiceIndex == other.ChoiceIndex;
        }

        /// <summary>
        /// Alternate Equals method to only determine directional equality (i.e. ignores the ChoiceIndex)
        /// </summary>
        /// <param name="other">The connection to compare with this connection</param>
        /// <returns>Whether the connections have the same direction</returns>
        public bool DirectionsEqual(DescantConnectionData other)
        {
            return
                From == other.From && FromID == other.FromID &&
                To == other.To && ToID == other.ToID;
        }

        /// <summary>
        /// Determines whether the connection points to itself
        /// </summary>
        /// <returns></returns>
        public bool ToItself()
        {
            return From == To && FromID == ToID;
        }

        public override string ToString()
        {
            return GetType() + " (" + FromID + From + " " + ToID + To + " " + ChoiceIndex + ")";
        }
    }
}