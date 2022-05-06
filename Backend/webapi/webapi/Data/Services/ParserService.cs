using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using webapi.Models;

namespace webapi.Data.Services
{
    public class ParserService
    {
        private readonly IConfiguration Configuration;

        public ParserService(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void Parse(string path)
        {
            string paths = Configuration.GetValue<string>("loadFile");
            string connectionString = Configuration.GetConnectionString("DefaultConnectionString");

            string queryString = $"insert into Logger values ('{Path.GetFileNameWithoutExtension(path)}', 'true','false','false' )";


            OdbcCommand command = new OdbcCommand(queryString);

            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                command.Connection = connection;
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void ReadCsv()
        {
             string parseFile = Configuration.GetValue<string>("parseFile");

                var lines = File.ReadAllLines(parseFile);
                //inject in a list
                var list = new List<ModelClass>();

                //new list for the parsed objects
                var parsedlist = new List<ModelClass>();

                //read each line ignoring the header
                int i = 0;
                foreach (var line in lines)
                {
                    if (i == 0)
                    {
                        i++;
                        continue;
                    }

                    //split the values by the comma delimeter
                    var values = line.Split(',');
                    if (values.Length == 15)
                    {
                        {
                            //condition to ignore "-" from failure Description (value[14]) and equals to "Unreachable Bulk FC" from object (value[2])
                            if (values[14] != "-" || values[2] == "Unreachable Bulk FC")
                            {
                                continue;
                            }

                            //create an instance
                            var modelClass = new ModelClass();
                            //get data from the array and store it 
                            modelClass.NeId = int.Parse(values[0]);
                            modelClass.Object = values[1];
                            modelClass.time = DateTime.Parse(values[2]);
                            modelClass.Interval_t = int.Parse(values[3]);
                            modelClass.Direction = values[4];
                            modelClass.NeAlias = values[5];
                            modelClass.NeType = values[6];
                            modelClass.RxLevelBelowTS1 = int.Parse(values[7]);
                            modelClass.RxLevelBelowTS2 = int.Parse(values[8]);
                            modelClass.MinRxLevel = float.Parse(values[9]);
                            modelClass.MaxRxLevel = float.Parse(values[10]);
                            modelClass.TxLevelAboveTS1 = int.Parse(values[11]);
                            modelClass.MinTxLevel = float.Parse(values[12]);
                            modelClass.MaxTxLevel = float.Parse(values[13]);
                            modelClass.FailureDescription = values[14];
                            list.Add(modelClass);
                        }
                    }
                }

                //iterate over the list to addd conditions
                foreach (ModelClass r in list)
                {
                    //---------NetworkSId hashed generated column----------------------------
                    using (var md5Hash = MD5.Create())
                    {
                        var hashValue = r.Object + r.NeAlias;
                        MD5 md5Hasher = MD5.Create();
                        var hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(hashValue));
                        r.NetworkSId = BitConverter.ToInt32(hashed, 0);
                    }

                    //---------Create KeytimeKey generated column from filename--------------
                    string OriginalName = Path.GetFileNameWithoutExtension(parseFile);
                    string name = OriginalName.Substring(0, OriginalName.LastIndexOf("_"));
                    name = OriginalName.Substring(name.LastIndexOf("_") + 1).Replace("_", "");
                    DateTime dt = DateTime.ParseExact(name, "yyyyMMddhhmmss", null);
                    r.DatatimeKey = dt;

                    //---------Create TId generated column from the object-------------------
                    string first = r.Object.Substring(r.Object.IndexOf("__") + 2);
                    string result = first.Substring(0, first.IndexOf("_"));
                    r.TId = result;

                    //---------Create FareId generated column from the object-----------------
                    string last = r.Object.Split("__").Last();
                    //string result2 = last.Substring(0, first.IndexOf("__"));
                    r.FarendTId = last;

                    //---------Create Slot/Port/Link generated columns------------------------
                    string removeLast = r.Object.Substring(0, r.Object.IndexOf("_"));
                    if (removeLast.Contains("."))
                    {
                        r.Slot = removeLast.Split(".").First();
                        r.Slot = r.Slot.Split("/").Last();
                        r.Port = removeLast.Split(".").Last();
                        r.Port = r.Port.Split("/").First();
                        r.Link = r.Slot + "/" + r.Port;
                    }
                    else if (removeLast.Contains("+"))
                    {
                        string slot = removeLast.Substring(removeLast.IndexOf("/") + 1);
                        r.Slot = slot.Remove(slot.IndexOf("+"));
                        int s = slot.IndexOf("/");
                        r.Slot2 = slot.Remove(s);
                        r.Slot2 = slot.Substring(slot.IndexOf("+") + 1);
                        //slot 2 value
                        r.Slot2 = r.Slot2.Remove(r.Slot2.IndexOf("/"));
                        r.Port = slot.Substring(slot.IndexOf("/") + 1);
                        r.Link = slot;
                    }
                    else
                    {
                        string linkSlot = removeLast.Substring(removeLast.IndexOf("/") + 1);
                        r.Link = linkSlot;
                        r.Slot = linkSlot.Split("/").First();
                        r.Port = linkSlot.Split("/").Last();
                    }

                    //Add values to the new parsed list
                    parsedlist.Add(r);
                }
                string loadFile = Configuration.GetValue<string>("loadFile");
            string ParsedFile = Configuration.GetValue<string>("fileExist");

            if (File.Exists("parseFile"))
            {
                File.Move(parseFile, ParsedFile);
            }
            using (StreamWriter sw = new StreamWriter(loadFile))
                {
                    foreach (ModelClass r in list)
                    {
                        if (r.Link.Contains("+"))
                        {
                            sw.WriteLine(r.NetworkSId + "," + r.DatatimeKey + "," + r.NeId + "," + r.Object + "," + r.time + "," + r.Interval_t + "," +
                                r.Direction + "," + r.NeAlias + "," + r.NeType + "," + r.RxLevelBelowTS1 + "," + r.RxLevelBelowTS2 + "," + r.MinRxLevel + "," +
                                r.MaxRxLevel + "," + r.TxLevelAboveTS1 + "," + r.MinRxLevel + "," + r.MaxRxLevel + "," + r.FailureDescription + "," + r.Link + "," +
                                r.TId + "," + r.FarendTId + "," + r.Slot + "," + r.Port);
                            sw.WriteLine(r.NetworkSId + "," + r.DatatimeKey + "," + r.NeId + "," + r.Object + "," + r.time + "," + r.Interval_t + "," +
                                r.Direction + "," + r.NeAlias + "," + r.NeType + "," + r.RxLevelBelowTS1 + "," + r.RxLevelBelowTS2 + "," + r.MinRxLevel + "," +
                                r.MaxRxLevel + "," + r.TxLevelAboveTS1 + "," + r.MinRxLevel + "," + r.MaxRxLevel + "," + r.FailureDescription + "," + r.Link + "," +
                                r.TId + "," + r.FarendTId + "," + r.Slot2 + "," + r.Port);
                        }
                        else
                        {
                            sw.WriteLine(r.NetworkSId + "," + r.DatatimeKey + "," + r.NeId + "," + r.Object + "," + r.time + "," + r.Interval_t + "," +
                                r.Direction + "," + r.NeAlias + "," + r.NeType + "," + r.RxLevelBelowTS1 + "," + r.RxLevelBelowTS2 + "," + r.MinRxLevel + "," +
                                r.MaxRxLevel + "," + r.TxLevelAboveTS1 + "," + r.MinRxLevel + "," + r.MaxRxLevel + "," + r.FailureDescription + "," + r.Link + "," +
                                r.TId + "," + r.FarendTId + "," + r.Slot + "," + r.Port);
                        }
                    }
                }
            }





        public void ReadCsv(string path)
        {
            string parseFile = Configuration.GetValue<string>("parseFile");

            var lines = File.ReadAllLines(path);
            var list = new List<ModelClass>();
            var parsedlist = new List<ModelClass>();
            int i = 0;
            foreach (var line in lines)
            {
                if (i == 0)
                {
                    i++;
                    continue;
                }
                var values = line.Split(',');
                if (values.Length == 15)
                {
                    {
                        if (values[14] != "-" || values[2] == "Unreachable Bulk FC")
                        {
                            continue;
                        }
                        //create an instanse
                        var modelClass = new ModelClass();
                        modelClass.NeId = int.Parse(values[0]);
                        modelClass.Object = values[1];
                        modelClass.time = DateTime.Parse(values[2]);
                        modelClass.Interval_t = int.Parse(values[3]);
                        modelClass.Direction = values[4];
                        modelClass.NeAlias = values[5];
                        modelClass.NeType = values[6];
                        modelClass.RxLevelBelowTS1 = int.Parse(values[7]);
                        modelClass.RxLevelBelowTS2 = int.Parse(values[8]);
                        modelClass.MinRxLevel = float.Parse(values[9]);
                        modelClass.MaxRxLevel = float.Parse(values[10]);
                        modelClass.TxLevelAboveTS1 = int.Parse(values[11]);
                        modelClass.MinTxLevel = float.Parse(values[12]);
                        modelClass.MaxTxLevel = float.Parse(values[13]);
                        modelClass.FailureDescription = values[14];
                        list.Add(modelClass);
                    }
                }
            }

            foreach (ModelClass r in list)
            {
                using (var md5Hash = MD5.Create())
                {
                    var hashValue = r.Object + r.NeAlias;
                    MD5 md5Hasher = MD5.Create();
                    var hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(hashValue));
                    r.NetworkSId = BitConverter.ToInt32(hashed, 0);
                }
                //----------  ----------------------------------------------------------------------------
                string OriginalName = Path.GetFileNameWithoutExtension(path);
                string name = OriginalName.Substring(0, OriginalName.LastIndexOf("_"));
                name = OriginalName.Substring(name.LastIndexOf("_") + 1).Replace("_", "");
                DateTime dt = DateTime.ParseExact(name, "yyyyMMddhhmmss", null);
                r.DatatimeKey = dt;
                //--------------------------------------------------------------------------------------
                //r.TimeCreated = DateTime.Now +" "+ Path.GetFileNameWithoutExtension(path);
                //--------------------------------------------------------------------------------------
                string first = r.Object.Substring(r.Object.IndexOf("__") + 2);
                string result = first.Substring(0, first.IndexOf("_"));
                r.TId = result;
                //--------------------------------------------------------------------------------------
                string last = r.Object.Split("__").Last();
                //string result2 = last.Substring(0, first.IndexOf("__"));
                r.FarendTId = last;
                //--------------------------------------------------------------------------------------
                string removeLast = r.Object.Substring(0, r.Object.IndexOf("_"));
                if (removeLast.Contains("."))
                {
                    r.Slot = removeLast.Split(".").First();
                    r.Slot = r.Slot.Split("/").Last();
                    r.Port = removeLast.Split(".").Last();
                    r.Port = r.Port.Split("/").First();
                    r.Link = r.Slot + "/" + r.Port;
                }
                else if (removeLast.Contains("+"))
                {
                    string slot = removeLast.Substring(removeLast.IndexOf("/") + 1);
                    r.Slot = slot.Remove(slot.IndexOf("+"));
                    int s = slot.IndexOf("/");
                    r.Slot2 = slot.Remove(s);
                    r.Slot2 = slot.Substring(slot.IndexOf("+") + 1);
                    //slot 2 value
                    r.Slot2 = r.Slot2.Remove(r.Slot2.IndexOf("/"));
                    r.Port = slot.Substring(slot.IndexOf("/") + 1);
                    r.Link = slot;
                }
                else
                {
                    string linkSlot = removeLast.Substring(removeLast.IndexOf("/") + 1);
                    r.Link = linkSlot;
                    r.Slot = linkSlot.Split("/").First();
                    r.Port = linkSlot.Split("/").Last();
                }
                parsedlist.Add(r);
            }
            string loadFile = Configuration.GetValue<string>("loadFile");

            string ParsedFile = Configuration.GetValue<string>("parsedFile");

            if (!File.Exists("parseFile"))
            {
                File.Copy(parseFile, ParsedFile);
                File.Delete(parseFile);
            }
          


            using (StreamWriter sw = new StreamWriter(loadFile))
            {
                foreach (ModelClass r in list)
                {
                    if (r.Link.Contains("+"))
                    {
                        sw.WriteLine(r.NetworkSId + "," + r.DatatimeKey + "," + r.NeId + "," + r.Object + "," + r.time + "," + r.Interval_t + "," +
                            r.Direction + "," + r.NeAlias + "," + r.NeType + "," + r.RxLevelBelowTS1 + "," + r.RxLevelBelowTS2 + "," + r.MinRxLevel + "," +
                            r.MaxRxLevel + "," + r.TxLevelAboveTS1 + "," + r.MinTxLevel + "," + r.MaxTxLevel + "," + r.FailureDescription + ","+
                            r.TId + "," + r.FarendTId + "," + r.Link + ","  +r.Slot + "," + r.Port);
                        sw.WriteLine(r.NetworkSId + "," + r.DatatimeKey + "," + r.NeId + "," + r.Object + "," + r.time + "," + r.Interval_t + "," +
                            r.Direction + "," + r.NeAlias + "," + r.NeType + "," + r.RxLevelBelowTS1 + "," + r.RxLevelBelowTS2 + "," + r.MinRxLevel + "," +
                            r.MaxRxLevel + "," + r.TxLevelAboveTS1 + "," + r.MinTxLevel + "," + r.MaxTxLevel + "," + r.FailureDescription + ","  +
                            r.TId + "," + r.FarendTId + "," + r.Link + "," + r.Slot2 + "," + r.Port);
                    }
                    else
                    {
                        sw.WriteLine(r.NetworkSId + "," + r.DatatimeKey + "," + r.NeId + "," + r.Object + "," + r.time + "," + r.Interval_t + "," +
                            r.Direction + "," + r.NeAlias + "," + r.NeType + "," + r.RxLevelBelowTS1 + "," + r.RxLevelBelowTS2 + "," + r.MinRxLevel + "," +
                            r.MaxRxLevel + "," + r.TxLevelAboveTS1 + "," + r.MinTxLevel + "," + r.MaxTxLevel + "," + r.FailureDescription + "," +
                            r.TId + "," + r.FarendTId + "," + r.Link + "," + r.Slot + "," + r.Port);
                    }


                }
            }
        }
     }
}
