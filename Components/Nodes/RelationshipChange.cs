using System;

namespace DescantComponents
{
    [Serializable, MaxQuantity(Single.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class RelationshipChange : DescantNodeComponent
    {
        [ParameterGroup("First actor")] public string FirstActorName;
        
        [ParameterGroup("Second actor")] public string SecondActorName;
        
        [ParameterGroup("Operation to perform")] public OperationType OperationType;
        [ParameterGroup("Operation to perform")] public float OperationValue;

        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            /*
            DescantActor actor = Controller.GetActor(FirstActorName);
            
            switch (OperationType)
            {
                case OperationType.IncreaseBy:
                    actor.SetRelationship(SecondActorName, actor.Relationships[SecondActorName] + OperationValue);
                    break;
                
                case OperationType.DecreaseBy:
                    actor.SetRelationship(SecondActorName, actor.Relationships[SecondActorName] - OperationValue);
                    break;
                
                case OperationType.Set:
                    actor.SetRelationship(SecondActorName, OperationValue);
                    break;
            }
            */

            return result;
        }
    }
}