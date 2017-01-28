using System;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using Litics.DAL.Elasticsearch.Helpers;
using NLog;
using System.Threading;
using Elasticsearch.Net;
using System.Diagnostics;

namespace Litics.DAL.Elasticsearch
{
    public class ElasticsearchRepository
    {
        private static readonly object LockObject = new object();
        private static ulong _instanceId;
        private static long _pendingWrites;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private string _elasticsearchIndexName;
        private string _commonAlias;
        private string _currentAlias;
        private string _postfix;
        private ElasticClient _client;
        private const int DefaultLimit = 1000;
        private const int MaxResultWindow = 10000;

        private readonly EventWaitHandle EventWaitHandle = new AutoResetEvent(true);

        public ElasticsearchRepository(string elasticsearchIndexName, ElasticsearchClientConfig clientConfig)
        {
            _instanceId = unchecked(_instanceId + 1);
            // Logger.Trace($"Creating an instance of type ElasticsearchRepository<{typeof(T).Name}> with id {_instanceId}");

            bool indexNeedsToBeRotated;
            string indexName;

            indexNeedsToBeRotated = true;
            indexName = elasticsearchIndexName;

            _commonAlias = elasticsearchIndexName;

            var elasticUrl = clientConfig.Uri;
            if (indexNeedsToBeRotated)
            {
                _currentAlias = "current" + elasticsearchIndexName;
                _postfix = GeneratePostfixForCurrentDate();
                _elasticsearchIndexName = indexName + "_" + _postfix;
            }
            else
            {
                _elasticsearchIndexName = indexName;
                _currentAlias = indexName;
            }
            var settings = CreateConnectionSettings(elasticUrl);
            _client = new ElasticClient(settings);
            if (indexNeedsToBeRotated)
            {
                IndexRotating(elasticsearchIndexName);
            }
            else if (!_client.IndexExists(_elasticsearchIndexName).Exists)
            {
                var indexresult = _client.CreateIndex(_elasticsearchIndexName);
                Logger.Debug($"Index: {indexName} creation is {indexresult.IsValid}");
            }
            Logger.Trace($"Instance with id {_instanceId} succesfully created.");
        }

        public void IndexRotating(string elasticsearchIndexName)
        {
            var currentPostfix = GeneratePostfixForCurrentDate();
            if (long.Parse(_postfix) < long.Parse(currentPostfix))
            {
                _postfix = currentPostfix;
                _elasticsearchIndexName = elasticsearchIndexName + "_" + _postfix;
                DeleteCurrentAlias();
                CreateAliases();
            }
            else if (!_client.IndexExists(_elasticsearchIndexName).Exists)
            {
                DeleteCurrentAlias();
                CreateAliases();
            }
        }

        #region Pending Writes
        private void BeginWrite()
        {
            Interlocked.Increment(ref _pendingWrites);
            EventWaitHandle.Reset();
            Logger.Trace($"EventWaitHandle.Reset(), _pendingWrites: {_pendingWrites}");
        }
        private void EndWrite()
        {
            if (Interlocked.Decrement(ref _pendingWrites) != 0) return;
            EventWaitHandle.Set();
            Logger.Trace($"EventWaitHandle.Set(), _pendingWrites: {_pendingWrites}");
        }

        public void WaitForPendingWrites(int timeoutMillisec)
        {
            var waitOne = EventWaitHandle.WaitOne(timeoutMillisec);
            Logger.Debug($"EventWaitHandle.WaitOne returned {waitOne},  _pendingWrites: {_pendingWrites}");
        }
        #endregion

        #region ConnectionInit

        private ConnectionSettings CreateConnectionSettings(Uri elasticUrl)
        {
            return new ConnectionSettings(elasticUrl)
                .MapDefaultTypeIndices(m => m.Add(typeof(ElasticsearchBase<object>), _elasticsearchIndexName))
                .ThrowExceptions()
                .DisableDirectStreaming()
                .OnRequestCompleted(new Action<IApiCallDetails>(DumpApiRequestDetails));
        }

        private static void DumpApiRequestDetails(IApiCallDetails callDetails)
        {
            var logMessage = $"ES request to {callDetails.Uri} completed. HTTP Status: {callDetails.HttpStatusCode}, Method: {callDetails.HttpMethod}";
            Logger.Log(callDetails.Success ? NLog.LogLevel.Trace : NLog.LogLevel.Error, logMessage);
            Logger.Trace($"ES request debug information: {callDetails.DebugInformation}");
        }

        private static string GeneratePostfixForCurrentDate()
        {
            return DateTime.UtcNow.ToString("yyyyMM");
        }

        private void CreateAliases()
        {
            var indexresult = _client.CreateIndex(_elasticsearchIndexName);
            Logger.Debug($"Index: {_elasticsearchIndexName} creation is {indexresult.IsValid}");
            var commonAlias = _client.Alias(a => a.Add(add => add.Index(_elasticsearchIndexName).Alias(_commonAlias)));
            Logger.Debug($"Common alias: {_commonAlias} creation is {commonAlias.IsValid}");
            var currentAlias = _client.Alias(a => a.Add(add => add.Index(_elasticsearchIndexName).Alias(_currentAlias)));
            Logger.Debug($"Current alias: {_currentAlias} creation is {currentAlias.IsValid}");
        }

        private void DeleteCurrentAlias()
        {
            if (_client.IndexExists(_currentAlias).Exists)
            {
                var current = _client.GetIndicesPointingToAlias(_currentAlias);
                foreach (var index in current)
                {
                    if (index != _elasticsearchIndexName)
                    {
                        _client.DeleteAlias(index, _currentAlias);
                        Logger.Info($"Delete current alias from: {index} index");
                    }
                }
            }
        }
        #endregion

        #region Commands

        public async Task<bool> AddDocumentAsync(ElasticsearchBase<object> document)
        {
            try
            {
                Logger.Debug($"Add Document Async... IndexName: {_elasticsearchIndexName}, Type: {document.DocumentType}");
                var w = Stopwatch.StartNew();
                BeginWrite();
                var json = ElasticsearchHelper.Serialize(document, "Value", document.DocumentType);
                var result = await _client.LowLevel.IndexAsync<object>(_currentAlias, document.DocumentType, new PostData<object>(json));
                Logger.Debug($"Add Document Async Done!!! IndexName: {_elasticsearchIndexName}, Type: {document.DocumentType}");
                return result.Success;
            }
            finally
            {
                EndWrite();
            }
        }
        public async Task<byte[]> GetDocumentsAsync(string typeName, string fromDateMath, string toDateMath = "now")
        {
            try
            {
                Logger.Debug($"Get Document Async... IndexName: {_elasticsearchIndexName}, Type: {typeName}, From DateMath {fromDateMath}, To DateMath: {toDateMath}");
                var result = await _client.SearchAsync<object>(search => search.Index(_currentAlias).Type(typeName)
                .Query(query => query.DateRange(q => q.Field("Timestamp").GreaterThanOrEquals(DateMath.FromString(fromDateMath)).LessThanOrEquals(DateMath.FromString(toDateMath)))).Sort(sort => sort.Descending("Timestamp")));

                var doc = result.ApiCall.ResponseBodyInBytes;
                Logger.Debug($"Get Document Async Done! IndexName: {_elasticsearchIndexName}, From DateMath {fromDateMath}, To DateMath: {toDateMath}");
                return doc;
            }
            catch (Exception ex)
            {
                Logger.
                    Error($"Get Document Async Error! IndexName: {_elasticsearchIndexName}, Type: {typeName}, From DateMath {fromDateMath}, To DateMath: {toDateMath}, Msg: {ex.Message}");
                throw;
            }
        }

        public async Task<byte[]> GetFieldSumAsync(string fieldName, string typeName, string fromDateMath, string toDateMath = "now")
        {
            try
            {
                Logger.Debug($"Get Document Async... IndexName: {_elasticsearchIndexName}, Type: {typeName}, From DateMath {fromDateMath}, To DateMath: {toDateMath}");
                var result = await _client.SearchAsync<object>(search => search.Index(_currentAlias).Type(typeName)
                .Aggregations(a => a.DateHistogram("projects_started_per_month", dh => dh.Field("Timestamp").Interval(DateInterval.Minute)
                    .Aggregations(aa => aa.Sum("commits", sm => sm.Field($"{typeName}.{fieldName}"))))                
                .SumBucket("sum_of_commits", aaa => aaa.BucketsPath("projects_started_per_month>commits")))
                .Query(query => query.DateRange(q => q.Field("Timestamp").GreaterThanOrEquals(DateMath.FromString(fromDateMath)).LessThanOrEquals(DateMath.FromString(toDateMath)))).Sort(sort => sort.Descending("Timestamp")));

                return result.ApiCall.ResponseBodyInBytes;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<byte[]> GetFieldAvgAsync(string fieldName, string typeName, string fromDateMath, string toDateMath = "now")
        {
            try
            {
                Logger.Debug($"Get Document Async... IndexName: {_elasticsearchIndexName}, Type: {typeName}, From DateMath {fromDateMath}, To DateMath: {toDateMath}");
                var result = await _client.SearchAsync<object>(search => search.Index(_currentAlias).Type(typeName)
                .Aggregations(a => a.DateHistogram("projects_started_per_month", dh => dh.Field("Timestamp").Interval(DateInterval.Minute)
                    .Aggregations(aa => aa.Average("commits", sm => sm.Field($"{typeName}.{fieldName}"))))
                .AverageBucket("sum_of_commits", aaa => aaa.BucketsPath("projects_started_per_month>commits")))
                .Query(query => query.DateRange(q => q.Field("Timestamp").GreaterThanOrEquals(DateMath.FromString(fromDateMath)).LessThanOrEquals(DateMath.FromString(toDateMath)))).Sort(sort => sort.Descending("Timestamp")));

                return result.ApiCall.ResponseBodyInBytes;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<byte[]> GetFieldStatsAsync(string fieldName, string typeName, string fromDateMath, string toDateMath = "now")
        {
            try
            {
                Logger.Debug($"Get Document Async... IndexName: {_elasticsearchIndexName}, Type: {typeName}, From DateMath {fromDateMath}, To DateMath: {toDateMath}");
                var result = await _client.SearchAsync<object>(search => search.Index(_currentAlias).Type(typeName)
                .Aggregations(a => a.DateHistogram("projects_started_per_month", dh => dh.Field("Timestamp").Interval(DateInterval.Minute)
                    .Aggregations(aa => aa.Stats("commits", sm => sm.Field($"{typeName}.{fieldName}")))))
                //.StatsBucket("sum_of_commits", aaa => aaa.BucketsPath("projects_started_per_month>commits")))
                .Query(query => query.DateRange(q => q.Field("Timestamp").GreaterThanOrEquals(DateMath.FromString(fromDateMath)).LessThanOrEquals(DateMath.FromString(toDateMath)))).Sort(sort => sort.Descending("Timestamp")));

                return result.ApiCall.ResponseBodyInBytes;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
    }
}
