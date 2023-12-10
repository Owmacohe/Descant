using System;
using System.Collections.Generic;
using DescantComponents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DescantRuntime
{
    public class DescantDialogueUI : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField, Tooltip("The Descant Graph that will be converted into dialogue")] TextAsset graph;
        [SerializeField, Tooltip("The player's DescantActor")] TextAsset player;
        [SerializeField, Tooltip("The NPC DescantActor being interacted with")] TextAsset NPC;
        [SerializeField, Tooltip("Any more extra NPCs that the Descant Graph references")] TextAsset[] actors;
        
        [Header("Portraits")]
        [SerializeField, Tooltip("A list of portraits that can be applied to actors during the dialogue")] List<Sprite> actorPortraits;
        [SerializeField, Tooltip("The name of the player's default portrait")] string playerPortraitName;
        [SerializeField, Tooltip("The name of the NPC's default portrait")] string npcPortraitName;

        [Header("UI")]
        [SerializeField, Tooltip("Whether to start the dialogue when the scene starts")] bool displayOnStart;
        [SerializeField, Tooltip("The parent UI object for the player's choices (ideally a LayoutGroup)")] Transform choices;
        [SerializeField, Tooltip("The player choice prefab to be spawned with the choice text")] GameObject choice;
        [SerializeField, Tooltip("The player's portrait image")] Image playerPortrait;
        [SerializeField, Tooltip("The NPC response text")] TMP_Text response;
        [SerializeField, Tooltip("The NPC's portrait image")] Image npcPortrait;
        
        DescantDialogueController dialogueController; // The controller for the dialogue backend
        GameObject background; // The parent of all the UI members
        
        // Whether the UI is waiting for the player to click on the screen for the dialogue to continue
        // (rather than continuing automatically)
        bool waitForClick;

        string targetTypewriterText; // The full text that the typewriter is typing out
        int typewriterIndex; // The index in the target text that the typewriter is currently at
        
        void Awake()
        {
            dialogueController = gameObject.AddComponent<DescantDialogueController>();

            // Hiding the UI to start
            background = transform.GetChild(0).gameObject;
            background.SetActive(false);
        }
        
        void Start()
        {
            // Initializing the dialogue (so that it doesn't have to be in Initialized when the interaction begins)
            Initialize(graph, player, NPC, actors, displayOnStart);
        }

        void Update()
        {
            if (dialogueController.HasEnded) EndDialogue(); // Constantly checking to see if the dialogue has ended
            
            // Advancing the dialogue when the player clicks (if it's waiting for that)
            if (waitForClick && Input.GetButtonDown("Fire1"))
            {
                if (dialogueController.Current.Next.Count == 0) EndDialogue();
                else DisplayNode();
            }
        }

        /// <summary>
        /// Initializes the DescantDialogueController's data
        /// (to be called before a dialogue is displayed)
        /// (generally called at the beginning of a scene or right before a new dialogue is about to begin)
        /// </summary>
        /// <param name="g">The JSON graph to be loaded</param>
        /// <param name="p">The dialogue's player to be loaded</param>
        /// <param name="npc">The dialogue's NPC to be loaded</param>
        /// <param name="a">The dialogue's extra actors to be loaded</param>
        /// <param name="display">Whether to immediately display the UI after its been Initialized</param>
        public void Initialize(TextAsset g, TextAsset p, TextAsset npc, TextAsset[] a, bool display)
        {
            dialogueController.Initialize(g, p, npc, a, playerPortraitName, npcPortraitName);
            
            if (display) BeginDialogue();
        }

        /// <summary>
        /// Displays the UI (to be called only after the dialogue has been Initialized)
        /// </summary>
        public void BeginDialogue()
        {
            dialogueController.BeginDialogue();
            DisplayNode();
        }
        
        /// <summary>
        /// Hiding the UI (to be called after the dialogue has ended)
        /// </summary>
        public void EndDialogue()
        {
            background.SetActive(false);
            waitForClick = false;

            dialogueController.HasEnded = true;
        }

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
            DescantNodeInvokeResult temp = dialogueController.Next(choiceIndex);
            
            // Stopping if there are no more nodes
            if (temp == null)
            {
                EndDialogue();
                return;
            }
            
            // Setting the actor portraits accordingly
            playerPortrait.sprite = GetPortrait(dialogueController.PlayerPortrait);
            npcPortrait.sprite = GetPortrait(dialogueController.NPCPortrait);
            
            // Enabling/disabling the portraits based on whether they're null and/or enabled/disabled
            playerPortrait.gameObject.SetActive(playerPortrait.sprite != null && dialogueController.PlayerPortraitEnabled);
            npcPortrait.gameObject.SetActive(npcPortrait.sprite != null && dialogueController.NPCPortraitEnabled);

            // Displaying the ResponseNodes...
            if (temp.Choices.Count == 1 && dialogueController.Current.Data.Type.Equals("Response"))
            {
                // Either starting the typewriter or just sticking the text right in
                if (dialogueController.Typewriter) StartTypewriter(temp.Choices[0].Value);
                else response.text = temp.Choices[0].Value;

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
                    // If the next is an End or Response node, we show the click message
                    case "Response":
                    case "End":
                        SetClickMessage(true);
                        break;
                    
                    // Otherwise, we display the next node (which will be a ChoiceNode)
                    default:
                        // Once the response text has been shown, we skip ahead to show the player's possible choices
                        DisplayNode();
                        break;
                }
            }
            // Displaying the ChoiceNodes...
            else
            {
                foreach (var j in temp.Choices)
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
                }
            }
        }

        /// <summary>
        /// Shows/hides the click message to indicate that the player has to click to advance the dialogue
        /// </summary>
        /// <param name="visible">Whether to show or hide it</param>
        void SetClickMessage(bool visible)
        {
            waitForClick = visible;
            response.transform.GetChild(0).gameObject.SetActive(visible);
        }

        /// <summary>
        /// Quick method to check through all the available actor portraits to find teh one with the given name
        /// </summary>
        /// <param name="name">The name of the portrait image being searched for</param>
        /// <returns>The portrait image</returns>
        Sprite GetPortrait(string name)
        {
            foreach (var i in actorPortraits)
                if (i.name == name)
                    return i;

            return null;
        }

        #region Typewriter

        /// <summary>
        /// Starts the typewriter process to display text one character at a time into the Response section of the UI
        /// </summary>
        /// <param name="text">The text to be typed out</param>
        void StartTypewriter(string text)
        {
            response.text = "";
            
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
            if (response.text != targetTypewriterText)
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
                Invoke(nameof(Type), (1f / dialogueController.TypewriterSpeed) / 10f);
            }
        }

        #endregion
    }
}