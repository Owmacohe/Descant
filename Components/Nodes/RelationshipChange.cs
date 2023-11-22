using System;

namespace DescantComponents
{
    [Serializable, MaxQuantity(Single.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class RelationshipChange : DescantNodeComponent
    {
        [ParameterGroup("Actors")] public string FirstActorName;
        [ParameterGroup("Actors")] public string SecondActorName;
        
        [ParameterGroup("Operation to perform")] public OperationType OperationType;
        [ParameterGroup("Operation to perform")] public float OperationValue;

        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            DescantActor actor = DescantComponentUtilities.GetActor(result.Actors, FirstActorName);
            
            switch (OperationType)
            {
                case OperationType.IncreaseBy:
                    actor.RelationshipValues[actor.RelationshipKeys.IndexOf(SecondActorName)] += OperationValue;
                    break;
                
                case OperationType.DecreaseBy:
                    actor.RelationshipValues[actor.RelationshipKeys.IndexOf(SecondActorName)] -= OperationValue;
                    break;
                
                case OperationType.Set:
                    actor.RelationshipValues[actor.RelationshipKeys.IndexOf(SecondActorName)] = OperationValue;
                    break;
            }

            return result;
        }
    }
}