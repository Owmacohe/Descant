using System;
using Descant.Package.Editor.Nodes;
using Descant.Package.Runtime;
using UnityEngine.Serialization;

namespace Descant.Package.Components
{
    [Serializable, MaxQuantity(Single.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class StatisticChange :
        DescantNodeComponent, IInvokedDescantComponent
    {
        [InlineGroup(1)] public string ActorName;
        [InlineGroup(1)] public string StatisticName;
        [InlineGroup(2)] public OperationType OperationType;
        [InlineGroup(2)] public float OperationValue;

        public StatisticChange(
            DescantConversationController controller,
            int nodeID,
            int id,
            string actorName, string statisticName, OperationType operationType, float operationValue)
            : base(controller, nodeID, id)
        {
            ActorName = actorName;
            StatisticName = statisticName;
            OperationType = operationType;
            OperationValue = operationValue;
        }

        public void Invoke()
        {
            DescantActor actor = Controller.GetActor(ActorName);
            
            switch (OperationType)
            {
                case OperationType.IncreaseBy:
                    actor.SetStatistic(StatisticName, actor.Statistics[StatisticName] + OperationValue);
                    break;
                
                case OperationType.DecreaseBy:
                    actor.SetStatistic(StatisticName, actor.Statistics[StatisticName] - OperationValue);
                    break;
                
                case OperationType.Set:
                    actor.SetStatistic(StatisticName, OperationValue);
                    break;
            }
        }
    }
}