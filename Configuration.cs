using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

namespace SqlBulkCopyRunner.Model
{
    [Serializable]
    public class Configuration
    {
        public List<string> tables;
        // @"Server=localhost;Database=WIP;Trusted_Connection=true;User ID=WIP;Password=<password>"
        public string sourceConnectionString;
        public string targetConnectionString;

        public int batchSize;
        public int notifyAfter;

        private static string CONFIG_FILE = "config.json";

        public static Configuration Load()
        {
            try
            {
                string configContent = File.ReadAllText(CONFIG_FILE);
                Configuration config = JsonConvert.DeserializeObject<Configuration>(configContent);
                return config;
            }
            catch (Exception)
            {
                Console.Error.WriteLine("Exception thrown when reading configuration file.");
                return new Configuration {
                    batchSize = -1  // invalid value which is <= 0
                };
            }
        }

    }
}
