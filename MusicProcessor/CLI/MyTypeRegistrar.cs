using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace MusicProcessor.CLI;

public sealed class MyTypeRegistrar : ITypeRegistrar
{
    private readonly IServiceCollection _builder;

    public MyTypeRegistrar(IServiceCollection builder)
    {
        _builder = builder;
    }

    public ITypeResolver Build()
    {
        return new MyTypeResolver(_builder.BuildServiceProvider());
    }

    // 👇 Change to Transient (commands and handlers should not be singletons)
    public void Register(Type service, Type implementation)
    {
        _builder.AddTransient(service, implementation);
    }

    public void RegisterInstance(Type service, object implementation)
    {
        _builder.AddSingleton(service, implementation);
    }

    public void RegisterLazy(Type service, Func<object> func)
    {
        if (func is null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        _builder.AddTransient(service, _ => func());
    }
}
