using System;
using Editor.Nodes;
using Runtime;

namespace Components
{
    [Serializable]
    public class TopicChange : DescantNodeComponent, IInvokedDescantComponent, IResponseNodeComponent
    {
        public DescantActor Actor;
        public string Topic;
        public ListChangeType ChangeType;

        public TopicChange(
            DescantConversationController controller,
            int nodeID,
            int id,
            DescantActor actor, string topic, ListChangeType changeType)
            : base(controller, nodeID, id, float.PositiveInfinity)
        {
            Actor = actor;
            Topic = topic;
            ChangeType = changeType;
        }

        public void Invoke()
        {
            switch (ChangeType)
            {
                case ListChangeType.Add:
                    Actor.SetTopic(Topic, true);
                    break;
                
                case ListChangeType.Remove:
                    Actor.SetTopic(Topic, false);
                    break;
            }
        }
    }
}