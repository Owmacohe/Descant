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
    
    public class DescantDialogueController : MonoBehaviour // TODO: create log file
    {
        [HideInInspector] public RuntimeNode Current; // The current runtime node being accessed
        [HideInInspector] public string CurrentType; // The string NodeType type of the runtime node being accessed
        [HideInInspector] public List<RuntimeNode> Nodes = new List<RuntimeNode>(); // The list of RuntimeNodes derived from the data
        [HideInInspector] public List<DescantActor> Actors = new List<DescantActor>(); // The list of DescantActors derived from the data

        DescantActor player; // The player DescantActor for this dialogue
        DescantActor NPC; // The NPC DescantActor for this dialogue

        [HideInInspector] public bool HasEnded; // Whether the current dialogue has finished
        [HideInInspector] public bool Typewriter; // Whether the current dialogue is using a typewriter
        [HideInInspector] public float TypewriterSpeed; // The speed of the dialogue's typewriter (if it's using one)

        /// <summary>
        /// Initializes the conversation controller
        /// </summary>
        /// <param name="g">The JSON graph to be loaded</param>
        /// <param name="a">The dialogue's extra actors to be loaded</param>
        /// <param name="p">The dialogue's player to be loaded</param>
        /// <param name="npc">The dialogue's NPC to be loaded</param>
        public void Initialize(TextAsset g, TextAsset[] a, TextAsset p, TextAsset npc)
        {
            #if UNITY_EDITOR
            AssetDatabase.Refresh();
            #endif
            
            GenerateRuntimeNodes(g); // Initializing the RuntimeNodes

            Actors = new List<DescantActor>();
            
            // Adding in all the extra actors first
            foreach (var i in a)
                Actors.Add(DescantEditorUtilities.LoadActorFromString(i.text));

            // Initializing the player (if one has been set)
            if (p != null)
            {
                player = DescantEditorUtilities.LoadActorFromString(p.text);
                
                if (!Actors.Contains(player)) Actors.Add(player);
            }
            
            // Initializing the NPC (if one has been set)
            if (npc != null)
            {
                NPC = DescantEditorUtilities.LoadActorFromString(npc.text);
                
                if (!Actors.Contains(NPC)) Actors.Add(NPC);
            }
        }

        /// <summary>
        /// Quick method to begin the dialogue
        /// (to be called from a DescantDialogueUI, and only after this has been Initialized)
        /// </summary>
        public void BeginDialogue()
        {
            Current = Nodes[0];
            CurrentType = "Start";

            // Upping the NPC's dialogue attempts
            if (NPC != null)
            {
                NPC.DialogueAttempts++;
                DescantEditorUtilities.SaveActor(false, NPC);   
            }
        }
        
        void FixedUpdate()
        {
            foreach (var j in Current.Data.NodeComponents)
                if (!j.FixedUpdate()) HasEnded = true;
        }

        void Update()
        {
            foreach (var j in Current.Data.NodeComponents)
                if (!j.Update()) HasEnded = true;
        }

        /// <summary>
        /// Generates all the runtime nodes that will be necessary to display the data when in-game
        /// </summary>
        /// <param name="descantGraph"></param>
        void GenerateRuntimeNodes(TextAsset descantGraph)
        {
            Nodes = new List<RuntimeNode>();
            
            // Creating the graph data object first
            DescantGraphData data = DescantGraphData.LoadGraphFromString(descantGraph.text);
            
            Typewriter = data.Typewriter;
            TypewriterSpeed = data.TypewriterSpeed;
            
            // Then synthesizing all the runtime nodes from its node list
            AddToRuntimeNodes(new List<DescantStartNodeData>() { data.StartNode });
            AddToRuntimeNodes(data.ChoiceNodes);
            AddToRuntimeNodes(data.ResponseNodes);
            AddToRuntimeNodes(data.EndNodes);

            Current = Nodes[0];

            // Finally, checking its connections to know how to connect the runtime nodes up
            foreach (var i in data.Connections)
                FindRuntimeNode(i.From, i.FromID).Next
                    .Add(i.To == "null"
                        ? null
                        : FindRuntimeNode(i.To, i.ToID));
        }

        /// <summary>
        /// Takes a list of DescantNode data objects, synthesizes them, and adds them to the list of runtime nodes
        /// </summary>
        /// <param name="lst">The data to be synthesized</param>
        /// <returns>The last runtime node in the list</returns>
        void AddToRuntimeNodes<T>(List<T> lst) where T : DescantNodeData
        {
            foreach (var i in lst)
                Nodes.Add(new RuntimeNode(i));
        }

        /// <summary>
        /// Finds a runtime node from the list
        /// </summary>
        /// <param name="type">The node's DescantNodeType</param>
        /// <param name="id">The node's ID</param>
        /// <returns></returns>
        RuntimeNode FindRuntimeNode(string type, int id)
        {
            foreach (var i in Nodes)
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
            if (HasEnded) return null;
            
            DescantNodeInvokeResult currentResult = new DescantNodeInvokeResult(
                new List<KeyValuePair<int, string>>(),
                Actors
            );

            if (Current.Data.Type.Equals("Start"))
                InvokeComponents(currentResult);

            if (Current.Next == null ||
                Current.Next.Count == 0 ||
                Current.Next[choiceIndex] == null)
            {
                DescantUtilities.ErrorMessage(
                    GetType(),
                    "Dialogue path contains no end node!"
                );
                
                return null;
            }

            Current = Current.Next[choiceIndex];
            CurrentType = Current.Data.Type;

            switch (CurrentType)
            {
                case "End":
                    InvokeComponents(currentResult);
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
            Actors = currentResult.Actors;

            foreach (var i in Actors)
                DescantEditorUtilities.SaveActor(false, i);
            
            return currentResult.Choices.Count == 0 ? null : currentResult; // Stopping if there are no choices
        }

        DescantNodeInvokeResult InvokeComponents(DescantNodeInvokeResult currentResult)
        {
            foreach (var i in Current.Data.NodeComponents)
                currentResult = i.Invoke(currentResult);

            return currentResult;
        }
    }
}