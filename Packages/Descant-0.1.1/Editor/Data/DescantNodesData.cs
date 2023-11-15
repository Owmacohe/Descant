using System;
using System.Collections.Generic;
using DescantComponents;
using UnityEngine;

namespace DescantEditor
{
    /// <summary>
    /// Parent class to hold the data for saving and loading Descant nodes
    /// </summary>
    public abstract class DescantNodeData
    {
        public string Name;
        public string Type;
        public int ID;
        public Vector2 Position;
        
        [SerializeReference] public List<DescantNodeComponent> NodeComponents;

        protected DescantNodeData(
            string name,
            string type,
            int id,
            Vector2 position,
            List<DescantNodeComponent> nodeComponents)
        {
            Name = name;
            Type = type;
            ID = id;
            Position = position;
            NodeComponents = nodeComponents;
        }

        public override bool Equals(object other)
        {
            return Equals((DescantNodeData)other);
        }

#if UNITY_EDITOR
        protected bool Equals(DescantNodeData other)
        {
            return
                Name == other.Name &&
                ID == other.ID &&
                Position == other.Position &&
                DescantEditorUtilities.AreListsEqual(NodeComponents, other.NodeComponents);
        }
#endif

        public override string ToString()
        {
            return GetType() + " (" + ID + Name + " " + Position + ")";
        }
    }

    /// <summary>
    /// Serializable class to hold the data for saving and loading Descant choice nodes
    /// </summary>
    [Serializable]
    public class DescantChoiceNodeData : DescantNodeData
    {
        public List<string> Choices = new List<string>();

        public DescantChoiceNodeData(
            string name,
            string type,
            int id,
            Vector2 position,
            List<string> choices,
            List<DescantNodeComponent> nodeComponents)
            : base(name, type, id, position, nodeComponents)
        {
            Choices = choices;
        }

        public override bool Equals(object other)
        {
            return Equals((DescantChoiceNodeData)other);
        }
        
#if UNITY_EDITOR
        public bool Equals(DescantChoiceNodeData other)
        {
            return
                base.Equals(other) &&
                DescantEditorUtilities.AreListsEqual(Choices, other.Choices);
        }
#endif
        
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
        public string Response;
        
        public DescantResponseNodeData(
            string name,
            string type,
            int id,
            Vector2 position,
            string response,
            List<DescantNodeComponent> nodeComponents)
            : base(name, type, id, position, nodeComponents)
        {
            Response = response;
        }

        public override bool Equals(object other)
        {
            return Equals((DescantResponseNodeData)other);
        }
        
        public bool Equals(DescantResponseNodeData other)
        {
            return
                base.Equals(other) &&
                Response == other.Response;
        }
        
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
        public DescantStartNodeData(
            string name,
            string type,
            Vector2 position,
            List<DescantNodeComponent> nodeComponents)
            : base(name, type, 0, position, nodeComponents) { }
    }
    
    /// <summary>
    /// Serializable class to hold the data for saving and loading Descant end nodes
    /// </summary>
    [Serializable]
    public class DescantEndNodeData : DescantNodeData
    {
        public DescantEndNodeData(
            string name,
            string type,
            int id,
            Vector2 position,
            List<DescantNodeComponent> nodeComponents)
            : base(name, type, id, position, nodeComponents) { }
    }
}