using System;
using Editor.Nodes;
using Runtime;

namespace Components
{
    [Serializable]
    public class EndingType : DescantNodeComponent, IInvokedDescantComponent, IEndNodeComponent
    {
        public string Ending;
        
        public EndingType(
            DescantConversationController controller,
            int nodeID,
            int id,
            string ending)
            : base(controller, nodeID, id, 1)
        {
            Ending = ending;
        }

        public void Invoke()
        {
            Controller.Ending = Ending;
        }
    }
}