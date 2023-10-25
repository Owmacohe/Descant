using System;
using Editor.Nodes;
using Runtime;

namespace Components
{
    [Serializable]
    public class StatisticReveal :
        DescantNodeComponent, IInvokedDescantComponent,
        IChoiceNodeComponent, IResponseNodeComponent, IStartNodeComponent, IEndNodeComponent
    {
        public DescantActor Actor;
        public string Statistic;

        public StatisticReveal(
            DescantConversationController controller,
            int nodeID,
            int id,
            DescantActor actor, string statistic)
            : base(controller, nodeID, id, float.PositiveInfinity)
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