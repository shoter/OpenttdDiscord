global using AutoFixture;
using Xunit;

#if NO_PARALLEL
[assembly: CollectionBehavior(DisableTestParallelization = true)]
#endif