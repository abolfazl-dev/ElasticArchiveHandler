using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticArchiveHandler
{
    public interface IExcelReaderService
    {
        IDictionary<string, IEnumerable<dynamic>> GetExcel(byte[] bArray);
        
        IEnumerable<TItem> GetExcel<TItem>(byte[] bArray, string sheetName = null) where TItem: class;

        IDictionary<string, IEnumerable<string>> ReadExcelDictionary(byte[] bArray, string sheetName);

        IEnumerable<dynamic> GetExcel(byte[] bArray, string sheetName = null);
    }
}
