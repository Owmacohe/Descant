using System;
using UnityEngine.Events;
using Editor.Nodes;
using Runtime;

namespace Components
{
    [Serializable]
    public class Event :
        DescantNodeComponent, IInvokedDescantComponent,
        IChoiceNodeComponent, IResponseNodeComponent, IStartNodeComponent, IEndNodeComponent
    {
        public UnityEvent UnityEvent;
        
        public Event(
            DescantConversationController controller,
            int nodeID,
            int id,
            UnityEvent unityEvent)
            : base(controller, nodeID, id, float.PositiveInfinity)
        {
            UnityEvent = unityEvent;
        }

        public void Invoke()
        {
            UnityEvent.Invoke();
        }
    }
}