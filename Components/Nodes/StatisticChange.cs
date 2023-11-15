using System;
using System.Collections.Generic;
using DescantUtilities;

namespace DescantComponents
{
    [Serializable, MaxQuantity(Single.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class StatisticChange : DescantNodeComponent
    {
        [Inline] public string ActorName;
        
        [ParameterGroup("Statistic to change")] public string StatisticName;
        
        [ParameterGroup("Operation to perform")] public OperationType OperationType;
        [ParameterGroup("Operation to perform")] public float OperationValue;

        public override List<string> Invoke(List<string> choices)
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
            
            return choices;
        }

        public override void FixedUpdate()
        {
            
        }
    }
}