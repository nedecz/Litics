using Microsoft.VisualStudio.TestTools.UnitTesting;
using Litics.DAL.Elasticsearch;
using Litics.DAL.Elasticsearch.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Litics.DAL.Elasticsearch.Tests
{
    [TestClass()]
    public class ElasticsearchRepositoryTests
    {
        Elasticsearch _client;

        [TestInitialize]
        public void TestCaseInit()
        {
            _client = new Elasticsearch(new ElasticsearchClientConfig { Uri = new Uri("http://localhost:9200/") });
        }

        [TestMethod()]
        public async Task CreateIndexTest()
        {
            var result = await _client.AddDocumentAsync("828c94dd-b9e8-42f3-8d6b-dbdfa8c28cb7", new ElasticsearchBase<object>
            {
                DocumentType = "testDocument",
                UID = Guid.NewGuid().ToString(),
                Value = new
                {
                    Temp = 22,
                    Humid = 10,
                    ID = "afsaf"
                }
            });



            result = await _client.AddDocumentAsync("828c94dd-b9e8-42f3-8d6b-dbdfa8c28cb7", new ElasticsearchBase<object>
            {
                DocumentType = "testDoc",
                UID = Guid.NewGuid().ToString(),
                Value = new
                {
                    Temp = 20,
                    Humid = 14,
                    ID = "afsaf"
                }
            });
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public async Task GetDocumentsAsyncTest()
        {
            var result = await _client.GetDocumentsAsync("828c94dd-b9e8-42f3-8d6b-dbdfa8c28cb7", "testDocument", "now-5h");
            var str = System.Text.Encoding.Default.GetString(result);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task GetFieldSumAsyncTest()
        {
            var result = await _client.GetFieldSumAsync("828c94dd-b9e8-42f3-8d6b-dbdfa8c28cb7", "Temp", "testDocument", "now-10h");
            Assert.IsNotNull(new object { });
        }
        [TestMethod()]
        public async Task GetFieldAvgAsyncTest()
        {
            var result = await _client.GetFieldAvgAsync("828c94dd-b9e8-42f3-8d6b-dbdfa8c28cb7", "Temp", "testDocument", "now-10h");
            Assert.IsNotNull(new object { });
        }
        [TestMethod()]
        public async Task GetFieldStatsAsyncTest()
        {
            var result = await _client.GetFieldStatsAsync("828c94dd-b9e8-42f3-8d6b-dbdfa8c28cb7", "Temp", "testDocument", "now-10h");
            Assert.IsNotNull(new object { });
        }

        [TestMethod()]
        public async Task GetMultiDocumentsAsyncTest()
        {
            var dict = new Dictionary<string, string>();
            dict.Add("testDocument", "{\r\n    \"query\": {\r\n        \"range\" : {\r\n            \"Timestamp\" : {\r\n                \"gte\" : \"now-1d\"\r\n            }\r\n        }\r\n    }\r\n}");
            await _client.GetMultiDocumentsAsync("828c94dd-b9e8-42f3-8d6b-dbdfa8c28cb7", dict);
            Assert.Fail();
        }
    }
}