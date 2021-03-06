using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticArchiveHandler
{
    public interface IExcelWriterService
    {
        byte[] Write(IEnumerable<object> anonimusObjects);
        
        byte[] Write(IDictionary<string, IEnumerable<object>> anonimusObjects);
    }
}
