using System;
using System.Collections.Generic;
using Descant.Utilities;
using UnityEngine;

namespace Descant.Editor
{
    /// <summary>
    /// Data file containing the log events for the Descant Log
    /// </summary>
    public class DescantLogData : ScriptableObject
    {
        /// <summary>
        /// A DescantLogEvent's type
        /// </summary>
        public enum LogEventType { Begin, End, Node, Component }
        
        /// <summary>
        /// A simple class containing data from a single event within Descant
        /// </summary>
        [Serializable]
        public class DescantLogEvent
        {
            /// <summary>
            /// The time that the event occurred at (HH:MM:SS)
            /// </summary>
            public string Time;
            
            /// <summary>
            /// The type of the event
            /// </summary>
            public LogEventType Type;
            
            /// <summary>
            /// The subtype of the Event's type
            /// (e.g. a ChoiceNode is a subtype of Node)
            /// </summary>
            public string Subtype;
            
            /// <summary>
            /// A name associated with the event
            /// (e.g. the custom name given to a DescantNode in the Descant Graph Editor)
            /// </summary>
            public string Name;

            /// <summary>
            /// Parameterized constructor
            /// </summary>
            /// <param name="time">The time that the event occurred at</param>
            /// <param name="type">The type of the event</param>
            /// <param name="subtype">The subtype of the Event's type</param>
            /// <param name="name">A name associated with the event</param>
            public DescantLogEvent(string time, LogEventType type, string subtype, string name)
            {
                Time = time;
                Type = type;
                Subtype = subtype;
                Name = name;

                // Making sure to trim the subtype down to only the last name
                if (Subtype.Contains('.'))
                {
                    int startIndex = Subtype.LastIndexOf('.') + 1;
                    Subtype = Subtype.Substring(startIndex, Subtype.Length - startIndex);
                }
            }
        }
        
        /// <summary>
        /// Whether or not to capture the Descant Log Events during runtime
        /// </summary>
        [HideInInspector] public bool Capture;
        
        /// <summary>
        /// The actual list of events that occurred the last time that the game was played
        /// </summary>
        [HideInInspector] public List<DescantLogEvent> Events = new List<DescantLogEvent>();

        /// <summary>
        /// Quick method to clear the file's data
        /// </summary>
        public void Clear()
        {
            Events = new List<DescantLogEvent>();
            
            #if UNITY_EDITOR
            DescantUtilities.SaveSerializedObject(this);
            #endif
        }

        /// <summary>
        /// Adds a new Descant Log Event to the list
        /// </summary>
        /// <param name="type">The type of the event</param>
        /// <param name="subtype">The subtype of the Event's type</param>
        /// <param name="name">A name associated with the event</param>
        public void Log(LogEventType type, string subtype = "", string name = "")
        {
            if (Capture) Events.Add(new DescantLogEvent(
                DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second,
                type,
                subtype,
                name
            ));
        }
    }
}