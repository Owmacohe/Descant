using System;
using System.Collections.Generic;
using DescantUtilities;

namespace DescantComponents
{
    [Serializable, MaxQuantity(1), NodeType(DescantNodeType.End)]
    public class EndingType : DescantNodeComponent
    {
        [Inline] public string Ending;

        public override List<string> Invoke(List<string> choices)
        {
            //Controller.Ending = Ending;
            return choices;
        }

        public override void FixedUpdate()
        {
            
        }
    }
}