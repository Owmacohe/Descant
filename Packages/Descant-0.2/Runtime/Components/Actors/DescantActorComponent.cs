namespace Runtime.Components.Actors
{
    public abstract class DescantActorComponent : DescantComponent
    {
        DescantActorType Type { get; }
        
        protected DescantActorComponent(DescantGraphController controller, int id, DescantActorType type, float max)
            : base(controller, id, max)
        {
            Type = type;
        }
    }
}