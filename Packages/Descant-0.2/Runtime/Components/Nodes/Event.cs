using UnityEngine.Events;
using Editor.Nodes;

namespace Runtime.Components.Nodes
{
    public class Event : DescantNodeComponent, IInvokedDescantComponent
    {
        public UnityEvent UnityEvent { get; }
        
        public Event(DescantConversationController controller, int id, UnityEvent unityEvent)
            : base(controller, id, DescantNodeType.Any, float.PositiveInfinity)
        {
            UnityEvent = unityEvent;
        }

        public void Invoke()
        {
            UnityEvent.Invoke();
        }
    }
}