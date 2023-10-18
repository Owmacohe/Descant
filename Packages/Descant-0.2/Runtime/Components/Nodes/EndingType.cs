using System;
using Editor.Nodes;

namespace Runtime.Components.Nodes
{
    public class EndingType : DescantNodeComponent, IInvokedDescantComponent
    {
        public string Ending { get; }
        
        public EndingType(DescantConversationController controller, int id, string ending)
            : base(controller, id, DescantNodeType.End, 1)
        {
            Ending = ending;
        }

        public void Invoke()
        {
            Controller.Ending = Ending;
        }
    }
}