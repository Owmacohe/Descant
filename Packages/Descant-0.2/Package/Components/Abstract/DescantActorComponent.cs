using System;
using Descant.Package.Runtime;

namespace Descant.Package.Components
{
    [Serializable]
    public abstract class DescantActorComponent : DescantComponent
    {
        DescantActorType Type;
        
        protected DescantActorComponent(DescantConversationController controller, int id, DescantActorType type)
            : base(controller, id)
        {
            Type = type;
        }
    }
}