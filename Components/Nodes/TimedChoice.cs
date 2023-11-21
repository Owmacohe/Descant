using System;

namespace DescantComponents
{
    [Serializable, MaxQuantity(1), NodeType(DescantNodeType.Choice)]
    public class TimedChoice : DescantNodeComponent
    {
        [Inline] public float Time;
        [Inline] public bool TimerVisible;

        public override bool FixedUpdate()
        {
            // TODO: update timer using
            // (UnityEngine.Time.time - Controller.CurrentStartTime) / Time
            // TimerVisible

            return true;
        }
    }
}