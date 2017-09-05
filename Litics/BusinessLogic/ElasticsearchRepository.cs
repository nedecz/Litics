using Litics.BusinessLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Litics.DAL.Elasticsearch.Helpers;
using Litics.DAL.Elasticsearch;

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
    }
}
