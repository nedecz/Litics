using Litics.BusinessLogic;
using Litics.BusinessLogic.Interfaces;
using Litics.Controller.Filters;
using Litics.DAL.Elasticsearch;
using Litics.DAL.Elasticsearch.Helpers;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Litics.Controller.Controllers
{
    [HMACAuthenticationAttribute]
    [RoutePrefix("api/Metric")]
    public class MetricController : ApiController
    {
        private readonly IConfiguration _configuration;
        private readonly IElasticsearchRepository _elasticsearchRepository;
        public MetricController(IConfiguration configuration, IElasticsearchRepository elasticsearchRepository)
        {
            if (configuration != null || elasticsearchRepository != null)
            {
                _configuration = configuration;
                _elasticsearchRepository = elasticsearchRepository;
            }
        }
        [Route("PostData")]
        [HttpPost]
        public async Task PostData(string type,Object value)
        {
            // Get ES id and acc id from db
            var result = await _elasticsearchRepository.AddDocumentAsync("828c94dd-b9e8-42f3-8d6b-dbdfa8c28cb7", new ElasticsearchBase<object>
            {
                DocumentType = type,
                UID = Guid.NewGuid().ToString(),
                Value = value
            });
            Console.WriteLine();
        }
        [Route("GetData")]
        [HttpGet]
        public async Task GetData()//Filters, aggregation, time
        {
            
        }
    }
}
