using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticArchiveHandler
{
    public interface IElasticArchiveDataSourceStrategy
    {
        void Save(IDictionary<string, IEnumerable<object>> dataToSave);

        Task SaveAsync(IDictionary<string, IEnumerable<object>> dataToSave);
    }
}
