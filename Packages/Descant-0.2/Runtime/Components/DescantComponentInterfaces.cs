namespace Runtime.Components
{
    public interface IInvokedDescantComponent
    {
        public void Invoke();
    }
    
    public interface IUpdatedDescantComponent
    {
        public void FixedUpdate();
    }
}