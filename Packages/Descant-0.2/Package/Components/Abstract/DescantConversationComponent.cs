using System;
using Descant.Package.Runtime;

namespace Descant.Package.Components
{
    [Serializable]
    public abstract class DescantConversationComponent : DescantComponent
    {
        protected DescantConversationComponent(DescantConversationController controller, int id)
            : base(controller, id)
        {
            
        }
    }
}