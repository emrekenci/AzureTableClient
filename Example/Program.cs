using System;

namespace Example
{
    using Microsoft.WindowsAzure.Storage.Table;
    using AzureTableClient;

    internal class Program
    {
        static void Main()
        {
            var tableName = "TableName";
            var tableConString = "ConnectionString";
            var tableClient = new AzureTableClient(tableConString, tableName);
            var student = new Student("Kenci", "0x23423");
            tableClient.InsertEntity(student);
        }
    }

    public class Student : TableEntity
    {
        /// <summary>
        ///  Partition and rowkey pairs uniquely identify an entity on an Azure Table.
        /// </summary>
        /// <param name="lastName"></param>
        /// <param name="studentId"></param>
        public Student(string lastName, string studentId)
        {
            // The partition key is a unique identifier for the partition within a given table
            PartitionKey = lastName;

            // The second part of the primary key is the row key, specified by the RowKey property. 
            RowKey = studentId;

            // The Timestamp property is a DateTime value that is maintained on the server side to record the time an entity was last modified. 
            Timestamp = DateTimeOffset.UtcNow;
        }

        public string S { get; set; }
    }
}
