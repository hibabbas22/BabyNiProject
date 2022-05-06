using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace webapi.Data.Services
{
    public class AggregatorService
    {
        private readonly IConfiguration Configuration;

        public AggregatorService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void aggregateDataHourly()
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnectionString");
            string path = Configuration.GetValue<string>("loadFile");

            string queryString = @"INSERT INTO HourlyAgg (
                TimeKey, Link, Slot,Max_Rx_Level, Max_Tx_Level, Rsl_Deviation)
                SELECT date_trunc('hour', Time) as TimeKey, Link, Slot,
                Max(MaxRxLevel) as Max_Rx_Level,
                Max(MaxTxLevel) as Max_Tx_Level,
                abs(Max_Rx_Level) - abs(Max_Tx_Level) as Rsl_Deviation
                from radio_link
                group by 1,2,3
                order by 1;";

            OdbcCommand command = new OdbcCommand(queryString);
            //Open db connection
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                command.Connection = connection;
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

            }
        }
        public void aggregateDataDaily()
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnectionString");
            string path = Configuration.GetValue<string>("loadFile");

            string queryString2 = @"INSERT INTO DailyAgg (
                TimeKey_daily, Link, Slot,Max_Rx_Level_daily, Max_Tx_Level_daily, Rsl_Deviation_daily)
                SELECT date_trunc('day', TimeKey) as TimeKey_daily, Link, Slot,
                Max(Max_Rx_Level) as Max_Rx_Level_daily,
                Max(Max_Tx_Level) as Max_Tx_Level_daily,
                abs(Max_Rx_Level_daily) - abs(Max_Tx_Level_daily) as Rsl_Deviation_daily
                from HourlyAgg
                where TimeKey not in (select distinct date_trunc('day', TimeKey_daily) from DailyAgg) 
                group by 1,2,3
                order by 1; 


                ";


            OdbcCommand command = new OdbcCommand(queryString2);

            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                command.Connection = connection;
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

            }
        }
        public void aggregateDataHourly(string pathFile)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnectionString");
            //string path = Configuration.GetValue<string>("loadFile");

            string queryString = @"INSERT INTO HourlyAgg (
                TimeKey, Link, Slot,Max_Rx_Level, Max_Tx_Level, Rsl_Deviation)
                SELECT date_trunc('hour', Time) as TimeKey, Link, Slot,
                Max(MaxRxLevel) as Max_Rx_Level,
                Max(MaxTxLevel) as Max_Tx_Level,
                abs(Max_Rx_Level) - abs(Max_Tx_Level) as Rsl_Deviation
                from radio_link
                group by 1,2,3
                order by 1;";



            OdbcCommand command = new OdbcCommand(queryString);

            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                command.Connection = connection;
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

            }
        }
        public void aggregateDataDaily(string pathFile)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnectionString");
            //string path = Configuration.GetValue<string>("loadFile");

            string queryString2 = @"INSERT INTO DailyAgg (
                TimeKey_daily, Link, Slot,Max_Rx_Level_daily, Max_Tx_Level_daily, Rsl_Deviation_daily)
                SELECT date_trunc('day', TimeKey) as TimeKey_daily, Link, Slot,
                Max(Max_Rx_Level) as Max_Rx_Level_daily,
                Max(Max_Tx_Level) as Max_Tx_Level_daily,
                abs(Max_Rx_Level_daily) - abs(Max_Tx_Level_daily) as Rsl_Deviation_daily
                from HourlyAgg
                where TimeKey not in (select distinct date_trunc('day', TimeKey_daily) from DailyAgg) 
                group by 1,2,3
                order by 1; ";


            OdbcCommand command = new OdbcCommand(queryString2);

            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                command.Connection = connection;
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

            }
        }
        public void AggregateNew()
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnectionString");
            string paths = Configuration.GetValue<string>("parseFile");
            string queryString = $"update Logger set isParsed='true',isLoaded='true',isAggregated='true' where FileName='{Path.GetFileNameWithoutExtension(paths)}'";
            OdbcCommand command = new OdbcCommand(queryString);
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                command.Connection = connection;
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}
