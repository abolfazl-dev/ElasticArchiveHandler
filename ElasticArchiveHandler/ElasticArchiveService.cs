using Nest;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticArchiveHandler
{
    public class ElasticArchiveService : IElasticArchiveService
    {
        private readonly IEnumerable<IElasticArchiveDataSourceStrategy> _elasticArchiveDataSourceServices;

        private readonly IElasticClient _elasticClient;

        public ElasticArchiveService(IElasticClient elasticClient, IEnumerable<IElasticArchiveDataSourceStrategy> elasticArchiveDataSourceServices)
        {
            _elasticClient = elasticClient;
            _elasticArchiveDataSourceServices = elasticArchiveDataSourceServices;
        }

        public virtual void Archive<TArchiveDataSourceService>(string indiceName, DateTime to, DateTime? from = null, bool IsPhysicalDelete = false) where TArchiveDataSourceService : IElasticArchiveDataSourceStrategy
        {
            GetIndexResponse allIndices;
            if(indiceName == null)
            {
                allIndices = _elasticClient.Indices.Get(new GetIndexRequest(Indices.All));
            }
            else
            {
                allIndices = _elasticClient.Indices.Get(indiceName);

            }

            int toDt = to.Year * 100 + to.Month ;
            int frDt = from == null ? 0 : from.Value.Year * 100 + from.Value.Month ;

            var mustDeleteIndices = new List<string>();
            var mustDeleteDocuments = new Dictionary<string, IEnumerable<object>>();

            if (indiceName != null)
            {
                var indexName = indiceName;

                int st = Convert.ToInt32(indiceName.Substring(indiceName.Length - 6, 6));
                if (st <= toDt && st >= frDt)
                {
                    mustDeleteIndices.Add(indexName);
                }
                var documents = _elasticClient.Search<object>(s => s
                    .Index(indexName)
                    .Query(q => q.MatchAll())
                    .Size(int.MaxValue)
                    ).Documents;

                mustDeleteDocuments.Add(indexName, documents.ToList());
            }
            else
            {
                foreach (var index in allIndices.Indices.Where(x => x.Key.Name.StartsWith("openBanking")))
                {
                    var indexName = index.Key.Name;

                    int st = Convert.ToInt32(index.Key.Name.Substring(index.Key.Name.Length - 6, 6));
                    if (st <= toDt && st >= frDt)
                    {
                        mustDeleteIndices.Add(indexName);
                    }
                    var documents = _elasticClient.Search<object>(s => s
                        .Index(indexName)
                        .Query(q => q.MatchAll())
                        .Size(int.MaxValue)
                        ).Documents;

                    mustDeleteDocuments.Add(indexName, documents.ToList());
                }
            }
            

            var archiverSourceService = _elasticArchiveDataSourceServices.FirstOrDefault();
            
            
            if (!IsPhysicalDelete)
            {
                archiverSourceService.Save(mustDeleteDocuments);
            }
            foreach(var index in mustDeleteIndices)
            {
                _elasticClient.Indices.Delete(index);
            }

            //_elasticClient.DeleteMany<object>(rslt);

        }

        public virtual async Task ArchiveAsync<TArchiveDataSourceService>(string indiceName, DateTime to, DateTime? from = null, bool IsPhysicalDelete = false) where TArchiveDataSourceService : IElasticArchiveDataSourceStrategy
        {
            GetIndexResponse allIndices;
            if(indiceName == null)
            {
                allIndices = await _elasticClient.Indices.GetAsync(new GetIndexRequest(Indices.All));
            }
            else
            {
                allIndices = await _elasticClient.Indices.GetAsync(indiceName);
            }

            int toDt = to.Year * 100 + to.Month ;
            int frDt = from == null ? 0 : from.Value.Year * 100 + from.Value.Month ;

            var mustDeleteIndices = new List<string>();
            var mustDeleteDocuments = new Dictionary<string, IEnumerable<object>>();

            if (indiceName != null)
            {
                var indexName = indiceName;

                int st = Convert.ToInt32(indiceName.Substring(indiceName.Length - 6, 6));
                if (st <= toDt && st >= frDt)
                {
                    mustDeleteIndices.Add(indexName);
                }
                var documents = (await _elasticClient.SearchAsync<object>(s => s
                    .Index(indexName)
                    .Query(q => q.MatchAll())
                    .Size(int.MaxValue)
                    )).Documents;

                mustDeleteDocuments.Add(indexName, documents.ToList());
            }
            else
            {
                foreach (var index in allIndices.Indices.Where(x => x.Key.Name.StartsWith("openBanking")))
                {
                    var indexName = index.Key.Name;

                    int st = Convert.ToInt32(index.Key.Name.Substring(index.Key.Name.Length - 6, 6));
                    if (st <= toDt && st >= frDt)
                    {
                        mustDeleteIndices.Add(indexName);
                    }
                    var documents = (await _elasticClient.SearchAsync<object>(s => s
                        .Index(indexName)
                        .Query(q => q.MatchAll())
                        .Size(int.MaxValue)
                        )).Documents;

                    mustDeleteDocuments.Add(indexName, documents.ToList());
                }
            }
            

            var archiverSourceService = _elasticArchiveDataSourceServices.FirstOrDefault();
            
            
            if (!IsPhysicalDelete)
            {
                await archiverSourceService.SaveAsync(mustDeleteDocuments);
            }
            foreach(var index in mustDeleteIndices)
            {
                _elasticClient.Indices.Delete(index);
            }

            //_elasticClient.DeleteMany<object>(rslt);

        }

        public virtual string MapDateTimeToIndex(DateTime dateTime, string indeiceName = null)
        {
            return null;
        }

    }
}
