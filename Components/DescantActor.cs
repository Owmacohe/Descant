using System;
using System.Collections.Generic;
using Descant.Utilities;
using UnityEngine;

namespace Descant.Components
{
    /// <summary>
    /// A data class representing an actor (player or NPC) in a Descant Dialogue
    /// </summary>
    [Serializable, CreateAssetMenu(menuName = "Descant/Actor")]
    public class DescantActor : ScriptableObject
    {
        /// <summary>
        /// The actor's display name
        /// </summary>
        public string DisplayName;

        /// <summary>
        /// The actor's associated colour
        /// </summary>
        public Color Colour;

        /// <summary>
        /// The actor's display name, formatted bold with colour applied
        /// </summary>
        public String FormattedDisplayName => $"<b><color=#{ColorUtility.ToHtmlStringRGBA(Colour)}>{DisplayName}</color></b>";
        
        /// <summary>
        /// Whether or not to show the actor's portrait during runtime
        /// </summary>
        public bool PortraitEnabled;

        /// <summary>
        /// The actor's currently set portrait (Portraits[0] by default)
        /// </summary>
        [HideInInspector] public Sprite Portrait;
        
        /// <summary>
        /// All the possible portraits that this Actor can switch between (the first Sprite is used by default)
        /// </summary>
        public Sprite[] Portraits;

        /// <summary>
        /// The actor's statistics
        /// </summary>
        public ListDictionary<string, float> Statistics;
        
        /// <summary>
        /// The actor's topics list
        /// </summary>
        public List<string> Topics;

        /// <summary>
        /// The actor's relationship values
        /// </summary>
        public ListDictionary<string, float> Relationships;
        
        /// <summary>
        /// The number of times that the player has attempted to start a dialogue with the actor
        /// </summary>
        public int DialogueAttempts;

        /// <summary>
        /// Overridden ToString method
        /// </summary>
        public override string ToString()
        {
            string statistics = "";

            foreach (var i in Statistics)
                statistics += "(" + i.Key + " : " + i.Value + ")";
            
            string topics = "";

            foreach (var j in Topics)
                topics += " " + j;
            
            string relationships = "";
            
            foreach (var k in Relationships)
                statistics += "(" + k.Key + " : " + k.Value + ")";

            return GetType() +
               " (" + (statistics.Length > 0 ? statistics.Substring(1) : "") + ")" +
               " (" + (topics.Length > 0 ? topics.Substring(1) : "") + ")" +
               " (" + (relationships.Length > 0 ? relationships.Substring(1) : "") + ")" +
               " (" + DialogueAttempts + ")";
        }
    }
}