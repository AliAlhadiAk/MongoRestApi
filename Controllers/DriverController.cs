using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoCrud.Model;
using MongoCrud.Services;

namespace MongoCrud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly DriverService _driverService;
        private readonly ILogger<DriverController> _logger;

        public DriverController(DriverService driverService, ILogger<DriverController> logger)
        {
            _driverService = driverService;
            _logger = logger;
        }
        [HttpGet("Id")]

        public async Task<IActionResult> GetDriver(string Id)
        {
            var driver = await _driverService.GetAsyncById(Id);
            if(driver is null)
                return NotFound();
            return Ok(driver);

        }
        [HttpGet("GetAll")]

        public async Task<IActionResult> GetAllDriver(string Id)
        {
            var driver = await _driverService.GetAsync();
            if (driver is null)
                return NotFound();
         
            return Ok(driver);

        }

        [HttpPost("PostDriver")]

        public async Task<IActionResult> AddDriver(Drivers driver)
        {
           await _driverService.InsertDriver(driver);
           return CreatedAtAction(nameof(GetAllDriver), driver);

        }

        [HttpPut("PostDriver")]

        public async Task<IActionResult> UpdateDriver(Drivers driver)
        {
            await _driverService.UpdateDriver(driver);
            return Ok("Uodated Succefully");
            

        }

        [HttpPut("Delete")]

        public async Task<IActionResult> DeleteDriver(string Id)
        {
            await _driverService.DeleteDriver(Id);
            return NotFound();


        }

    }
}
