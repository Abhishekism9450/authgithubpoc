using Microsoft.AspNetCore.Mvc;
using dotnet.service;

namespace dotnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private IActionResult callbackHandler(){
            return Ok("Working");
        }
        private IndexModel _repositories;
        public ValuesController(){
            _repositories=new IndexModel();
        }
        public ActionResult  Get()
        {
            return Ok(_repositories);
        }
    }
}
