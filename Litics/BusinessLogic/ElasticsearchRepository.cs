using Litics.BusinessLogic.Interfaces;
using System.Threading.Tasks;
using Litics.DAL.Elasticsearch.Helpers;
using Litics.DAL.Elasticsearch;
using System.Collections.Generic;

namespace Litics.BusinessLogic
{
    public class ElasticsearchRepository : IElasticsearchRepository
    {
        private readonly IConfiguration _configuration;
        private readonly Elasticsearch _client;
        public ElasticsearchRepository(IConfiguration configuration)
        {
            if (configuration != null)
            {
                _configuration = configuration;
                _client = new Elasticsearch(configuration.ElasticsearchClientConfig);
            }
        }
        public async Task<bool> AddDocumentAsync(string elasticsearchIndexName, ElasticsearchBase<object> document)
        {
            return await _client.AddDocumentAsync(elasticsearchIndexName, document);
        }

        public async Task<byte[]> GetDocumentsAsync(string elasticsearchIndexName, string typeName, string fromDateMath, string toDateMath = "now")
        {
            return await _client.GetDocumentsAsync(elasticsearchIndexName, typeName, fromDateMath, toDateMath);
        }

        public async Task<byte[]> GetMultiDocumentsAsync(string elasticsearchIndexName, Dictionary<string, string> queries)
        {
            return await _client.GetMultiDocumentsAsync(elasticsearchIndexName, queries);
        }
    }
}
