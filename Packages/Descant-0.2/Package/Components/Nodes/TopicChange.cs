using System;
using Descant.Package.Editor.Nodes;
using Descant.Package.Runtime;
using UnityEngine.Serialization;

namespace Descant.Package.Components
{
    [Serializable, MaxQuantity(Single.PositiveInfinity), NodeType(DescantNodeType.Response)]
    public class TopicChange : DescantNodeComponent, IInvokedDescantComponent
    {
        [InlineGroup(0)] public string ActorName;
        [InlineGroup(1)] public string TopicName;
        [InlineGroup(1)] public ListChangeType ChangeType;

        public TopicChange(
            DescantConversationController controller,
            int nodeID,
            int id,
            string actorName, string topicName, ListChangeType changeType)
            : base(controller, nodeID, id)
        {
            ActorName = actorName;
            TopicName = topicName;
            ChangeType = changeType;
        }

        public void Invoke()
        {
            DescantActor actor = Controller.GetActor(ActorName);
            
            switch (ChangeType)
            {
                case ListChangeType.Add:
                    actor.SetTopic(TopicName, true);
                    break;
                
                case ListChangeType.Remove:
                    actor.SetTopic(TopicName, false);
                    break;
            }
        }
    }
}