namespace Example
{
    using System;
    using System.Collections.Generic;
    using Microsoft.WindowsAzure.Storage.Table;
    using AzureTableClient;

    internal class Program
    {
        private static void Main()
        {
            const string tableName = "GeoLocations";
            const string tableConString = "<YourConnectionString>";
            var tableClient = new AzureTableClient(tableConString, tableName);

            // Insert single entity to table
            tableClient.InsertEntity(new GeoLocation
            {
                Latitude = "41", // Partition key
                Longitude = "29", // Row key
                Continent = "Europe",
                Country = "Turkey",
                City = "Istanbul"
            });

            // Insert multiple entities to the table
            tableClient.InsertEntities(new List<GeoLocation>
            {
                new GeoLocation
                {
                    Latitude = "36", // Partition key
                    Longitude = "140", // Row key
                    Continent = "Asia",
                    Country = "Japan",
                    City = "Tokio"
                },
                new GeoLocation
                {
                    Latitude = "40", // Partition key
                    Longitude = "-74", // Row key
                    Continent = "North America",
                    Country = "United States",
                    City = "New York"
                },
            });

            // Retrieve a single entity. Fast query
            var istanbul = tableClient.GetEntity<GeoLocation>("41", "29");

            // Retrieve multiple entities that have the same partition key. Fast query
            var locationsByLatitude = tableClient.GetEntitiesByPartitionKey<GeoLocation>("41");

            // Retrieve multiple entities that have the same row key. Fast query
            var locationsByLongtitude = tableClient.GetEntitiesByRowKey<GeoLocation>("29");

            // Retrieve entities by custom property (not row or partition key). Slow query
            var locationsByCountry = tableClient.GetEntities<GeoLocation>("Country", "Turkey");
        }
    }

    /// <summary>
    /// The Azure Storage SDK requires entities to be inherited from TableEntity class.
    /// If you don't want to do this, you can use the rest API to implement your own client.
    /// </summary>
    public class GeoLocation : TableEntity
    {
        private string _latitude;
        private string _longtitude;

        public GeoLocation()
        {
            // TableEntity class includes a useful timestamp property
            this.Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Partition key is a unique identifier for the partition within a given table.
        /// Partition key and row key together constitute a unique identifier for an entity on Azure table.
        /// It is indexed by default and you should assign values that you will use in queries to
        /// your partition keys.
        /// </summary>
        public string Latitude
        {
            get { return _latitude; }
            set
            {
                this.PartitionKey = value;
                _latitude = value;
            }
        }

        /// <summary>
        /// The second part of the primary key is the row key, specified by the RowKey property. 
        /// It is indexed by default and you should assign values that you will use in queries to
        /// your row keys.
        /// </summary>
        public string Longitude
        {
            get { return _longtitude; }
            set
            {
                this.RowKey = value;
                _longtitude = value;
            }
        }

        /// <summary>
        /// Custom property, not indexed. Queries with this property will be slow.
        /// </summary>
        public string Continent { get; set; }

        /// <summary>
        /// Custom property, not indexed. Queries with this property will be slow.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Custom property, not indexed. Queries with this property will be slow.
        /// </summary>
        public string City { get; set; }
    }
}
