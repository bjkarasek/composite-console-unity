namespace CompositeArchitecture
{
    public class CompositeElementState : ICompositeElementState
    {
        public bool IsInstalled { get; set; }
        public bool IsInjected { get; set; }
        public bool IsInitialized { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeinitialized { get; set; }
    }
}