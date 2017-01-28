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
        ElasticsearchRepository _client;

        [TestInitialize]
        public void TestCaseInit()
        {
            _client = new ElasticsearchRepository("828c94dd-b9e8-42f3-8d6b-dbdfa8c28cb7", new ElasticsearchClientConfig { Uri = new Uri("http://localhost:9200/") });
        }

        [TestMethod()]
        public async Task CreateIndexTest()
        {
            var result = await _client.AddDocumentAsync(new ElasticsearchBase<object>
            {
                DocumentType = "testDocument",
                UID = Guid.NewGuid().ToString(),
                Value = new
                {
                    Temp = 20,
                    Humid = 10,
                    ID = "afsaf"
                }
            });
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public async Task GetDocumentsAsyncTest()
        {
            var result = await _client.GetDocumentsAsync("testDocument", "now-5h");
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task GetFieldSumAsyncTest()
        {
            var result = await _client.GetFieldSumAsync("Temp","testDocument", "now-10h");
            Assert.IsNotNull(new object { });
        }
        [TestMethod()]
        public async Task GetFieldAvgAsyncTest()
        {
            var result = await _client.GetFieldAvgAsync("Temp", "testDocument", "now-10h");
            Assert.IsNotNull(new object { });
        }
        [TestMethod()]
        public async Task GetFieldStatsAsyncTest()
        {
            var result = await _client.GetFieldStatsAsync("Temp", "testDocument", "now-10h");
            Assert.IsNotNull(new object { });
        }
    }
}