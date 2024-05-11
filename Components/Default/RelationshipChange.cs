// Please see https://omch.tech/descant/#relationshipchange for documentation

using System;
using System.Linq;
using Descant.Utilities;

namespace Descant.Components
{
    [Serializable, MaxQuantity(Single.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class RelationshipChange : DescantComponent
    {
        [ParameterGroup("Actors")] public DescantActor FirstActor;
        [ParameterGroup("Actors")] public DescantActor SecondActor;
        
        [ParameterGroup("Operation to perform")] public OperationType OperationType;
        [ParameterGroup("Operation to perform")] public float OperationValue;

        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            if (!FirstActor.Relationships.Keys.Contains(SecondActor.name))
                FirstActor.Relationships.Add(SecondActor.name, 0);
            
            switch (OperationType)
            {
                case OperationType.IncreaseBy:
                    FirstActor.Relationships[SecondActor.name] += OperationValue;
                    break;
                
                case OperationType.DecreaseBy:
                    FirstActor.Relationships[SecondActor.name] -= OperationValue;
                    break;
                
                case OperationType.Set:
                    FirstActor.Relationships[SecondActor.name] = OperationValue;
                    break;
            }

            return result;
        }
    }
}