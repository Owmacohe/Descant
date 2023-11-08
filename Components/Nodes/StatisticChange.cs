using System;

namespace DescantComponents
{
    [Serializable, MaxQuantity(Single.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class StatisticChange :
        DescantNodeComponent, IInvokedDescantComponent
    {
        [Inline] public string ActorName;
        
        [ParameterGroup("Statistic to change")] public string StatisticName;
        
        [ParameterGroup("Operation to perform")] public OperationType OperationType;
        [ParameterGroup("Operation to perform")] public float OperationValue;

        public StatisticChange(
            //DescantConversationController controller,
            int nodeID,
            int id,
            string actorName, string statisticName, OperationType operationType, float operationValue)
            : base(nodeID, id)
        {
            ActorName = actorName;
            StatisticName = statisticName;
            OperationType = operationType;
            OperationValue = operationValue;
        }

        public void Invoke()
        {
            /*
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
            */
        }
    }
}