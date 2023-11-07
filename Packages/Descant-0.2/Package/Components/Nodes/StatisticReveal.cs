using System;
using Descant.Package.Editor.Nodes;
using Descant.Package.Runtime;
using UnityEngine.Serialization;

namespace Descant.Package.Components
{
    [Serializable, MaxQuantity(Single.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class StatisticReveal :
        DescantNodeComponent, IInvokedDescantComponent
    {
        [InlineGroup(1)] public string ActorName;
        [InlineGroup(1)] public string StatisticName;

        public StatisticReveal(
            DescantConversationController controller,
            int nodeID,
            int id,
            string actorName, string statisticName)
            : base(controller, nodeID, id)
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