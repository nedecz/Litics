using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litics.DAL.Elasticsearch.Helpers
{
    public class ElasticsearchBase<T>
    {
        //Account id
        public string UID { get; set; } 
        public long Timestamp { get; set; }
        public T Value { get; set; }
    }
}
