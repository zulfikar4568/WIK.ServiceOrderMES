using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WIK.ServiceOrderMES.Config;
using WIK.ServiceOrderMES.Util;

namespace WIK.ServiceOrderMES.Repository
{
    public interface IOrder
    {
        List<Entity.Order> Reading(string delimiter, string sourceFile);
    }
    public class Order : IOrder
    {
        public List<Entity.Order> Reading(string delimiter, string sourceFile)
        {
            List<Entity.Order> result = new List<Entity.Order>();
            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Encoding = Encoding.UTF8, // Our file uses UTF-8 encoding.
                Delimiter = delimiter
            };

            try
            {

                using (var reader = new StreamReader(sourceFile))
                using (var csv = new CsvReader(reader, configuration))
                {
                    var records = csv.GetRecords<Entity.Order>();
                    result = records.ToList();
                }
            }
            catch (Exception ex)
            {
                ex.Source = AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source;
                EventLogUtil.LogErrorEvent(ex.Source, ex);
            }
            return result;
        }
    }
}
