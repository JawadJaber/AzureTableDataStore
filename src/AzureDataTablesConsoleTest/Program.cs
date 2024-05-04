using Azure.Data.Tables;
using Azure;
using System.Text.Json;
using System.Runtime.Serialization;

namespace AzureDataTablesConsoleTest
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            string TableConnectingString = "DefaultEndpointsProtocol=https;AccountName=sadhanstorage;AccountKey=8DIuxqaDAa8Bimj+h/15V/pjqvKBsBTel1TOZoDGU13kzReQK5ipTa0Vv5AzlSSSBOCSKTaRRbgp8moR0TxkOw==;EndpointSuffix=core.windows.net";

            // Usage
            var tableClient = new TableClient(TableConnectingString, "testTable");
            var entity = new MyEntity
            {
                PartitionKey = "partition",
                RowKey = "row",
                ComplexProperty = new ComplexType { Id = 1, Description = "Example" }
            };

            await tableClient.CreateIfNotExistsAsync();

           // await tableClient.UpdateEntityAsync(entity,entity.ETag);


            var entity2 = await tableClient.GetEntityAsync<MyEntity>("partition", "row");


            Console.WriteLine("Hello, World!");

            Console.ReadLine();
        }


        public class MyEntity : ITableEntity
        {
            public string PartitionKey { get; set; }
            public string RowKey { get; set; }
            public DateTimeOffset? Timestamp { get; set; }
            public ETag ETag { get; set; }

            // This property will be stored as a JSON string
            public string ComplexPropertyJson { get; set; }



            // Non-stored, helper property to manage the complex object
            [IgnoreDataMember]
            public ComplexType ComplexProperty
            {
                get => JsonSerializer.Deserialize<ComplexType>(ComplexPropertyJson);
                set => ComplexPropertyJson = JsonSerializer.Serialize(value);
            }
        }

        public class ComplexType
        {
            public int Id { get; set; }
            public string Description { get; set; }
        }
    }
}
