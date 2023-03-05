// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Migrations naming rules are not so important to error out whole project", Scope = "namespaceanddescendants", Target = "~N:OpenttdDiscord.Database.Migrations")]
[assembly: SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1413:Use trailing comma in multi-line initializers", Justification = "Migrations are violating this rule", Scope = "namespaceanddescendants", Target = "~N:OpenttdDiscord.Database.Migrations")]
