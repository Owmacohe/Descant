using UnityEngine.Events;
using Editor.Nodes;

namespace Runtime.Components.Nodes
{
    public class Event : DescantNodeComponent, IDescantComponentInvokable
    {
        public UnityEvent UnityEvent { get; }
        
        public Event(DescantGraphController controller, int id, UnityEvent unityEvent)
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