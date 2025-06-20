using Cache.Library.Core;
using Microsoft.AspNetCore.Mvc;

namespace Simple.Ecommerce.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TesteCacheController : ControllerBase
    {
        private readonly ICache _cache;

        public TesteCacheController(ICache cache)
        {
            _cache = cache;
        }

        [HttpGet("inserir")]
        public IActionResult InserirDado()
        {
            var data = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { "Id", 1 },
                    { "Nome", "Exemplo" }
                }
            };

            _cache.AddItem("meuDado", "Id", data, false, null, out var msg);

            return Ok(new { Mensagem = msg });
        }
    }
}
