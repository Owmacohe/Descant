using System;

namespace DescantComponents
{
    [Serializable, MaxQuantity(Single.PositiveInfinity), NodeType(DescantNodeType.Response)]
    public class TopicChange : DescantNodeComponent
    {
        [Inline] public string ActorName;
        
        [ParameterGroup("Topic to change")] public string TopicName;
        
        [ParameterGroup("Change to perform")] public ListChangeType ChangeType;

        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
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
            
            return result;
        }
    }
}