using System;
using System.Collections.Generic;

namespace Litics.Entities
{
    public class Shards
    {
        public int total { get; set; }
        public int successful { get; set; }
        public int failed { get; set; }
    }

    public class Hit
    {
        public string _type { get; set; }
        public string _id { get; set; }
        public object _score { get; set; }
        public Dictionary<String, Object> _source { get; set; }
        public List<object> sort { get; set; }
    }

    public class Hits
    {
        public int total { get; set; }
        public object max_score { get; set; }
        public List<Hit> hits { get; set; }
    }

    public class ElasticsearchResponse
    {
        public int took { get; set; }
        public bool timed_out { get; set; }
        public Shards _shards { get; set; }
        public Hits hits { get; set; }
    }
}
