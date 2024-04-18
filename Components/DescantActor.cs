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
        public int DialogueAttempts;
        
        /// <summary>
        /// Parameterized constructor
        /// (most of the DescantActor's properties are set after
        /// it has been initialized, as part of the saving process)
        /// </summary>
        /// <param name="name">The name of the actor</param>
        public DescantActor(string name)
        {
            StatisticKeys = new List<string>();
            StatisticValues = new List<float>();
            
            Topics = new List<string>();

            RelationshipKeys = new List<string>();
            RelationshipValues = new List<float>();
        }

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