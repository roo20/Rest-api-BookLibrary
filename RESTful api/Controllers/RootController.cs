using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RESTful_api.Dtos;

namespace RESTful_api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    public class RootController : ControllerBase
    {
        [HttpGet(Name = "GetRoot")]
        public IActionResult GetRoot()
        {
            // create links for root
            var links = new List<LinkDto>();

            links.Add(
              new(Url.Link("GetRoot", new { }),
              "self",
              "GET"));

            links.Add(
              new(Url.Link("GetBooks", new { }),
              "books",
              "GET"));

            links.Add(
              new(Url.Link("CreateBook", new { }),
              "create_book",
              "POST"));

            return Ok(links);
        }
    }
}
