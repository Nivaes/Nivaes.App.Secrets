using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace Nivaes.App.Secrets.SourceGenerator;

[Generator]
public class SecretsIncrementalGenerator
    : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
//#if DEBUG
//        System.Diagnostics.Debugger.Launch();
//#endif

        var appNameProvider = context.AnalyzerConfigOptionsProvider
        .Select((optionsProvider, _) =>
        {
            optionsProvider.GlobalOptions.TryGetValue("dotnet_diagnostic.MyAppName", out var value);
            //optionsProvider.GlobalOptions.TryGetValue("build_property.MyAppName", out var value);
            return value ?? "DefaultApp";
        });

        context.RegisterSourceOutput(appNameProvider, (spc, appName) =>
        {
            var source = $@"
                    namespace Generated
                    {{
                    public static class AppInfo
                    {{
                        public const string Name = ""{appName}"";
                    }}
                    }}
                    ";

            var aa = Microsoft.CodeAnalysis.Text.SourceText.From(source, Encoding.UTF8);
            spc.AddSource("AppInfo.g.cs", Microsoft.CodeAnalysis.Text.SourceText.From(source, Encoding.UTF8));
        });
    }
}
