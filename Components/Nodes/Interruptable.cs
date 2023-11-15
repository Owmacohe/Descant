using System;
using System.Collections.Generic;
using DescantUtilities;

namespace DescantComponents
{
    [Serializable, MaxQuantity(1), NodeType(DescantNodeType.Response)]
    public class Interruptable : DescantNodeComponent
    {
        [Inline] public bool ResumeAfter;

        public override List<string> Invoke(List<string> choices)
        {
            return choices;
        }

        public override void FixedUpdate()
        {
            // TODO: check for interruptions
            // TODO: check for resumption using
            // ResumeAfter
        }
    }
}