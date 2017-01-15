using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litics.Controller.BusinessLogic.Interfaces
{
    public interface IConfiguration
    {
        string ElasticsearchNode { get; }
    }
}
