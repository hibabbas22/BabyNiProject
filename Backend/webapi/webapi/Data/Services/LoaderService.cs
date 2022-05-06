using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Odbc;
using System.IO;

namespace webapi.Data.Services
{
    public class LoaderService
    {
        private readonly IConfiguration Configuration;

        public AggregatorService _aggregatorService;
        public LoaderService(IConfiguration configuration)
        {
            Configuration = configuration;
            _aggregatorService = new AggregatorService(Configuration);

        }
        public void Load(string path)
        {
            //string paths = Configuration.GetValue<string>("loadFile");
            string connectionString = Configuration.GetConnectionString("DefaultConnectionString");
            string queryString = $"update Logger set isParsed='true',isLoaded='true',isAggregated='false' where FileName='{Path.GetFileNameWithoutExtension(path)}'";
            OdbcCommand command = new OdbcCommand(queryString);
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                command.Connection = connection;
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        public void ConnectVertica()
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnectionString");
            string path = Configuration.GetValue<string>("loadFile");
            string queryString = $"Copy radio_link from local '{path}' delimiter ',' direct";

            OdbcCommand command = new OdbcCommand(queryString);
            
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                command.Connection = connection;
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        public void ConnectVertica(string pathFull)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnectionString");
            string path = Configuration.GetValue<string>("loadFile");

            string queryString = $"Copy radio_link from local '{path}' delimiter ',' direct";

            OdbcCommand command = new OdbcCommand(queryString);

            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                command.Connection = connection;
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            _aggregatorService.aggregateDataHourly();
            _aggregatorService.aggregateDataDaily();
        }
        
    }    
}