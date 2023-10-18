using System;
using Editor.Nodes;

namespace Runtime.Components.Nodes
{
    public class TopicChange : DescantNodeComponent, IInvokedDescantComponent
    {
        public DescantActor Actor { get; }
        public string Topic { get; }
        public ListChangeType Type { get; }

        public TopicChange(DescantConversationController controller, int id, DescantActor actor, string topic, ListChangeType type)
            : base(controller, id, DescantNodeType.Response, float.PositiveInfinity)
        {
            Actor = actor;
            Topic = topic;
            Type = type;
        }

        public void Invoke()
        {
            switch (Type)
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