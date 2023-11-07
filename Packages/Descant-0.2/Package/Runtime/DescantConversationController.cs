using System;
using System.Collections.Generic;
using Descant.Package.Components;
using Descant.Package.Editor;
using Descant.Package.Editor.Data;
using UnityEditor;
using UnityEngine;

namespace Descant.Package.Runtime
{
    public class RuntimeNode
    {
        public DescantNodeData Data;

        public RuntimeNode Previous;
        public List<RuntimeNode> Next;

        public List<DescantNodeComponent> Components;

        public RuntimeNode(DescantNodeData data)
        {
            Data = data;
            Next = new List<RuntimeNode>();
            Components = new List<DescantNodeComponent>();
        }
    }
    
    public class DescantConversationController : MonoBehaviour
    {
        [HideInInspector] public RuntimeNode Current;
        [HideInInspector] public List<string> CurrentText;
        [HideInInspector] public float CurrentStartTime;
        
        [HideInInspector] public DescantActor Player;
        [HideInInspector] public DescantActor NPC;

        [HideInInspector] public string Ending;
        
        DefaultAsset descantGraph;
        List<RuntimeNode> nodes = new List<RuntimeNode>();
        
        public void Initialize(DefaultAsset graph)
        {
            descantGraph = graph;
            
            GenerateActors();
            GenerateRuntimeNodes();
        }

        void FixedUpdate()
        {
            foreach (var i in nodes)
                foreach (var j in i.Components)
                    if (j is IUpdatedDescantComponent component)
                        component.FixedUpdate();
        }

        void GenerateActors()
        {
            // TODO: from file

            Player = new DescantActor("Player", DescantActorType.Player);
            NPC = new DescantActor("NPC", DescantActorType.NPC);
            
            // TODO: subscribe to Actor change methods
        }

        public DescantActor GetActor(string actorName)
        {
            return null; // TODO
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
            
            // TODO: generate components
            
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
            CurrentText = new List<string>();
            CurrentStartTime = Time.time;

            switch (Current.Data.Type)
            {
                case "Choice":
                    foreach (var i in ((DescantChoiceNodeData)Current.Data).Choices)
                        CurrentText.Add(i);
                    break;
                
                case "Response":
                    CurrentText.Add(((DescantResponseNodeData)Current.Data).Response);
                    break;
                
                case "End": return null;
            }
            
            foreach (var i in nodes)
                foreach (var j in i.Components)
                    if (j is IInvokedDescantComponent component)
                        component.Invoke();

            return CurrentText.Count == 0 ? null : CurrentText;
        }
    }
}