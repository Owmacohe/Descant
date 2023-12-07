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
        [SerializeField, Tooltip("The Descant Graph that will be played")] TextAsset graph;
        [SerializeField] TextAsset[] actors;
        [SerializeField] TextAsset player;
        [SerializeField] TextAsset NPC;

        [Header("UI")]
        [SerializeField] bool displayOnStart;
        [SerializeField, Tooltip("The NPC response text")] TMP_Text response;
        [SerializeField, Tooltip("The parent UI object for the player's choices (ideally a LayoutGroup)")] Transform choices;
        [SerializeField, Tooltip("The player choice prefab to be spawned with the choice text")] GameObject choice;
        
        DescantDialogueController dialogueController;
        GameObject UI;
        bool waitForClick;

        bool isTyping;
        string targetTypewriterText;
        int typewriterIndex;
        List<string> tags = new List<string>();

        void Awake()
        {
            dialogueController = gameObject.AddComponent<DescantDialogueController>();

            UI = transform.GetChild(0).gameObject;
            UI.SetActive(false);
        }
        
        void Start()
        {
            Initialize(graph, actors, player, NPC, displayOnStart);
        }

        void Update()
        {
            if (dialogueController.HasEnded) End();
            
            if (waitForClick && Input.GetButtonDown("Fire1"))
            {
                if (dialogueController.Current.Next.Count == 0) End();
                else DisplayNode();
            }
        }

        public void Initialize(TextAsset g, TextAsset[] a, TextAsset p, TextAsset npc, bool display)
        {
            dialogueController.Initialize(g, a, p, npc);
            
            if (display) BeginDialogue();
        }

        public void BeginDialogue()
        {
            dialogueController.BeginDialogue();
            DisplayNode();
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
            UI.SetActive(true);
            
            SetClickMessage(false);
            
            // Destroying all the old choices
            for (int i = 0; i < choices.childCount; i++)
                Destroy(choices.GetChild(i).gameObject);
            
            DescantNodeInvokeResult temp = dialogueController.Next(choiceIndex);
            
            if (temp == null)
            {
                End();
                return; // Stopping if there are no more nodes
            }
            
            // Displaying the ResponseNodes...
            if (temp.Choices.Count == 1 && dialogueController.Current.Data.Type.Equals("Response"))
            {
                if (dialogueController.Typewriter) StartTypewriter(temp.Choices[0].Value);
                else response.text = temp.Choices[0].Value;

                var next = dialogueController.Current.Next;

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
                    case "Response":
                    case "End":
                        SetClickMessage(true);
                        break;
                    
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

        void SetClickMessage(bool visible)
        {
            waitForClick = visible;
            response.transform.GetChild(0).gameObject.SetActive(visible);
        }

        void End()
        {
            UI.SetActive(false);
            waitForClick = false;
        }

        void StartTypewriter(string text)
        {
            response.text = "";
            
            isTyping = true;
            targetTypewriterText = text;
            typewriterIndex = 0;
            
            Type();
        }

        void Type()
        {
            if (isTyping && response.text != targetTypewriterText && typewriterIndex < targetTypewriterText.Length)
            {
                char temp = targetTypewriterText[typewriterIndex];

                if (temp == '<' && targetTypewriterText.Substring(typewriterIndex).Contains('>'))
                {
                    int skipLength = 0;
                    
                    for (int i = typewriterIndex; i < targetTypewriterText.Length; i++)
                    {
                        skipLength++;
                        response.text += targetTypewriterText[i];
                        
                        if (targetTypewriterText[i] == '>')
                        {
                            typewriterIndex += skipLength;
                            break;
                        }
                    }
                }
                else
                {
                    response.text += temp;
                    typewriterIndex++;   
                }

                Invoke(nameof(Type), (1f / dialogueController.TypewriterSpeed) / 10f);
            }
        }
    }
}