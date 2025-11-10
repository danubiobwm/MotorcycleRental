using Application.Dtos;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MotorcyclesController : ControllerBase
    {
        private readonly MotorcycleService _motorcycleService;

        public MotorcyclesController(MotorcycleService motorcycleService)
        {
            _motorcycleService = motorcycleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var motorcycles = await _motorcycleService.GetAllAsync();
            return Ok(motorcycles);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MotorcycleCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _motorcycleService.CreateAsync(dto);

            if (!result.Success)
                return BadRequest(result.Errors);

            return Ok(result.Data);
        }
    }
}
