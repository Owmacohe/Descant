using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace DescantRuntime
{
    public class DescantConversationUI : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField, Tooltip("The Descant Graph that will be played")] TextAsset descantGraph;
        
        [Header("UI")]
        [SerializeField, Tooltip("The NPC response text")] TMP_Text response;
        [SerializeField, Tooltip("The parent UI object for the player's choices (ideally a LayoutGroup)")] Transform choices;
        [SerializeField, Tooltip("The player choice prefab to be spawned with the choice text")] GameObject choice;
        
        DescantConversationController conversationController;
    
        void Awake()
        {
            conversationController = gameObject.AddComponent<DescantConversationController>();
            conversationController.Initialize(descantGraph);
        }
        
        void Start()
        {
            DisplayNode();
        }
        
        /// <summary>
        /// Calls the Next() method in the conversation controller, gets the data, and displays it on-screen
        /// </summary>
        /// <param name="choiceIndex">
        /// The index of the choice being made (base 0)
        /// (default 0 if the current node is a ResponseNode)
        /// </param>
        void DisplayNode(int choiceIndex = 0)
        {
            // Destroying all the old choices
            for (int i = 0; i < choices.childCount; i++)
                Destroy(choices.GetChild(i).gameObject);
            
            List<string> temp = conversationController.Next(choiceIndex);
            if (temp == null) return; // Stopping if there are no more nodes
    
            // Displaying the ResponseNodes...
            if (temp.Count == 1)
            {
                response.text = temp[0];
                // Once the response text has been shown, we skip ahead to show the player's possible choices
                DisplayNode();
            }
            // Displaying the ChoiceNodes...
            else
            {
                for (int j = 0; j < temp.Count; j++)
                {
                    // Instantiating the player choices in teh player choice parent
                    GameObject tempChoice = Instantiate(choice, choices);
                    
                    // Setting the text of the choice
                    tempChoice.GetComponentInChildren<TMP_Text>().text = temp[j]; 
                    
                    // Copying the current index to a copy variable so that it can be used in the listener below
                    // (absolutely no idea why this must be done, but it must)
                    var copy = j;
                    
                    // Adding a listener to the player choice's button to display the next node when clicked
                    tempChoice.GetComponentInChildren<Button>().onClick.AddListener(() =>
                    {
                        DisplayNode(copy); 
                    });
                }
            }
        }
    }
}