using System;

namespace DescantComponents
{
    [Serializable, MaxQuantity(1), NodeType(DescantNodeType.End)]
    public class EndingType : DescantNodeComponent, IInvokedDescantComponent
    {
        [Inline] public string Ending;
        
        public EndingType(
            //DescantConversationController controller,
            int nodeID,
            int id,
            string ending)
            : base(nodeID, id)
        {
            Ending = ending;
        }

        public void Invoke()
        {
            //Controller.Ending = Ending;
        }
    }
}