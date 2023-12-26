using System.Collections.Generic;

namespace DescantComponents
{
    /// <summary>
    /// Enum of DescantActors' variable types
    /// (used by DescantComponents when querying such variables)
    /// </summary>
    public enum VariableType { Statistic, Topic, Relationship, DialogueAttempts }
    
    /// <summary>
    /// Enum of different ways to compare DescantActors' variables
    /// (used by DescantComponents when querying such variables)
    /// </summary>
    public enum ComparisonType { LessThan, LessThanOrEqualTo, EqualTo, GreaterThanOrEqualTo, GreaterThan, NotEqualTo }
    
    /// <summary>
    /// Enum of ways to perform operations upon DescantActors' variables
    /// (used by DescantComponents when querying such variables)
    /// </summary>
    public enum OperationType { IncreaseBy, DecreaseBy, Set }
    
    /// <summary>
    /// Enum ways to perform operations upon DescantActors' lists/dictionaries
    /// (used by DescantComponents when querying such lists)
    /// </summary>
    public enum ListChangeType { Add, Remove }
    
    /// <summary>
    /// Enum of ways to perform operations upon DescantActors' portraits
    /// (used by DescantComponents when querying such portraits)
    /// </summary>
    public enum PortraitChangeType { Enable, Disable, Set }

    /// <summary>
    /// Data class all the choice/response text for the current node,
    /// the current actors, and info regarding the actor portraits
    /// </summary>
    public class DescantNodeInvokeResult
    {
        /// <summary>
        /// The list of texts to display for the current node (as well index for their position when displayed)
        /// </summary>
        public List<KeyValuePair<int, string>> Text;
        
        /// <summary>
        /// The list of current DescantActors
        /// </summary>
        public List<DescantActor> Actors;
        
        /// <summary>
        /// The current name of the player's portrait
        /// </summary>
        public string PlayerPortrait;
        
        /// <summary>
        /// Whether the player's portrait is currently enabled
        /// </summary>
        public bool PlayerPortraitEnabled;
        
        /// <summary>
        /// The current name of the NPC's portrait
        /// </summary>
        public string NPCPortrait;
        
        /// <summary>
        /// Whether the NPC's portrait is currently enabled
        /// </summary>
        public bool NPCPortraitEnabled;

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="text">
        /// The list of texts to display for the current node (as well index for their position when displayed)
        /// </param>
        /// <param name="actors">The list of current DescantActors</param>
        /// <param name="playerPortrait">The current name of the player's portrait</param>
        /// <param name="playerPortraitEnabled">Whether the player's portrait is currently enabled</param>
        /// <param name="npcPortrait">The current name of the NPC's portrait</param>
        /// <param name="npcPortraitEnabled">Whether the NPC's portrait is currently enabled</param>
        public DescantNodeInvokeResult(
            List<KeyValuePair<int, string>> text,
            List<DescantActor> actors,
            string playerPortrait,
            bool playerPortraitEnabled,
            string npcPortrait,
            bool npcPortraitEnabled)
        {
            Text = text;
            Actors = actors;
            PlayerPortrait = playerPortrait;
            PlayerPortraitEnabled = playerPortraitEnabled;
            NPCPortrait = npcPortrait;
            NPCPortraitEnabled = npcPortraitEnabled;
        }
    }
}