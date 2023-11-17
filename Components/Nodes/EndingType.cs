using System;

namespace DescantComponents
{
    [Serializable, MaxQuantity(1), NodeType(DescantNodeType.End)]
    public class EndingType : DescantNodeComponent
    {
        [Inline] public string Ending;

        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            //Controller.Ending = Ending;
            return result;
        }
    }
}