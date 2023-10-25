using System;
using Editor.Nodes;
using Runtime;

namespace Components
{
    [Serializable]
    public class Interruptable : DescantNodeComponent, IUpdatedDescantComponent, IResponseNodeComponent
    {
        public bool ResumeAfter;
        
        public Interruptable(
            DescantConversationController controller,
            int nodeID,
            int id,
            bool resumeAfter)
            : base(controller, nodeID, id, 1)
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