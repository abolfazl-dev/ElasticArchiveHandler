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
            var archiverSourceService = _elasticArchiveDataSourceServices.FirstOrDefault();
            var boolQuery = GetQuery(to, from);

            IEnumerable<dynamic> rslt;
            if(boolQuery != null)
            {
                if (indiceName == null) {
                    rslt = _elasticClient.Search<object>(s => s
                    .AllIndices()
                    .Query(q => boolQuery)
                    .Size(int.MaxValue)
                    ).Documents;
                }
                else
                {
                    rslt = _elasticClient.Search<dynamic>(s => s
                .Index(indiceName)
                .Query(q => boolQuery)
                .Size(int.MaxValue)
                ).Documents;
                }
            }
            else
            {
                var index = MapDateTimeToIndex(to);
                rslt = new[] { _elasticClient.Get<dynamic>(index, g => g.Index(indiceName)).Source };
            }

            if (!IsPhysicalDelete)
            {
                archiverSourceService.Save(rslt);
            }
            _elasticClient.DeleteMany(rslt);

        }

        public virtual void Archive<TArchiveDataSourceService, TObject>(string indiceName, DateTime to, DateTime? from = null, bool IsPhysicalDelete = false) where TArchiveDataSourceService : IElasticArchiveDataSourceStrategy where TObject : class
        {
            var archiverSourceService = _elasticArchiveDataSourceServices.FirstOrDefault(x => x.GetType() == typeof(TArchiveDataSourceService));
            var boolQuery = GetQuery(to, from);

            IEnumerable<TObject> rslt;
            if (boolQuery != null)
            {
                rslt =  _elasticClient.Search<TObject>(s => {
                    var c = s
                        .Query(q => boolQuery)
                        .Size(int.MaxValue);
                    if (indiceName != null)
                    {
                        c = c.Index(indiceName);
                    }
                    return c;
                }
                ).Documents;
            }
            else
            {
                var index = MapDateTimeToIndex(to);
                rslt = new[] { _elasticClient.Get<TObject>(index, g => g.Index(indiceName)).Source };
            }

            if (!IsPhysicalDelete)
            {
                archiverSourceService.Save(rslt);
            }
            _elasticClient.DeleteMany(rslt);

        }

        public virtual async Task ArchiveAsync<TArchiveDataSourceService, TObject>(string indiceName, DateTime to, DateTime? from = null, bool IsPhysicalDelete = false) where TArchiveDataSourceService : IElasticArchiveDataSourceStrategy where TObject : class
        {
            var archiverSourceService = _elasticArchiveDataSourceServices.FirstOrDefault(x => x.GetType() == typeof(TArchiveDataSourceService));
            var boolQuery = GetQuery(to, from);

            IEnumerable<TObject> rslt;
            if (boolQuery != null)
            {
                rslt = (await _elasticClient.SearchAsync<TObject>(s => {
                    var c = s
                        .Query(q => boolQuery)
                        .Size(int.MaxValue);
                    if (indiceName != null)
                    {
                        c = c.Index(indiceName);
                    }
                    return c;
                }
                )).Documents;
            }
            else
            {
                var index = MapDateTimeToIndex(to);
                if(indiceName != null)
                {
                    rslt = new[] { (await _elasticClient.GetAsync<TObject>(index, g => g.Index(indiceName))).Source };
                }
                else
                {
                    rslt = new[] { (await _elasticClient.GetAsync<TObject>(index)).Source };
                }
            }

            if (!IsPhysicalDelete)
            {
                await archiverSourceService.SaveAsync(rslt);
            }
            await _elasticClient.DeleteManyAsync(rslt);
        }

        public virtual async Task ArchiveAsync<TArchiveDataSourceService>(string indiceName, DateTime to, DateTime? from = null, bool IsPhysicalDelete = false) where TArchiveDataSourceService : IElasticArchiveDataSourceStrategy 
        {
            var archiverSourceService = _elasticArchiveDataSourceServices.FirstOrDefault(x => x.GetType() == typeof(TArchiveDataSourceService));
            var boolQuery = GetQuery(to, from);

            IEnumerable<dynamic> rslt;
            if (boolQuery != null)
            {
                rslt = (await _elasticClient.SearchAsync<dynamic>(s => s
                .Index(indiceName)
                .Query(q => boolQuery)
                .Size(int.MaxValue)
                )).Documents;
            }
            else
            {
                var index = MapDateTimeToIndex(to);
                rslt = new[] { _elasticClient.Get<dynamic>(index, g => g.Index(indiceName)).Source };
            }

            if (!IsPhysicalDelete)
            {
                archiverSourceService.Save(rslt);
            }
            _elasticClient.DeleteMany(rslt);
        }

        public virtual string MapDateTimeToIndex(DateTime dateTime, string indeiceName = null)
        {
            return null;
        }

        #region helper

        private BoolQuery GetQuery(DateTime to, DateTime? from = null)
        {
            if (!from.HasValue)
            {
                var index = MapDateTimeToIndex(to);
                if (index != null)
                {
                    return null;
                }
            }

            var query = new DateRangeQuery
            {
                Field = "timestamp",
                LessThan = to,
                //Format = "dd/MM/yyyy||yyyy"
            };
            if (from.HasValue)
            {
                query.GreaterThan = from.Value;
            }

            var boolQuery = new BoolQuery()
            {
                Filter = new QueryContainer[]
                {
                    query
                }

            };

            return boolQuery;

        }
        #endregion
    }
}
