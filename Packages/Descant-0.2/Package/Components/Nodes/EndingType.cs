using System;
using Descant.Package.Editor.Nodes;
using Descant.Package.Runtime;

namespace Descant.Package.Components
{
    [Serializable, MaxQuantity(1), NodeType(DescantNodeType.End)]
    public class EndingType : DescantNodeComponent, IInvokedDescantComponent
    {
        [InlineGroup(0)] public string Ending;
        
        public EndingType(
            DescantConversationController controller,
            int nodeID,
            int id,
            string ending)
            : base(controller, nodeID, id)
        {
            Ending = ending;
        }

        public void Invoke()
        {
            Controller.Ending = Ending;
        }
    }
}