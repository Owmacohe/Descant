// Please see https://omch.tech/descant/#topicchange for documentation

using System;
using Descant.Utilities;

namespace Descant.Components
{
    [Serializable, MaxQuantity(Single.PositiveInfinity), NodeType(DescantNodeType.Response)]
    public class TopicChange : DescantComponent
    {
        [Inline] public DescantActor Actor;
        
        [ParameterGroup("Topic to change")] public string TopicName;
        
        [ParameterGroup("Change to perform")] public ListChangeType ChangeType;

        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            switch (ChangeType)
            {
                case ListChangeType.Add:
                    if (!Actor.Topics.Contains(TopicName)) Actor.Topics.Add(TopicName);
                    break;
                case ListChangeType.Remove:
                    if (Actor.Topics.Contains(TopicName)) Actor.Topics.Remove(TopicName);
                    break;
            }
            
            return result;
        }
    }
}