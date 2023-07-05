namespace CompositeConsole
{
    public interface IDebugBehaviourNamed : IDebugBehaviour
    {
        string DebugHierarchyName { get; }
    }
}