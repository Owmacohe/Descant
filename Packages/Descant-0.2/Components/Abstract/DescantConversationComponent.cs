using System;
using Runtime;

namespace Components
{
    [Serializable]
    public abstract class DescantConversationComponent : DescantComponent
    {
        protected DescantConversationComponent(DescantConversationController controller, int id, float max)
            : base(controller, id, max)
        {
            
        }
    }
}