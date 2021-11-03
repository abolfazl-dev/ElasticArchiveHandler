using Aspose.Cells;
using Aspose.Cells.Utility;
using ExcelDataReader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ElasticArchiveHandler
{
    public class ExcelReaderService : IExcelReaderService
    {
        public IDictionary<string, IEnumerable<dynamic>> GetExcel(byte[] bArray)
        {
            var rslt = new Dictionary<string, IEnumerable<dynamic>>();
            var dictionaryList = ReadExcelDictionary(bArray);
         
            foreach(var indexDictionary in dictionaryList)
            {
                var dictionary = indexDictionary.Value;

                var rowCount = dictionary?.FirstOrDefault().Value?.Count();
                var colCount = dictionary?.Count;
                if (!(rowCount > 0 && colCount > 0))
                {
                    return null;
                }
                var l = new List<dynamic>();

                for (int i = 0; i < rowCount; i++)
                {
                    var dynamicObj = new ExpandoObject();

                    foreach (var col in dictionary)
                    {
                        dynamicObj.TryAdd(col.Key, col.Value.ElementAt(i));
                    }
                    l.Add(dynamicObj);
                }

                rslt.Add(indexDictionary.Key, l);
            }

            return rslt;
        }

        public IEnumerable<dynamic> GetExcel(byte[] bArray, string sheetName)
        {
            var dictionary = ReadExcelDictionary(bArray, sheetName);
            var rowCount = dictionary?.FirstOrDefault().Value?.Count();
            var colCount = dictionary?.Count;
            if (!(rowCount > 0 && colCount > 0))
            {
                return null;
            }
            var l = new List<dynamic>();

            for (int i = 0; i < rowCount; i++)
            {
                var dynamicObj = new ExpandoObject();

                foreach (var col in dictionary)
                {
                    dynamicObj.TryAdd(col.Key, col.Value.ElementAt(i));
                }
                l.Add(dynamicObj);
            }

            return l;
        }

        public IEnumerable<TItem> GetExcel<TItem>(byte[] bArray, string sheetName) where TItem: class
        {
            var l = GetExcel(bArray, sheetName);
            string json = JsonConvert.SerializeObject(l);
            var rslt = JsonConvert.DeserializeObject<IList<TItem>>(json);
            return rslt;
        }

        public IDictionary<string, IEnumerable<string>> ReadExcelDictionary(byte[] bArray, string sheetName)
        {
            Dictionary<string, IEnumerable<string>> rslt = null;
            using MemoryStream memStream = new MemoryStream(bArray);
            using IExcelDataReader excelreader = ExcelReaderFactory.CreateOpenXmlReader(memStream); //'97-03 USE: CreateBinaryReader, 07+ USE: CreateOpenXmlReader
            //CREATE COLUMN NAMES FROM FIRST ROW
            DataSet ds = excelreader.AsDataSet();
            DataTable dt = null;
            foreach (var item in ds.Tables)
            {
                dt = item as DataTable;
                if (dt.TableName == sheetName)
                {
                    break;
                }
            }
            if (excelreader.Read())
            {
                var headers = dt.Rows[0].ItemArray;
                rslt = new Dictionary<string, IEnumerable<string>>(headers.Length);
                foreach (var item in headers)
                {
                    rslt.Add(item.ToString(), new List<string>(dt.Columns.Count));
                }

                for (var i = 1; i < dt.Rows.Count; i++)
                {
                    var values = dt.Rows[i].ItemArray;
                    for (int j = 0; j < values.Length; j++)
                    {
                        var value = values[j];
                        (rslt.ElementAt(j).Value as List<string>).Add(value.ToString());
                    }
                }
            }
            return rslt;
        }

        public IDictionary<string, IDictionary<string, IEnumerable<string>>> ReadExcelDictionary(byte[] bArray)
        {
            var rslt = new Dictionary<string, IDictionary<string, IEnumerable<string>>>();
            Dictionary<string, IEnumerable<string>> indexDocuments = null;
            using MemoryStream memStream = new MemoryStream(bArray);
            using IExcelDataReader excelreader = ExcelReaderFactory.CreateOpenXmlReader(memStream); //'97-03 USE: CreateBinaryReader, 07+ USE: CreateOpenXmlReader
            //CREATE COLUMN NAMES FROM FIRST ROW
            DataSet ds = excelreader.AsDataSet();
            var dtList = new List<DataTable>();
            
            foreach (var item in ds.Tables)
            {
                dtList.Add(item as DataTable);
            }
            if (excelreader.Read())
            {
                foreach(var dt in dtList)
                {
                    var headers = dt.Rows[0].ItemArray;
                    indexDocuments = new Dictionary<string, IEnumerable<string>>(headers.Length);
                    foreach (var item in headers)
                    {
                        indexDocuments.Add(item.ToString(), new List<string>(dt.Columns.Count));
                    }

                    for (var i = 1; i < dt.Rows.Count; i++)
                    {
                        var values = dt.Rows[i].ItemArray;
                        for (int j = 0; j < values.Length; j++)
                        {
                            var value = values[j];
                            (indexDocuments.ElementAt(j).Value as List<string>).Add(value.ToString());
                        }
                    }
                    rslt.Add(dt.TableName, indexDocuments);
                }
            }
            return rslt;
        }

    }
}
