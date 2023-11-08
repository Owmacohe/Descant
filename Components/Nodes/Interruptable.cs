using System;

namespace DescantComponents
{
    [Serializable, MaxQuantity(1), NodeType(DescantNodeType.Response)]
    public class Interruptable : DescantNodeComponent, IUpdatedDescantComponent
    {
        [Inline] public bool ResumeAfter;
        
        public Interruptable(
            //DescantConversationController controller,
            int nodeID,
            int id,
            bool resumeAfter)
            : base(nodeID, id)
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