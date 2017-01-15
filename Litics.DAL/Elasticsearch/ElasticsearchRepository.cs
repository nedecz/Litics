using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using Litics.DAL.Elasticsearch.Helpers;

namespace Litics.DAL.Elasticsearch
{
    public class ElasticsearchRepository<T>
    {
        private readonly ElasticClient _client;
        public ElasticsearchRepository(ClientConfig clientConfig)
        {
            try
            {
                var connectionSettings = new ConnectionSettings(clientConfig.Uri).ThrowExceptions();
                _client = new ElasticClient(connectionSettings);
            }
            catch (Exception)
            {
                throw;
            }
           
        }

        public async Task<bool> CreateIndex(string indexName)
        {
            try
            {
                var response = await _client.CreateIndexAsync(indexName, index => index);
                return response.IsValid;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Add(ElasticsearchBase<T> document, string indexName, string typeName)
        {
            try
            {
                var response = await _client.IndexAsync(document, index => index.Index(indexName).Type(typeName));
                return response.Created;
            }
            catch (Exception)
            {
                throw;
            }
            
        }
    }
}
