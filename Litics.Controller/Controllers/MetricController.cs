using Litics.BusinessLogic.Interfaces;
using Litics.Controller.Filters;
using Litics.Controller.Models;
using Litics.DAL.Elasticsearch.Helpers;
using Litics.Entities;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Litics.Controller.Controllers
{
    [HMACAuthentication]
    [RoutePrefix("api/Metric")]
    public class MetricController : ApiController
    {
        private readonly IConfiguration _configuration;
        private readonly IElasticsearchRepository _elasticsearchRepository;
        private readonly ApplicationUserManager _userManager;
        public MetricController(IConfiguration configuration, IElasticsearchRepository elasticsearchRepository)
        {
            if (configuration != null || elasticsearchRepository != null)
            {
                _configuration = configuration;
                _elasticsearchRepository = elasticsearchRepository;
            }
        }

        protected ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }
        [Route("PostData")]
        [HttpPost]
        public async Task<IHttpActionResult> PostData(PostDataBindigModel postData)
        {
            try
            {
                var appId = ClaimsPrincipal.Current.Identity.Name;
                var esIndex = await UserManager.GetEsIndexByAppIdAsync(appId);
                var result = await _elasticsearchRepository.AddDocumentAsync(esIndex, new ElasticsearchBase<object>
                {
                    DocumentType = postData.Type,
                    UID = Guid.NewGuid().ToString(),
                    Value = postData.Value
                });
                return result ? Ok("PostData Succed!") : Ok("PosData Failed!");

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [Route("GetData")]
        [HttpGet]
        public async Task<IHttpActionResult> GetData([FromUri]GetDataBindigModel getData)
        {
            try
            {
                var appId = ClaimsPrincipal.Current.Identity.Name;
                var esIndex = await UserManager.GetEsIndexByAppIdAsync(appId);
                var result = await _elasticsearchRepository.GetDocumentsAsync(esIndex, getData.Type, getData.FromDateMath);
                var jsonStr = Encoding.UTF8.GetString(result);
                var json = JsonConvert.DeserializeObject<ElasticsearchResponse>(jsonStr);
                return Ok(json);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("GetMultiData")]
        [HttpGet]
        public async Task<IHttpActionResult> GetMultiData()
        {
            try
            {
                var appId = ClaimsPrincipal.Current.Identity.Name;
                var esIndex = await UserManager.GetEsIndexByAppIdAsync(appId);
                var result = await _elasticsearchRepository.GetMultiDocumentsAsync(esIndex,null);
                var jsonStr = Encoding.UTF8.GetString(result);
                var json = JsonConvert.DeserializeObject<ElasticsearchResponse>(jsonStr);
                return Ok(json);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}