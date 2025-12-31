using Microsoft.CodeAnalysis;
using Shouldly;
using Xunit;

namespace Nivaes.App.Secrets.SourceGenerator.UnitTest;

public class BasicContainerTest
{
    [Fact]
    public async Task CompilesWithoutErrors()
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
    public async Task CanResolveSimpleSingleton()
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

//    [Fact]
//    public async Task CanResolveSimpleTransient()
//    {
//        var project = await TestProject.Project.ApplyToProgram(@"

//        public interface IService
//        {

//        }

//        public class Service : IService
//        {

//        }

//        public partial class TestContainer : IoCServiceContainer
//        {
//            protected override void Bootstrap(IIoCServiceContainerBootstrapper bootstrapper)
//            {
//                bootstrapper.AddTransient<IService, Service>();
//            }
//        }
//");

//        var newProject = await project.ApplyIoCGenerator();

//        var assembly = await newProject.CompileToRealAssembly();
//        var containerType = assembly.GetType("TestProject.TestContainer");
//        containerType.ShouldNotBeNull();
        
//        var serviceType = assembly.GetType("TestProject.IService");
//        serviceType.ShouldNotBeNull();

//        var container = (IIoCResolver?)Activator.CreateInstance(containerType!);
//        container.ShouldNotBeNull();

//        var firstService = container!.Resolve(serviceType!);
//        firstService.ShouldNotBeNull();

//        var secondService = container.Resolve(serviceType!);
//        secondService.ShouldNotBeNull();

//        firstService.ShouldNotBe(secondService);
//    }

//    [Fact]
//    public async Task HandlesMultipleContainersInTheSameTime()
//    {
//        var project = await TestProject.Project.ApplyToProgram(@"

//        public interface IService
//        {

//        }

//        public class Service : IService
//        {

//        }

//        public partial class TestContainer : IoCServiceContainer
//        {
//            protected override void Bootstrap(IIoCServiceContainerBootstrapper bootstrapper)
//            {
//                bootstrapper.AddTransient<IService, Service>();
//            }
//        }

//        public partial class SingleContainer : IoCServiceContainer
//        {
//            protected override void Bootstrap(IIoCServiceContainerBootstrapper bootstrapper)
//            {
//                bootstrapper.AddSingleton<IService, Service>();
//            }
//        }
//");

//        var newProject = await project.ApplyIoCGenerator();

//        var assembly = await newProject.CompileToRealAssembly();
//        var containerType1 = assembly.GetType("TestProject.TestContainer");
//        var containerType2 = assembly.GetType("TestProject.SingleContainer");
//        containerType1.ShouldNotBeNull();
//        containerType2.ShouldNotBeNull();

//        var serviceType = assembly.GetType("TestProject.IService");
//        serviceType.ShouldNotBeNull();

//        var container1 = (IIoCResolver?)Activator.CreateInstance(containerType1!);
//        var container2 = (IIoCResolver?)Activator.CreateInstance(containerType2!);
//        container1.ShouldNotBeNull();
//        container2.ShouldNotBeNull();

//        var firstService1 = container1!.Resolve(serviceType!);
//        var secondService1 = container1!.Resolve(serviceType!);

//        var firstService2 = container2!.Resolve(serviceType!);
//        var secondService2 = container2!.Resolve(serviceType!);

//        Assert.True(!firstService1!.Equals(secondService1));
//        Assert.True(!firstService1!.Equals(firstService2));
//        Assert.True(!firstService1!.Equals(secondService2));
//        Assert.True(firstService2!.Equals(secondService2));
//    }

//    [Fact]
//    public async Task SingletonsAreTheSameBetweenScopes()
//    {
//        var project = await TestProject.Project.ApplyToProgram(@"

//        public interface IService
//        {

//        }

//        public class Service : IService
//        {

//        }

//        public partial class TestContainer : IoCServiceContainer
//        {
//            protected override void Bootstrap(IIoCServiceContainerBootstrapper bootstrapper)
//            {
//                bootstrapper.AddSingleton<IService, Service>();
//            }
//        }
//");

//        var newProject = await project.ApplyIoCGenerator();

//        var assembly = await newProject.CompileToRealAssembly();
//        var containerType = assembly.GetType("TestProject.TestContainer");
//        var serviceType = assembly.GetType("TestProject.IService");
//        serviceType.ShouldNotBeNull();
//        var container = (IIoCResolver?)Activator.CreateInstance(containerType!);
//        container.ShouldNotBeNull();
//        var firstService = container!.Resolve(serviceType!);

//        var scoped = container.CreateScope();
//        var secondService = scoped.Resolve(serviceType!);

//        Assert.True(firstService != null && secondService != null && firstService.Equals(secondService));
//    }

//    [Fact]
//    public async Task RegisterOnlyImplementation()
//    {
//        var project = await TestProject.Project.ApplyToProgram(@"

//        public class Service
//        {
//        }

//        public partial class TestContainer : IoCServiceContainer
//        {
//            protected override void Bootstrap(IIoCServiceContainerBootstrapper bootstrapper)
//            {
//                bootstrapper.AddSingleton<Service>();
//            }
//        }
//");

//        var newProject = await project.ApplyIoCGenerator();

//        var assembly = await newProject.CompileToRealAssembly();
//        var containerType = assembly.GetType("TestProject.TestContainer");
//        containerType.ShouldNotBeNull();
//        var serviceType = assembly.GetType("TestProject.Service");
//        serviceType.ShouldNotBeNull();

//        var container = (IIoCResolver?)Activator.CreateInstance(containerType!);
//        container.ShouldNotBeNull();
//        var service = container!.Resolve(serviceType!);

//        service.ShouldNotBeNull();
//    }

//    [Fact]
//    public async Task TypedOnlyAPartOfServiceName()
//    {
//        var project = await TestProject.Project.ApplyToProgram(@"

//        public class Service
//        {
//        }

//        public partial class TestContainer : IoCServiceContainer
//        {
//            protected override void Bootstrap(IIoCServiceContainerBootstrapper bootstrapper)
//            {
//                bootstrapper.AddSingleton<Servi>();
//            }
//        }
//");

//        var newProject = await project.ApplyIoCGenerator();
//        newProject.ShouldNotBeNull();
//    }

//    [Fact]
//    public async Task BootstrapMethodIsMissing()
//    {
//        var project = await TestProject.Project.ApplyToProgram(@"

//        public class Service
//        {
//        }

//        public partial class TestContainer : IoCContainer
//        {
           
//        }
//");

//        var newProject = await project.ApplyIoCGenerator();
//        newProject.ShouldNotBeNull();
//    }
}
