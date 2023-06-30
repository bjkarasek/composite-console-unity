namespace CompositeArchitecture
{
    public interface ICompositeElementState
    {
        bool IsInstalled { get; }
        bool IsInjected { get; }
        bool IsInitialized { get; }
        bool IsActive { get; }
        bool IsDeinitialized { get; }
    }
}