using System;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Nivaes.App.Secrets.SourceGenerator;

[Generator]
public class SecretsIncrementalGenerator
    : IIncrementalGenerator
{
    public struct Data
    {
        public string AssemblyName;
        public string? AppDns;
        public string? SentryDns;
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
#if DEBUG
        System.Diagnostics.Debugger.Launch();
#endif

        var appNameProvider = context.AnalyzerConfigOptionsProvider
            .Select((optionsProvider, _) =>
            {
                optionsProvider.GlobalOptions.TryGetValue("build_property.AssemblyName", out var assemblyName);
                
                optionsProvider.GlobalOptions.TryGetValue("build_property.AppDns", out var buildAppDns);
                optionsProvider.GlobalOptions.TryGetValue("build_property.SentryDns", out var buildSentryDns);

                var appDnsEnvironment = Environment.GetEnvironmentVariable("APP_DNS");
                var sentryDnsEnvironment = Environment.GetEnvironmentVariable("SENTRY_DNS");

                return new Data
                {
                    AssemblyName = assemblyName ?? "Nivaes.App.Secretrs",
                    AppDns = appDnsEnvironment ?? buildAppDns,
                    SentryDns = sentryDnsEnvironment ?? buildSentryDns
                };
            });

        context.RegisterSourceOutput(appNameProvider, (SourceProductionContext spc, Data info) =>
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
