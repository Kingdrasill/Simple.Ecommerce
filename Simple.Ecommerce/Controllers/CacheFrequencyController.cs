using Simple.Ecommerce.Api.Services;
using Simple.Ecommerce.App.Interfaces.Queries.CacheFrequencyQueries;
using Simple.Ecommerce.Contracts.CacheFrequencyContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Simple.Ecommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CacheFrequencyController : ControllerBase
    {
        private readonly IListCacheFrequencyQuery _listCacheFrequencyQuery;

        public CacheFrequencyController(
            IListCacheFrequencyQuery listCacheFrequencyQuery
        )
        {
            _listCacheFrequencyQuery = listCacheFrequencyQuery;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<CacheFrequencyResponse>>> List()
        {
            var result = await _listCacheFrequencyQuery.Execute();

            return ResultHandler.HandleResult(this, result);
        }
    }
}
