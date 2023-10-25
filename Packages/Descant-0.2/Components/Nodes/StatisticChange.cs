using System;
using Editor.Nodes;
using Runtime;

namespace Components
{
    [Serializable]
    public class StatisticChange :
        DescantNodeComponent, IInvokedDescantComponent,
        IChoiceNodeComponent, IResponseNodeComponent, IStartNodeComponent, IEndNodeComponent
    {
        public DescantActor Actor;
        public string Statistic;
        public ValueChangeType ChangeType;
        public float Change;

        public StatisticChange(
            DescantConversationController controller,
            int nodeID,
            int id,
            DescantActor actor, string statistic, ValueChangeType changeType, float change)
            : base(controller, nodeID, id, float.PositiveInfinity)
        {
            Actor = actor;
            Statistic = statistic;
            ChangeType = changeType;
            Change = change;
        }

        public void Invoke()
        {
            switch (ChangeType)
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