namespace Descant.Package.Components
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