using System;
using Descant.Components;
using Descant.Editor;
using UnityEngine;

namespace Descant.Runtime
{
    public class DescantDialogueTrigger : MonoBehaviour
    {
        [SerializeField, Tooltip("The Descant UI script")] DescantDialogueUI UI;
        [SerializeField, Tooltip("Whether to start the dialogue when the scene starts")] bool displayOnStart;
        
        [Header("Data")]
        [SerializeField, Tooltip("The Descant Graph that will be converted into dialogue")] DescantGraph graph;
        [SerializeField, Tooltip("The player's DescantActor")] DescantActor player;
        [SerializeField, Tooltip("The NPC DescantActor being interacted with")] DescantActor NPC;
        [SerializeField, Tooltip("Any more extra NPCs that the Descant Graph references")] DescantActor[] extraActors;

        void Start()
        {
            if (displayOnStart) Display();
        }

        /// <summary>
        /// Method to call to initialize all of the supplied data in the Descant UI
        /// </summary>
        public void Display() => UI.Initialize(
            graph,
            player,
            NPC, 
            extraActors
        );
    }
}