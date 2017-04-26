using System;
using System.Data.SqlClient;
using System.Diagnostics;

using SqlBulkCopyRunner.Model;

namespace SqlBulkCopyRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            Configuration config = Configuration.Load();

            if (config.batchSize <= 0) {
                Console.Error.WriteLine("Incorrect configuration file content. Please check and adjust it.");
                return;
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            foreach (string table in config.tables) {
                Console.Out.WriteLine("Now copying table: {0}", table);
                bulkCopy(config.sourceConnectionString, config.targetConnectionString, table, config.batchSize, config.notifyAfter);
                //System.Threading.Thread.Sleep(2000);
                Console.Out.WriteLine("");
            }

            stopwatch.Stop();
            long elapsedTime = stopwatch.ElapsedMilliseconds;
            Console.Out.WriteLine("Execution completed in {0} seconds.", elapsedTime / 1000);
        }

        private static void bulkCopy(string sourceConnectionString, string targetConnectionString, string table, int batchSize, int notifyAfter)
        {
            // get the source data
            using (SqlConnection sourceConnection =
                    new SqlConnection(sourceConnectionString)) {
                SqlCommand sql = new SqlCommand("SELECT * FROM " + table, sourceConnection);
                sourceConnection.Open();
                SqlDataReader reader = sql.ExecuteReader();

                // open the destination data
                using (SqlConnection destinationConnection =
                            new SqlConnection(targetConnectionString)) {
                    // open the connection
                    destinationConnection.Open();

                    using (SqlBulkCopy bulkCopy =
                    new SqlBulkCopy(destinationConnection.ConnectionString)) {
                        bulkCopy.BatchSize = batchSize;
                        bulkCopy.NotifyAfter = notifyAfter;
                        bulkCopy.SqlRowsCopied +=
                            new SqlRowsCopiedEventHandler(bulkCopy_SqlRowsCopied);
                        bulkCopy.DestinationTableName = table;
                        bulkCopy.WriteToServer(reader);
                    }
                }
                reader.Close();
            }
        }

        private static void bulkCopy_SqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            Console.Write("=");
        }

    }
}
