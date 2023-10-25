namespace Components
{
    public interface IInvokedDescantComponent
    {
        public void Invoke();
    }
    
    public interface IUpdatedDescantComponent
    {
        public void FixedUpdate();
    }

    public interface IChoiceNodeComponent { }
    public interface IResponseNodeComponent { }
    public interface IStartNodeComponent { }
    public interface IEndNodeComponent { }
}