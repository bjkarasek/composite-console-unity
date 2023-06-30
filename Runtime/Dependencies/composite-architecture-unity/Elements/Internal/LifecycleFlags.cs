using System;

namespace CompositeArchitecture
{
    [Flags]
    public enum LifecycleFlags
    {
        Empty = 0,

        Inject = 1 << 0,
        Initialize = 1 << 1,
        Activate = 1 << 2,
        EarlyRefresh = 1 << 3,
        Refresh = 1 << 4,
        LateRefresh = 1 << 5,
        FixedRefresh = 1 << 6,
        Deactivate = 1 << 7,
        Deinitialize = 1 << 8,

        All = Inject | Initialize | Activate | EarlyRefresh | Refresh | LateRefresh | FixedRefresh | Deactivate | Deinitialize,
        WithoutRefresh = Inject | Initialize | Activate | Deactivate | Deinitialize,
        WithoutActivate = Inject | Initialize | EarlyRefresh | Refresh | LateRefresh | FixedRefresh | Deactivate | Deinitialize 
    }
}