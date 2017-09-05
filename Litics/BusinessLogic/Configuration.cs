using Litics.BusinessLogic.Interfaces;
using System;
using Litics.DAL.Elasticsearch.Helpers;
using System.Web.Configuration;

namespace Litics.BusinessLogic
{
    public class Configuration : IConfiguration
    {
        public ElasticsearchClientConfig ElasticsearchClientConfig
        {
            get
            {
                return new ElasticsearchClientConfig { Uri = new Uri(WebConfigurationManager.AppSettings["ElasticsearchNode"]) };
            }
        }
    }
}
