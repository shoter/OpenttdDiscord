using Xunit;
#if LINUX
    [assembly: CollectionBehavior(DisableTestParallelization = true)]
#else
    [assembly: CollectionBehavior(DisableTestParallelization = false)]
#endif