using System;
using Runtime;

namespace Components
{
    [Serializable]
    public abstract class DescantActorComponent : DescantComponent
    {
        DescantActorType Type;
        
        protected DescantActorComponent(DescantConversationController controller, int id, DescantActorType type, float max)
            : base(controller, id, max)
        {
            Type = type;
        }
    }
}