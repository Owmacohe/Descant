using System;
using Descant.Package.Editor.Nodes;
using Descant.Package.Runtime;

namespace Descant.Package.Components
{
    [Serializable, MaxQuantity(Single.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class RelationshipChange :
        DescantNodeComponent, IInvokedDescantComponent
    {
        [ParameterGroup("First actor")] public string FirstActorName;
        
        [ParameterGroup("Second actor")] public string SecondActorName;
        
        [ParameterGroup("Operation to perform")] public OperationType OperationType;
        [ParameterGroup("Operation to perform")] public float OperationValue;

        public RelationshipChange(
            DescantConversationController controller,
            int nodeID,
            int id,
            string firstActorName, string secondActorName, OperationType operationType, float operationValue)
            : base(controller, nodeID, id)
        {
            FirstActorName = firstActorName;
            SecondActorName = secondActorName;
            OperationType = operationType;
            OperationValue = operationValue;
        }

        public void Invoke()
        {
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
        }
    }
}