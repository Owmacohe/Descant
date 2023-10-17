using System;
using System.Collections.Generic;

namespace Runtime
{
    public enum DescantActorType { Player, NPC, Both }
    
    public class DescantActor
    {
        public string Name { get; }
        public DescantActorType Type { get; }
        public Dictionary<string, ValueType> Statistics { get; private set; }

        public DescantActor(string name, DescantActorType type)
        {
            Statistics = new Dictionary<string, ValueType>();
        }

        public void SetStatistic(string name, ValueType value) // TODO: something subscribes to this
        {
            Statistics[name] = value;
        }
    }
}