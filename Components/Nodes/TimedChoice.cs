using System;
using System.Collections.Generic;
using DescantUtilities;

namespace DescantComponents
{
    [Serializable, MaxQuantity(1), NodeType(DescantNodeType.Choice)]
    public class TimedChoice : DescantNodeComponent
    {
        [Inline] public float Time;
        [Inline] public bool TimerVisible;

        public override List<string> Invoke(List<string> choices)
        {
            return choices;
        }

        public override void FixedUpdate()
        {
            // TODO: update timer using
            // (UnityEngine.Time.time - Controller.CurrentStartTime) / Time
            // TimerVisible
        }
    }
}