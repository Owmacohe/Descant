using System;
using Descant.Components;
using Descant.Editor;
using Descant.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Descant.Runtime
{
    public class DescantDialogueUI : MonoBehaviour
    {
        [SerializeField, Tooltip("Whether to display Debug.Log messages from Log Components")] bool verbose = true;
        
        [Header("Player")]
        [SerializeField, Tooltip("The parent UI object for the player's choices (ideally a LayoutGroup)")] Transform choices;
        [SerializeField, Tooltip("The player choice prefab to be spawned with the choice text")] GameObject choice;
        [SerializeField, Tooltip("The player's portrait image")] Image playerPortrait;
        
        [Header("NPC")]
        [SerializeField, Tooltip("The NPC response text")] TMP_Text response;
        [SerializeField, Tooltip("The NPC's portrait image")] Image npcPortrait;
        
        DescantDialogueController dialogueController; // The controller for the dialogue backend
        GameObject background; // The parent of all the UI members
        
        // Whether the UI is waiting for the player to click on the screen for the dialogue to continue
        // (rather than continuing automatically)
        bool waitForClick;
        
        // Whether this UI has been ended (usually after it finishes)
        // (also used to make sure that the Update method doesn't just keep checking after the dialogue is done)
        bool hasEnded;

        bool typing; // Whether the typewriter is currently typing (i.e. it hasn't finished typing yet)
        string targetTypewriterText; // The full text that the typewriter is typing out
        int typewriterIndex; // The index in the target text that the typewriter is currently at
        float currentTypewriterSpeed; // The current speed that the typewriter is typing at

        [Header("Callbacks")]
        
        [Tooltip("A callback that triggers when the dialogue begins")]
        public UnityEvent OnBegin;
        
        [Tooltip("A callback that triggers when the next node in the dialogue is reached")]
        public UnityEvent OnDisplay;
        
        [Tooltip("A callback that triggers when ChoiceNodes are added to the UI")]
        public UnityEvent<Button> OnAddChoice;
        
        [Tooltip("A callback that triggers when the dialogue ends")]
        public UnityEvent OnEnd;

        #region Initialization

        /// <summary>
        /// Initializes the DescantDialogueController's data
        /// (to be called before a dialogue is displayed)
        /// (generally called at the beginning of a scene or right before a new dialogue is about to begin)
        /// </summary>
        /// <param name="graph">The graph to be loaded</param>
        /// <param name="player">The dialogue's player to be loaded</param>
        /// <param name="npc">The dialogue's NPC to be loaded</param>
        /// <param name="extraActors">The dialogue's extra actors to be loaded</param>
        /// <param name="display">Whether to immediately display the UI after its been Initialized</param>
        public void Initialize(
            DescantGraph graph,
            DescantActor player, DescantActor npc, DescantActor[] extraActors,
            bool display = true)
        {
            dialogueController.Initialize(graph, player, npc, extraActors);

            currentTypewriterSpeed = dialogueController.TypewriterSpeed;
            
            if (display) BeginDialogue();
        }

        /// <summary>
        /// Displays the UI (to be called only after the dialogue has been Initialized)
        /// </summary>
        public void BeginDialogue()
        {
            dialogueController.BeginDialogue();

            response.text = "";
            SetClickMessage(false);
            
            DisplayNode();
            
            OnBegin?.Invoke();
        }
        
        /// <summary>
        /// Hiding the UI (to be called after the dialogue has ended)
        /// </summary>
        public void EndDialogue()
        {
            background.SetActive(false);
            waitForClick = false;

            dialogueController.EndDialogue();
            hasEnded = true;
            
            OnEnd?.Invoke();
        }

        #endregion
        
        void Awake()
        {
            // Making sure to warn the user if there are no EventSystems present
            if (!FindObjectOfType(typeof(EventSystem)))
                DescantUtilities.ErrorMessage(
                    GetType(),
                    "Don't forget to add an EventSystem to the scene with Create/UI/Event System!");
            
            dialogueController = gameObject.AddComponent<DescantDialogueController>();

            // Hiding the UI to start
            background = transform.GetChild(0).gameObject;
            background.SetActive(false);
        }

        void Update()
        {
            if (dialogueController.HasEnded && !hasEnded) EndDialogue(); // Constantly checking to see if the dialogue has ended

            if (Input.GetButtonDown("Fire1"))
            {
                if (typing)
                {
                    currentTypewriterSpeed *= 10;
                }
                // Advancing the dialogue when the player clicks (if it's waiting for that)
                else if (waitForClick)
                {
                    if (dialogueController.Current.Next.Count == 0) EndDialogue();
                    else DisplayNode();
                }
            }
        }

        #region Node processing

        /// <summary>
        /// Calls the Next() method in the conversation controller, gets the data, and displays it on-screen
        /// </summary>
        /// <param name="choiceIndex">
        /// The index of the choice being made (base 0)
        /// (default 0 if the current node is a ResponseNode)
        /// </param>
        public void DisplayNode(int choiceIndex = 0)
        {
            // Hiding the UI and click message by default (will be turned on later if there is a next node)
            background.SetActive(true);
            SetClickMessage(false);

            // Destroying all the old choices (if there are any)
            for (int i = 0; i < choices.childCount; i++)
                Destroy(choices.GetChild(i).gameObject);
            
            // Getting the next node in the path from the DescantDialogueController
            DescantNodeInvokeResult temp = dialogueController.Next(choiceIndex, verbose);
            
            // Stopping if there are no more nodes
            if (temp == null)
            {
                EndDialogue();
                return;
            }
            
            // Setting the actor portraits accordingly
            playerPortrait.sprite = temp.Player != null ? temp.Player.Portrait : null;
            npcPortrait.sprite = temp.NPC != null ? temp.NPC.Portrait : null;
            
            // Enabling/disabling the portraits based on whether they're null and/or enabled/disabled
            playerPortrait.gameObject.SetActive(playerPortrait.sprite != null && temp.Player.PortraitEnabled);
            npcPortrait.gameObject.SetActive(npcPortrait.sprite != null && temp.NPC.PortraitEnabled);

            string currentNodeType = dialogueController.Current.Data.Type;

            // Displaying the ResponseNodes...
            if (currentNodeType.Equals("Response"))
            {
                // Either starting the typewriter or just sticking the text right in
                if (dialogueController.Typewriter) StartTypewriter(temp.Text[0].Value, temp.NPC);
                else
                {
                    var overrideSpeaker = ((DescantResponseNodeData) dialogueController.Current.Data).OverrideSpeaker;
                    
                    response.text = (dialogueController.SpeakerName || overrideSpeaker != null
                        ? (overrideSpeaker == null
                            ? temp.NPC != null
                                ? temp.NPC.FormattedDisplayName
                                : temp.Text[0].Value
                            : overrideSpeaker.FormattedDisplayName) + ": "
                        : "") + temp.Text[0].Value;
                }

                // Quickly checking to see what the next node is
                var next = dialogueController.Current.Next;

                // If there is no next, we show the click message and print an error
                if (next == null || next.Count == 0)
                {
                    SetClickMessage(true);
                    
                    DescantUtilities.ErrorMessage(
                        GetType(),
                        "Dialogue path contains no end node!"
                    );
                    
                    return;
                }
                
                switch (next[0].Data.Type)
                {
                    // If the next is a Response, If, or End node, we show the click message
                    case "Response":
                    case "If":
                    case "End":
                        SetClickMessage(true);
                        break;
                    
                    // Otherwise, we display the next node (which will be a ChoiceNode)
                    default:
                        // Once the response text has been shown, we skip ahead to show the player's possible choices
                        // (we only do this if there is no typewriter)
                        if (!dialogueController.Typewriter) DisplayNode();
                        break;
                }
            }
            // Displaying the ChoiceNodes...
            else if (currentNodeType.Equals("Choice"))
            {
                foreach (var j in temp.Text)
                {
                    // Instantiating the player choices in the player choice parent
                    GameObject tempChoice = Instantiate(choice, choices);
                    
                    // Setting the text of the choice
                    tempChoice.GetComponentInChildren<TMP_Text>().text = j.Value;
                    
                    // Copying the current index to a copy variable so that it can be used in the listener below
                    // (absolutely no idea why this must be done, but it must)
                    var copy = j.Key;
                    
                    // Adding a listener to the player choice's button to display the next node when clicked
                    tempChoice.GetComponentInChildren<Button>().onClick.AddListener(() =>
                    {
                        DisplayNode(copy); 
                    });
                    
                    OnAddChoice?.Invoke(tempChoice.GetComponent<Button>());
                }
            }
            else if (currentNodeType.Equals("If"))
            {
                // Displaying the correct next node depending on the checked result
                DisplayNode(((DescantIfNodeData) dialogueController.Current.Data).IfComponent.Check() ? 0 : 1);
            }
            
            OnDisplay?.Invoke();
        }

        #endregion

        /// <summary>
        /// Shows/hides the click message to indicate that the player has to click to advance the dialogue
        /// </summary>
        /// <param name="visible">Whether to show or hide it</param>
        void SetClickMessage(bool visible)
        {
            waitForClick = visible;
            response.transform.GetChild(0).gameObject.SetActive(visible);
        }

        #region Typewriter

        /// <summary>
        /// Starts the typewriter process to display text one character at a time into the Response section of the UI
        /// </summary>
        /// <param name="text">The text to be typed out</param>
        /// <param name="npc">The DescantActor associated with this response</param>
        void StartTypewriter(string text, DescantActor npc)
        {
            typing = true;

            currentTypewriterSpeed = dialogueController.TypewriterSpeed;
            
            var overrideSpeaker = ((DescantResponseNodeData) dialogueController.Current.Data).OverrideSpeaker;
            
            response.text = dialogueController.SpeakerName || overrideSpeaker != null
                ? (overrideSpeaker == null
                    ? npc != null
                        ? npc.FormattedDisplayName
                        : ""
                    : overrideSpeaker.FormattedDisplayName) + ": "
                : "";
            
            targetTypewriterText = text;
            typewriterIndex = 0;
            
            Type();
        }

        /// <summary>
        /// A recursively-called method to display one character at a time into the Response section of the UI
        /// (skipping over TMP styling tags)
        /// </summary>
        void Type()
        {
            // Making sure we haven't reached the end yet
            if (response.text != targetTypewriterText && typewriterIndex < targetTypewriterText.Length)
            {
                char temp = targetTypewriterText[typewriterIndex]; // The next character to be typed

                // Skipping a tag when we find it (provided it has a closing bracket)
                if (temp == '<' && targetTypewriterText.Substring(typewriterIndex).Contains('>'))
                {
                    int skipLength = 0;
                    
                    // Creating a small inner loop that goes until we find the end of the tag
                    for (int i = typewriterIndex; i < targetTypewriterText.Length; i++)
                    {
                        skipLength++; // Upping the length of the skip
                        
                        // We still want to put the text into the UI
                        // (TMP's inherent styling will hide it once it gets typed out entirely)
                        response.text += targetTypewriterText[i];
                        
                        if (targetTypewriterText[i] == '>')
                        {
                            // Skipping over the tag for future Type() calls once it's done
                            typewriterIndex += skipLength;
                            break;
                        }
                    }
                }
                // Otherwise just typing out the character
                else
                {
                    response.text += temp;
                    typewriterIndex++;   
                }

                // Calling itself again after some amount of time (dependant on the typewriterSpeed)
                Invoke(nameof(Type), (1f / currentTypewriterSpeed) / 10f);
            }
            else
            {
                typing = false;
                
                // Once the typewriter is finished, then we can display the choices
                if (dialogueController.Current.Next[0].Data.Type == "Choice")
                    DisplayNode();
            }
        }

        #endregion
    }
}