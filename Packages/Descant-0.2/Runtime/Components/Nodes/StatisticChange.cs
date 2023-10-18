using System;
using Editor.Nodes;

namespace Runtime.Components.Nodes
{
    public class StatisticChange : DescantNodeComponent, IInvokedDescantComponent
    {
        public DescantActor Actor { get; }
        public string Statistic { get; }
        public ValueChangeType Type { get; }
        public float Change { get; }

        public StatisticChange(DescantConversationController controller, int id, DescantActor actor, string statistic, ValueChangeType type, float change)
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
                case ValueChangeType.IncreaseBy:
                    Actor.SetStatistic(Statistic, Actor.Statistics[Statistic] + Change);
                    break;
                
                case ValueChangeType.DecreaseBy:
                    Actor.SetStatistic(Statistic, Actor.Statistics[Statistic] - Change);
                    break;
                
                case ValueChangeType.Set:
                    Actor.SetStatistic(Statistic, Change);
                    break;
            }
        }
    }
}