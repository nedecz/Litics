using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Litics.Controller.BusinessLogic.Interfaces;
using System.Configuration;

namespace Litics.Controller.BusinessLogic
{
    public class Configuration : IConfiguration
    {
        public string ElasticsearchNode
        {
            get
            {
                return ConfigurationManager.AppSettings["ElasticsearchNode"];
            }
        }
    }
}