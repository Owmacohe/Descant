using System;
using UnityEngine.Events;
using Descant.Package.Editor.Nodes;
using Descant.Package.Runtime;

namespace Descant.Package.Components
{
    [Serializable, MaxQuantity(float.PositiveInfinity), NodeType(DescantNodeType.Any)]
    public class Event :
        DescantNodeComponent, IInvokedDescantComponent
    {
        public UnityEvent UnityEvent; // TODO: find a way to visualize this
        
        public Event(
            DescantConversationController controller,
            int nodeID,
            int id,
            UnityEvent unityEvent)
            : base(controller, nodeID, id)
        {
            UnityEvent = unityEvent;
        }

        public void Invoke()
        {
            UnityEvent.Invoke();
        }
    }
}