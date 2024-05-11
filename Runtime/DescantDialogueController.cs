using System.Collections.Generic;
using Descant.Components;
using Descant.Editor;
using Descant.Utilities;
using UnityEditor;
using UnityEngine;

namespace Descant.Runtime
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

        /// <summary>
        /// RuntimeNode constructor
        /// </summary>
        /// <param name="data">The data for this runtime node that will be used later</param>
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

        RuntimeSerializedObjectSaver saver; // A runtime script used to save SerializedObjects

        DescantLogData log; // The log file where the Descant Graph log events are stored

        [HideInInspector] public bool HasEnded; // Whether the current dialogue has finished
        [HideInInspector] public bool Typewriter; // Whether the current dialogue is using a typewriter
        [HideInInspector] public float TypewriterSpeed; // The speed of the dialogue's typewriter (if it's using one)

        #region Initialization

        /// <summary>
        /// Initializes the conversation controller
        /// </summary>
        /// <param name="g">The graph to be loaded</param>
        /// <param name="p">The dialogue's player to be loaded</param>
        /// <param name="npc">The dialogue's NPC to be loaded</param>
        /// <param name="a">The dialogue's extra actors to be loaded</param>
        public void Initialize(DescantGraph g, DescantActor p, DescantActor npc, DescantActor[] a)
        {
            #if UNITY_EDITOR
            AssetDatabase.Refresh();
            #endif
            
            GenerateRuntimeNodes(g); // Initializing the RuntimeNodes

            Actors = new List<DescantActor>();
            
            // Adding in all the extra actors first
            foreach (var i in a)
                Actors.Add(i);

            // Initializing the player (if one has been set)
            if (p != null)
            {
                player = p;

                // Setting the first portrait by default
                if (player.Portraits.Length > 0) player.Portrait = player.Portraits[0];
                
                if (!Actors.Contains(player)) Actors.Add(player);
            }
            
            // Initializing the NPC (if one has been set)
            if (npc != null)
            {
                NPC = npc;

                // Setting the first portrait by default
                if (NPC.Portraits.Length > 0) NPC.Portrait = NPC.Portraits[0];
                
                if (!Actors.Contains(NPC)) Actors.Add(NPC);
            }

            saver = GetComponent<RuntimeSerializedObjectSaver>();
            if (saver == null) saver = gameObject.AddComponent<RuntimeSerializedObjectSaver>();
            
            log = Resources.Load<DescantLogData>("Default Log (DO NOT DELETE)"); // Accessing the log file
        }

        /// <summary>
        /// Quick method to begin the dialogue
        /// (to be called from a DescantDialogueUI, and only after this has been Initialized)
        /// </summary>
        public void BeginDialogue()
        {
            Current = Nodes[0];
            CurrentType = "Start";

            HasEnded = false;

            // Upping the NPC's dialogue attempts
            if (NPC != null) NPC.DialogueAttempts++;
            
            // Clearing and adding a start event to the log
            log.Clear();
            log.Log(DescantLogData.LogEventType.Begin);
            saver.AddObjectToQueue(log);
        }

        /// <summary>
        /// Quick method to end the dialogue
        /// (to be called from a DescantDialogueUI)
        /// </summary>
        public void EndDialogue()
        {
            HasEnded = true;
            
            // Adding an end event to the log
            log.Log(DescantLogData.LogEventType.End);
            saver.AddObjectToQueue(log);
        }

        #endregion
        
        void FixedUpdate()
        {
            // Calling each of the current Components' FixedUpdates, then ending the dialogue if any return false
            if (Current != null)
                foreach (var j in Current.Data.NodeComponents)
                    if (!j.FixedUpdate()) HasEnded = true;
        }

        void Update()
        {
            // Calling each of the current Components' Updates, then ending the dialogue if any return false
            if (Current != null)
                foreach (var j in Current.Data.NodeComponents)
                    if (!j.Update()) HasEnded = true;
        }

        #region RuntimeNodes

        /// <summary>
        /// Generates all the runtime nodes that will be necessary to display the data when in-game
        /// </summary>
        /// <param name="descantGraph"></param>
        void GenerateRuntimeNodes(DescantGraph descantGraph)
        {
            Nodes = new List<RuntimeNode>();
            
            Typewriter = descantGraph.Typewriter;
            TypewriterSpeed = descantGraph.TypewriterSpeed;
            
            // Then synthesizing all the runtime nodes from its node list
            AddToRuntimeNodes(new List<DescantStartNodeData>() { descantGraph.StartNode });
            AddToRuntimeNodes(descantGraph.ChoiceNodes);
            AddToRuntimeNodes(descantGraph.ResponseNodes);
            AddToRuntimeNodes(descantGraph.EndNodes);

            Current = Nodes[0];

            // Finally, checking its connections to know how to connect the runtime nodes up
            foreach (var i in descantGraph.Connections)
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

        #endregion
        
        # region Node processing

        /// <summary>
        /// Sets the current runtime node to one of the next ones
        /// </summary>
        /// <param name="choiceIndex">
        /// The index of the choice being made (base 0)
        /// (default 0 if the current node is a ResponseNode)
        /// </param>
        /// <param name="verbose">Whether to display Debug.Log messages from Log Components</param>
        /// <returns>
        /// The results object containing relevant information from the graph after this node has been invoked
        /// </returns>
        public DescantNodeInvokeResult Next(int choiceIndex, bool verbose)
        {
            if (HasEnded) return null; // Not allowing the dialogue to proceed if it has ended
            
            // Creating a new results object to be passed in and out of the Components
            DescantNodeInvokeResult currentResult = new DescantNodeInvokeResult(
                new List<KeyValuePair<int, string>>(),
                player,
                NPC,
                Actors
            );

            // Invoking the StartNode's components if we're at the beginning of the dialogue
            if (Current.Data.Type.Equals("Start"))
                InvokeComponents(currentResult, verbose);

            // If there's no next node in the path, stop and throw an error
            if (Current.Next == null ||
                Current.Next.Count == 0 ||
                Current.Next[choiceIndex] == null)
            {
                DescantUtilities.ErrorMessage(
                    GetType(),
                    "No next node in path!"
                );
                
                return null;
            }

            // Getting the next node
            Current = Current.Next[choiceIndex];
            CurrentType = Current.Data.Type;
            
            // Logging the next node reached
            log.Log(DescantLogData.LogEventType.Node, CurrentType, Current.Data.Name);
            saver.AddObjectToQueue(log);

            switch (CurrentType)
            {
                // If we're at the end, we invoke the final Components and stop
                case "End":
                    InvokeComponents(currentResult, verbose);
                    return null;

                // Add the ChoiceNode's choices to the result object
                case "Choice":
                    List<string> choices = ((DescantChoiceNodeData) Current.Data).Choices;
                    
                    for (int i = 0; i < choices.Count; i++)
                        currentResult.Text.Add(new KeyValuePair<int, string>(i, choices[i]));
                    break;
                
                // Add the ResponseNode's response to the result object
                case "Response":
                    currentResult.Text.Add(
                        new KeyValuePair<int, string>(0, ((DescantResponseNodeData)Current.Data).Response));
                    break;
            }
            
            // Invoking the Components for the new node, and setting the actors and their portraits accordingly
            currentResult = InvokeComponents(currentResult, verbose);

            // Making sure to assign and save the SerializedObjects properly
            for (int i = 0; i < Actors.Count; i++)
            {
                DescantComponentUtilities.AssignActor(Actors[i], currentResult.Actors[i]);
                saver.AddObjectToQueue(Actors[i]);
            }

            // Checking through the text, replacing all in instances of statistic injections
            for (int j = 0; j < currentResult.Text.Count; j++)
            {
                var temp = currentResult.Text[j];
                currentResult.Text[j] = new KeyValuePair<int, string>(temp.Key, CheckForActorProperties(temp.Value));
            }
            
            return currentResult.Text.Count == 0 ? null : currentResult; // Stopping if there are no choices
        }

        /// <summary>
        /// Invokes all of the Components of the current node, passing a result object through them
        /// </summary>
        /// <param name="currentResult">The current text and actor result data</param>
        /// <param name="verbose">Whether to display Debug.Log messages from Log Components</param>
        DescantNodeInvokeResult InvokeComponents(DescantNodeInvokeResult currentResult, bool verbose)
        {
            foreach (var i in Current.Data.NodeComponents)
            {
                if (verbose || i.GetType() != typeof(Log))
                {
                    currentResult = i.Invoke(currentResult);
                    
                    // Logging the current node's invoked DescantComponents
                    log.Log(DescantLogData.LogEventType.Component, i.GetType().ToString());
                    saver.AddObjectToQueue(log);
                }
            }

            return currentResult;
        }

        /// <summary>
        /// Method to check through a string, replacing all instances of DescantActor property injections
        /// (statistics, topics, and relationships)
        /// </summary>
        /// <param name="text">A DescantChoiceNode's or DescantResponseNode's text to be checked through</param>
        /// <returns>The string, with the injection replaced with its corresponding property</returns>
        string CheckForActorProperties(string text)
        {
            string temp = "";
            
            for (int i = 0; i < text.Length; i++)
            {
                // If we've found the beginning of an injection...
                if (text[i] == '{' && text.Substring(i).Contains('}'))
                {
                    // Getting the injection text (minus the brackets),
                    // and splitting it into actor name and statistic name
                    string injection = text.Substring(i + 1, text.Substring(i).IndexOf('}') - 1);
                    var split = injection.Split(':');
                    
                    // Appending the statistic value
                    foreach (var j in Actors)
                        if (j.name == split[0])
                            temp += j.StatisticValues[j.StatisticKeys.IndexOf(split[1])];

                    i += injection.Length + 1; // Skipping ahead to avoid adding any of the injection text
                }
                // Otherwise, we just spit out the next character and continue on
                else
                {
                    temp += text[i];
                }
            }

            return temp;
        }
        
        #endregion
    }
}