using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litics.DAL.Elasticsearch.Helpers
{
    public class ElasticsearchHelper
    {
        public static string Serialize(object obj, string oldName, string newName)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new CustomNamesContractResolver(oldName, newName);
            return JsonConvert.SerializeObject(obj, settings);
        }
        public static T Deserialize<T>(string text, string oldName, string newName)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new CustomNamesContractResolver(oldName, newName);
            return JsonConvert.DeserializeObject<T>(text, settings);
        }
    }
    public class CustomNamesContractResolver : DefaultContractResolver
    {
        private readonly string _oldName;
        private readonly string _newName;
        public CustomNamesContractResolver(string oldName, string newName)
        {
            _oldName = oldName;
            _newName = newName;
        }
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            // Let the base class create all the JsonProperties 
            // using the short names
            IList<JsonProperty> list = base.CreateProperties(type, memberSerialization);

            // Now inspect each property and replace the 
            // short name with the real property name
            foreach (JsonProperty prop in list)
            {
                if (prop.UnderlyingName == _oldName)
                    prop.PropertyName = _newName;

            }
            return list;
        }
    }
}

