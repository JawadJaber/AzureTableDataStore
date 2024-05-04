

// -----------------------------------------------------------------------------------------
// <copyright file="EntityPropertyConverter.cs" company="Microsoft">
//    Copyright 2013 Microsoft Corporation
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//      http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>
// -----------------------------------------------------------------------------------------

namespace AzureTableDataStore
{
    using Microsoft.Azure.Cosmos.Table;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

#if NETCORE
    using Microsoft.WindowsAzure.Storage.Extensions;
#endif


    internal class SR
    {
        public const string ArgumentEmptyError = "The argument must not be empty string.";

        public const string ArgumentOutOfRangeError = "The argument is out of range. Value passed: {0}";

        public const string ArgumentTooLargeError = "The argument '{0}' is larger than maximum of '{1}'";

        public const string ArgumentTooSmallError = "The argument '{0}' is smaller than minimum of '{1}'";

        public const string AttemptedEdmTypeForTheProperty = "Attempting to deserialize '{0}' as type '{1}'";

        public const string BatchWithRetreiveContainsOtherOperations = "A batch transaction with a retrieve operation cannot contain any other operations.";

        public const string BatchExceededMaximumNumberOfOperations = "The maximum number of operations allowed in one batch has been exceeded.";

        public const string BatchOperationRequiresPartitionKeyRowKey = "A batch non-retrieve operation requires a non-null partition key and row key.";

        public const string BatchErrorInOperation = "Element {0} in the batch returned an unexpected response code.";

        public const string BlobTypeMismatch = "Blob type of the blob reference doesn't match blob type of the blob.";

        public const string BufferManagerProvidedIncorrectLengthBuffer = "The IBufferManager provided an incorrect length buffer to the stream, Expected {0}, received {1}. Buffer length should equal the value returned by IBufferManager.GetDefaultBufferSize().";

        public const string CannotCreateSASSignatureForGivenCred = "Cannot create Shared Access Signature as the credentials does not have account name information. Please check that the credentials used support creating Shared Access Signature.";

        public const string CannotCreateSASWithoutAccountKey = "Cannot create Shared Access Signature unless Account Key credentials are used.";

        public const string Container = "container";

        public const string DelegatingHandlerNonNullInnerHandler = "Innermost DelegatingHandler must have a null InnerHandler.";

        public const string EmptyBatchOperation = "Cannot execute an empty batch operation";

        public const string ETagMissingForDelete = "Delete requires an ETag (which may be the '*' wildcard).";

        public const string ETagMissingForMerge = "Merge requires an ETag (which may be the '*' wildcard).";

        public const string ETagMissingForReplace = "Replace requires an ETag (which may be the '*' wildcard).";

        public const string ExtendedErrorUnavailable = "An unknown error has occurred, extended error information not available.";

        public const string File = "file";

        public const string FailParseProperty = "Failed to parse property '{0}' with value '{1}' as type '{2}'";

        public const string GetServiceStatsInvalidOperation = "GetServiceStats cannot be run with a 'PrimaryOnly' location mode.";

        public const string InternalStorageError = "Unexpected internal storage client error.";

        public const string InvalidCorsRule = "A CORS rule must contain at least one allowed origin and allowed method, and MaxAgeInSeconds cannot have a value less than zero.";

        public const string InvalidLoggingLevel = "Invalid logging operations specified.";

        public const string InvalidMetricsLevel = "Invalid metrics level specified.";

        public const string InvalidDeleteRetentionDaysValue = "The delete retention policy is enabled but the RetentionDays property is not specified or has an invalid value. RetentionDays must be greater than 0 and less than or equal to 365 days.";

        public const string InvalidProtocolsInSAS = "Invalid value {0} for the SharedAccessProtocol parameter when creating a SharedAccessSignature.  Use 'null' if you do not wish to include a SharedAccessProtocol.";

        public const string InvalidTypeInJsonDictionary = "Invalid type in JSON object. Detected type is {0}, which is not a valid JSON type.";

        public const string IQueryableExtensionObjectMustBeTableQuery = "Query must be a TableQuery<T>";

        public const string JsonReaderNotInCompletedState = "The JSON reader has not yet reached the completed state.";

        public const string LoggingVersionNull = "The logging version is null or empty.";

        public const string MetricVersionNull = "The metrics version is null or empty.";

        public const string MissingAccountInformationInUri = "Cannot find account information inside Uri '{0}'";

        public const string MissingCredentials = "No credentials provided.";

        public const string StorageUriMustMatch = "Primary and secondary location URIs in a StorageUri must point to the same resource.";

        public const string NegativeBytesRequestedInCopy = "Internal Error - negative copyLength requested when attempting to copy a stream.  CopyLength = {0}, totalBytes = {1}.";

        public const string NoPropertyResolverAvailable = "No property resolver available. Deserializing the entity properties as strings.";

        public const string OperationCanceled = "Operation was canceled by user.";

        public const string ParseError = "Error parsing value";

        public const string PartitionKey = "All entities in a given batch must have the same partition key.";

        public const string PayloadFormat = "Setting payload format for the request to '{0}'.";

        public const string PropertyDelimiterExistsInPropertyName = "Property delimiter: {0} exists in property name: {1}. Object Path: {2}";

        public const string PropertyResolverCacheDisabled = "Property resolver cache is disabled.";

        public const string PropertyResolverThrewError = "The custom property resolver delegate threw an exception. Check the inner exception for more details.";

        public const string RecursiveReferencedObject = "Recursive reference detected. Object Path: {0} Property Type: {1}.";

        public const string RelativeAddressNotPermitted = "Address '{0}' is a relative address. Only absolute addresses are permitted.";

        public const string RetrieveWithContinuationToken = "Retrieved '{0}' results with continuation token '{1}'.";

        public const string SetServicePropertiesRequiresNonNullSettings = "At least one service property needs to be non-null for SetServiceProperties API.";

        public const string Share = "share";

        public const string StreamLengthError = "The length of the stream exceeds the permitted length.";

        public const string StreamLengthMismatch = "Cannot specify both copyLength and maxLength.";

        public const string StreamLengthShortError = "The requested number of bytes exceeds the length of the stream remaining from the specified position.";

        public const string Table = "table";

        public const string TableEndPointNotConfigured = "No table endpoint configured.";

        public const string TableQueryDynamicPropertyAccess = "Accessing property dictionary of DynamicTableEntity requires a string constant for property name.";

        public const string TableQueryEntityPropertyInQueryNotSupported = "Referencing {0} on EntityProperty only supported with properties dictionary exposed via DynamicTableEntity.";

        public const string TableQueryMustHaveQueryProvider = "Unknown Table. The TableQuery does not have an associated CloudTable Reference. Please execute the query via the CloudTable ExecuteQuery APIs.";

        public const string TableQueryTypeMustImplementITableEnitty = "TableQuery Generic Type must implement the ITableEntity Interface";

        public const string TableQueryTypeMustHaveDefaultParameterlessCtor = "TableQuery Generic Type must provide a default parameterless constructor.";

        public const string TakeCountNotPositive = "Take count must be positive and greater than 0.";

        public const string TimeoutExceptionMessage = "The client could not finish the operation within specified timeout.";

        public const string TraceDownloadError = "Downloading error response body.";

        public const string TraceRetryInfo = "The extended retry policy set the next location to {0} and updated the location mode to {1}.";

        public const string TraceGenericError = "Exception thrown during the operation: {0}.";

        public const string TraceGetResponse = "Waiting for response.";

        public const string TraceIgnoreAttribute = "Omitting property '{0}' from serialization/de-serialization because IgnoreAttribute has been set on that property.";

        public const string TraceInitLocation = "Starting operation with location {0} per location mode {1}.";

        public const string TraceInitRequestError = "Exception thrown while initializing request: {0}.";

        public const string TraceMissingDictionaryEntry = "Omitting property '{0}' from de-serialization because there is no corresponding entry in the dictionary provided.";

        public const string TraceNextLocation = "The next location has been set to {0}, based on the location mode.";

        public const string TraceNonPublicGetSet = "Omitting property '{0}' from serialization/de-serialization because the property's getter/setter are not public.";

        public const string TraceNonExistingGetter = "Omitting property: {0} from serialization/de-serialization because the property does not have a getter. Object path: {1}";

        public const string TraceNonExistingSetter = "Omitting property: {0} from serialization/de-serialization because the property does not have a setter. The property needs to have at least a private setter. Object Path: {1}";

        public const string TracePreProcessDone = "Response headers were processed successfully, proceeding with the rest of the operation.";

        public const string TraceResponse = "Response received. Status code = {0}, Request ID = {1}, Content-MD5 = {2}, ETag = {3}.";

        public const string TraceRetry = "Retrying failed operation.";

        public const string TraceRetryCheck = "Checking if the operation should be retried. Retry count = {0}, HTTP status code = {1}, Retryable exception = {2}, Exception = {3}.";

        public const string TraceRetryDecisionPolicy = "Retry policy did not allow for a retry. Failing with {0}.";

        public const string TraceRetryDecisionTimeout = "Operation cannot be retried because the maximum execution time has been reached. Failing with {0}.";

        public const string TraceRetryDelay = "Operation will be retried after {0}ms.";

        public const string TraceSetPropertyError = "Exception thrown while trying to set property value. Property Path: {0} Property Value: {1}. Exception Message: {2}";

        public const string TraceStartRequestAsync = "Starting asynchronous request to {0}.";

        public const string TraceStringToSign = "StringToSign = {0}.";

        public const string UnexpectedEDMType = "Unexpected EDM type from the Table Service: {0}.";

        public const string UnexpectedParameterInSAS = "The parameter `api-version` should not be included in the SAS token. Please allow the library to set the  `api-version` parameter.";

        public const string UnexpectedResponseCode = "Unexpected response code, Expected:{0}, Received:{1}";

        public const string UnsupportedPropertyTypeForEntityPropertyConversion = "Unsupported type : {0} encountered during conversion to EntityProperty. Object Path: {1}";

        public const string UsingDefaultPropertyResolver = "Using the default property resolver to deserialize the entity.";

        public const string UsingUserProvidedPropertyResolver = "Using the property resolver provided via TableRequestOptions to deserialize the entity.";

        public const string ALinqCouldNotConvert = "Could not convert constant {0} expression to string.";

        public const string ALinqMethodNotSupported = "The method '{0}' is not supported.";

        public const string ALinqUnaryNotSupported = "The unary operator '{0}' is not supported.";

        public const string ALinqBinaryNotSupported = "The binary operator '{0}' is not supported.";

        public const string ALinqConstantNotSupported = "The constant for '{0}' is not supported.";

        public const string ALinqTypeBinaryNotSupported = "An operation between an expression and a type is not supported.";

        public const string ALinqConditionalNotSupported = "The conditional expression is not supported.";

        public const string ALinqParameterNotSupported = "The parameter expression is not supported.";

        public const string ALinqMemberAccessNotSupported = "The member access of '{0}' is not supported.";

        public const string ALinqLambdaNotSupported = "Lambda Expressions not supported.";

        public const string ALinqNewNotSupported = "New Expressions not supported.";

        public const string ALinqMemberInitNotSupported = "Member Init Expressions not supported.";

        public const string ALinqListInitNotSupported = "List Init Expressions not supported.";

        public const string ALinqNewArrayNotSupported = "New Array Expressions not supported.";

        public const string ALinqInvocationNotSupported = "Invocation Expressions not supported.";

        public const string ALinqUnsupportedExpression = "The expression type {0} is not supported.";

        public const string ALinqCanOnlyProjectTheLeaf = "Can only project the last entity type in the query being translated.";

        public const string ALinqCantCastToUnsupportedPrimitive = "Can't cast to unsupported type '{0}'";

        public const string ALinqCantTranslateExpression = "The expression {0} is not supported.";

        public const string ALinqCantNavigateWithoutKeyPredicate = "Navigation properties can only be selected from a single resource. Specify a key predicate to restrict the entity set to a single instance.";

        public const string ALinqCantReferToPublicField = "Referencing public field '{0}' not supported in query option expression.  Use public property instead.";

        public const string ALinqCannotConstructKnownEntityTypes = "Construction of entity type instances must use object initializer with default constructor.";

        public const string ALinqCannotCreateConstantEntity = "Referencing of local entity type instances not supported when projecting results.";

        public const string ALinqExpressionNotSupportedInProjectionToEntity = "Initializing instances of the entity type {0} with the expression {1} is not supported.";

        public const string ALinqExpressionNotSupportedInProjection = "Constructing or initializing instances of the type {0} with the expression {1} is not supported.";

        public const string ALinqProjectionMemberAssignmentMismatch = "Cannot initialize an instance of entity type '{0}' because '{1}' and '{2}' do not refer to the same source entity.";

        public const string ALinqPropertyNamesMustMatchInProjections = "Cannot assign the value from the {0} property to the {1} property.  When projecting results into a entity type, the property names of the source type and the target type must match for the properties being projected.";

        public const string ALinqQueryOptionOutOfOrder = "The {0} query option cannot be specified after the {1} query option.";

        public const string ALinqQueryOptionsOnlyAllowedOnLeafNodes = "Can only specify query options (orderby, where, take, skip) after last navigation.";
    }


    internal static class Logger
    {
        public static Guid ProviderId;

        private static TraceSource TraceSourceInternal;

        static Logger()
        {
            ProviderId = new Guid("C58AB3E3-D96B-4BDD-B085-E71304E8C75F");
            TraceSourceInternal = new TraceSource("Microsoft.Azure.Cosmos.Table");
            Trace.UseGlobalLock = false;
            SourceSwitch @switch = new SourceSwitch("ClientSwitch", "Information");
            TraceSourceInternal.Switch = @switch;
        }

        internal static void LogError(OperationContext operationContext, string format, params object[] args)
        {
            TraceSourceInternal.TraceEvent(TraceEventType.Error, 0, FormatLine(operationContext, format, args));
        }

        internal static void LogWarning(OperationContext operationContext, string format, params object[] args)
        {
            TraceSourceInternal.TraceEvent(TraceEventType.Warning, 0, FormatLine(operationContext, format, args));
        }

        internal static void LogInformational(OperationContext operationContext, string format, params object[] args)
        {
            TraceSourceInternal.TraceInformation(FormatLine(operationContext, format, args));
        }

        internal static void LogVerbose(OperationContext operationContext, string format, params object[] args)
        {
            TraceSourceInternal.TraceEvent(TraceEventType.Verbose, 0, FormatLine(operationContext, format, args));
        }

        private static string FormatLine(OperationContext operationContext, string format, object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}: {1}", (operationContext == null) ? "*" : operationContext.ClientRequestID, (args == null) ? format : string.Format(CultureInfo.InvariantCulture, format, args).Replace('\n', '.'));
        }
    }

    /// <summary>
    /// EntityPropertyConverter class.
    /// </summary>
    public static class EntityPropertyConverter
    {
        /// <summary>
        /// The property delimiter.
        /// </summary>
        public const string DefaultPropertyNameDelimiter = "_";

        /// <summary>
        /// Traverses object graph, flattens and converts all nested (and not nested) properties to EntityProperties, stores them in the property dictionary.
        /// The keys are constructed by appending the names of the properties visited during pre-order depth first traversal from root to each end node property delimited by '_'.
        /// Allows complex objects to be stored in persistent storage systems or passed between web services in a generic way.
        /// </summary>
        /// <param name="root">The object to flatten and convert.</param>
        /// <param name="operationContext">An <see cref="OperationContext"/> object that represents the context for the current operation.</param>
        /// <returns>The result containing <see cref="IDictionary{TKey,TValue}"/> of <see cref="EntityProperty"/> objects for all properties of the flattened root object.</returns>
        public static Dictionary<string, EntityProperty> Flatten(object root, OperationContext operationContext)
        {
            return Flatten(root, new EntityPropertyConverterOptions { PropertyNameDelimiter = DefaultPropertyNameDelimiter }, operationContext);
        }

        /// <summary>
        /// Traverses object graph, flattens and converts all nested (and not nested) properties to EntityProperties, stores them in the property dictionary.
        /// The keys are constructed by appending the names of the properties visited during pre-order depth first traversal from root to each end node property delimited by '_'.
        /// Allows complex objects to be stored in persistent storage systems or passed between web services in a generic way.
        /// </summary>
        /// <param name="root">The object to flatten and convert.</param>
        /// <param name="entityPropertyConverterOptions">A <see cref="EntityPropertyConverterOptions"/> object that specifies options for the entity property conversion.</param>
        /// <param name="operationContext">An <see cref="OperationContext"/> object that represents the context for the current operation.</param>
        /// <returns>The result containing <see cref="IDictionary{TKey,TValue}"/> of <see cref="EntityProperty"/> objects for all properties of the flattened root object.</returns>
        public static Dictionary<string, EntityProperty> Flatten(object root, EntityPropertyConverterOptions entityPropertyConverterOptions, OperationContext operationContext)
        {
            if (root == null)
            {
                return null;
            }

            Dictionary<string, EntityProperty> propertyDictionary = new Dictionary<string, EntityProperty>();
            HashSet<object> antecedents = new HashSet<object>(new ObjectReferenceEqualityComparer());

            return Flatten(propertyDictionary, root, string.Empty, antecedents, entityPropertyConverterOptions, operationContext) ? propertyDictionary : null;
        }

        /// <summary>
        /// Reconstructs the complete object graph of type T using the flattened entity property dictionary and returns reconstructed object.
        /// The property dictionary may contain only basic properties, only nested properties or a mix of both types.
        /// </summary>
        /// <typeparam name="T">The type of the object to populate</typeparam>
        /// <param name="flattenedEntityProperties">The flattened entity property dictionary.</param>
        /// <param name="operationContext">An <see cref="OperationContext"/> object that represents the context for the current operation.</param>
        /// <returns>The result containing the reconstructed object with its full object hierarchy.</returns>
        public static T ConvertBack<T>(IDictionary<string, EntityProperty> flattenedEntityProperties, OperationContext operationContext)
        {
            return ConvertBack<T>(flattenedEntityProperties, new EntityPropertyConverterOptions { PropertyNameDelimiter = DefaultPropertyNameDelimiter }, operationContext);
        }

        /// <summary>
        /// Reconstructs the complete object graph of type T using the flattened entity property dictionary and returns reconstructed object.
        /// The property dictionary may contain only basic properties, only nested properties or a mix of both types.
        /// </summary>
        /// <typeparam name="T">The type of the object to populate</typeparam>
        /// <param name="flattenedEntityProperties">The flattened entity property dictionary.</param>
        /// <param name="entityPropertyConverterOptions">A <see cref="EntityPropertyConverterOptions"/> object that specifies options for the entity property conversion.</param>
        /// <param name="operationContext">An <see cref="OperationContext"/> object that represents the context for the current operation.</param>
        /// <returns>The result containing the reconstructed object with its full object hierarchy.</returns>
        public static T ConvertBack<T>(
            IDictionary<string, EntityProperty> flattenedEntityProperties,
            EntityPropertyConverterOptions entityPropertyConverterOptions,
            OperationContext operationContext)
        {
            if (flattenedEntityProperties == null)
            {
                return default(T);
            }

#if WINDOWS_DESKTOP || WINDOWS_RT || NETCORE
            T root = (T)Activator.CreateInstance(typeof(T));
#else
            T root = (T)FormatterServices.GetUninitializedObject(typeof(T));
#endif

            return flattenedEntityProperties.Aggregate(root, (current, kvp) => (T)SetProperty2(current, kvp.Key, kvp.Value.PropertyAsObject, entityPropertyConverterOptions, operationContext));
        }

        /// <summary>
        /// Traverses object graph, flattens and converts all nested (and not nested) properties to EntityProperties, stores them in the property dictionary.
        /// The keys are constructed by appending the names of the properties visited during pre-order depth first traversal from root to each end node property delimited by '.'.
        /// Allows complex objects to be stored in persistent storage systems or passed between web services in a generic way.
        /// </summary>
        /// <param name="propertyDictionary">The property dictionary.</param>
        /// <param name="current">The current object.</param>
        /// <param name="objectPath">The object path.</param>
        /// <param name="antecedents">The antecedents of current object, used to detect circular references in object graph.</param>
        /// <param name="entityPropertyConverterOptions">A <see cref="EntityPropertyConverterOptions"/> object that specifies options for the entity property conversion.</param>
        /// <param name="operationContext">An <see cref="OperationContext"/> object that represents the context for the current operation.</param>
        /// <returns>The <see cref="bool"/> to indicate success of conversion to flattened EntityPropertyDictionary.</returns>
        private static bool Flatten(
            Dictionary<string, EntityProperty> propertyDictionary,
            object current,
            string objectPath,
            HashSet<object> antecedents,
            EntityPropertyConverterOptions entityPropertyConverterOptions,
            OperationContext operationContext)
        {
            if (current == null)
            {
                return true;
            }

            Type type = current.GetType();
            EntityProperty entityProperty = CreateEntityPropertyWithType(current, type);

            if (entityProperty != null)
            {
                propertyDictionary.Add(objectPath, entityProperty);
                return true;
            }

#if WINDOWS_RT
            IEnumerable<PropertyInfo> propertyInfos = type.GetRuntimeProperties();
#elif NETCORE
            IEnumerable<PropertyInfo> propertyInfos = type.GetTypeInfo().GetAllProperties();
#else
            IEnumerable<PropertyInfo> propertyInfos = type.GetProperties();
#endif
            if (!propertyInfos.Any())
            {
                throw new SerializationException(string.Format(CultureInfo.InvariantCulture, SR.UnsupportedPropertyTypeForEntityPropertyConversion, type, objectPath));
            }

            bool isAntecedent = false;

#if WINDOWS_RT || NETCORE
            if (!type.GetTypeInfo().IsValueType)
#else
            if (!type.IsValueType)
#endif
            {
                if (antecedents.Contains(current))
                {
                    throw new SerializationException(string.Format(CultureInfo.InvariantCulture, SR.RecursiveReferencedObject, objectPath, type));
                }

                antecedents.Add(current);
                isAntecedent = true;
            }

            string propertyNameDelimiter = entityPropertyConverterOptions != null ? entityPropertyConverterOptions.PropertyNameDelimiter : DefaultPropertyNameDelimiter;

            bool success = propertyInfos
                .Where(propertyInfo => !ShouldSkip(propertyInfo, objectPath, operationContext))
                .All(propertyInfo =>
                {
                    if (propertyInfo.Name.Contains(propertyNameDelimiter))
                    {
                        throw new SerializationException(
                            string.Format(CultureInfo.InvariantCulture, SR.PropertyDelimiterExistsInPropertyName, propertyNameDelimiter, propertyInfo.Name, objectPath));
                    }

                    return Flatten(
                        propertyDictionary,
                        propertyInfo.GetValue(current, index: null),
                        string.IsNullOrWhiteSpace(objectPath) ? propertyInfo.Name : objectPath + propertyNameDelimiter + propertyInfo.Name,
                        antecedents,
                        entityPropertyConverterOptions,
                        operationContext);
                });

            if (isAntecedent)
            {
                antecedents.Remove(current);
            }

            return success;
        }

        /// <summary>Creates entity property with given type.</summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <returns>The <see cref="EntityProperty"/>.</returns>
        private static EntityProperty CreateEntityPropertyWithType(object value, Type type)
        {
            if (type == typeof(string))
            {
                return new EntityProperty((string)value);
            }
            else if (type == typeof(byte[]))
            {
                return new EntityProperty((byte[])value);
            }
            else if (type == typeof(byte))
            {
                byte[] temp = new byte[] { (byte)value };
                return new EntityProperty(temp);
            }
            else if (type == typeof(bool))
            {
                return new EntityProperty((bool)value);
            }
            else if (type == typeof(bool?))
            {
                return new EntityProperty((bool?)value);
            }
            else if (type == typeof(DateTime))
            {
                return new EntityProperty((DateTime)value);
            }
            else if (type == typeof(DateTime?))
            {
                return new EntityProperty((DateTime?)value);
            }
            else if (type == typeof(DateTimeOffset))
            {
                return new EntityProperty((DateTimeOffset)value);
            }
            else if (type == typeof(DateTimeOffset?))
            {
                return new EntityProperty((DateTimeOffset?)value);
            }
            else if (type == typeof(double))
            {
                return new EntityProperty((double)value);
            }
            else if (type == typeof(double?))
            {
                return new EntityProperty((double?)value);
            }
            else if (type == typeof(Guid?))
            {
                return new EntityProperty((Guid?)value);
            }
            else if (type == typeof(Guid))
            {
                return new EntityProperty((Guid)value);
            }
            else if (type == typeof(int))
            {
                return new EntityProperty((int)value);
            }
            else if (type == typeof(int?))
            {
                return new EntityProperty((int?)value);
            }
            else if (type == typeof(uint))
            {
                return new EntityProperty(unchecked((int)Convert.ToUInt32(value, CultureInfo.InvariantCulture)));
            }
            else if (type == typeof(uint?))
            {
                return new EntityProperty(unchecked((int?)Convert.ToUInt32(value, CultureInfo.InvariantCulture)));
            }
            else if (type == typeof(long))
            {
                return new EntityProperty((long)value);
            }
            else if (type == typeof(long?))
            {
                return new EntityProperty((long?)value);
            }
            else if (type == typeof(ulong))
            {
                return new EntityProperty(unchecked((long)Convert.ToUInt64(value, CultureInfo.InvariantCulture)));
            }
            else if (type == typeof(ulong?))
            {
                return new EntityProperty(unchecked((long?)Convert.ToUInt64(value, CultureInfo.InvariantCulture)));
            }
#if WINDOWS_RT || NETCORE
            else if (type.GetTypeInfo().IsEnum)
#else
            else if (type.IsEnum)
#endif
            {
                return new EntityProperty(value.ToString());
            }
            else if (type == typeof(TimeSpan))
            {
                return new EntityProperty(value.ToString());
            }
            else if (type == typeof(TimeSpan?))
            {
                return new EntityProperty(value != null ? value.ToString() : null);
            }
            else
            {
                return null;
            }
        }

        /// <summary>Sets the property given with the property path on the passed in object.</summary>
        /// <param name="root">The root object.</param>
        /// <param name="propertyPath">The full property path formed by the name of properties from root object to the target property(included), appended by '.'.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <param name="entityPropertyConverterOptions">A <see cref="EntityPropertyConverterOptions"/> object that specifies options for the entity property conversion.</param>
        /// <param name="operationContext">An <see cref="OperationContext"/> object that represents the context for the current operation.</param>
        /// <returns>The updated <see cref="object"/>.</returns>
        private static object SetProperty(
            object root,
            string propertyPath,
            object propertyValue,
            EntityPropertyConverterOptions entityPropertyConverterOptions,
            OperationContext operationContext)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }

            if (propertyPath == null)
            {
                throw new ArgumentNullException("propertyPath");
            }

            try
            {
                string propertyNameDelimiter = entityPropertyConverterOptions != null ? entityPropertyConverterOptions.PropertyNameDelimiter : DefaultPropertyNameDelimiter;

                Stack<Tuple<object, object, PropertyInfo>> valueTypePropertyHierarchy = new Stack<Tuple<object, object, PropertyInfo>>();
                string[] properties = propertyPath.Split(new[] { propertyNameDelimiter }, StringSplitOptions.RemoveEmptyEntries);

                object parentProperty = root;
                bool valueTypeDetected = false;

                for (int i = 0; i < properties.Length - 1; i++)
                {
#if WINDOWS_RT || NETCORE
                    PropertyInfo propertyToGet = parentProperty.GetType().GetRuntimeProperty(properties[i]);
#else
                    PropertyInfo propertyToGet = parentProperty.GetType().GetProperty(properties[i]);
#endif

                    if (propertyToGet == null)
                    {
                        return root;
                    }

                    object temp = propertyToGet.GetValue(parentProperty, null);
                    Type type = propertyToGet.PropertyType;

                    if (temp == null)
                    {
#if WINDOWS_DESKTOP || WINDOWS_RT || NETCORE
                        temp = Activator.CreateInstance(type);
#else
                        temp = FormatterServices.GetUninitializedObject(type);
#endif
                        propertyToGet.SetValue(parentProperty, ChangeType(temp, propertyToGet.PropertyType), index: null);
                    }

#if WINDOWS_RT || NETCORE
                    if (valueTypeDetected || type.GetTypeInfo().IsValueType)
#else
                    if (valueTypeDetected || type.IsValueType)
#endif
                    {
                        valueTypeDetected = true;
                        valueTypePropertyHierarchy.Push(new Tuple<object, object, PropertyInfo>(temp, parentProperty, propertyToGet));
                    }

                    parentProperty = temp;
                }

#if WINDOWS_RT || NETCORE
                PropertyInfo propertyToSet = parentProperty.GetType().GetRuntimeProperty(properties.Last());
#else
                PropertyInfo propertyToSet = parentProperty.GetType().GetProperty(properties.Last());
#endif

                if (propertyToSet != null)
                {
                    propertyToSet.SetValue(parentProperty, ChangeType(propertyValue, propertyToSet.PropertyType), index: null);


                    object termValue = parentProperty;
                    while (valueTypePropertyHierarchy.Count != 0)
                    {
                        Tuple<object, object, PropertyInfo> propertyTuple = valueTypePropertyHierarchy.Pop();
                        propertyTuple.Item3.SetValue(propertyTuple.Item2, ChangeType(termValue, propertyTuple.Item3.PropertyType), index: null);
                        termValue = propertyTuple.Item2;
                    }

                }

               

                return root;
            }
            catch (Exception ex)
            {
                Logger.LogError(operationContext, SR.TraceSetPropertyError, propertyPath, propertyValue, ex.Message);
                throw;
            }
        }



        private static object SetProperty2(object root, string propertyPath, object propertyValue, EntityPropertyConverterOptions entityPropertyConverterOptions, OperationContext operationContext)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }

            if (propertyPath == null)
            {
                throw new ArgumentNullException("propertyPath");
            }

            try
            {
                string text = ((entityPropertyConverterOptions != null) ? entityPropertyConverterOptions.PropertyNameDelimiter : "_");
                Stack<Tuple<object, object, PropertyInfo>> stack = new Stack<Tuple<object, object, PropertyInfo>>();
                string[] array = propertyPath.Split(new string[1] { text }, StringSplitOptions.RemoveEmptyEntries);
                object obj = root;
                bool flag = false;
                for (int i = 0; i < array.Length - 1; i++)
                {
                    PropertyInfo property = obj.GetType().GetProperty(array[i]);

                    if(property == null)
                    {
                        return root;
                    }

                    object obj2 = property.GetValue(obj, null);
                    Type propertyType = property.PropertyType;
                    if (obj2 == null)
                    {
                        obj2 = Activator.CreateInstance(propertyType);
                        property.SetValue(obj, ChangeType(obj2, property.PropertyType), null);
                    }

                    if (flag || propertyType.IsValueType)
                    {
                        flag = true;
                        stack.Push(new Tuple<object, object, PropertyInfo>(obj2, obj, property));
                    }

                    obj = obj2;
                }

                PropertyInfo property2 = obj.GetType().GetProperty(array.Last());
                property2.SetValue(obj, ChangeType(propertyValue, property2.PropertyType), null);
                object propertyValue2 = obj;
                while (stack.Count != 0)
                {
                    Tuple<object, object, PropertyInfo> tuple = stack.Pop();
                    tuple.Item3.SetValue(tuple.Item2, ChangeType(propertyValue2, tuple.Item3.PropertyType), null);
                    propertyValue2 = tuple.Item2;
                }

                return root;
            }
            catch (Exception ex)
            {
                Logger.LogError(operationContext, "Exception thrown while trying to set property value. Property Path: {0} Property Value: {1}. Exception Message: {2}", propertyPath, propertyValue, ex.Message);
                throw;
            }
        }


        /// <summary>Creates an object of specified propertyType from propertyValue.</summary>
        /// <param name="propertyValue">The property value.</param>
        /// <param name="propertyType">The property type.</param>
        /// <returns>The <see cref="object"/>.</returns>
        private static object ChangeType(object propertyValue, Type propertyType)
        {
            Type underlyingType = Nullable.GetUnderlyingType(propertyType);
            Type type = underlyingType ?? propertyType;

#if WINDOWS_RT || NETCORE
            if (type.GetTypeInfo().IsEnum)
#else
            if (type.IsEnum)
#endif
            {
                return Enum.Parse(type, propertyValue.ToString());
            }

            if (type == typeof(DateTimeOffset))
            {
                return new DateTimeOffset((DateTime)propertyValue);
            }

            if (type == typeof(TimeSpan))
            {
                return TimeSpan.Parse(propertyValue.ToString(), CultureInfo.InvariantCulture);
            }

            if (type == typeof(uint))
            {
                return unchecked((uint)(int)propertyValue);
            }

            if (type == typeof(ulong))
            {
                return unchecked((ulong)(long)propertyValue);
            }

            if (type == typeof(byte))
            {
                return ((byte[])propertyValue)[0];
            }

            return Convert.ChangeType(propertyValue, type, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Indicates whether the object member should be skipped from being flattened
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="objectPath">The object path.</param>
        /// <param name="operationContext">An <see cref="OperationContext"/> object that represents the context for the current operation.</param>
        /// <returns>The <see cref="bool"/> to indicate whether the object member should be skipped from being flattened.</returns>
        private static bool ShouldSkip(PropertyInfo propertyInfo, string objectPath, OperationContext operationContext)
        {
            if (!propertyInfo.CanWrite)
            {
                Logger.LogInformational(operationContext, SR.TraceNonExistingSetter, propertyInfo.Name, objectPath);
                return true;
            }

            if (!propertyInfo.CanRead)
            {
                Logger.LogInformational(operationContext, SR.TraceNonExistingGetter, propertyInfo.Name, objectPath);
                return true;
            }

#if WINDOWS_RT || NETCORE
            return propertyInfo.GetCustomAttribute(typeof(IgnorePropertyAttribute)) != null;
#else
            return Attribute.IsDefined(propertyInfo, typeof(IgnorePropertyAttribute));
#endif
        }

        /// <summary>
        /// The object reference equality comparer.
        /// </summary>
        private class ObjectReferenceEqualityComparer : IEqualityComparer<object>
        {
            public new bool Equals(object x, object y)
            {
                return ReferenceEquals(x, y);
            }

            public int GetHashCode(object obj)
            {
                return RuntimeHelpers.GetHashCode(obj);
            }
        }
    }
}
