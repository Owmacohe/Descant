using System;
using Editor.Nodes;
using Runtime;

namespace Components
{
    [Serializable]
    public class TimedChoice : DescantNodeComponent, IUpdatedDescantComponent, IChoiceNodeComponent
    {
        public float Time;
        public bool TimerVisible;
        
        public TimedChoice(
            DescantConversationController controller,
            int nodeID,
            int id,
            float time, bool timerVisible)
            : base(controller, nodeID, id, 1)
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