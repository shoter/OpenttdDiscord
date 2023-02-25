global using OpenttdDiscord.Base.Ext;
global using EitherUnit = LanguageExt.Either<OpenttdDiscord.Base.Ext.IError, LanguageExt.Unit>;
global using EitherAsyncUnit = LanguageExt.EitherAsync<OpenttdDiscord.Base.Ext.IError, LanguageExt.Unit>;
global using System;
global using static LanguageExt.Prelude;
global using System.Collections.Generic;
global using System.Linq;
global using System.Runtime.CompilerServices;
global using System.Threading.Tasks;

[assembly: InternalsVisibleTo("OpenttdDiscord.Database.Tests")]
[assembly: InternalsVisibleTo("OpenttdDiscord.Infrastructure")]
