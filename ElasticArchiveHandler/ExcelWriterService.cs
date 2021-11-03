//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Aspose.Cells;
//using Aspose.Cells.Utility;

//namespace ElasticArchiveHandler
//{
//    public class ExcelWriterService: IExcelWriterService
//    {
//        public byte[] Write(IEnumerable<object> anonimusObjects)
//        {
//            if (model == null || model.Count() == 0)
//            {
//                return null;
//            }

//            Workbook workbook = new Workbook();
//            Worksheet worksheet = workbook.Worksheets[0];


//            //JsonUtility.ImportData(jsonInput, worksheet.Cells, 0, 0, options);

//            worksheet.Cells.ImportCustomObjects(model.ToList(), 0, 0, new ImportTableOptions()
//            {
//                InsertRows = true,
//                TotalColumns = model.First().GetType().GetProperties().Count(),
//                TotalRows = model.Count()
//            });

//            worksheet.AutoFitColumns();
//            worksheet.AutoFitRows();
//            using var stream = workbook.SaveToStream();

//            return stream.ToArray();
//        }
//    }
//}
