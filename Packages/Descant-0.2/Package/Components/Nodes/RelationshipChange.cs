using System;
using Descant.Package.Editor.Nodes;
using Descant.Package.Runtime;
using UnityEngine.Serialization;

namespace Descant.Package.Components
{
    [Serializable, MaxQuantity(Single.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class RelationshipChange :
        DescantNodeComponent, IInvokedDescantComponent
    {
        [InlineGroup(1)] public string ActorName;
        [InlineGroup(1)] public string OtherActorName;
        [InlineGroup(2)] public OperationType OperationType;
        [InlineGroup(2)] public float OperationValue;

        public RelationshipChange(
            DescantConversationController controller,
            int nodeID,
            int id,
            string actorName, string otherActorName, OperationType operationType, float operationValue)
            : base(controller, nodeID, id)
        {
            ActorName = actorName;
            OtherActorName = otherActorName;
            OperationType = operationType;
            OperationValue = operationValue;
        }

        public void Invoke()
        {
            DescantActor actor = Controller.GetActor(ActorName);
            
            switch (OperationType)
            {
                case OperationType.IncreaseBy:
                    actor.SetRelationship(OtherActorName, actor.Relationships[OtherActorName] + OperationValue);
                    break;
                
                case OperationType.DecreaseBy:
                    actor.SetRelationship(OtherActorName, actor.Relationships[OtherActorName] - OperationValue);
                    break;
                
                case OperationType.Set:
                    actor.SetRelationship(OtherActorName, OperationValue);
                    break;
            }
        }
    }
}