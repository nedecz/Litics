using Litics.DAL.Elasticsearch.Helpers;

namespace Litics.BusinessLogic.Interfaces
{
    public interface IConfiguration
    {
        ElasticsearchClientConfig ElasticsearchClientConfig { get; }
    }
}
