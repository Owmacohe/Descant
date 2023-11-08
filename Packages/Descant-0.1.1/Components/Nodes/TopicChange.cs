using System;

namespace DescantComponents
{
    [Serializable, MaxQuantity(Single.PositiveInfinity), NodeType(DescantNodeType.Response)]
    public class TopicChange : DescantNodeComponent, IInvokedDescantComponent
    {
        [Inline] public string ActorName;
        
        [ParameterGroup("Topic to change")] public string TopicName;
        
        [ParameterGroup("Change to perform")] public ListChangeType ChangeType;

        public TopicChange(
            //DescantConversationController controller,
            int nodeID,
            int id,
            string actorName, string topicName, ListChangeType changeType)
            : base(nodeID, id)
        {
            ActorName = actorName;
            TopicName = topicName;
            ChangeType = changeType;
        }

        public void Invoke()
        {
            /*
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
            */
        }
    }
}