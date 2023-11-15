using System;
using System.Collections.Generic;
using DescantUtilities;

namespace DescantComponents
{
    [Serializable, MaxQuantity(Single.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class StatisticReveal : DescantNodeComponent
    {
        [Inline] public string ActorName;
        
        [ParameterGroup("Statistic to change")] public string StatisticName;

        public override List<string> Invoke(List<string> choices)
        {
            // TODO: reveal statistic using
            // Actor.Statistics[Statistic]
            
            return choices;
        }

        public override void FixedUpdate()
        {
            
        }
    }
}