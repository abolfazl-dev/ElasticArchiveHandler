using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticArchiveHandler
{
    public interface IElasticArchiveDataSourceStrategy
    {
        Task SaveAsync<TObject>(IEnumerable<TObject> dataToSave);
        
        Task SaveAsync(IEnumerable<object> dataToSave);

        void Save<TObject>(IEnumerable<TObject> dataToSave);
        
        void Save(IEnumerable<object> dataToSave);
    }
}
