# Nivaes.App.Secrets
Nivaes App Secrets

## Funcionalidad 

Genera variables de entorno dentro de la app, en función de ciertos valores.

### Variable de entorno

Recoge las variables de entorno APP_DNS y SENTRY_DNS.


Se puede definir un fichero **launchSettings.json**, pero esto solo funcionara al ejecutar el proyecto.


```json
{
  "profiles": {
    "Nivaes.App.Secrets": {
      "commandName": "Project",
      "environmentVariables": {
        "SENTRY_DNS": "http://localhost:8900",
        "APP_DNS": "http://localhost:9100"
      }
    }
  }
}
```

### Fichero .editorconfig

```ini
[*.cs]
secret_app_dns = https://localhost/
secret_sentry_dns = https://sentry/
```

### Fichero csprog

```xml
<PropertyGroup>
    <AppDns>http://localhost:8000</AppDns>
    <SentryDns>http://sentry</SentryDns>
  </PropertyGroup>

  <ItemGroup>
    <CompilerVisibleProperty Include="AssemblyName" />
    <CompilerVisibleProperty Include="AppDns" />
    <CompilerVisibleProperty Include="SentryDns" />
  </ItemGroup>
```


## Componentes

### Nivaes.App.Secrets.SourceGenerator

Generador de codigo

### Nivaes.App.Secrets

Librerias compartidas


## Integration

[![CI](https://github.com/Nivaes/Nivaes.App.Secrets/actions/workflows/ci.yaml/badge.svg)](https://github.com/Nivaes/Nivaes.App.Secrets/actions/workflows/ci.yaml)

[![Build Release](https://github.com/Nivaes/Nivaes.App.Secrets/actions/workflows/build_release.yaml/badge.svg)](https://github.com/Nivaes/Nivaes.App.Secrets/actions/workflows/build_release.yaml)

[![Publish Release](https://github.com/Nivaes/Nivaes.App.Secrets/actions/workflows/publish_release.yaml/badge.svg)](https://github.com/Nivaes/Nivaes.App.Secrets/actions/workflows/publish_release.yaml)


