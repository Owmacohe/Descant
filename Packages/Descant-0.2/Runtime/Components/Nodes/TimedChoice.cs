using System;
using Editor.Nodes;

namespace Runtime.Components.Nodes
{
    public class TimedChoice : DescantNodeComponent
    {
        public float Time { get; }
        public bool TimerVisible { get; }
        
        public TimedChoice(DescantGraphController controller, int id, float time, bool timerVisible)
            : base(controller, id, DescantNodeType.Choice, 1)
        {
            Time = time;
            TimerVisible = timerVisible;
        }

        public override void Invoke()
        {
            throw new NotImplementedException();
        }
    }
}