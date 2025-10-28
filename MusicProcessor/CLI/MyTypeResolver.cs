using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace MusicProcessor.CLI;

public sealed class MyTypeResolver : ITypeResolver, IDisposable
{
    private readonly IServiceProvider _provider;
    private IServiceScope? _scope;

    public MyTypeResolver(IServiceProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        // Create a scope to resolve scoped services
        _scope = _provider.CreateScope();
    }

    public object? Resolve(Type? type)
    {
        if (type == null)
        {
            return null;
        }

        // Resolve from the scoped provider
        return _scope?.ServiceProvider.GetService(type);
    }

    public void Dispose()
    {
        _scope?.Dispose();

        if (_provider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}