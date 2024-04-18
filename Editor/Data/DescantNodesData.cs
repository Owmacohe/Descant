using System;
using System.Collections.Generic;
using Descant.Components;
using UnityEngine;

namespace Descant.Editor
{
    /// <summary>
    /// Parent class to hold the data for saving and loading Descant nodes
    /// </summary>
    public abstract class DescantNodeData
    {
        /// <summary>
        /// The custom name of the node
        /// </summary>
        public string Name;
        
        /// <summary>
        /// The unique identifier ID for the node
        /// </summary>
        [HideInInspector] public string Type;
        
        /// <summary>
        /// The type of this node
        /// </summary>
        [HideInInspector] public int ID;
        
        /// <summary>
        /// The node's current position
        /// </summary>
        [HideInInspector] public Vector2 Position;
        
        /// <summary>
        /// The list of Components attached to the node
        /// ([SerializeReference] is necessary to ensure that the Serialized
        /// DescantNodeComponents are saved to the file as a list)
        /// </summary>
        [SerializeReference] public List<DescantComponent> NodeComponents;

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="name">The custom name of the node</param>
        /// <param name="type">The unique identifier ID for the node</param>
        /// <param name="id">The type of this node</param>
        /// <param name="position">The node's current position</param>
        /// <param name="nodeComponents">The list of Components attached to the node</param>
        protected DescantNodeData(
            string name,
            string type,
            int id,
            Vector2 position,
            List<DescantComponent> nodeComponents)
        {
            Name = name;
            Type = type;
            ID = id;
            Position = position;
            NodeComponents = nodeComponents;
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
                _ = (DescantNodeData) other;
            }
            catch
            {
                return false;
            }
            
            return Equals((DescantNodeData)other);
        }

        #if UNITY_EDITOR
        /// <summary>
        /// Custom Equals method
        /// </summary>
        /// <param name="other">The data object being compared against</param>
        /// <returns>Whether the other DescantNodeData has the same data as this one</returns>
        protected bool Equals(DescantNodeData other)
        {
            return
                Name == other.Name &&
                ID == other.ID &&
                Position == other.Position &&
                DescantEditorUtilities.AreListsEqual(NodeComponents, other.NodeComponents);
        }
        #endif

        /// <summary>
        /// Overridden ToString method
        /// </summary>
        public override string ToString()
        {
            string temp = "";

            foreach (var i in NodeComponents)
                temp += " " + i;
            
            return GetType() + " (" + ID + Name + " " + Type + " " + Position + ") (" + (temp.Length > 1 ? temp.Substring(1) : "") + ")";
        }
    }

    /// <summary>
    /// Serializable class to hold the data for saving and loading Descant choice nodes
    /// </summary>
    [Serializable]
    public class DescantChoiceNodeData : DescantNodeData
    {
        /// <summary>
        /// The list of possible choices that the player can make at the ChoiceNode
        /// </summary>
        public List<string> Choices = new List<string>();

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="name">The custom name of the node</param>
        /// <param name="type">The unique identifier ID for the node</param>
        /// <param name="id">The type of this node</param>
        /// <param name="position">The node's current position</param>
        /// <param name="nodeComponents">The list of Components attached to the node</param>
        /// <param name="choices">The list of possible choices that the player can make at the ChoiceNode</param>
        public DescantChoiceNodeData(
            string name,
            string type,
            int id,
            Vector2 position,
            List<string> choices,
            List<DescantComponent> nodeComponents)
            : base(name, type, id, position, nodeComponents)
        {
            Choices = choices;
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
                _ = (DescantChoiceNodeData) other;
            }
            catch
            {
                return false;
            }
            
            return Equals((DescantChoiceNodeData)other);
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Custom Equals method
        /// </summary>
        /// <param name="other">The data object being compared against</param>
        /// <returns>Whether the other DescantChoiceNodeData has the same data as this one</returns>
        public bool Equals(DescantChoiceNodeData other)
        {
            return
                base.Equals(other) &&
                DescantEditorUtilities.AreListsEqual(Choices, other.Choices);
        }
        #endif
        
        /// <summary>
        /// Overridden ToString method
        /// </summary>
        public override string ToString()
        {
            string temp = "";

            foreach (var i in Choices)
                temp += " " + i;
            
            return base.ToString() + " (" + (temp.Length > 1 ? temp.Substring(1) : "") + ")";
        }
    }
    
    /// <summary>
    /// Serializable class to hold the data for saving and loading Descant response nodes
    /// </summary>
    [Serializable]
    public class DescantResponseNodeData : DescantNodeData
    {
        /// <summary>
        /// The response text at the ResponseNode
        /// </summary>
        public string Response;
        
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="name">The custom name of the node</param>
        /// <param name="type">The unique identifier ID for the node</param>
        /// <param name="id">The type of this node</param>
        /// <param name="position">The node's current position</param>
        /// <param name="nodeComponents">The list of Components attached to the node</param>
        /// <param name="response">The response text at the ResponseNode</param>
        public DescantResponseNodeData(
            string name,
            string type,
            int id,
            Vector2 position,
            string response,
            List<DescantComponent> nodeComponents)
            : base(name, type, id, position, nodeComponents)
        {
            Response = response;
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
                _ = (DescantResponseNodeData) other;
            }
            catch
            {
                return false;
            }
            
            return Equals((DescantResponseNodeData)other);
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Custom Equals method
        /// </summary>
        /// <param name="other">The data object being compared against</param>
        /// <returns>Whether the other DescantResponseNodeData has the same data as this one</returns>
        public bool Equals(DescantResponseNodeData other)
        {
            return
                base.Equals(other) &&
                Response == other.Response;
        }
        #endif
        
        /// <summary>
        /// Overridden ToString method
        /// </summary>
        public override string ToString()
        {
            return base.ToString() + " (" + Response + ")";
        }
    }

    /// <summary>
    /// Serializable class to hold the data for saving and loading Descant start nodes
    /// </summary>
    [Serializable]
    public class DescantStartNodeData : DescantNodeData
    {
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="name">The custom name of the node</param>
        /// <param name="type">The unique identifier ID for the node</param>
        /// <param name="position">The node's current position</param>
        /// <param name="nodeComponents">The list of Components attached to the node</param>
        public DescantStartNodeData(
            string name,
            string type,
            Vector2 position,
            List<DescantComponent> nodeComponents)
            : base(name, type, 0, position, nodeComponents) { }
    }
    
    /// <summary>
    /// Serializable class to hold the data for saving and loading Descant end nodes
    /// </summary>
    [Serializable]
    public class DescantEndNodeData : DescantNodeData
    {
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="name">The custom name of the node</param>
        /// <param name="type">The unique identifier ID for the node</param>
        /// <param name="id">The type of this node</param>
        /// <param name="position">The node's current position</param>
        /// <param name="nodeComponents">The list of Components attached to the node</param>
        public DescantEndNodeData(
            string name,
            string type,
            int id,
            Vector2 position,
            List<DescantComponent> nodeComponents)
            : base(name, type, id, position, nodeComponents) { }
    }
}