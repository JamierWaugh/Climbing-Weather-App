using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;
using RocksStartService.RockTypeService;



//Controller used to call the /rocks api that returns the cached rocks list
[ApiController]
[Route("/rocks")]
public class RocksController : ControllerBase
{

    private readonly RockTypeService _rocksService;

    public RocksController(RockTypeService rocksService)
    {
        _rocksService = rocksService;
    }

    [HttpGet]
    public IActionResult GetRocks()
    {
        //Get cachedRocks from rocksService object
        HashSet<string> cachedRocks = _rocksService.GetRocks();
        return Ok(cachedRocks);
    }

}