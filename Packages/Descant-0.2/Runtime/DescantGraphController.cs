using System;
using System.Collections.Generic;
using Editor;
using Editor.Data;
using UnityEditor;
using UnityEngine;

namespace Runtime
{
    public class RuntimeNode
    {
        public DescantNodeData Data;

        public RuntimeNode Previous;
        public List<RuntimeNode> Next;

        public RuntimeNode(DescantNodeData data)
        {
            Data = data;
            Next = new List<RuntimeNode>();
        }
    }
    
    public class DescantGraphController : MonoBehaviour
    {
        [SerializeField] DefaultAsset descantGraph;

        List<RuntimeNode> nodes = new List<RuntimeNode>();
        public RuntimeNode Current;
        
        void Awake()
        {
            GenerateRuntimeNodes();
        }

        void GenerateRuntimeNodes()
        {
            DescantGraphData data = DescantGraphData.Load(
                DescantUtilities.GetFullPathFromInstanceID(
                    descantGraph.GetInstanceID()));
            
            AddToRuntimeNodes(data.ChoiceNodes);
            AddToRuntimeNodes(data.ResponseNodes);
            Current = AddToRuntimeNodes(new List<DescantStartNodeData>() { data.StartNode });
            AddToRuntimeNodes(data.EndNodes);
            
            foreach (var i in data.Connections)
            {
                RuntimeNode a = FindRuntimeNode(i.From, i.FromID);
                RuntimeNode b = FindRuntimeNode(i.To, i.ToID);
                
                a.Next.Add(b);
                b.Previous = a;
            }
        }

        RuntimeNode AddToRuntimeNodes<T>(List<T> lst) where T : DescantNodeData
        {
            RuntimeNode temp = null;
            
            foreach (var i in lst)
            {
                temp = new RuntimeNode(i);
                nodes.Add(temp);
            }

            return temp;
        }

        RuntimeNode FindRuntimeNode(string type, int id)
        {
            foreach (var i in nodes)
                if (i.Data.Type == type && i.Data.ID == id)
                    return i;

            return null;
        }

        public List<string> Next(int choiceIndex = 0)
        {
            Current = Current.Next[choiceIndex];
            
            List<string> temp = new List<string>();

            switch (Current.Data.Type)
            {
                case "Choice":
                    foreach (var i in ((DescantChoiceNodeData)Current.Data).Choices)
                        temp.Add(i);
                    break;
                
                case "Response":
                    temp.Add(((DescantResponseNodeData)Current.Data).Response);
                    break;
                
                case "End": return null;
            }
            
            return temp.Count == 0 ? null : temp;
        }
    }
}