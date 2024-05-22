using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Descant.Editor
{
    #region DescantGroupData
    
    /// <summary>
    /// Serializable class to hold the data for saving and loading Descant groups
    /// </summary>
    [Serializable]
    public class DescantGroupData
    {
        /// <summary>
        /// The group's custom name
        /// </summary>
        public string Name;
        
        /// <summary>
        /// The group's ID
        /// </summary>
        public int ID;
        
        /// <summary>
        /// The group's position
        /// </summary>
        public Vector2 Position;
        
        /// <summary>
        /// The names of the nodes contained within the group
        /// </summary>
        public List<string> Nodes;
        
        /// <summary>
        /// The IDs of the nodes contained within the group
        /// </summary>
        public List<int> NodeIDs;

        /// <summary>
        /// The comments associated with this group
        /// </summary>
        public string Comments;

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="name">The group's custom name</param>
        /// <param name="id">The group's ID</param>
        /// <param name="position">The group's position</param>
        /// <param name="nodes">The names of the nodes contained within the group</param>
        /// <param name="nodeIDs">The IDs of the nodes contained within the group</param>
        /// <param name="comments">The comments associated with this group</param>
        public DescantGroupData(string name, int id, Vector2 position, List<string> nodes, List<int> nodeIDs, string comments)
        {
            Name = name;
            ID = id;
            Position = position;
            Nodes = nodes;
            NodeIDs = nodeIDs;
            Comments = comments;
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
                _ = (DescantGroupData) other;
            }
            catch
            {
                return false;
            }
            
            return Equals((DescantGroupData)other);
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(Name, ID, Position, Nodes, NodeIDs, Comments);
        }

        #if UNITY_EDITOR
        /// <summary>
        /// Custom Equals method
        /// </summary>
        /// <param name="other">The data object being compared against</param>
        /// <returns>Whether the other DescantGroupData has the same data as this one</returns>
        public bool Equals(DescantGroupData other)
        {
            return
                Name == other.Name &&
                ID == other.ID &&
                Position == other.Position &&
                DescantEditorUtilities.AreListsEqual(Nodes, other.Nodes) &&
                DescantEditorUtilities.AreListsEqual(NodeIDs, other.NodeIDs) &&
                Comments == other.Comments;
        }
        #endif
        
        /// <summary>
        /// Overridden ToString method
        /// </summary>
        public override string ToString()
        {
            string temp = "";

            for (int i = 0; i < Nodes.Count; i++)
                temp += " " + NodeIDs[i] + Nodes[i];
            
            return GetType() + " (" + ID + Name + " " + Position + ") (" +
                   (temp.Length > 1 ? temp.Substring(1) : "") + ") (" +
                   Comments + " )";
        }
    }
    
    #endregion
    
    #region DescantConnectionData
    
    /// <summary>
    /// Serializable class to hold the data for saving and loading Descant node connections
    /// </summary>
    [Serializable]
    public class DescantConnectionData
    {
        /// <summary>
        /// The name of the node the connection is coming from
        /// </summary>
        public string From;
        
        /// <summary>
        /// The ID of the node the connection is coming from
        /// </summary>
        public int FromID;
        
        /// <summary>
        /// The index of the port that this connection is coming from (base 1)
        /// </summary>
        public int FromIndex;
        
        /// <summary>
        /// The name of the node the connection is going to
        /// </summary>
        public string To;
        
        /// <summary>
        /// The ID of the node the connection is going to
        /// </summary>
        public int ToID;

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="from">The name of the node the connection is coming from</param>
        /// <param name="fromID">The ID of the node the connection is coming from</param>
        /// <param name="to">The name of the node the connection is going to</param>
        /// <param name="toID">The ID of the node the connection is going to</param>
        /// <param name="fromIndex">
        /// The index of the port that this connection is coming from (base 1 for ChoiceNodes and ResponseNodes)
        /// </param>
        public DescantConnectionData(string from, int fromID, string to, int toID, int fromIndex = 0)
        {
            From = from;
            FromID = fromID;
            FromIndex = fromIndex;
            To = to;
            ToID = toID;
        }
        
        /// <summary>
        /// Checks to make sure that the connection isn't an illegal one coming from a Choice or If node's input port
        /// </summary>
        public bool IllegalChoiceOrIf()
        {
            return (From.Equals("Choice") || From.Equals("If")) && FromIndex == 0;
        }

        /// <summary>
        /// Determines whether the connection points to itself
        /// </summary>
        public bool ToItself()
        {
            return From == To && FromID == ToID;
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
                _ = (DescantConnectionData) other;
            }
            catch
            {
                return false;
            }
            
            return Equals((DescantConnectionData)other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(From, FromID, FromIndex, To, ToID);
        }

        #if UNITY_EDITOR
        /// <summary>
        /// Custom Equals method
        /// </summary>
        /// <param name="other">The data object being compared against</param>
        /// <returns>Whether the other DescantConnectionData has the same data as this one</returns>
        public bool Equals(DescantConnectionData other)
        {
            return
                From == other.From && FromID == other.FromID &&
                FromIndex == other.FromIndex &&
                To == other.To && ToID == other.ToID;
        }
        #endif

        /// <summary>
        /// Overridden ToString method
        /// </summary>
        public override string ToString()
        {
            return GetType() + " (" + FromID + From + " " + FromIndex + " " + ToID + To + ")";
        }
    }
    
    #endregion
}