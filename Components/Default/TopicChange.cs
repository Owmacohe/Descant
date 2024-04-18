﻿// Please see https://omch.tech/descant/#topicchange for documentation

using System;
using Descant.Utilities;

namespace Descant.Components
{
    [Serializable, MaxQuantity(Single.PositiveInfinity), NodeType(DescantNodeType.Response)]
    public class TopicChange : DescantComponent
    {
        [Inline] public string ActorName;
        
        [ParameterGroup("Topic to change")] public string TopicName;
        
        [ParameterGroup("Change to perform")] public ListChangeType ChangeType;

        public override DescantNodeInvokeResult Invoke(DescantNodeInvokeResult result)
        {
            DescantActor actor = DescantComponentUtilities.GetActor(this, result.Actors, ActorName);

            if (actor == null) return result;

            switch (ChangeType)
            {
                case ListChangeType.Add:
                    if (!actor.Topics.Contains(TopicName)) actor.Topics.Add(TopicName);
                    break;
                case ListChangeType.Remove:
                    if (actor.Topics.Contains(TopicName)) actor.Topics.Remove(TopicName);
                    break;
            }
            
            return result;
        }
    }
}