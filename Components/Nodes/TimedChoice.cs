using System;

namespace DescantComponents
{
    [Serializable, MaxQuantity(1), NodeType(DescantNodeType.Choice)]
    public class TimedChoice : DescantNodeComponent, IUpdatedDescantComponent
    {
        [Inline] public float Time;
        [Inline] public bool TimerVisible;
        
        public TimedChoice(
            //DescantConversationController controller,
            int nodeID,
            int id,
            float time, bool timerVisible)
            : base(nodeID, id)
        {
            Time = time;
            TimerVisible = timerVisible;
        }

        public void FixedUpdate()
        {
            // TODO: update timer using
            // (UnityEngine.Time.time - Controller.CurrentStartTime) / Time
            // TimerVisible
        }
    }
}