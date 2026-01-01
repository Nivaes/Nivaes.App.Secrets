using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Shouldly;
using Xunit;


namespace Nivaes.App.Secrets.SourceGenerator.UnitTest;

public class BasicContainerTest
{
    [Fact]
    public async Task Compiles_without_errorsOld()
    {
        var project = TestProject.Project;

        var newProject = await project.ApplySecretsGenerator();

        var compilation = await newProject.GetCompilationAsync();
        compilation.ShouldNotBeNull();
        var errors = compilation.GetDiagnostics()
            .Where(o => o.Severity == DiagnosticSeverity.Error)
            .ToArray();

        Assert.False(errors.Any(), errors.Select(o => o.GetMessage()).JoinWithNewLine());
    }

    [Fact]
    public async Task Compiles_without_errors()
    {
        var test = new CSharpSourceGeneratorTest<SecretsIncrementalGenerator, DefaultVerifier>()
        {
            TestCode = @"
                public partial class TestClass
                {
                }
                ",
        };

        await test.RunAsync();
    }

    [Fact]
    public async Task Can_resolve_simple_singleton_Old()
    {
        var project = await TestProject.Project.ApplyToProgram(@"
                public partial class TestClass
                {
                }
        ");
        var newProject = await project.ApplySecretsGenerator();

        var compilation = await newProject.GetCompilationAsync();
        compilation.ShouldNotBeNull();
        var errors = compilation.GetDiagnostics()
           .Where(o => o.Severity == DiagnosticSeverity.Error)
           .ToArray();

        Assert.False(errors.Any(), errors.Select(o => o.GetMessage()).JoinWithNewLine());

    }

    //[Fact]
    //public async Task Can_resolve_simple_singleton()
    //{
    //    var test = new CSharpSourceGeneratorTest<SecretsIncrementalGenerator, DefaultVerifier>()
    //    {
    //        TestCode = @"
    //            using Nivaes.App.Secrets;
    //            public partial class TestClass
    //            {
    //            }
    //            ",
    //        TestState =
    //        {
    //            Sources = { "public partial class TestClass\r\n{\r\n}\r\n "},
    //            AnalyzerConfigFiles =  {
    //            ("/.editorconfig", """
    //                root = true

    //                [*]
    //                build_property.AssemblyName = Nivaes.App1
    //                secret_app_dns = http://test
    //                secret_sentry_dns = http://sentry.test
    //                """)
    //            },
    //        },
    //    };
    //    test.ReferenceAssemblies = ReferenceAssemblies.Net.Net90;
    //    //test.ReferenceAssemblies = test.ReferenceAssemblies.AddAssemblies(ImmutableArray.Create(nameof(SecretsSerializer)));
    //    test.TestState.GeneratedSources.Add((typeof(SecretsIncrementalGenerator), "Secrets.g.cs", """

    //                        using Nivaes.App.Secrets;
    //                        namespace Nivaes.App.Secretrs
    //                        {
    //                            public static class Secrets
    //                            {
    //                                private static Lazy<string[]> _secres = new Lazy<string[]>(() => SecretsSerializer.Deserialize(new byte[] { 2,0,0,0,11,104,116,116,112,58,47,47,116,101,115,116,18,104,116,116,112,58,47,47,115,101,110,116,114,121,46,116,101,115,116,0,0 }).ToArray());
    //                                public static string AppDns = SecretsSerializer.Read(_secres, 0);
    //    public static string SentryDns = SecretsSerializer.Read(_secres, 1);

    //                            }
    //                        }
                            
    //    """));
    //    test.TestState.AdditionalReferences.Add(typeof(System.String).Assembly);
    //    test.TestState.AdditionalReferences.Add(typeof(Lazy<>).Assembly);
    //    test.TestState.AdditionalReferences.Add(typeof(IEnumerable<>).Assembly);
    //    test.TestState.AdditionalReferences.Add(typeof(SecretsSerializer).Assembly);

    //});

    //    await test.RunAsync();
    //}
}
