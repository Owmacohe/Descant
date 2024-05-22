using System;
using System.Collections.Generic;
using Descant.Components;
using UnityEngine;

namespace Descant.Editor
{
    #region DescantNodeData
    
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
        /// The comments associated with this node
        /// </summary>
        public string Comments;

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="name">The custom name of the node</param>
        /// <param name="type">The unique identifier ID for the node</param>
        /// <param name="id">The type of this node</param>
        /// <param name="position">The node's current position</param>
        /// <param name="nodeComponents">The list of Components attached to the node</param>
        /// <param name="comments">The comments associated with this node</param>
        protected DescantNodeData(
            string name,
            string type,
            int id,
            Vector2 position,
            List<DescantComponent> nodeComponents,
            string comments)
        {
            Name = name;
            Type = type;
            ID = id;
            Position = position;
            NodeComponents = nodeComponents;
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
                _ = (DescantNodeData) other;
            }
            catch
            {
                return false;
            }
            
            return Equals((DescantNodeData)other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Type, ID, Position, NodeComponents, Comments);
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
                DescantEditorUtilities.AreListsEqual(NodeComponents, other.NodeComponents) &&
                Comments == other.Comments;
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
            
            return GetType() + " (" + ID + Name + " " + Type + " " + Position + ") (" +
                   (temp.Length > 1 ? temp.Substring(1) : "") + ") (" +
                   Comments + ")";
        }
    }
    
    #endregion
    
    #region DescantChoiceNodeData

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
        /// <param name="choices">The list of possible choices that the player can make at the ChoiceNode</param>
        /// <param name="nodeComponents">The list of Components attached to the node</param>
        /// <param name="comments">The comments associated with this node</param>
        public DescantChoiceNodeData(
            string name,
            string type,
            int id,
            Vector2 position,
            List<string> choices,
            List<DescantComponent> nodeComponents,
            string comments)
            : base(name, type, id, position, nodeComponents, comments)
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
        
        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Choices);
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
    
    #endregion
    
    #region DescantResponseNodeData
    
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
        /// <param name="response">The response text at the ResponseNode</param>
        /// <param name="nodeComponents">The list of Components attached to the node</param>
        /// <param name="comments">The comments associated with this node</param>
        public DescantResponseNodeData(
            string name,
            string type,
            int id,
            Vector2 position,
            string response,
            List<DescantComponent> nodeComponents,
            string comments)
            : base(name, type, id, position, nodeComponents, comments)
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
        
        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Response);
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
    
    #endregion
    
    #region DescantIfNodeData
    
    /// <summary>
    /// Serializable class to hold the data for saving and loading Descant if nodes
    /// </summary>
    [Serializable]
    public class DescantIfNodeData : DescantNodeData
    {
        /// <summary>
        /// The component used to determine which path the dialogue will take after the node
        /// </summary>
        [SerializeReference] public IfComponent IfComponent;

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="name">The custom name of the node</param>
        /// <param name="type">The unique identifier ID for the node</param>
        /// <param name="id">The type of this node</param>
        /// <param name="position">The node's current position</param>
        /// <param name="ifComponent">
        /// The component used to determine which path the dialogue will take after the node
        /// </param>
        /// <param name="nodeComponents">The list of Components attached to the node</param>
        /// <param name="comments">The comments associated with this node</param>
        public DescantIfNodeData(
            string name,
            string type,
            int id,
            Vector2 position,
            IfComponent ifComponent,
            List<DescantComponent> nodeComponents,
            string comments)
            : base(name, type, id, position, nodeComponents, comments)
        {
            IfComponent = ifComponent;
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
                _ = (DescantIfNodeData) other;
            }
            catch
            {
                return false;
            }
            
            return Equals((DescantIfNodeData)other);
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), IfComponent.GetHashCode());
        }

        #if UNITY_EDITOR
        /// <summary>
        /// Custom Equals method
        /// </summary>
        /// <param name="other">The data object being compared against</param>
        /// <returns>Whether the other DescantIfNodeData has the same data as this one</returns>
        public bool Equals(DescantIfNodeData other)
        {
            return
                base.Equals(other) &&
                IfComponent.Equals(other.IfComponent);
        }
        #endif
        
        /// <summary>
        /// Overridden ToString method
        /// </summary>
        public override string ToString()
        {
            return base.ToString() + " (" + IfComponent + ")";
        }
    }
    
    #endregion
    
    #region DescantStartNodeData and DescantEndNodeData

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
        /// <param name="comments">The comments associated with this node</param>
        public DescantStartNodeData(
            string name,
            string type,
            Vector2 position,
            List<DescantComponent> nodeComponents,
            string comments)
            : base(name, type, 0, position, nodeComponents, comments) { }
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
        /// <param name="comments">The comments associated with this node</param>
        public DescantEndNodeData(
            string name,
            string type,
            int id,
            Vector2 position,
            List<DescantComponent> nodeComponents,
            string comments)
            : base(name, type, id, position, nodeComponents, comments) { }
    }
    
    #endregion
}