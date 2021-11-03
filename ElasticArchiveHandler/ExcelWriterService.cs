using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspose.Cells;
using Aspose.Cells.Utility;

namespace ElasticArchiveHandler
{
    public class ExcelWriterService : IExcelWriterService
    {
        public byte[] Write(IEnumerable<object> anonimusObjects)
        {
            if (anonimusObjects == null || anonimusObjects.Count() == 0)
            {
                return null;
            }

            Workbook workbook = new Workbook();
            Worksheet worksheet = workbook.Worksheets[0];


            //JsonUtility.ImportData(jsonInput, worksheet.Cells, 0, 0, options);

            worksheet.Cells.ImportCustomObjects(anonimusObjects.ToList(), 0, 0, new ImportTableOptions()
            {
                InsertRows = true,
                TotalColumns = anonimusObjects.First().GetType().GetProperties().Count(),
                TotalRows = anonimusObjects.Count()
            });

            worksheet.AutoFitColumns();
            worksheet.AutoFitRows();
            using var stream = workbook.SaveToStream();

            return stream.ToArray();
        }

        public byte[] Write(IDictionary<string, IEnumerable<object>> anonimusObjects)
        {
            if (anonimusObjects == null || anonimusObjects.Count() == 0)
            {
                return null;
            }

            Workbook workbook = new Workbook();
            foreach(var item in anonimusObjects)
            {
                var worksheet = workbook.Worksheets.Add(item.Key);
                if (item.Value?.Count() > 0)
                {
                    worksheet.Cells.ImportCustomObjects(item.Value.ToList(), 0, 0, new ImportTableOptions()
                    {
                        InsertRows = true,
                        TotalColumns = item.Value.First().GetType().GetProperties().Count(),
                        TotalRows = item.Value.Count()
                    });

                    worksheet.AutoFitColumns();
                    worksheet.AutoFitRows();
                }
            }


            //JsonUtility.ImportData(jsonInput, worksheet.Cells, 0, 0, options);

            using var stream = workbook.SaveToStream();

            return stream.ToArray();
        }
    }
}
