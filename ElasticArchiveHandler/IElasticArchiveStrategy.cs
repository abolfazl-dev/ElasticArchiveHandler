using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticArchiveHandler
{
    public interface IElasticArchiveStrategy
    {
        ///use protected on this
        /// <summary>
        /// 
        /// </summary>
        /// <param name="indeiceName">name of indice, to get indice list u can use 'ElasticBaseUrl/_cat/indices' in postman</param>
        /// <param name="dateTime">dateTime</param>
        /// <returns>indexKey</returns>
        string MapDateTimeToIndex(DateTime dateTime, string indeiceName= null);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TObject">set structure of document that must be archive</typeparam>
        /// <param name="indeiceName"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="IsPhysicalDelete"></param>
        /// <returns></returns>
        Task ArchiveAsync<TArchiveDataSourceService, TObject>(string indeiceName, DateTime to, DateTime? from = null, bool IsPhysicalDelete = false) where TArchiveDataSourceService : IElasticArchiveDataSourceStrategy where TObject: class;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indeiceName"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="IsPhysicalDelete"></param>
        /// <returns></returns>
        Task ArchiveAsync<TArchiveDataSourceService>(string indeiceName, DateTime to, DateTime ? from = null, bool IsPhysicalDelete = false) where TArchiveDataSourceService : IElasticArchiveDataSourceStrategy;


        /// <summary>
        ///  search against all documents in all indices
        /// </summary>
        /// <param name="indeiceName"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="IsPhysicalDelete"></param>
        /// <returns></returns>
        void Archive<TArchiveDataSourceService>(string indeiceName, DateTime to, DateTime? from = null, bool IsPhysicalDelete = false) where TArchiveDataSourceService : IElasticArchiveDataSourceStrategy;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TObject">set structure of document that must be archive</typeparam>
        /// <typeparam name="TArchiveDataSourceService">set implement instance of IElasticArchiveDataSourceService</typeparam>
        /// <param name="indeiceName"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="IsPhysicalDelete"></param>
        /// <returns></returns>
        void Archive<TArchiveDataSourceService, TObject>(string indeiceName, DateTime to, DateTime? from = null, bool IsPhysicalDelete = false) where TArchiveDataSourceService : IElasticArchiveDataSourceStrategy where TObject : class;
    }
}
