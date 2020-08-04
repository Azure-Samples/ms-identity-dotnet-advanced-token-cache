using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using Newtonsoft.Json;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        /// <summary>
        /// The Web API will only accept tokens 1) for users, and 
        /// 2) having the access_as_user scope for this API
        /// </summary>
        static readonly string[] scopeRequiredByApi = new string[] { "access_as_user" };
        private readonly GraphServiceClient _graphServiceClient;

        public ProfileController(GraphServiceClient graphServiceClient)
        {
            _graphServiceClient = graphServiceClient;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

            var userProfile = await _graphServiceClient.Me
                                    .Request()
                                    .Select(x => new 
                                    { 
                                        x.Country,
                                        x.City,
                                        x.EmployeeId,
                                        x.DisplayName,
                                        x.GivenName,
                                        x.Department
                                    })
                                    .GetAsync();

            return JsonConvert.SerializeObject(userProfile);
        }

    }
}
