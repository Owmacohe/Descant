using System;
using System.Collections.Generic;
using UnityEngine;

namespace Editor.Data
{
    [Serializable]
    public class NodeData
    {
        public string Name;
        public int ID;
        public Vector2 Position;

        public NodeData(string name, int id, Vector2 position)
        {
            Name = name;
            ID = id;
            Position = position;
        }
    }

    [Serializable]
    public class ChoiceNodeData : NodeData
    {
        public List<string> Choices = new List<string>();

        public ChoiceNodeData(string name, int id, Vector2 position, List<string> choices) : base(name, id, position)
        {
            Choices = choices;
        }
    }
    
    [Serializable]
    public class ResponseNodeData : NodeData
    {
        public string Response;
        
        public ResponseNodeData(string name, int id, Vector2 position, string response) : base(name, id, position)
        {
            Response = response;
        }
    }

    [Serializable]
    public class StartNodeData : NodeData
    {
        public StartNodeData(string name, Vector2 position) : base(name, 0, position) { }
    }
    
    [Serializable]
    public class EndNodeData : NodeData
    {
        public EndNodeData(string name, int id, Vector2 position) : base(name, id, position) { }
    }

    [Serializable]
    public class GroupData
    {
        public string Name;
        public int ID;
        public Vector2 Position;
        public List<string> Nodes;
        public List<int> NodeIDs;

        public GroupData(string name, int id, Vector2 position, List<string> nodes, List<int> nodeIDs)
        {
            Name = name;
            ID = id;
            Position = position;
            Nodes = nodes;
            NodeIDs = nodeIDs;
        }
    }
}