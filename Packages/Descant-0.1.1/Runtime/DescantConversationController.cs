using System.Collections.Generic;
using DescantComponents;
using DescantEditor;
using UnityEngine;

namespace DescantRuntime
{
    /// <summary>
    /// A simple class used to house a DescantNode's data at runtime,
    /// with easily-accessible references to past and future such runtime nodes
    /// </summary>
    public class RuntimeNode
    {
        /// <summary>
        /// The data of the node, which will be polled at runtime
        /// </summary>
        public DescantNodeData Data;

        /// <summary>
        /// The list of next possible nodes that can be switched to
        /// (if Next.Count==1, the current node is a ResponseNode)
        /// (if Next.Count>1, the current node is a ChoiceNode)
        /// </summary>
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
        /// <summary>
        /// The current runtime node being accessed
        /// </summary>
        [HideInInspector] public RuntimeNode Current;
        
        List<RuntimeNode> nodes = new List<RuntimeNode>();
        
        /// <summary>
        /// Initializes the conversation controller
        /// </summary>
        /// <param name="graph">The JSON graph to be loaded</param>
        public void Initialize(TextAsset graph)
        {
            GenerateActors();
            GenerateRuntimeNodes(graph);
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
            // TODO: subscribe to Actor change methods
        }

        /// <summary>
        /// Generates all the runtime nodes that will be necessary to display the data when in-game
        /// </summary>
        /// <param name="descantGraph"></param>
        void GenerateRuntimeNodes(TextAsset descantGraph)
        {
            // Creating the graph data object first
            DescantGraphData data = DescantGraphData.LoadFromString(descantGraph.text);
            
            // Then synthesizing all the runtime nodes from its node list
            AddToRuntimeNodes(data.ChoiceNodes);
            AddToRuntimeNodes(data.ResponseNodes);
            Current = AddToRuntimeNodes(new List<DescantStartNodeData>() { data.StartNode });
            AddToRuntimeNodes(data.EndNodes);

            // Finally, checking its connections to know how to connect the runtime nodes up
            foreach (var i in data.Connections)
            {
                RuntimeNode a = FindRuntimeNode(i.From, i.FromID);
                RuntimeNode b = FindRuntimeNode(i.To, i.ToID);
                
                a.Next.Add(b);
            }
            
            // TODO: generate components
        }

        /// <summary>
        /// Takes a list of DescantNode data objects, synthesizes them, and adds them to the list of runtime nodes
        /// </summary>
        /// <param name="lst">The data to be synthesized</param>
        /// <returns>The last runtime node in the list</returns>
        RuntimeNode AddToRuntimeNodes<T>(List<T> lst) where T : DescantNodeData
        {
            foreach (var i in lst)
                nodes.Add(new RuntimeNode(i));

            return nodes[^1];
        }

        /// <summary>
        /// Finds a runtime node from the list
        /// </summary>
        /// <param name="type">The node's DescantNodeType</param>
        /// <param name="id">The node's ID</param>
        /// <returns></returns>
        RuntimeNode FindRuntimeNode(string type, int id)
        {
            foreach (var i in nodes)
                if (i.Data.Type == type && i.Data.ID == id)
                    return i;

            return null;
        }

        /// <summary>
        /// Sets the current runtime node to one of the next ones
        /// </summary>
        /// <param name="choiceIndex">
        /// The index of the choice being made (base 0)
        /// (default 0 if the current node is a ResponseNode)
        /// </param>
        /// <returns>
        /// A list of all the possible choices at the next node
        /// (only length 1 if the current node is a ChoiceNode)
        /// </returns>
        public List<string> Next(int choiceIndex = 0)
        {
            if (Current.Next == null || Current.Next.Count == 0) return null; // Stopping if there are no more nodes
            
            Current = Current.Next[choiceIndex];

            List<string> currentText = new List<string>();
            
            switch (Current.Data.Type)
            {
                case "Choice":
                    foreach (var i in ((DescantChoiceNodeData)Current.Data).Choices)
                        currentText.Add(i);
                    break;
                
                case "Response":
                    currentText.Add(((DescantResponseNodeData)Current.Data).Response);
                    break;
                
                case "End": return null;
            }
            
            foreach (var i in nodes)
                foreach (var j in i.Components)
                    if (j is IInvokedDescantComponent component)
                        component.Invoke();

            return currentText.Count == 0 ? null : currentText; // Stopping if there are no choices
        }
    }
}