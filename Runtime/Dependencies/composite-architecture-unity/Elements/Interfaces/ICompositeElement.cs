namespace CompositeArchitecture
{
    public interface ICompositeElement : ILifecycle
    {
        ICompositeElementState State { get; }
    }
}