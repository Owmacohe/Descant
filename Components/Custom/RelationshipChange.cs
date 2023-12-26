// Please see https://omch.tech/descant/#relationshipchange for documentation

using System;

namespace DescantComponents
{
    [Serializable, MaxQuantity(Single.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class RelationshipChange : DescantComponent
    {
        [ParameterGroup("Actors")] public string FirstActorName;
        [ParameterGroup("Actors")] public string SecondActorName;
        
        [ParameterGroup("Operation to perform")] public OperationType OperationType;
        [ParameterGroup("Operation to perform")] public float OperationValue;

        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            DescantActor actor = DescantComponentUtilities.GetActor(this, result.Actors, FirstActorName);

            if (actor == null) return result;

            if (!actor.RelationshipKeys.Contains(SecondActorName))
            {
                actor.RelationshipKeys.Add(SecondActorName);
                actor.RelationshipValues.Add(0);
            }
            
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