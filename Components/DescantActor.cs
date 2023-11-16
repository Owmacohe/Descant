using System;
using System.Collections.Generic;

namespace DescantComponents
{
    [Serializable]
    public class DescantActor
    {
        public string Name;
        public string Path;
        
        public List<string> StatisticKeys;
        public List<float> StatisticValues;
        
        public List<string> Topics;
        
        public List<string> RelationshipKeys;
        public List<float> RelationshipValues;
        
        public int ConversationAttempts;

        public DescantActor(string name)
        {
            Name = name;

            StatisticKeys = new List<string>();
            StatisticValues = new List<float>();
            
            Topics = new List<string>();

            RelationshipKeys = new List<string>();
            RelationshipValues = new List<float>();
        }

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
               " (" + ConversationAttempts + ")";
        }
    }
}