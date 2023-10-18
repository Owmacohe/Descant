using System;
using UnityEngine;
using Editor.Nodes;

namespace Runtime.Components.Nodes
{
    public class TimedChoice : DescantNodeComponent, IUpdatedDescantComponent
    {
        public float Time { get; }
        public bool TimerVisible { get; }
        
        public TimedChoice(DescantConversationController controller, int id, float time, bool timerVisible)
            : base(controller, id, DescantNodeType.Choice, 1)
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