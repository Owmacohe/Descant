using System;
using System.Collections.Generic;
using UnityEngine;

namespace Editor.Data
{
    [Serializable]
    public class DescantNodeData
    {
        public string Name;
        public int ID;
        public Vector2 Position;

        public DescantNodeData(string name, int id, Vector2 position)
        {
            Name = name;
            ID = id;
            Position = position;
        }

        public override bool Equals(object other)
        {
            return Equals((DescantNodeData)other);
        }

        public bool Equals(DescantNodeData other)
        {
            return
                Name == other.Name &&
                ID == other.ID &&
                Position == other.Position;
        }

        public override string ToString()
        {
            return GetType() + " (" + ID + Name + " " + Position + ")";
        }
    }

    [Serializable]
    public class DescantChoiceNodeData : DescantNodeData
    {
        public List<string> Choices = new List<string>();

        public DescantChoiceNodeData(string name, int id, Vector2 position, List<string> choices) : base(name, id, position)
        {
            Choices = choices;
        }

        public override bool Equals(object other)
        {
            return Equals((DescantChoiceNodeData)other);
        }
        
        public bool Equals(DescantChoiceNodeData other)
        {
            return
                base.Equals(other) &&
                DescantUtilities.AreListsEqual(Choices, other.Choices);
        }
        
        public override string ToString()
        {
            string temp = "";

            foreach (var i in Choices)
                temp += " " + i;
            
            return base.ToString() + " (" + (temp.Length > 1 ? temp.Substring(1) : "") + ")";
        }
    }
    
    [Serializable]
    public class DescantResponseNodeData : DescantNodeData
    {
        public string Response;
        
        public DescantResponseNodeData(string name, int id, Vector2 position, string response) : base(name, id, position)
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

    [Serializable]
    public class DescantStartNodeData : DescantNodeData
    {
        public DescantStartNodeData(string name, Vector2 position) : base(name, 0, position) { }
    }
    
    [Serializable]
    public class DescantEndNodeData : DescantNodeData
    {
        public DescantEndNodeData(string name, int id, Vector2 position) : base(name, id, position) { }
    }

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

        public bool DirectionsEqual(DescantConnectionData other)
        {
            return
                From == other.From && FromID == other.FromID &&
                To == other.To && ToID == other.ToID;
        }

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