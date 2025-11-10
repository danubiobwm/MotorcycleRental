using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.Dtos;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CouriersController : ControllerBase
    {
        private readonly CourierService _courierService;

        public CouriersController(CourierService courierService)
        {
            _courierService = courierService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CourierCreateDto dto)
        {
            var result = await _courierService.CreateAsync(dto);

            if (!result.Success)
                return BadRequest(new { errors = result.Errors });

            return CreatedAtAction(nameof(Create), new { id = result.Data!.Id }, result.Data);
        }
    }
}
