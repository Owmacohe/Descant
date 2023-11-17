using System;

namespace DescantComponents
{
    [Serializable, MaxQuantity(Single.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class StatisticReveal : DescantNodeComponent
    {
        [Inline] public string ActorName;
        
        [ParameterGroup("Statistic to change")] public string StatisticName;

        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            // TODO: reveal statistic using
            // Actor.Statistics[Statistic]
            
            return result;
        }
    }
}