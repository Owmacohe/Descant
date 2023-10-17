using System;
using Editor.Nodes;

namespace Runtime.Components.Nodes
{
    public class Interruptable : DescantNodeComponent, IUpdatedDescantComponent
    {
        public bool ResumeAfter { get; }
        
        public Interruptable(DescantGraphController controller, int id, bool resumeAfter)
            : base(controller, id, DescantNodeType.Response, 1)
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