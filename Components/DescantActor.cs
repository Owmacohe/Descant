using System;
using System.Collections.Generic;
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
        /// The actor's statistics dictionary keys
        /// (C# Dictionaries can't be [Serialized], so we use two Lists instead)
        /// </summary>
        public List<string> StatisticKeys;
        
        /// <summary>
        /// The actor's statistics dictionary values
        /// (C# Dictionaries can't be [Serialized], so we use two Lists instead)
        /// </summary>
        public List<float> StatisticValues;
        
        /// <summary>
        /// The actor's topics list
        /// </summary>
        public List<string> Topics;
        
        /// <summary>
        /// The actor's relationships dictionary keys
        /// (C# Dictionaries can't be [Serialized], so we use two Lists instead)
        /// </summary>
        public List<string> RelationshipKeys;
        
        /// <summary>
        /// The actor's relationships dictionary values
        /// (C# Dictionaries can't be [Serialized], so we use two Lists instead)
        /// </summary>
        public List<float> RelationshipValues;
        
        /// <summary>
        /// The number of times that the player has attempted to start a dialogue with the actor
        /// </summary>
        [HideInInspector] public int DialogueAttempts;

        /// <summary>
        /// Overridden ToString method
        /// </summary>
        public override string ToString()
        {
            string statistics = "";

            for (int i = 0; i < StatisticKeys.Count; i++)
                statistics += " (" + StatisticKeys[i] + " " + StatisticValues[i] + ")";
            
            string topics = "";

            foreach (var j in Topics)
                topics += " " + j;
            
            string relationships = "";

            for (int i = 0; i < RelationshipKeys.Count; i++)
                relationships += " (" + RelationshipKeys[i] + " " + RelationshipValues[i] + ")";

            return GetType() +
               " (" + (statistics.Length > 0 ? statistics.Substring(1) : "") + ")" +
               " (" + (topics.Length > 0 ? topics.Substring(1) : "") + ")" +
               " (" + (relationships.Length > 0 ? relationships.Substring(1) : "") + ")" +
               " (" + DialogueAttempts + ")";
        }
    }
}