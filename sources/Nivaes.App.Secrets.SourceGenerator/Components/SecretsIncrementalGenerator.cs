using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

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
                var secres = new List<string>();
                StringBuilder sb = new StringBuilder();
                var n = 0;
                string readLine = "public static string {0} = SecretsSerializer.Read(_secres, {1});";
                if (!string.IsNullOrWhiteSpace(info.AppDns))
                {
                    secres.Add(info.AppDns);
                    sb.AppendLine(string.Format(readLine, "AppDns", n++));
                }

                if (!string.IsNullOrWhiteSpace(info.SentryDns))
                {
                    secres.Add(info.SentryDns);
                    sb.AppendLine(string.Format(readLine, "SentryDns", n++));
                }

                string secresSource = $@"new Lazy<string[]>(() => SecretsSerializer.Deserialize(new byte[] {{ {string.Join(",", SecretsSerializationHelper.Serializer(secres))} }}).ToArray())";
                var source = $@"
                    using Nivaes.App.Secrets;
                    namespace {info.AssemblyName}
                    {{
                        public static class Secrets
                        {{
                            private static Lazy<string[]> _secres = {secresSource};
                            {sb.ToString()}
                        }}
                    }}
                    ";

                spc.AddSource("Secrets.g.cs", Microsoft.CodeAnalysis.Text.SourceText.From(source, Encoding.UTF8));
            }
        });
    }
}
