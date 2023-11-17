using System.Collections.Generic;
using DescantComponents;
using DescantEditor;
using UnityEditor;
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

        public RuntimeNode(DescantNodeData data)
        {
            Data = data;
            Next = new List<RuntimeNode>();
        }
    }
    
    public class DescantDialogueController : MonoBehaviour
    {
        /// <summary>
        /// The current runtime node being accessed
        /// </summary>
        [HideInInspector] public RuntimeNode Current;
        [HideInInspector] public string CurrentType;
        
        List<RuntimeNode> nodes = new List<RuntimeNode>();
        List<DescantActor> actors = new List<DescantActor>();

        /// <summary>
        /// Initializes the conversation controller
        /// </summary>
        /// <param name="g">The JSON graph to be loaded</param>
        public void Initialize(TextAsset g, TextAsset[] a)
        {
            #if UNITY_EDITOR
            AssetDatabase.Refresh();
            #endif
            
            GenerateRuntimeNodes(g);

            actors = new List<DescantActor>();
            
            foreach (var i in a)
                actors.Add(DescantEditorUtilities.LoadActorFromString(i.text));
            
            Debug.Log(actors[0]);
        }

        void FixedUpdate()
        {
            foreach (var i in nodes)
                foreach (var j in i.Data.NodeComponents)
                    j.FixedUpdate();
        }

        /// <summary>
        /// Generates all the runtime nodes that will be necessary to display the data when in-game
        /// </summary>
        /// <param name="descantGraph"></param>
        void GenerateRuntimeNodes(TextAsset descantGraph)
        {
            nodes = new List<RuntimeNode>();
            
            // Creating the graph data object first
            DescantGraphData data = DescantGraphData.LoadGraphFromString(descantGraph.text);
            
            // Then synthesizing all the runtime nodes from its node list
            AddToRuntimeNodes(new List<DescantStartNodeData>() { data.StartNode });
            AddToRuntimeNodes(data.ChoiceNodes);
            AddToRuntimeNodes(data.ResponseNodes);
            AddToRuntimeNodes(data.EndNodes);

            Current = nodes[0];

            // Finally, checking its connections to know how to connect the runtime nodes up
            foreach (var i in data.Connections)
            {
                RuntimeNode a = FindRuntimeNode(i.From, i.FromID);
                RuntimeNode b = FindRuntimeNode(i.To, i.ToID);
                
                a.Next.Add(b);
            }
        }

        /// <summary>
        /// Takes a list of DescantNode data objects, synthesizes them, and adds them to the list of runtime nodes
        /// </summary>
        /// <param name="lst">The data to be synthesized</param>
        /// <returns>The last runtime node in the list</returns>
        void AddToRuntimeNodes<T>(List<T> lst) where T : DescantNodeData
        {
            foreach (var i in lst)
                nodes.Add(new RuntimeNode(i));
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
        /// 
        /// (only length 1 if the current node is a ChoiceNode)
        /// </returns>
        public DescantNodeInvokeResult Next(int choiceIndex = 0)
        {
            if (Current.Data.Type.Equals("Start"))
                InvokeComponents();

            Current = Current.Next[choiceIndex];
            CurrentType = Current.Data.Type;

            DescantNodeInvokeResult currentResult = new DescantNodeInvokeResult(
                new List<KeyValuePair<int, string>>(),
                actors
            );

            switch (CurrentType)
            {
                case "End":
                    InvokeComponents();
                    return null; // Stopping if there are no more nodes

                case "Choice":
                    List<string> choices = ((DescantChoiceNodeData) Current.Data).Choices;
                    for (int i = 0; i < choices.Count; i++)
                        currentResult.Choices.Add(new KeyValuePair<int, string>(i, choices[i]));
                    break;
                
                case "Response":
                    currentResult.Choices.Add(
                        new KeyValuePair<int, string>(0, ((DescantResponseNodeData)Current.Data).Response));
                    break;
            }
            
            currentResult = InvokeComponents(currentResult);
            actors = currentResult.Actors;

            foreach (var i in actors)
                DescantEditorUtilities.SaveActor(false, i);
            
            return currentResult.Choices.Count == 0 ? null : currentResult; // Stopping if there are no choices
        }

        DescantNodeInvokeResult InvokeComponents(DescantNodeInvokeResult currentResult = null)
        {
            foreach (var i in Current.Data.NodeComponents)
                currentResult = i.Invoke(currentResult);

            return currentResult;
        }
    }
}