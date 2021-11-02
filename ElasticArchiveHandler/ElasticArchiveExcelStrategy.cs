using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticArchiveHandler
{
    public class ElasticArchiveExcelStrategy : IElasticArchiveDataSourceStrategy
    {
        public void Save<TObject>(IEnumerable<TObject> dataToSave)
        {
            throw new NotImplementedException("excel strategy not implimented");
        }

        public void Save(IEnumerable<object> dataToSave)
        {
            throw new NotImplementedException("excel strategy not implimented");
        }

        public Task SaveAsync<TObject>(IEnumerable<TObject> dataToSave)
        {
            throw new NotImplementedException("excel strategy not implimented");
        }

        public Task SaveAsync(IEnumerable<object> dataToSave)
        {
            throw new NotImplementedException("excel strategy not implimented");
        }
    }
}
