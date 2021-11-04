﻿using Nest;
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


        public void SnapShot(DateTime to, DateTime? from = null)
        {
            int toDt = to.Year * 100 + to.Month;
            int frDt = from == null ? 0 : from.Value.Year * 100 + from.Value.Month;
            var mustDeleteIndices = new List<string>();
            var mustDeleteDocuments = new Dictionary<string, IEnumerable<object>>();

            var allIndices = _elasticClient.Indices.Get(new GetIndexRequest(Indices.All));
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

                    var isExist = IsRepositoryExist(indexName + "_repo");
                    if (!isExist)
                    {
                        CreateRepository(indexName + "_repo");
                    }



                    var rep = new CreateRepositoryRequest(indexName)
                    {

                    };

                }
            }

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

        private bool IsRepositoryExist(string repoName)
        {
            var RepoVerifiyResponse = _elasticClient.Snapshot.VerifyRepository(repoName);
            var rslt = RepoVerifiyResponse.IsValid;
            return rslt;
        }

        private void CreateRepository(string repoName)
        {
            _elasticClient.Snapshot.Snapshot("es_backup", "snapshot_4", x => x.WaitForCompletion(true));
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
