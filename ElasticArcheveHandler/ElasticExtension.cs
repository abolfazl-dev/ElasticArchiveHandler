using ElasticArchiveHandler;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticArcheveHandler
{
    public static class ElasticExtension
    {
        public static IServiceCollection AddElastic(this IServiceCollection services)
        {
            services.AddElasticArchiveService();
            services.AddSingleton(BuildElasticClient);
            return services;
        }

        private static IElasticClient BuildElasticClient(IServiceProvider serviceProvider)
        {
            var node = new Uri("http://localhost:9200");
            var settings = new ConnectionSettings(node)
                .ThrowExceptions(alwaysThrow: true)
                .PrettyJson();
            var client = new ElasticClient(settings);
            var allIndices = client.Indices.Get(new GetIndexRequest(Indices.All));
            
            foreach (var index in allIndices.Indices)
            {

                client.Indices.UpdateSettings(index.Key.Name, s => s
                .IndexSettings(i => i.Setting(UpdatableIndexSettings.MaxResultWindow, int.MaxValue)));
            }

            return client;
        }
    }
}
