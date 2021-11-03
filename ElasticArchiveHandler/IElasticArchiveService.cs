using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticArchiveHandler
{
    public interface IElasticArchiveService
    {

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TArchiveDataSourceStrategy">set structure of document that must be archive</typeparam>
        /// <param name="indiceName">
        /// set null if u want all indices
        /// or set target index that u want delete
        /// </param>
        /// <param name="from">from date, optional</param>
        /// <param name="to">to date</param>
        /// <param name="IsPhysicalDelete">if true then no archive and full delete
        /// else delete and archive</param>
        void Archive<TArchiveDataSourceStrategy>(string indiceName, DateTime to, DateTime? from = null, bool IsPhysicalDelete = false) where TArchiveDataSourceStrategy : IElasticArchiveDataSourceStrategy;


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TArchiveDataSourceStrategy">set structure of document that must be archive</typeparam>
        /// <param name="indiceName">
        /// set null if u want all indices
        /// or set target index that u want delete
        /// </param>
        /// <param name="from">from date, optional</param>
        /// <param name="to">to date</param>
        /// <param name="IsPhysicalDelete">if true then no archive and full delete
        /// else delete and archive</param>
        Task ArchiveAsync<TArchiveDataSourceStrategy>(string indiceName, DateTime to, DateTime? from = null, bool IsPhysicalDelete = false) where TArchiveDataSourceStrategy : IElasticArchiveDataSourceStrategy;

    }
}
