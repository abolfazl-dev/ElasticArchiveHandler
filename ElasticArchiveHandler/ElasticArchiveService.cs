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
       
        private readonly string _repositoryBasePath;

        public ElasticArchiveService(IElasticClient elasticClient, 
            IEnumerable<IElasticArchiveDataSourceStrategy> elasticArchiveDataSourceServices,
            string repositoryBasePath
            )
        {
            _elasticClient = elasticClient;
            _elasticArchiveDataSourceServices = elasticArchiveDataSourceServices;
            _repositoryBasePath = repositoryBasePath;
        }


        public void SnapShot(DateTime to, DateTime? from = null)
        {
            int toDt = to.Year * 100 + to.Month;
            int frDt = from == null ? 0 : from.Value.Year * 100 + from.Value.Month;
            var mustDeleteIndices = new List<string>();

            var allIndices = _elasticClient.Indices.Get(new GetIndexRequest(Indices.All));
            foreach (var index in allIndices.Indices.Where(x => x.Key.Name.StartsWith("openBanking")))
            {
                var indexName = index.Key.Name;
                
                if (IsIndexInDateRange(indexName, toDt, frDt))
                {
                    mustDeleteIndices.Add(indexName);

                    var isExist = IsRepositoryExist();
                    if (!isExist)
                    {
                        CreateRepository();
                    }
                    
                    CreatSnapShot(indexName);

                }
            }

            mustDeleteIndices.ForEach(x =>
                _elasticClient.Indices.Delete(x)
            );

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

                    var documents = _elasticClient.Search<object>(s => s
                        .Index(indexName)
                        .Query(q => q.MatchAll())
                        .Size(int.MaxValue)
                        ).Documents;

                    mustDeleteDocuments.Add(indexName, documents.ToList());
                }
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
                        var documents = _elasticClient.Search<object>(s => s
                            .Index(indexName)
                            .Query(q => q.MatchAll())
                            .Size(int.MaxValue)
                            ).Documents;

                        mustDeleteDocuments.Add(indexName, documents.ToList());
                    }
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
                    var documents = (await _elasticClient.SearchAsync<object>(s => s
                    .Index(indexName)
                    .Query(q => q.MatchAll())
                    .Size(int.MaxValue)
                    )).Documents;

                    mustDeleteDocuments.Add(indexName, documents.ToList());
                }
                
            }
            else
            {
                foreach (var index in allIndices.Indices.Where(x => x.Key.Name.StartsWith("openBanking") || x.Key.Name == "kibana_sample_data_logs"))
                {
                    var indexName = index.Key.Name;
                    if (indexName == "kibana_sample_data_logs")
                    {
                        mustDeleteIndices.Add(indexName);
                        var documents = (await _elasticClient.SearchAsync<object>(s => s
                            .Index(indexName)
                            .Query(q => q.MatchAll())
                            .Size(int.MaxValue)
                            )).Documents;
                        mustDeleteDocuments.Add(indexName, documents.ToList());

                    }
                    else
                    {
                        int st = Convert.ToInt32(index.Key.Name.Substring(index.Key.Name.Length - 6, 6));
                        if (st <= toDt && st >= frDt)
                        {
                            mustDeleteIndices.Add(indexName);
                            var documents = (await _elasticClient.SearchAsync<object>(s => s
                                .Index(indexName)
                                .Query(q => q.MatchAll())
                                .Size(int.MaxValue)
                                )).Documents;

                            mustDeleteDocuments.Add(indexName, documents.ToList());
                        }
                    }
                    
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


        #region helper

        private bool IsRepositoryExist()
        {
            var RepoVerifiyResponse = _elasticClient.Snapshot.VerifyRepository("baseRepo");
            var rslt = RepoVerifiyResponse.IsValid;
            return rslt;
        }

        private void CreateRepository()
        {
            var createRepoReq = new CreateRepositoryRequest("baseRepo")
            {
                Repository = new FileSystemRepository(new FileSystemRepositorySettings(_repositoryBasePath) {
                    Compress = true,
                    ChunkSize = "64m"
                })
            };

            _elasticClient.Snapshot.CreateRepository(createRepoReq);
        }
        
        private void CreatSnapShot(string name)
        {
            _elasticClient.Snapshot.Snapshot("baseRepo", name, x => x.WaitForCompletion(true));
        }


        private bool IsIndexInDateRange(string indexName, int toDt, int frDt)
        {
            if (indexName.Length < 6)
            {
                return false;
            }
            var mustNumericPart = indexName.Substring(indexName.Length - 6, 6);
            if (int.TryParse(mustNumericPart, out int st))
            {
                return st <= toDt && st >= frDt;
            }
            return false;

        }

        #endregion

    }
}
