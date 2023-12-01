using System;

namespace DescantComponents
{
    [Serializable, MaxQuantity(Single.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class StatisticChange : DescantNodeComponent
    {
        [Inline] public string ActorName;
        
        [ParameterGroup("Statistic to change")] public string StatisticName;
        
        [ParameterGroup("Operation to perform")] public OperationType OperationType;
        [ParameterGroup("Operation to perform")] public float OperationValue;

        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            DescantActor actor = DescantComponentUtilities.GetActor(result.Actors, ActorName);

            if (actor == null) return result;
            
            switch (OperationType)
            {
                case OperationType.IncreaseBy:
                    actor.StatisticValues[actor.StatisticKeys.IndexOf(StatisticName)] += OperationValue;
                    break;
                
                case OperationType.DecreaseBy:
                    actor.StatisticValues[actor.StatisticKeys.IndexOf(StatisticName)] -= OperationValue;
                    break;
                
                case OperationType.Set:
                    actor.StatisticValues[actor.StatisticKeys.IndexOf(StatisticName)] = OperationValue;
                    break;
            }
            
            return result;
        }
    }
}