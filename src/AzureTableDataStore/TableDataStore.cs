﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("AzureTableDataStore.Tests")]

namespace AzureTableDataStore
{
    public class TableDataStore<TData> : ITableDataStore<TData>
    {
        private class Configuration
        {
            public string BlobContainerName { get; set; }
            public PublicAccessType BlobContainerAccessType { get; set; }
            public string StorageTableName { get; set; }
            public string PartitionKeyProperty { get; set; }
            public string RowKeyProperty { get; set; }
        }

        private readonly object _syncLock = new object();
        public string Name { get; private set; }
        private CloudStorageAccount _cloudStorageAccount;
        private BlobServiceClient _blobServiceClient;
        private JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings();

        private Configuration _configuration;
        private bool _containerClientInitialized = false;
        private bool _tableClientInitialized = false;

        private PropertyInfo _entryTypeRowKeyPropertyInfo;
        private PropertyInfo _entryTypePartitionKeyPropertyInfo;

        public EntityPropertyConverterOptions EntityPropertyConverterOptions { get; set; } = new EntityPropertyConverterOptions();


        public TableDataStore(string tableStorageConnectionString, string tableName, string blobContainerName, PublicAccessType blobContainerAccessType,
            string blobStorageConnectionString = null, string storeName = null, string partitionKeyProperty = null, string rowKeyProperty = null)
        {
            Name = storeName ?? "default";
            
            
            _cloudStorageAccount = CloudStorageAccount.Parse(tableStorageConnectionString);
            _blobServiceClient = new BlobServiceClient(blobStorageConnectionString ?? tableStorageConnectionString);
            _configuration = new Configuration()
            {
                BlobContainerAccessType = blobContainerAccessType,
                BlobContainerName = blobContainerName,
                StorageTableName = tableName,
                PartitionKeyProperty = ResolvePartitionKeyProperty(partitionKeyProperty),
                RowKeyProperty = ResolveRowKeyProperty(rowKeyProperty)
            };
            PostConfigure();
        }

        public TableDataStore(StorageCredentials tableStorageCredentials, StorageUri tableStorageUri, string tableName,
            StorageSharedKeyCredential blobStorageCredentials, Uri blobStorageServiceUri, string blobContainerName, PublicAccessType blobContainerAccessType,
            string storeName = null, string partitionKeyProperty = null, string rowKeyProperty = null)
        {
            Name = storeName ?? "default";
            _cloudStorageAccount = new CloudStorageAccount(tableStorageCredentials, tableStorageUri);
            _blobServiceClient = new BlobServiceClient(blobStorageServiceUri, blobStorageCredentials);
            _configuration = new Configuration()
            {
                BlobContainerAccessType = blobContainerAccessType,
                BlobContainerName = blobContainerName,
                StorageTableName = tableName,
                PartitionKeyProperty = ResolvePartitionKeyProperty(partitionKeyProperty),
                RowKeyProperty = ResolveRowKeyProperty(rowKeyProperty)
            };
            PostConfigure();
        }

        private void PostConfigure()
        {
            _entryTypeRowKeyPropertyInfo = typeof(TData).GetProperty(_configuration.RowKeyProperty);
            _entryTypePartitionKeyPropertyInfo = typeof(TData).GetProperty(_configuration.PartitionKeyProperty);
        }

        private string ResolvePartitionKeyProperty(string inputPartitionKeyProperty)
        {
            var entryType = typeof(TData);
            var properties = entryType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            if (!string.IsNullOrEmpty(inputPartitionKeyProperty))
            {    
                if(properties.All(x => x.Name != inputPartitionKeyProperty))
                    throw new AzureTableDataStoreException($"Given partition key property name '{inputPartitionKeyProperty}' " +
                        $"is not a property in the data type '{entryType.Name}', please specify a valid property to act as partition key!",
                        AzureTableDataStoreException.ProblemSourceType.Configuration);

                return inputPartitionKeyProperty;
            }

            var partitionKeyProperty = properties.FirstOrDefault(x => x.GetCustomAttributes(typeof(TablePartitionKeyAttribute)).Any());
            if (partitionKeyProperty != null)
                return partitionKeyProperty.Name;

            throw new AzureTableDataStoreException($"Unable to resolve partition key for Type '{entryType.Name}', " +
                $"no explicit partition key was provided in {nameof(TableDataStore<TData>)} constructor and the Type has " +
                $"no property with the '{nameof(TablePartitionKeyAttribute)}' attribute.",
                AzureTableDataStoreException.ProblemSourceType.Configuration);
        }

        private string ResolveRowKeyProperty(string inputRowKeyProperty)
        {
            var entryType = typeof(TData);
            var properties = entryType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            if (!string.IsNullOrEmpty(inputRowKeyProperty))
            {
                if (properties.All(x => x.Name != inputRowKeyProperty))
                    throw new AzureTableDataStoreException($"Given row key property name '{inputRowKeyProperty}' " +
                        $"is not a property in the data type '{entryType.Name}', please specify a valid property to act as row key!");

                return inputRowKeyProperty;
            }

            var rowKeyProperty = properties.FirstOrDefault(x => x.GetCustomAttributes(typeof(TableRowKeyAttribute)).Any());
            if (rowKeyProperty != null)
                return rowKeyProperty.Name;

            throw new AzureTableDataStoreException($"Unable to resolve row key for Type '{entryType.Name}', " +
                $"no explicit row key was provided in {nameof(TableDataStore<TData>)} constructor and the Type has " +
                $"no property with the '{nameof(TableRowKeyAttribute)}' attribute.");
        }

        private (string partitionKey, string rowKey) GetEntryKeys(TData entry) =>
            (_entryTypePartitionKeyPropertyInfo.GetValue(entry).ToString(), _entryTypeRowKeyPropertyInfo.GetValue(entry).ToString());

        public async Task InsertAsync(params TData[] entries)
        {
            switch (entries?.Length)
            {
                case 0:
                    return;
                case 1:
                    await InsertOneAsync(entries[0]);
                    break;
                default:
                    await InsertBatched(entries);
                    return;
            }
        }

        private BlobContainerClient GetContainerClient()
        {
            lock (_syncLock)
            {
                if (!_containerClientInitialized)
                {
                    try
                    {
                        _blobServiceClient
                            .GetBlobContainerClient(_configuration.BlobContainerName)
                            .CreateIfNotExists(_configuration.BlobContainerAccessType);
                        _containerClientInitialized = true;
                    }
                    catch (Exception e)
                    {
                        throw new AzureTableDataStoreException("Unable to initialize blob container (CreateIfNotExists)",
                            AzureTableDataStoreException.ProblemSourceType.BlobStorage, e);
                    }
                }
            }

            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_configuration.BlobContainerName);
            return blobContainerClient;
        }

        private CloudTable GetTable()
        {
            lock (_syncLock)
            {
                try
                {
                    if (!_tableClientInitialized)
                    {
                        var cloudTableClient = _cloudStorageAccount.CreateCloudTableClient();
                        var tableRef = cloudTableClient.GetTableReference(_configuration.StorageTableName);
                        tableRef.CreateIfNotExists();
                        _tableClientInitialized = true;
                        return tableRef;
                    }
                }
                catch (Exception e)
                {
                    throw new AzureTableDataStoreException("Unable to initialize table (CreateIfNotExists)",
                        AzureTableDataStoreException.ProblemSourceType.TableStorage, e);
                }
                
            }

            return _cloudStorageAccount.CreateCloudTableClient()
                .GetTableReference(_configuration.StorageTableName);
        }

        private void StripSpeciallyHandledProperties(List<ReflectionUtils.PropertyRef> propertyRefs)
        {
            // Set these values to null to not attempt their serialization with EntityPropertyConverter.
            // Otherwise the conversion will be attempted, and an exception thrown.
            // We will set them back to what they were after we've performed the serialization.
            // It's not nice, but I can live with it.

            foreach (var propRef in propertyRefs)
                propRef.Property.SetValue(propRef.SourceObject, null);
        }

        private void RestoreSpeciallyHandledProperties(List<ReflectionUtils.PropertyRef> propertyRefs)
        {
            // Restore the specially serialized values back in their place.

            foreach (var propRef in propertyRefs)
                propRef.Property.SetValue(propRef.SourceObject, propRef.StoredInstanceAsObject);
        }

        private async Task InsertOneAsync(TData entry)
        {
            try
            {
                var blobPropertyRefs =
                    ReflectionUtils.GatherPropertiesWithBlobsRecursive(entry, EntityPropertyConverterOptions);
                var collectionPropertyRefs =
                    ReflectionUtils.GatherPropertiesWithCollectionsRecursive(entry, EntityPropertyConverterOptions);
                var allSpecialPropertyRefs = blobPropertyRefs.Cast<ReflectionUtils.PropertyRef>()
                    .Concat(collectionPropertyRefs).ToList();

                StripSpeciallyHandledProperties(allSpecialPropertyRefs);
                var propertyDictionary = EntityPropertyConverter.Flatten(entry, EntityPropertyConverterOptions, null);

                var entryKeys = GetEntryKeys(entry);
                var uploadTasks = blobPropertyRefs
                    .Select(x => UploadBlobAndUpdateReference(x, entryKeys.partitionKey, entryKeys.rowKey)).ToArray();
                await Task.WhenAll(uploadTasks);

                collectionPropertyRefs.ForEach(@ref =>
                    propertyDictionary.Add(@ref.FlattenedPropertyName, EntityProperty.GeneratePropertyForString(
                        JsonConvert.SerializeObject(@ref.StoredInstance, _jsonSerializerSettings))));

                blobPropertyRefs.ForEach(@ref =>
                    propertyDictionary.Add(@ref.FlattenedPropertyName, EntityProperty.GeneratePropertyForString(
                        JsonConvert.SerializeObject(@ref.StoredInstance, _jsonSerializerSettings))));

                RestoreSpeciallyHandledProperties(allSpecialPropertyRefs);

                var tableRef = GetTable();

                var entityRowKey = propertyDictionary[_configuration.RowKeyProperty];
                var entityPartitionKey = propertyDictionary[_configuration.PartitionKeyProperty];
                var tableEntity = new DynamicTableEntity(entityPartitionKey.StringValue, entityRowKey.StringValue, "*",
                    propertyDictionary);

                var insertOp = TableOperation.Insert(tableEntity);
                await tableRef.ExecuteAsync(insertOp);
            }
            catch (AzureTableDataStoreException)
            {
                throw;
            }
            catch (SerializationException e)
            {
                throw new AzureTableDataStoreException("Serialization of the data failed", 
                    AzureTableDataStoreException.ProblemSourceType.Data, e);
            }
            catch (Exception e)
            {
                if(e.GetType().Namespace.StartsWith("Microsoft.Azure.Cosmos"))
                    throw new AzureTableDataStoreException("Insert operation failed, outlying Table Storage threw an exception", 
                        AzureTableDataStoreException.ProblemSourceType.TableStorage, e);
                if(e.GetType().Namespace.StartsWith("Azure.Storage") || e is Azure.RequestFailedException)
                    throw new AzureTableDataStoreException("Insert operation failed and entry was not inserted, outlying Blob Storage threw an exception", 
                        AzureTableDataStoreException.ProblemSourceType.BlobStorage, e);
                throw new AzureTableDataStoreException("Insert operation failed, unhandlable exception",
                    AzureTableDataStoreException.ProblemSourceType.General, e);
            }
        }

        private async Task UploadBlobAndUpdateReference(ReflectionUtils.PropertyRef<StoredBlob> blobPropRef, string partitionKey, string rowKey)
        {
            var containerClient = GetContainerClient();
            var blobPath = string.Join("/",
                               _configuration.StorageTableName,
                               partitionKey,
                               rowKey,
                               blobPropRef.FlattenedPropertyName,
                               blobPropRef.StoredInstance.Filename);

            var uploadResponse = await containerClient.UploadBlobAsync(blobPath, blobPropRef.StoredInstance.DataStream.Value);
            // Should we compare the hashes just in case?
            blobPropRef.StoredInstance.DataStream.Value.Seek(0, SeekOrigin.Begin);
            var props = await containerClient.GetBlobClient(blobPath).GetPropertiesAsync();
            blobPropRef.StoredInstance.Length = props.Value.ContentLength;
            blobPropRef.StoredInstance.ContentType = props.Value.ContentType;
        }

        private async Task InsertBatched(TData[] entries)
        {

        }

        public Task UpsertAsync(params TData[] entries)
        {
            throw new NotImplementedException();
        }

        public Task<IList<TData>> FindAsync(Expression<Func<TData, bool>> queryExpression)
        {
            throw new NotImplementedException();
        }

        public Task<IList<TData>> FindAsync(string query)
        {
            throw new NotImplementedException();
        }

        public async Task<TData> GetAsync(Expression<Func<TData, bool>> queryExpression)
        {
            var filterString = AzureStorageQueryTranslator.TranslateExpression(queryExpression, 
                _configuration.PartitionKeyProperty, _configuration.RowKeyProperty);
            //TODO
            return default(TData);
        }

        public Task<TData> GetAsync(string query)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(params TData[] entries)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(params string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Expression<Func<TData, bool>> queryExpression)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string query)
        {
            throw new NotImplementedException();
        }

        public Task EnumerateAsync(Func<TData, Task> enumeratorFunc)
        {
            throw new NotImplementedException();
        }

        public Task EnumerateAsync(Expression<Func<TData, bool>> queryExpression, Func<TData, Task> enumeratorFunc)
        {
            throw new NotImplementedException();
        }

        public Task EnumerateAsync(string query, Func<TData, Task> enumeratorFunc)
        {
            throw new NotImplementedException();
        }
    }
}