using System;
using Descant.Package.Editor.Nodes;
using Descant.Package.Runtime;

namespace Descant.Package.Components
{
    [Serializable, MaxQuantity(1), NodeType(DescantNodeType.Response)]
    public class Interruptable : DescantNodeComponent, IUpdatedDescantComponent
    {
        [Inline] public bool ResumeAfter;
        
        public Interruptable(
            DescantConversationController controller,
            int nodeID,
            int id,
            bool resumeAfter)
            : base(controller, nodeID, id)
        {
            ResumeAfter = resumeAfter;
        }
        
        public void FixedUpdate()
        {
            // TODO: check for interruptions
            // TODO: check for resumption using
            // ResumeAfter
        }
    }
}