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
        public static IServiceCollection AddElastic(IServiceCollection services)
        {
            services.AddSingleton(BuildElasticClient);
            services.AddElasticArchiveService();
            return services;
        }

        private static IElasticClient BuildElasticClient(IServiceProvider serviceProvider)
        {
            var node = new Uri("http://localhost:9200");
            var settings = new ConnectionSettings(node)
                .ThrowExceptions(alwaysThrow: true)
                .PrettyJson();
            var client = new ElasticClient(settings);
            return client;
        }
    }
}
