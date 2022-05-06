using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace webapi.Data.Services
{
    public class SendDailyService
    {
        private readonly IConfiguration Configuration;

        public SendDailyService(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public List<Dictionary<string, object>> SendData(string startDate, string endDate)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnectionString");
            var returnData = new List<Dictionary<string, object>>();

            string queryString = $@"select  TimeKey_daily, Link, Slot, Max_Rx_Level_daily, Max_Tx_Level_daily, Rsl_Deviation_daily, TimeKey_daily || ' - ' || Link as DateLink
                                    from DailyAgg  where TimeKey_daily between '{startDate}'::DATE and '{endDate}'::DATE";
            
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString);
                command.Connection = connection;
                connection.Open();
                OdbcDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var dataSend = new Dictionary<string, object>
                    {
                        { "TimeKey_daily", (DateTime)reader[0] },
                        { "Link", (string)reader[1] },
                        { "Slot", (string)reader[2] },
                        { "Max_Rx_Level_daily", (double)reader[3] },
                        { "Max_Tx_Level_daily", (double)reader[4] },
                        { "Rsl_Deviation_daily", (double)reader[5] },
                        { "DateLink", (string)reader[6]} 
                    };

                    returnData.Add(dataSend);
                }
                reader.Close();
                connection.Close();
                return returnData;
            }

        }
    }
}
