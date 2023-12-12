using System;
using UnityEngine;

namespace DescantRuntime
{
    public class DescantDialogueTrigger : MonoBehaviour
    {
        [SerializeField, Tooltip("The Descant UI script")] DescantDialogueUI UI;
        [SerializeField, Tooltip("Whether to start the dialogue when the scene starts")] bool displayOnStart;
        
        [Header("Data")]
        [SerializeField, Tooltip("The Descant Graph that will be converted into dialogue")] TextAsset graph;
        [SerializeField, Tooltip("The player's DescantActor")] TextAsset player;
        [SerializeField, Tooltip("The NPC DescantActor being interacted with")] TextAsset NPC;
        [SerializeField, Tooltip("Any more extra NPCs that the Descant Graph references")] TextAsset[] extraActors;
        
        [Header("Portraits")]
        [SerializeField, Tooltip("A list of portraits that can be applied to actors during the dialogue")] Sprite[] portraits;
        [SerializeField, Tooltip("The name of the player's default portrait")] string playerPortraitName;
        [SerializeField, Tooltip("The name of the NPC's default portrait")] string npcPortraitName;

        void Start()
        {
            if (displayOnStart) Display();
        }

        /// <summary>
        /// Method to call to initialize all of the supplied data in the Descant UI
        /// </summary>
        public void Display()
        {
            UI.Initialize(
                graph,
                player,
                NPC, 
                extraActors,
                portraits,
                playerPortraitName,
                npcPortraitName
            );
        }
    }
}