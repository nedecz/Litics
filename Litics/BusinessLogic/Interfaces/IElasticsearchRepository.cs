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
    }
}
