global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Runtime.CompilerServices;
global using System.Threading.Tasks;
global using OpenttdDiscord.Base.Ext;
global using OpenttdDiscord.Database.Extensions;
global using static LanguageExt.Prelude;
global using EitherAsyncUnit = LanguageExt.EitherAsync<OpenttdDiscord.Base.Ext.IError, LanguageExt.Unit>;
global using EitherUnit = LanguageExt.Either<OpenttdDiscord.Base.Ext.IError, LanguageExt.Unit>;
[assembly: InternalsVisibleTo("OpenttdDiscord.Database.Tests")]
[assembly: InternalsVisibleTo("OpenttdDiscord.Infrastructure")]
[assembly: InternalsVisibleTo("OpenttdDiscord.Database.Migrator")]
