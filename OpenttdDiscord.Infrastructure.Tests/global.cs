global using AutoFixture;
global using LanguageExt;
global using NSubstitute;
global using OpenttdDiscord.Base.Ext;
global using Xunit;
global using static LanguageExt.Prelude;
global using EitherAsyncUnit = LanguageExt.EitherAsync<OpenttdDiscord.Base.Ext.IError, LanguageExt.Unit>;
global using EitherUnit = LanguageExt.Either<OpenttdDiscord.Base.Ext.IError, LanguageExt.Unit>;

#if NO_PARALLEL
[assembly: CollectionBehavior(DisableTestParallelization = true)]
#endif