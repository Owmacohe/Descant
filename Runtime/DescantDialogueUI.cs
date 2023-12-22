using System;
using DescantComponents;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DescantRuntime
{
    public class DescantDialogueUI : MonoBehaviour
    {
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

        Sprite[] portraits; // The current array of all possible actor portraits during the dialogue

        #region Initialization

        /// <summary>
        /// Initializes the DescantDialogueController's data
        /// (to be called before a dialogue is displayed)
        /// (generally called at the beginning of a scene or right before a new dialogue is about to begin)
        /// </summary>
        /// <param name="graph">The JSON graph to be loaded</param>
        /// <param name="player">The dialogue's player to be loaded</param>
        /// <param name="npc">The dialogue's NPC to be loaded</param>
        /// <param name="extraActors">The dialogue's extra actors to be loaded</param>
        /// <param name="portraits">The array of all possible actor portraits during the dialogue</param>
        /// <param name="playerPortrait">The name of the player's default portrait</param>
        /// <param name="npcPortrait">The name of the NPC's default portrait</param>
        /// <param name="display">Whether to immediately display the UI after its been Initialized</param>
        public void Initialize(
            TextAsset graph,
            TextAsset player, TextAsset npc, TextAsset[] extraActors,
            Sprite[] portraits, string playerPortrait, string npcPortrait,
            bool display = true)
        {
            this.portraits = portraits;
            dialogueController.Initialize(graph, player, npc, extraActors, playerPortrait, npcPortrait);
            
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
            if (dialogueController.HasEnded) EndDialogue(); // Constantly checking to see if the dialogue has ended
            
            // Advancing the dialogue when the player clicks (if it's waiting for that)
            if (waitForClick && Input.GetButtonDown("Fire1"))
            {
                if (dialogueController.Current.Next.Count == 0) EndDialogue();
                else DisplayNode();
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
            if (temp.Text.Count == 1 && dialogueController.Current.Data.Type.Equals("Response"))
            {
                // Either starting the typewriter or just sticking the text right in
                if (dialogueController.Typewriter) StartTypewriter(temp.Text[0].Value);
                else response.text = temp.Text[0].Value;

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
                }
            }
        }
        
        /// <summary>
        /// Quick method to check through all the available actor portraits to find teh one with the given name
        /// </summary>
        /// <param name="name">The name of the portrait image being searched for</param>
        /// <returns>The portrait image</returns>
        Sprite GetPortrait(string name)
        {
            foreach (var i in portraits)
                if (i.name == name)
                    return i;

            return null;
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