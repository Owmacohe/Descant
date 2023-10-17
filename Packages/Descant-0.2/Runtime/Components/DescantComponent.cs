namespace Runtime.Components
{
    public abstract class DescantComponent
    {
        public int ID { get; }
        public float MaxQuantity { get; }
        protected DescantGraphController Controller { get; }
        
        protected DescantComponent(DescantGraphController controller, int id, float max)
        {
            Controller = controller;
            ID = id;
            MaxQuantity = max;
        }
    }
}