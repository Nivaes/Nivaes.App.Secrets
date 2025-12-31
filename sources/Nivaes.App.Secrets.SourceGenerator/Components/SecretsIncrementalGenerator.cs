using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Nivaes.App.Secrets.SourceGenerator;

[Generator]
public class SecretsIncrementalGenerator
    : IIncrementalGenerator
{
    record struct Data
    {
        public string AssemblyName;
        public string? AppDns;
        public string? SentryDns;
    }

    public SecretsIncrementalGenerator()
    { }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
//#if DEBUG
//        System.Diagnostics.Debugger.Launch();
//#endif

        var compilationOnce = context.CompilationProvider
            .Combine(context.AnalyzerConfigOptionsProvider)
        .Select((pair, _) =>
        {
            (Compilation? compilation, AnalyzerConfigOptionsProvider? optionsProvider) = pair;
            var tree = compilation.SyntaxTrees.First();

            var optionsProviderTree = optionsProvider.GetOptions(tree);

            optionsProvider.GlobalOptions.TryGetValue("build_property.AssemblyName", out var assemblyName);

            optionsProvider.GlobalOptions.TryGetValue("build_property.AppDns", out var buildAppDns);
            optionsProvider.GlobalOptions.TryGetValue("build_property.SentryDns", out var buildSentryDns);

            optionsProviderTree.TryGetValue("secret_app_dns", out var _editorConfigAppDns);
            optionsProviderTree.TryGetValue("secret_sentry_dns", out var _editorConfigSentryDns);

            var appDnsEnvironment = Environment.GetEnvironmentVariable("APP_DNS");
            var sentryDnsEnvironment = Environment.GetEnvironmentVariable("SENTRY_DNS");

            return new Data
            {
                AssemblyName = assemblyName ?? "Nivaes.App.Secretrs",
                AppDns = appDnsEnvironment ?? _editorConfigAppDns ?? buildAppDns,
                SentryDns = sentryDnsEnvironment ?? _editorConfigSentryDns ?? buildSentryDns
            };
        });
       
        context.RegisterSourceOutput(compilationOnce, (SourceProductionContext spc, Data info) =>
        {
            if (!string.IsNullOrWhiteSpace(info.AppDns) || !string.IsNullOrWhiteSpace(info.SentryDns))
            {
                StringBuilder sb = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(info.AppDns))
                    sb.Append($@"public const string AppDns = ""{info.AppDns}"";");

                if (!string.IsNullOrWhiteSpace(info.SentryDns))
                    sb.Append($@"public const string SentryDns = ""{info.SentryDns}"";");

                var source = $@"
                    namespace {info.AssemblyName}
                    {{
                        public static class Secrets
                        {{
                            {sb.ToString()}
                        }}
                    }}
                    ";

                spc.AddSource("Secrets.g.cs", Microsoft.CodeAnalysis.Text.SourceText.From(source, Encoding.UTF8));
            }
        });
    }
}
