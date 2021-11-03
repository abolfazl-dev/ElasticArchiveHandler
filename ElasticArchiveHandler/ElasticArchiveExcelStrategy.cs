using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticArchiveHandler
{
    public class ElasticArchiveExcelStrategy : IElasticArchiveDataSourceStrategy
    {
        private readonly IExcelWriterService _excelWriterService;
        
        public ElasticArchiveExcelStrategy(IExcelWriterService excelWriterService)
        {
            _excelWriterService = excelWriterService;
        }
        public void Save(IDictionary<string, IEnumerable<object>> dataToSave)
        {
            var byteArray = _excelWriterService.Write(dataToSave);
            File.WriteAllBytes($"{DateTime.Now.Date.ToShortDateString().Replace("/", "_")}.csv", byteArray);

        }

        public async Task SaveAsync(IDictionary<string, IEnumerable<object>> dataToSave)
        {

            var byteArray = _excelWriterService.Write(dataToSave);
            await File.WriteAllBytesAsync($"{DateTime.Now.Date.ToShortDateString().Replace("/","_")}.csv", byteArray);
        }
    }
}
