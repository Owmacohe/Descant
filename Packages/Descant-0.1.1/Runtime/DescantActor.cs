using System.Collections.Generic;

namespace DescantRuntime
{
    public enum DescantActorType { Player, NPC, Both }
    
    public class DescantActor
    {
        public string Name { get; }
        public DescantActorType Type { get; }
        public Dictionary<string, float> Statistics { get; }
        public List<string> Topics { get; }
        public Dictionary<string, float> Relationships { get; }
        public int ReAttempts { get; private set; }

        public DescantActor(string name, DescantActorType type)
        {
            Statistics = new Dictionary<string, float>();
            Topics = new List<string>();
            Relationships = new Dictionary<string, float>();
            ReAttempts = 0;
        }

        public void SetStatistic(string stat, float value) // TODO: something may subscribe to this
        {
            Statistics[stat] = value;
        }
        
        public void SetTopic(string topic, bool add) // TODO: something may subscribe to this
        {
            if (add) Topics.Add(topic);
            else Topics.Remove(topic);
        }

        public void SetRelationship(string actor, float value) // TODO: something may subscribe to this
        {
            Relationships[actor] = value;
        }

        public void IncreaseReAttempts() // TODO: something may subscribe to this
        {
            ReAttempts++;
        }
    }
}