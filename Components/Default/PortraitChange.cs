// Please see https://omch.tech/descant/#portraitchange for documentation

using System;
using Descant.Utilities;

namespace Descant.Components
{
    [Serializable, MaxQuantity(Single.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class PortraitChange : DescantComponent
    {
        [Inline] public DescantActor Actor;
        
        [ParameterGroup("Change to perform")] public PortraitChangeType ChangeType;
        [ParameterGroup("Change to perform")] public int PortraitIndex;
        
        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            switch (ChangeType)
            {
                case PortraitChangeType.Set:
                    Actor.Portrait = Actor.Portraits[PortraitIndex];
                    break;
                
                case PortraitChangeType.Enable:
                    Actor.PortraitEnabled = true;
                    break;
                
                case PortraitChangeType.Disable:
                    Actor.PortraitEnabled = false;
                    break;
            }
            
            return result;
        }
    }
}