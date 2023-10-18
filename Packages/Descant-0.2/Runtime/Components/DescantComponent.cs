namespace Runtime.Components
{
    public abstract class DescantComponent
    {
        public int ID { get; }
        public float MaxQuantity { get; }
        protected DescantConversationController Controller { get; }
        
        protected DescantComponent(DescantConversationController controller, int id, float max)
        {
            ID = id;
            MaxQuantity = max;
            Controller = controller;
        }
    }
}