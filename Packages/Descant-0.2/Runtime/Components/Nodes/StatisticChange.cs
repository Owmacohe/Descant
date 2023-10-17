using System;
using Editor.Nodes;

namespace Runtime.Components.Nodes
{
    public enum StatisticChangeType { IncreaseBy, DecreaseBy, Set }
    
    public class StatisticChange : DescantNodeComponent, IDescantComponentInvokable
    {
        public DescantActor Actor { get; }
        public string Statistic { get; }
        public StatisticChangeType Type { get; }
        public ValueType Change { get; }

        public StatisticChange(DescantGraphController controller, int id, DescantActor actor, string statistic, StatisticChangeType type, ValueType change)
            : base(controller, id, DescantNodeType.Any, float.PositiveInfinity)
        {
            Actor = actor;
            Statistic = statistic;
            Type = type;
            Change = change;
        }

        public void Invoke()
        {
            switch (Type)
            {
                case StatisticChangeType.IncreaseBy:
                    Actor.SetStatistic(Statistic, (float)Actor.Statistics[Statistic] + (float)Change);
                    break;
                
                case StatisticChangeType.DecreaseBy:
                    Actor.SetStatistic(Statistic, (float)Actor.Statistics[Statistic] - (float)Change);
                    break;
                
                case StatisticChangeType.Set:
                    Actor.SetStatistic(Statistic, Change);
                    break;
            }
        }
    }
}