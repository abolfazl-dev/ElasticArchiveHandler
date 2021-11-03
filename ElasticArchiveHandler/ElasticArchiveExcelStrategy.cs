using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticArchiveHandler
{
    public class ElasticArchiveExcelStrategy : IElasticArchiveDataSourceStrategy
    {
        public void Save(IDictionary<string, IEnumerable<object>> dataToSave)
        {
            throw new NotImplementedException();
        }

        public Task SaveAsync(IDictionary<string, IEnumerable<object>> dataToSave)
        {
            throw new NotImplementedException();
        }
    }
}
