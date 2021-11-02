using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ElasticArchiveHandler
{
    public static class ElasticArchiveExtension
    {
        public static IServiceCollection AddElasticArchiveService<TElasticArchiveService>(this IServiceCollection services) where TElasticArchiveService : ElasticArchiveService
        {
            services.AddSingleton<IElasticArchiveService, TElasticArchiveService>();
            
            var strategies = GetAllImpls();
            foreach(var st in strategies)
            {
                services.AddSingleton(typeof(IElasticArchiveDataSourceStrategy), st);
            }

            return services;
        }
        
        public static IServiceCollection AddElasticArchiveService(this IServiceCollection services)
        {
            services.AddSingleton<IElasticArchiveService, ElasticArchiveService>();

            var strategies = GetAllImpls();
            foreach (var st in strategies)
            {
                services.AddSingleton(typeof(IElasticArchiveDataSourceStrategy), st);
            }
            return services;
        }

        private static IEnumerable<Type> GetAllImpls()
        {
            var assembly = Assembly.GetEntryAssembly();
            var assemblies = assembly.GetReferencedAssemblies();

            foreach (var assemblyName in assemblies)
            {
                assembly = Assembly.Load(assemblyName);

                foreach (var ti in assembly.DefinedTypes)
                {
                    if (ti.ImplementedInterfaces.Contains(typeof(IElasticArchiveDataSourceStrategy)))
                    {
                        yield return ti.AsType();
                    }
                }
            }
        }
    }
}
