using System;

namespace DescantComponents
{
    [Serializable, MaxQuantity(Single.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class StatisticReveal :
        DescantNodeComponent, IInvokedDescantComponent
    {
        [Inline] public string ActorName;
        
        [ParameterGroup("Statistic to change")] public string StatisticName;

        public StatisticReveal(
            //DescantConversationController controller,
            int nodeID,
            int id,
            string actorName, string statisticName)
            : base(nodeID, id)
        {
            ActorName = actorName;
            StatisticName = statisticName;
        }

        public void Invoke()
        {
            // TODO: reveal statistic using
            // Actor.Statistics[Statistic]
        }
    }
}