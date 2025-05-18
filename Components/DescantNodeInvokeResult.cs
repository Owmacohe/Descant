using System.Collections.Generic;

namespace Descant.Components
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
        /// The list of texts to display for the current node (as well as the index for their position when displayed)
        /// </summary>
        public List<KeyValuePair<int, string>> Text;
        
        /// <summary>
        /// The current player DescantActor
        /// </summary>
        public DescantActor Player;
        
        /// <summary>
        /// The current NPC DescantActor
        /// </summary>
        public DescantActor NPC;
        
        /// <summary>
        /// The list of all current DescantActors
        /// </summary>
        public List<DescantActor> Actors;

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="text">
        /// The list of texts to display for the current node (as well index for their position when displayed)
        /// </param>
        /// <param name="player">The current player DescantActor</param>
        /// <param name="npc">The current NPC DescantActor</param>
        /// <param name="actors">The list of all current DescantActors</param>
        public DescantNodeInvokeResult(
            List<KeyValuePair<int, string>> text,
            DescantActor player,
            DescantActor npc,
            List<DescantActor> actors)
        {
            Text = text;
            Player = player;
            NPC = npc;
            Actors = actors;
        }
    }
}