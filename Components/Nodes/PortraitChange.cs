using System;

namespace DescantComponents
{
    public enum PortraitChangeType { Set, Enable, Disable }
    
    [Serializable, MaxQuantity(Single.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class PortraitChange : DescantNodeComponent
    {
        [Inline] public bool PlayerPortrait;
        
        [ParameterGroup("Change to perform")] public PortraitChangeType ChangeType;
        [ParameterGroup("Change to perform")] public string PortraitName;
        
        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            switch (ChangeType)
            {
                case PortraitChangeType.Set:
                    if (PlayerPortrait) result.PlayerPortrait = PortraitName;
                    else result.NPCPortrait = PortraitName;
                    break;
                
                case PortraitChangeType.Enable:
                    if (PlayerPortrait) result.PlayerPortraitEnabled = true;
                    else result.NPCPortraitEnabled = true;
                    break;
                
                case PortraitChangeType.Disable:
                    if (PlayerPortrait) result.PlayerPortraitEnabled = false;
                    else result.NPCPortraitEnabled = false;
                    break;
            }
            
            return result;
        }
    }
}