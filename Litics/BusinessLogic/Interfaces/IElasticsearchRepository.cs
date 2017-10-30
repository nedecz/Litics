using Litics.DAL.Elasticsearch.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litics.BusinessLogic.Interfaces
{
    public interface IElasticsearchRepository
    {
        Task<bool> AddDocumentAsync(string elasticsearchIndexName, ElasticsearchBase<object> document);
        Task<byte[]> GetDocumentsAsync(string elasticsearchIndexName, string typeName, string fromDateMath, string toDateMath = "now");
        Task<byte[]> GetMultiDocumentsAsync(string elasticsearchIndexName, Dictionary<string, string> queries);
    }
}
