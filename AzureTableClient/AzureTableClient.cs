namespace AzureTableClient
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    public class AzureTableClient
    {
        public CloudTable Table { get; set; }

        /// <summary>
        /// Initializes the table client by the connection string and table name
        /// </summary>
        /// <param name="storageAccountConnectionString">Your connection string to Azure Storage account</param>
        /// <param name="tableName">The name of the Azure Table that this service instance will use</param>
        public AzureTableClient(string storageAccountConnectionString, string tableName)
        {
            var storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            Table = tableClient.GetTableReference(tableName);
            Table.CreateIfNotExists();
        }

        /// <summary>
        /// Inserts a single Entity into Azure Table.
        /// </summary>
        /// <typeparam name="T">A class that inherits from TableEntity class</typeparam>
        /// <param name="entity">The entity which will be inserted to the table</param>
        public void InsertEntity<T>(T entity) where T : TableEntity
        {
            var insertOrReplaceOperation = TableOperation.InsertOrReplace(entity);
            Table.Execute(insertOrReplaceOperation);
        }

        /// <summary>
        /// Inserts a list of entities to Azure Table in a single batch operation
        /// </summary>
        /// <typeparam name="T">A class that inherits from TableEntity class</typeparam>
        /// <param name="entities">A list of Entities which will be inserted to the table</param>
        public void InsertEntities<T>(IList<T> entities) where T : TableEntity
        {
            var batchOperation = new TableBatchOperation();
            foreach (var entity in entities) batchOperation.Insert(entity);
            Table.ExecuteBatch(batchOperation);
        }

        /// <summary>
        /// Given a partition key and row key returns the entity with that partition key and row key
        /// </summary>
        /// <typeparam name="T">A class that inherits from TableEntity class</typeparam>
        /// <param name="partitionKey">The partition key of the entity</param>
        /// <param name="rowKey">The row key of the entity</param>
        /// <returns>The table entity with the given partition and row key</returns>
        public T GetEntity<T>(string partitionKey, string rowKey) where T : TableEntity
        {
            var operation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            var operationResult = Table.Execute(operation);
            return operationResult.Result as T;
        }

        /// <summary>
        /// Given a partition key, returns the entities with that partition key sorted by Timestamp
        /// </summary>
        /// <typeparam name="T">A class that inherits from TableEntity class</typeparam>
        /// <param name="partitionKey">The partition key we're searching with</param>
        /// <returns>List of TableEntities with the given partition key sorted by Timestamp</returns>
        public List<T> GetEntitiesByPartitionKey<T>(string partitionKey) where T : TableEntity, new()
        {
            var query = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));
            var result = Table.ExecuteQuery(query).OrderByDescending(record => record.Timestamp).ToList();
            return result;
        }

        /// <summary>
        /// Given a row key, returns the entities with that row key sorted by Timestamp
        /// </summary>
        /// <typeparam name="T">A class that inherits from TableEntity class</typeparam>
        /// <param name="rowKey">The row key we're searching with</param>
        /// <returns>List of TableEntities with the given row key sorted by Timestamp</returns>
        public List<T> GetEntitiesByRowKey<T>(string rowKey) where T : TableEntity, new()
        {
            var query = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowKey));
            var result = Table.ExecuteQuery(query).OrderByDescending(record => record.Timestamp).ToList();
            return result;
        }

        /// <summary>
        /// Returns the TableEntities that match the given property name and value. 
        /// </summary>
        /// <typeparam name="T">A class that inherits from TableEntity class</typeparam>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="propertyValue">The value of the property</param>
        /// <returns>TableEntities that match the given property name and value sorted by Timestamp</returns>
        public List<T> GetEntities<T>(string propertyName, string propertyValue) where T : TableEntity, new()
        {
            var query = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition(propertyName, QueryComparisons.Equal, propertyValue));
            var result = Table.ExecuteQuery(query).OrderByDescending(record => record.Timestamp).ToList();
            return result;
        }

        /// <summary>
        /// Given a dictionary of Property names and values, finds the entities with the given properties and values
        /// </summary>
        /// <typeparam name="T">A class that inherits from TableEntity class</typeparam>
        /// <param name="propertyValuePairs">Dictionary of property value pairs. The key must be the Propert name, the value must be the property value</param>
        /// <returns>A list of entities with the given properties and values</returns>
        public List<T> GetEntities<T>(Dictionary<string, string> propertyValuePairs) where T : TableEntity, new()
        {
            var query = new TableQuery<T>();
            foreach (var propertyValuePair in propertyValuePairs)
            {
                query = query.Where(TableQuery.GenerateFilterCondition(propertyValuePair.Key, QueryComparisons.Equal, propertyValuePair.Value));
            }
            var result = Table.ExecuteQuery(query).ToList();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="partitionKey"></param>
        /// <param name="rowKey"></param>
        /// <returns></returns>
        public bool DeleteEntity<T>(string partitionKey, string rowKey) where T : TableEntity
        {
            var retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            var retrievedResult = Table.Execute(retrieveOperation);
            var entityToDelete = (T)retrievedResult.Result;
            if (entityToDelete == null) return false;
            var deleteOperation = TableOperation.Delete(entityToDelete);
            Table.Execute(deleteOperation);
            return true;
        }

        /// <summary>
        /// Deletes the current table if it exists.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the table was deleted; otherwise, <c>false</c>.
        /// </returns>
        public bool DeleteTable()
        {
            return Table.DeleteIfExists();
        }
    }
}
