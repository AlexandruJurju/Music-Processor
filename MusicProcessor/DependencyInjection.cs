using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using Raven.Embedded;

namespace MusicProcessor;

public static class DependencyInjection
{
    public static void AddRavenDb(this IServiceCollection services, IConfiguration configuration)
    {
        try
        {
            EmbeddedServer.Instance.StartServer(new ServerOptions
            {
                DataDirectory = configuration["RavenDB:DataDirectory"]!,
                ServerUrl = configuration["RavenDB:ServerUrl"]!
            });

            services.AddSingleton<IDocumentStore>(_ =>
            {
                IDocumentStore store = EmbeddedServer.Instance.GetDocumentStore(configuration["RavenDB:DatabaseName"]!);
                return store;
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}