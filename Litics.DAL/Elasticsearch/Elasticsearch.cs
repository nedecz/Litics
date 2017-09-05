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
    public class Elasticsearch
    {
        private static readonly object LockObject = new object();
        private static long _pendingWrites;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private ElasticClient _client;
        private const int DefaultLimit = 1000;
        private const int MaxResultWindow = 10000;

        private readonly EventWaitHandle EventWaitHandle = new AutoResetEvent(true);

        public Elasticsearch(ElasticsearchClientConfig clientConfig)
        {
            var settings = CreateConnectionSettings(clientConfig.Uri);
            _client = new ElasticClient(settings);
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

        private void WaitForPendingWrites(int timeoutMillisec)
        {
            var waitOne = EventWaitHandle.WaitOne(timeoutMillisec);
            Logger.Debug($"EventWaitHandle.WaitOne returned {waitOne},  _pendingWrites: {_pendingWrites}");
        }
        #endregion

        #region ConnectionInit

        private ConnectionSettings CreateConnectionSettings(Uri elasticUrl)
        {
            return new ConnectionSettings(elasticUrl)
                //.MapDefaultTypeIndices(m => m.Add(typeof(ElasticsearchBase<object>), _elasticsearchIndexName))
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
        #endregion

        #region Commands

        public async Task<bool> AddDocumentAsync(string elasticsearchIndexName, ElasticsearchBase<object> document)
        {
            try
            {
                var currentPostfix = GeneratePostfixForCurrentDate();
                elasticsearchIndexName = elasticsearchIndexName + "_" + currentPostfix;
                Logger.Debug($"Add Document Async... IndexName: {elasticsearchIndexName}, Type: {document.DocumentType}");
                var w = Stopwatch.StartNew();
                BeginWrite();
                var json = ElasticsearchHelper.Serialize(document, "Value", document.DocumentType);
                var result = await _client.LowLevel.IndexAsync<object>(elasticsearchIndexName, document.DocumentType, new PostData<object>(json));
                Logger.Debug($"Add Document Async Done!!! IndexName: {elasticsearchIndexName}, Type: {document.DocumentType}");
                return result.Success;
            }
            finally
            {
                EndWrite();
            }
        }
        public async Task<byte[]> GetDocumentsAsync(string elasticsearchIndexName, string typeName, string fromDateMath, string toDateMath = "now")
        {
            try
            {
                elasticsearchIndexName = elasticsearchIndexName + "*";
;                Logger.Debug($"Get Document Async... IndexName: {elasticsearchIndexName}, Type: {typeName}, From DateMath {fromDateMath}, To DateMath: {toDateMath}");
                var result = await _client.SearchAsync<object>(search => search.Index(elasticsearchIndexName).Type(typeName)
                .Query(query => query.DateRange(q => q.Field("Timestamp").GreaterThanOrEquals(DateMath.FromString(fromDateMath)).LessThanOrEquals(DateMath.FromString(toDateMath)))).Sort(sort => sort.Descending("Timestamp")));

                var doc = result.ApiCall.ResponseBodyInBytes;
                Logger.Debug($"Get Document Async Done! IndexName: {elasticsearchIndexName}, From DateMath {fromDateMath}, To DateMath: {toDateMath}");
                return doc;
            }
            catch (Exception ex)
            {
                Logger.
                    Error($"Get Document Async Error! IndexName: {elasticsearchIndexName}, Type: {typeName}, From DateMath {fromDateMath}, To DateMath: {toDateMath}, Msg: {ex.Message}");
                throw;
            }
        }

        public async Task<byte[]> GetFieldSumAsync(string elasticsearchIndexName, string fieldName, string typeName, string fromDateMath, string toDateMath = "now")
        {
            try
            {
                Logger.Debug($"Get Document Async... IndexName: {elasticsearchIndexName}, Type: {typeName}, From DateMath {fromDateMath}, To DateMath: {toDateMath}");
                var result = await _client.SearchAsync<object>(search => search.Index(elasticsearchIndexName).Type(typeName)
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
        public async Task<byte[]> GetFieldAvgAsync(string elasticsearchIndexName, string fieldName, string typeName, string fromDateMath, string toDateMath = "now")
        {
            try
            {
                Logger.Debug($"Get Document Async... IndexName: {elasticsearchIndexName}, Type: {typeName}, From DateMath {fromDateMath}, To DateMath: {toDateMath}");
                var result = await _client.SearchAsync<object>(search => search.Index(elasticsearchIndexName).Type(typeName)
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
        public async Task<byte[]> GetFieldStatsAsync(string elasticsearchIndexName, string fieldName, string typeName, string fromDateMath, string toDateMath = "now")
        {
            try
            {
                Logger.Debug($"Get Document Async... IndexName: {elasticsearchIndexName}, Type: {typeName}, From DateMath {fromDateMath}, To DateMath: {toDateMath}");
                var result = await _client.SearchAsync<object>(search => search.Index(elasticsearchIndexName).Type(typeName)
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
