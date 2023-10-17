using System;
using Editor.Nodes;

namespace Runtime.Components.Nodes
{
    public class StatisticReveal : DescantNodeComponent, IInvokedDescantComponent
    {
        public DescantActor Actor { get; }
        public string Statistic { get; }

        public StatisticReveal(DescantGraphController controller, int id, DescantActor actor, string statistic)
            : base(controller, id, DescantNodeType.Any, float.PositiveInfinity)
        {
            Actor = actor;
            Statistic = statistic;
        }

        public void Invoke()
        {
            // TODO: reveal statistic using
            // Actor.Statistics[Statistic]
        }
    }
}