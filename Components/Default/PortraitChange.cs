// Please see https://omch.tech/descant/#portraitchange for documentation

using System;
using Descant.Utilities;

namespace Descant.Components
{
    [Serializable, MaxQuantity(Single.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class PortraitChange : DescantComponent
    {
        [Inline] public string ActorName;
        
        [ParameterGroup("Change to perform")] public PortraitChangeType ChangeType;
        [ParameterGroup("Change to perform")] public int PortraitIndex;
        
        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            DescantActor actor = DescantComponentUtilities.GetActor(this, result.Actors, ActorName);

            if (actor == null) return result;
            
            switch (ChangeType)
            {
                case PortraitChangeType.Set:
                    actor.Portrait = actor.Portraits[PortraitIndex];
                    break;
                
                case PortraitChangeType.Enable:
                    actor.PortraitEnabled = true;
                    break;
                
                case PortraitChangeType.Disable:
                    actor.PortraitEnabled = false;
                    break;
            }
            
            return result;
        }
    }
}